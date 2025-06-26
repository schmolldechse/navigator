using System.Data;
using System.Text.Json;
using daemon.Database;
using daemon.Models.Database;
using daemon.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace daemon.Manager;

public class GatheringRisIdsDaemon : Daemon
{
	private readonly ILogger<GatheringRisIdsDaemon> _logger;
	private readonly IServiceProvider _serviceProvider;
	private readonly ProxyRotator _proxyRotator;

	private readonly string _apiUrl =
		"https://regio-guide.de/@prd/zupo-travel-information/api/public/ri/board/{0}/{1}?timeStart={2}&timeEnd={3}&expandTimeFrame=TIME_START&modeOfTransport=HIGH_SPEED_TRAIN,INTERCITY_TRAIN,INTER_REGIONAL_TRAIN,REGIONAL_TRAIN,CITY_TRAIN,BUS,FERRY,SUBWAY,TRAM,SHUTTLE,UNKNOWN";

	public GatheringRisIdsDaemon(
		ILogger<GatheringRisIdsDaemon> logger,
		IServiceProvider serviceProvider,
		ProxyRotator proxyRotator
	)
		: base("Gathering RIS IDs", TimeSpan.FromSeconds(300), logger)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null");
		_serviceProvider =
			serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider), "Service provider cannot be null");
		_proxyRotator = proxyRotator ?? throw new ArgumentNullException(nameof(proxyRotator), "Proxy rotator cannot be null");
	}

	protected override async Task ExecuteCoreAsync(CancellationToken cancellationToken)
	{
		using var scope = _serviceProvider.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<NavigatorDbContext>();

		var date = DateTime.UtcNow;

		Station? randomStation = null;
		await using (
			var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken)
		)
		{
			randomStation = await dbContext
				.Stations.Where(station => !station.IsLocked)
				.Where(station => station.QueryingEnabled)
				.Where(station => station.LastQueried == null || station.LastQueried < date.Date.AddDays(-1))
				.Include(station => station.Products)
				.OrderBy(_ => Guid.NewGuid())
				.FirstOrDefaultAsync(cancellationToken);
			if (randomStation == null)
			{
				await transaction.RollbackAsync(cancellationToken);
				return;
			}

			randomStation.IsLocked = true;
			await dbContext.SaveChangesAsync(cancellationToken);
			await transaction.CommitAsync(cancellationToken);
		}

		try
		{
			if (randomStation.LastQueried == null)
			{
				// the API above saves RIS id's up to 7 days in the past
				var startDate = date.AddDays(-7);
				_logger.LogInformation(
					"{Name} (evaNumber: {EvaNumber}) has not been queried yet! Set 'last_queried to' {StartDate}",
					randomStation.Name,
					randomStation.EvaNumber,
					startDate
				);

				await ProcessStation(randomStation, startDate, dbContext, cancellationToken);
			}
			else if (randomStation.LastQueried.Value.Date < date.Date)
			{
				var newLastQueried = new DateTime(
					DateOnly.FromDateTime(randomStation.LastQueried.Value.Date.AddDays(1)),
					TimeOnly.FromTimeSpan(date.TimeOfDay),
					DateTimeKind.Utc
				);
				_logger.LogInformation(
					"Querying {Name} (evaNumber: {EvaNumber}) for date {Date}",
					randomStation.Name,
					randomStation.EvaNumber,
					newLastQueried
				);

				await ProcessStation(randomStation, newLastQueried, dbContext, cancellationToken);
			}
		}
		finally
		{
			await using var unlockTransaction = await dbContext.Database.BeginTransactionAsync(
				IsolationLevel.ReadCommitted,
				cancellationToken
			);
			randomStation.IsLocked = false;
			await dbContext.SaveChangesAsync(cancellationToken);
			await unlockTransaction.CommitAsync(cancellationToken);
		}
	}

	private async Task ProcessStation(
		Station station,
		DateTime date,
		NavigatorDbContext dbContext,
		CancellationToken cancellationToken
	)
	{
		var results = (
			await Task.WhenAll(
				[
					CallApi(station.EvaNumber, date.Date),
					CallApi(station.EvaNumber, date.Date, false),
					CallApi(station.EvaNumber, date.Date.AddHours(12)),
					CallApi(station.EvaNumber, date.Date.AddHours(12), false),
				]
			)
		)
			.SelectMany(risId => risId)
			.DistinctBy(risId => risId.Id)
			.ToList();

		// filter out RIS IDs that don't match enabled products
		var enabledProducts = station
			.Products.Where(product => product.QueryingEnabled)
			.Select(product => product.ProductName)
			.ToHashSet();
		var filteredByProduct = results.Where(risId => enabledProducts.Contains(risId.Product)).ToList();

		var (inserted, updated) = await UpsertRisIds(station, date, filteredByProduct, dbContext, cancellationToken);
		_logger.LogInformation(
			"Retrieved a total of {Count} RIS IDs for station {StationName} (evaNumber: {EvaNumber}): Filtered out by product: {Filtered}, Already in DB (updated `active` bool): {Updated}, Inserted: {Inserted}",
			results.Count,
			station.Name,
			station.EvaNumber,
			(results.Count - filteredByProduct.Count),
			updated,
			inserted
		);
	}

	private async Task<(int inserted, int updated)> UpsertRisIds(
		Station station,
		DateTime date,
		List<IdentifiedRisId> risIds,
		NavigatorDbContext dbContext,
		CancellationToken cancellationToken
	)
	{
		station.LastQueried = date;

		if (!risIds.Any())
		{
			await dbContext.SaveChangesAsync(cancellationToken);
			return (0, 0);
		}

		// existing RIS IDs from the database
		var existingRisIds = await dbContext
			.RisIds.Where(risId => risIds.Select(gatheredRisId => gatheredRisId.Id).Contains(risId.Id))
			.ToDictionaryAsync(risId => risId.Id, cancellationToken);

		// separate new & existing
		var newRisIds = risIds.Where(risId => !existingRisIds.ContainsKey(risId.Id)).ToList();
		var existingToUpdate = risIds.Where(risId => existingRisIds.ContainsKey(risId.Id)).ToList();

		if (newRisIds.Any())
			dbContext.RisIds.AddRange(newRisIds);

		foreach (var risId in existingToUpdate)
		{
			var dbEntity = existingRisIds[risId.Id];
			dbEntity.Active = true;
		}

		await dbContext.SaveChangesAsync(cancellationToken);
		return (newRisIds.Count, existingToUpdate.Count);
	}

	private async Task<List<IdentifiedRisId>> CallApi(int evaNumber, DateTime timeStart, bool isDeparture = true)
	{
		var boardType = isDeparture ? "departure" : "arrival";
		var timeEnd = timeStart.AddMinutes(720);

		// random proxy
		var httpClient = _proxyRotator.GetRandomProxy();
		var request = await httpClient.GetAsync(
			string.Format(
				_apiUrl,
				boardType,
				evaNumber,
				Uri.EscapeDataString(timeStart.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")),
				Uri.EscapeDataString(timeEnd.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"))
			)
		);
		if (!request.IsSuccessStatusCode)
			return new();

		await using var stream = await request.Content.ReadAsStreamAsync();
		var content = (await JsonDocument.ParseAsync(stream)).RootElement;

		var items = content.GetProperty("items");
		if (items.ValueKind != JsonValueKind.Array)
			throw new InvalidOperationException("Expected 'items' to be an array");
		if (items.GetArrayLength() == 0)
			return new();

		return items
			.EnumerateArray()
			.Select(item =>
			{
				if (!item.TryGetProperty("train", out JsonElement trainElement))
					throw new InvalidOperationException("Expected 'train' property in item");
				return new IdentifiedRisId()
				{
					Id = TryParse(trainElement.GetProperty("journeyId")),
					Product =
						Product.MapProduct(trainElement.GetProperty("type").GetString())
						?? throw new InvalidOperationException("Expected 'product' to be a non-null string"),
					DiscoveryDate = DateTime.UtcNow,
					Active = true,
				};
			})
			.ToList();
	}

	private string TryParse(JsonElement jsonElement)
	{
		if (jsonElement.ValueKind != JsonValueKind.String)
			throw new InvalidOperationException("Expected JSON element to be a string");

		string fullTripId =
			jsonElement.GetString() ?? throw new InvalidOperationException("Expected JSON element to be a non-null string");

		// Format: yyyyMMdd-{UUID}[-UUID]
		// {} - everything inside the curly brackets is necessary
		// [] - everything inside the square brackets is optional
		string datePart = fullTripId.Substring(0, 8);
		if (!DateTime.TryParseExact(datePart, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out _))
			throw new FormatException("Input does not start with a valid date");

		return fullTripId.Substring(9);
	}
}
