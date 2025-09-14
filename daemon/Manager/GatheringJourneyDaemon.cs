using System.Data;
using System.Text.Json;
using daemon.Database;
using daemon.Models.Database;
using daemon.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace daemon.Manager;

public class GatheringJourneyDaemon : Daemon
{
	private readonly ILogger<GatheringJourneyDaemon> _logger;
	private readonly IServiceProvider _serviceProvider;
	private readonly ProxyRotator _proxyRotator;

	private readonly string _apiUrl = "https://regio-guide.de/@prd/zupo-travel-information/api/public/ri/journey/{0}";

	public GatheringJourneyDaemon(
		ILogger<GatheringJourneyDaemon> logger,
		IServiceProvider serviceProvider,
		ProxyRotator proxyRotator
	)
		: base("Gather Journey", TimeSpan.FromSeconds(15), logger)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null");
		_serviceProvider =
			serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider), "Service provider cannot be null");
		_proxyRotator = proxyRotator ?? throw new ArgumentNullException(nameof(proxyRotator), "Proxy rotator cannot be null");
	}

	// timetable changes
	private readonly DateTime[] _timetableChanges =
	{
		new(2024, 12, 17, 0, 0, 0, DateTimeKind.Utc),
		new(2025, 6, 15, 0, 0, 0, DateTimeKind.Utc),
	};

	private DateTime GetLastTimetableChange(DateTime? compareTo = null)
	{
		compareTo ??= DateTime.UtcNow;
		return _timetableChanges.Where(change => change <= compareTo).DefaultIfEmpty(_timetableChanges.Min()).Max();
	}

	protected override async Task ExecuteCoreAsync(CancellationToken cancellationToken)
	{
		using var scope = _serviceProvider.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<NavigatorDbContext>();

		var date = DateTime.UtcNow;

		IdentifiedRisId? randomRisId = null;
		await using (
			var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken)
		)
		{
			var risIds = await dbContext
				.RisIds.Where(risId => !risId.IsLocked)
				.Where(risId => risId.Active)
				.Where(risId => risId.LastSeen == null || risId.LastSeen < date.Date.AddDays(-1))
				.OrderBy(risId => risId.LastSeen ?? DateTime.MinValue)
				.Take(1000)
				.ToListAsync(cancellationToken);
			randomRisId = risIds.Count > 0 ? risIds[Random.Shared.Next(risIds.Count)] : null;
			if (randomRisId == null)
			{
				await transaction.RollbackAsync(cancellationToken);
				return;
			}

			randomRisId.IsLocked = true;
			await dbContext.SaveChangesAsync(cancellationToken);
			await transaction.CommitAsync(cancellationToken);
		}

		try
		{
			/*
			 * For RIS IDs that have never been processed (LastSeen is null):
			 * - Gets the last timetable change date (starting from the previous day at midnight).
			 * - Uses this date, going back 7 days (=> this could be lowered, but 7 days is a good buffer)
			 */
			if (randomRisId.LastSeen == null)
			{
				var timetableChange = GetLastTimetableChange(date.Date.AddDays(-1));
				date = new DateTime(
					DateOnly.FromDateTime(timetableChange.AddDays(-7)),
					TimeOnly.FromTimeSpan(date.TimeOfDay),
					DateTimeKind.Utc
				);
				await ProcessJourney(randomRisId, date, dbContext, cancellationToken);
			}
			/*
			 * For previously processed RIS IDs, we are comparing:
			 * - Midnight of LastSeen (+1 day)              |       exp: 2025-06-13 20:17:00     ->    2025-06-14 00:00:00
			 * - Midnight of current date (-1 day)          |       exp: 2025-06-15 13:00:00     ->    2025-06-14 00:00:00
			 *
			 * We add one day to LastSeen (as this is the day which we want to gather) and then compare it to the current date (-1 day).
			 * Result: LastSeen of the RIS ID is going to be set to : 2025-06-14 00:00:00

			 * If LastSeen is older than the current date, the RIS ID is processed. Would it be newer, the journey could be not complete yet.
			 */
			else if (randomRisId.LastSeen.Value.Date.AddDays(1) < date.Date.AddDays(-1))
			{
				date = new DateTime(
					DateOnly.FromDateTime(randomRisId.LastSeen!.Value.Date.AddDays(1)),
					TimeOnly.FromTimeSpan(date.TimeOfDay),
					DateTimeKind.Utc
				);
				await ProcessJourney(randomRisId, date, dbContext, cancellationToken);
			}
		}
		finally
		{
			await using var unlockTransaction = await dbContext.Database.BeginTransactionAsync(
				IsolationLevel.ReadCommitted,
				cancellationToken
			);
			randomRisId.IsLocked = false;
			await dbContext.SaveChangesAsync(cancellationToken);
			await unlockTransaction.CommitAsync(cancellationToken);
		}
	}

	private async Task ProcessJourney(
		IdentifiedRisId risId,
		DateTime date,
		NavigatorDbContext dbContext,
		CancellationToken cancellationToken
	)
	{
		JourneyResponse journeyResponse = await CallApi(risId.Id, date, cancellationToken);
		if (journeyResponse.ParsingError)
		{
			_logger.LogError("Failed to parse journey for RIS ID {RisId}", risId.Id);
			return;
		}

		// first update risId
		risId.LastSeen = date;

		if (journeyResponse.Journey != null)
		{
			risId.LastSucceededAt = date;

			// check before adding
			var exists = await dbContext.Journeys.AnyAsync(j => j.Id == journeyResponse.Journey.Id, cancellationToken);
			if (!exists)
				dbContext.Journeys.Add(journeyResponse.Journey);
		}

		await dbContext.SaveChangesAsync(cancellationToken);
	}

	private async Task<JourneyResponse> CallApi(string risId, DateTime when, CancellationToken cancellationToken)
	{
		string formattedId = when.ToString("yyyyMMdd") + "-" + risId;
		_logger.LogDebug("Gathering journey for RIS ID {0}", formattedId);

		var httpClient = _proxyRotator.GetRandomProxy();
		var request = await httpClient.GetAsync(string.Format(_apiUrl, formattedId), cancellationToken);

		// Everything except 200 is considered as no journey found
		if (request.StatusCode != System.Net.HttpStatusCode.OK)
			return new() { Journey = null, ParsingError = false };

		await using var stream = await request.Content.ReadAsStreamAsync(cancellationToken);
		var content = (await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken)).RootElement;

		if (content.GetProperty("journeyId").ValueKind == JsonValueKind.Null)
			return new() { Journey = null, ParsingError = false };
		var journeyId = content.GetProperty("journeyId").GetString()!;

		var journey = new Journey()
		{
			Id = journeyId,
			Operator = Operator.CreateOperator(content, journeyId: journeyId),
			LineInformation = LineInformation.CreateLineInformation(content, journeyId: journeyId),
			Messages = content
				.GetProperty("hims")
				.EnumerateArray()
				.Select(message => JourneyMessage.CreateJourneyMessage(message, journeyId))
				.ToList(),
			ViaStops = content.GetProperty("stops").EnumerateArray().Select(stop => Stop.CreateStop(stop, journeyId)).ToList(),
		};

		return new JourneyResponse() { Journey = journey, ParsingError = false };
	}
}

class JourneyResponse
{
	public Journey? Journey { get; set; }
	public bool ParsingError { get; set; } = false;
}
