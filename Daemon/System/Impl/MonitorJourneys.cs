using Daemon.Models;
using Daemon.Models.Mapper;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace Daemon.System.Impl;

public class MonitorJourneys : Daemon
{
    private readonly HttpClient _httpClient;
    private readonly string _apiUrl = "https://vendo-prof-db.voldechse.wtf/trips/{0}?stopovers=true";

    // timetable change
    private readonly DateTime _startTime = new(2024, 12, 17, 0, 0, 0);

    public MonitorJourneys(ILogger<MonitorJourneys> logger) : base("MonitorJourneys", TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(5), logger)
    {
        _httpClient = new HttpClient();
    }

    protected override async Task ExecuteCoreAsync(CancellationToken cancellationToken)
    {
        var date = DateTime.Now;

        var risCollection = await MongoDriver.GetCollectionAsync<RisDocument>("ris-ids");
        var lastSuccessfulQueriedFilter = Builders<RisDocument>.Filter.Or(
            Builders<RisDocument>.Filter.Exists(x => x.LastSuccessfulQueried, false),
            Builders<RisDocument>.Filter.Lt(x => x.LastSuccessfulQueried, date.Date.AddDays(-1))
        );

        var risDocument = await risCollection
            .Aggregate()
            .Match(lastSuccessfulQueriedFilter)
            .Sample(1)
            .FirstOrDefaultAsync(cancellationToken);
        if (risDocument == null) return;

        if (risDocument.LastSuccessfulQueried == null)
        {
            await ProcessNewDocument(risDocument, date, risCollection, cancellationToken);
        }
        // do not fetch connections for the same day, as the connection may not reach their destination
        else if (risDocument.LastSuccessfulQueried.Value.Date < date.Date) 
        {
            await ProcessExistingDocument(risDocument, date, risCollection, cancellationToken);
        }
    }

    private async Task ProcessNewDocument(
        RisDocument risDocument,
        DateTime date,
        IMongoCollection<RisDocument> risCollection,
        CancellationToken cancellationToken)
    {
        var newLastQueried = new DateTime(
            DateOnly.FromDateTime(_startTime),
            TimeOnly.FromTimeSpan(date.TimeOfDay)
        );
        await risCollection.FindOneAndUpdateAsync(
            Builders<RisDocument>.Filter.Eq(x => x.risId, risDocument.risId),
            Builders<RisDocument>.Update.Set(x => x.LastSuccessfulQueried, newLastQueried),
            cancellationToken: cancellationToken
        );
        
        Trip trip = await CallApi(risDocument.risId, _startTime, cancellationToken);
        if (trip == null) return;

        await StoreTrip(trip, cancellationToken);
    }

    private async Task ProcessExistingDocument(
        RisDocument risDocument,
        DateTime date,
        IMongoCollection<RisDocument> risCollection,
        CancellationToken cancellationToken)
    {
        var newLastQueried = new DateTime(
            DateOnly.FromDateTime(risDocument.LastSuccessfulQueried.Value.Date.AddDays(1)),
            TimeOnly.FromTimeSpan(date.TimeOfDay)
        );
        await risCollection.FindOneAndUpdateAsync(
            Builders<RisDocument>.Filter.Eq(x => x.risId, risDocument.risId),
            Builders<RisDocument>.Update.Set(x => x.LastSuccessfulQueried, newLastQueried),
            cancellationToken: cancellationToken
        );
        
        Trip trip = await CallApi(risDocument.risId, newLastQueried, cancellationToken);
        if (trip == null) return;
        
        await StoreTrip(trip, cancellationToken);
    }

    private async Task<Trip> CallApi(string risId, DateTime when, CancellationToken cancellationToken)
    {
        string formattedId = when.ToString("yyyyMMdd") + "-" + risId;
        
        _logger.LogDebug($"Calling Trip API for: {formattedId}");
        var response = await _httpClient.GetAsync(string.Format(_apiUrl, formattedId), cancellationToken);
        if (!response.IsSuccessStatusCode) return null;

        var content = JObject.Parse(await response.Content.ReadAsStringAsync(cancellationToken));
        if (content["trip"] == null)
        {
            _logger.LogDebug($"No response from Trip API for: {formattedId}");
            return null;
        }

        var trip = new Trip()
        {
            RIS_JourneyId = content["trip"]["id"].ToString(),
            Origin = new Stop()
            {
                EvaNumber = content["trip"]["origin"]["id"].ToObject<int>(),
                Name = content["trip"]["origin"]["name"].ToString(),
                Cancelled = content["trip"]["origin"]["cancelled"]?.ToObject<bool>() ?? false,
            },
            Destination = new Stop()
            {
                EvaNumber = content["trip"]["destination"]["id"].ToObject<int>(),
                Name = content["trip"]["destination"]["name"].ToString(),
                Cancelled = content["trip"]["destination"]["cancelled"]?.ToObject<bool>() ?? false,
            },
            LineInformation = new LineInformation()
            {
                Type = TransportProductMapper.GetTransportProduct(content["trip"]["line"]["product"].ToString()),
                Product = content["trip"]["line"]["productName"].ToString(),
                LineName = content["trip"]["line"]["name"].ToString(),
                FahrtNr = content["trip"]["line"]["fahrtNr"].ToString(),
                Operator = new Operator()
                {
                    Id = content["trip"]["line"]["operator"]["id"].ToString(),
                    Name = content["trip"]["line"]["operator"]["name"].ToString(),
                }
            },
            ViaStops = content["trip"]["stopovers"].Select(stop =>
                {
                    return new Stop()
                    {
                        Name = stop["stop"]["name"].ToString(),
                        EvaNumber = stop["stop"]["id"].ToObject<int>(),
                        Arrival = new Time()
                        {
                            PlannedTime = !string.IsNullOrEmpty(stop["plannedArrival"]?.ToString())
                                ? DateTime.Parse(stop["plannedArrival"].ToString()).ToLocalTime()
                                : null,
                            ActualTime = !string.IsNullOrEmpty(stop["arrival"]?.ToString())
                                ? DateTime.Parse(stop["arrival"].ToString()).ToLocalTime()
                                : null,
                            Delay = stop["arrivalDelay"]?.ToObject<int?>() ?? 0,
                            PlannedPlatform = stop["plannedArrivalPlatform"]?.ToString() ?? null,
                            ActualPlatform = stop["arrivalPlatform"]?.ToString() ?? null,
                        },
                        Departure = new Time()
                        {
                            PlannedTime = !string.IsNullOrEmpty(stop["plannedDeparture"]?.ToString())
                                ? DateTime.Parse(stop["plannedDeparture"].ToString()).ToLocalTime()
                                : null,
                            ActualTime = !string.IsNullOrEmpty(stop["departure"]?.ToString())
                                ? DateTime.Parse(stop["departure"].ToString()).ToLocalTime()
                                : null,
                            Delay = stop["departureDelay"]?.ToObject<int?>() ?? 0,
                            PlannedPlatform = stop["plannedDeparturePlatform"]?.ToString() ?? null,
                            ActualPlatform = stop["departurePlatform"]?.ToString() ?? null,
                        },
                        Cancelled = stop["cancelled"]?.ToObject<bool>() ?? false,
                        Messages = stop["remarks"] != null
                            ? stop["remarks"]!.Select(remark => new Message()
                            {
                                Code = remark["code"].ToObject<int>(),
                                Summary = remark["summary"].ToString(),
                                Text = remark["text"].ToString(),
                            }).ToList()
                            : null,
                    };
                })
                .ToList(),
            Messages = content["trip"]["remarks"] != null
                ? content["trip"]["remarks"]!.Select(remark => new Message()
                {
                    Code = remark["code"].ToObject<int>(),
                    Summary = remark["summary"].ToString(),
                    Text = remark["text"].ToString(),
                }).ToList()
                : null
        };
        return trip;
    }

    private async Task StoreTrip(Trip trip, CancellationToken cancellationToken)
    {
        var tripsCollection = await MongoDriver.GetCollectionAsync<Trip>("trips");
        await tripsCollection.InsertOneAsync(trip, cancellationToken: cancellationToken);
    }

    public override void Dispose()
    {
        _httpClient?.Dispose();
        base.Dispose();
    }
}