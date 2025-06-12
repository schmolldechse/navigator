using System.Data;
using System.Globalization;
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

    public GatheringJourneyDaemon(ILogger<GatheringJourneyDaemon> logger, IServiceProvider serviceProvider,
        ProxyRotator proxyRotator)
        : base("Gather Journey", TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(300), logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null");
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _proxyRotator = proxyRotator ?? throw new ArgumentNullException(nameof(proxyRotator));
    }

    // timetable changes
    private readonly DateTime[] _timetableChanges =
    {
        new(2024, 12, 17, 0, 0, 0, DateTimeKind.Utc),
        new(2025, 6, 15, 0, 0, 0, DateTimeKind.Utc)
    };

    private DateTime GetLastTimetableChange(DateTime? compareTo = null)
    {
        compareTo ??= DateTime.UtcNow;
        return _timetableChanges
            .Where(change => change <= compareTo)
            .DefaultIfEmpty(_timetableChanges.Min())
            .Max();
    }

    protected override async Task ExecuteCoreAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<NavigatorDbContext>();

        var date = DateTime.UtcNow;

        IdentifiedRisId? randomRisId = null;
        await using (var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken))
        {
            var risIds = await dbContext.RisIds
                .Where(risId => !risId.IsLocked)
                .Where(risId => risId.Active)
                .Where(risId => risId.LastSeen == null || risId.LastSeen < date.Date.AddDays(-1))
                .OrderBy(risId => risId.LastSeen ?? DateTime.MinValue)
                .Take(100)
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
            if (randomRisId.LastSeen == null)
            {
                date = new DateTime(
                    DateOnly.FromDateTime(GetLastTimetableChange()),
                    TimeOnly.FromTimeSpan(date.TimeOfDay),
                    DateTimeKind.Utc
                );
                await ProcessJourney(randomRisId, date, dbContext, cancellationToken);
            }
            // do not fetch journeys for the same day, as the journey may not reach their destination yet
            else if (randomRisId.LastSeen.Value.Date < date.Date)
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
            await using var unlockTransaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
            randomRisId.IsLocked = false;
            await dbContext.SaveChangesAsync(cancellationToken);
            await unlockTransaction.CommitAsync(cancellationToken);
        }
    }

    private async Task ProcessJourney(IdentifiedRisId risId, DateTime date, NavigatorDbContext dbContext,
        CancellationToken cancellationToken)
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
            if (!exists) dbContext.Journeys.Add(journeyResponse.Journey);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<JourneyResponse> CallApi(string risId, DateTime when, CancellationToken cancellationToken)
    {
        string formattedId = when.ToString("yyyyMMdd") + "-" + risId;
        _logger.LogDebug("Gathering journey for RIS id {0}", formattedId);

        // random proxy
        using var httpClient = _proxyRotator.GetRandomProxy();
        var request = await httpClient.GetAsync(string.Format(_apiUrl, formattedId), cancellationToken);

        // 500 code is thrown if the journey does not exist
        if (!request.IsSuccessStatusCode)
            return new()
            {
                Journey = null,
                ParsingError = false
            };

        await using var stream = await request.Content.ReadAsStreamAsync(cancellationToken);
        var content = (await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken)).RootElement;

        if (content.GetProperty("journeyId").ValueKind == JsonValueKind.Null)
            return new()
            {
                Journey = null,
                ParsingError = false
            };

        var journey = new Journey()
        {
            Id = content.GetProperty("journeyId").GetString() ??
                 throw new ArgumentNullException("Journey ID cannot be null"),
            Operator = new Operator()
            {
                Code = content.GetProperty("operatorCode").GetString() ?? string.Empty,
                Name = content.GetProperty("operatorName").GetString() ?? string.Empty
            },
            LineInformation = new LineInformation()
            {
                ProductType = Product.MapProduct(content.GetProperty("type").GetString()),
                ProductName = content.GetProperty("category").GetString() ?? string.Empty,
                JourneyName = content.GetProperty("name").GetString() ?? string.Empty,
                JourneyNumber = content.GetProperty("no").GetInt32().ToString() ?? string.Empty
            },
            Messages = content.GetProperty("hims").EnumerateArray().Select(message => new JourneyMessage()
            {
                Code = int.Parse(
                    message.TryGetProperty("code", out var codeProp) && codeProp.ValueKind != JsonValueKind.Null &&
                    codeProp.GetString() != null ? codeProp.GetString() :
                    message.TryGetProperty("id", out var idProp) && idProp.ValueKind != JsonValueKind.Null &&
                    idProp.GetString() != null ? idProp.GetString() :
                    throw new ArgumentNullException("Message code or id cannot be null")
                ),
                Message = message.GetProperty("caption").GetString() ?? string.Empty,
                Summary = message.GetProperty("shortText").GetString() ?? string.Empty
            }).ToList(),
            ViaStops = content.GetProperty("stops").EnumerateArray().Select(stop =>
            {
                var cancelled = stop.GetProperty("status").GetString() == "Canceled" ||
                                (stop.TryGetProperty("canceled", out var canceledProp) && canceledProp.GetBoolean());
                var stopObj = new Stop()
                {
                    Cancelled = cancelled,
                    EvaNumber = int.Parse(stop.GetProperty("station").GetProperty("evaNo").GetString() ??
                                          throw new ArgumentNullException("evaNo cannot be null")),
                    Name = stop.GetProperty("station").GetProperty("name").GetString() ??
                           throw new ArgumentNullException("Station name cannot be null"),
                    Messages = stop.GetProperty("messages").EnumerateArray().Select(message => new StopMessage()
                    {
                        Code = int.Parse(message.GetProperty("code").GetString() ??
                                         throw new ArgumentNullException("Message code cannot be null")),
                        Message = message.GetProperty("text").GetString() ?? string.Empty,
                        Summary = message.TryGetProperty("textShort", out var textShortProp) &&
                                  textShortProp.ValueKind != JsonValueKind.Null &&
                                  textShortProp.GetString() != null
                            ? textShortProp.GetString()!
                            : message.GetProperty("text").GetString()!
                    }).ToList()
                };

                if (stop.TryGetProperty("arrivalTime", out var arrivalTimeProp) &&
                    arrivalTimeProp.ValueKind != JsonValueKind.Null)
                {
                    var actualTime = DateTime.Parse(
                        arrivalTimeProp.GetProperty("predicted").GetString() ?? string.Empty,
                        null,
                        DateTimeStyles.AdjustToUniversal
                    );
                    var plannedTime = DateTime.Parse(
                        arrivalTimeProp.GetProperty("target").GetString() ?? string.Empty,
                        null,
                        DateTimeStyles.AdjustToUniversal
                    );
                    var arrivalTime = new Time()
                    {
                        ActualTime = actualTime,
                        PlannedTime = plannedTime,
                        Delay = (int)(actualTime - plannedTime).TotalSeconds,
                        ActualPlatform =
                            stop.GetProperty("track").GetProperty("prediction").GetString() ?? string.Empty,
                        PlannedPlatform = stop.GetProperty("track").GetProperty("target").GetString() ?? string.Empty
                    };
                    stopObj.Arrival = arrivalTime;
                }

                if (stop.TryGetProperty("departureTime", out var departureTimeProp) &&
                    departureTimeProp.ValueKind != JsonValueKind.Null)
                {
                    var actualTime = DateTime.Parse(
                        departureTimeProp.GetProperty("predicted").GetString() ?? string.Empty,
                        null,
                        DateTimeStyles.AdjustToUniversal
                    );
                    var plannedTime = DateTime.Parse(
                        departureTimeProp.GetProperty("target").GetString() ?? string.Empty,
                        null,
                        DateTimeStyles.AdjustToUniversal
                    );
                    var departureTime = new Time()
                    {
                        ActualTime = actualTime,
                        PlannedTime = plannedTime,
                        Delay = (int)(actualTime - plannedTime).TotalSeconds,
                        ActualPlatform =
                            stop.GetProperty("track").GetProperty("prediction").GetString() ?? string.Empty,
                        PlannedPlatform = stop.GetProperty("track").GetProperty("target").GetString() ?? string.Empty
                    };
                    stopObj.Departure = departureTime;
                }

                return stopObj;
            }).ToList()
        };

        return new JourneyResponse()
        {
            Journey = journey,
            ParsingError = false
        };
    }
}

class JourneyResponse
{
    public Journey? Journey { get; set; }
    public bool ParsingError { get; set; } = false;
}