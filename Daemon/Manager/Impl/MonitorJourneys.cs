using Daemon.Models;
using Daemon.Models.Journey;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace Daemon.Manager.Impl;

public class MonitorJourneys : Daemon
{
    private readonly HttpClient _httpClient;
    private readonly string _apiUrl = "https://vendo-prof-db.voldechse.wtf/trips/{0}?stopovers=true";

    // timetable change
    private readonly DateTime[] _timetableChanges =
    {
        new(2024, 12, 17, 0, 0, 0),
        new(2025, 6, 9, 0, 0, 0)
    };

    public MonitorJourneys(ILogger<MonitorJourneys> logger) : base("MonitorJourneys", TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(5), logger)
    {
        _httpClient = new HttpClient();
    }
    
    private DateTime GetLastTimetableChange(DateTime? compareTo = null)
    {
        compareTo ??= DateTime.Now;
        return _timetableChanges.Where(change => change <= compareTo)
            .DefaultIfEmpty(_timetableChanges.Min())
            .Max();
    }

    protected override async Task ExecuteCoreAsync(CancellationToken cancellationToken)
    {
        var date = DateTime.Now;

        var risCollection = await MongoDriver.GetCollectionAsync<IdentifiedRisId>("ris-ids");
        var lastSuccessfulQueriedFilter = Builders<IdentifiedRisId>.Filter.Or(
            Builders<IdentifiedRisId>.Filter.Exists(x => x.LastSuccessfulQueried, false), // lastSuccessfulQueried is null
            Builders<IdentifiedRisId>.Filter.Lt(x => x.LastSuccessfulQueried, date.Date.AddDays(-1)) // lastSuccessfulQueried is older than current date at Midnight
        );

        var risDocument = await risCollection
            .Aggregate()
            .Match(lastSuccessfulQueriedFilter)
            .Sample(1)
            .FirstOrDefaultAsync(cancellationToken);
        if (risDocument == null) return;

        if (risDocument.LastSuccessfulQueried == null)
        {
            date = new DateTime(
                DateOnly.FromDateTime(GetLastTimetableChange()),
                TimeOnly.FromTimeSpan(date.TimeOfDay)
            );
            await ProcessTrip(risDocument, date, cancellationToken);
        }
        // do not fetch connections for the same day, as the connection may not reach their destination
        else if (risDocument.LastSuccessfulQueried.Value.Date < date.Date)
        {
            date = new DateTime(
                DateOnly.FromDateTime(risDocument.LastSuccessfulQueried!.Value.Date.AddDays(1)),
                TimeOnly.FromTimeSpan(date.TimeOfDay)
            );
            await ProcessTrip(risDocument, date, cancellationToken);
        }
    }

    private async Task ProcessTrip(
        IdentifiedRisId risDocument,
        DateTime date,
        CancellationToken cancellationToken)
    {
        TripResult result = await CallApi(risDocument.RisId, date, cancellationToken);
        if (result.ParsingError) return;

        var collection = await MongoDriver.GetCollectionAsync<IdentifiedRisId>("ris-ids");
        
        // first update lastQueried
        await collection.FindOneAndUpdateAsync(
            Builders<IdentifiedRisId>.Filter.Eq(x => x.RisId, risDocument.RisId),
            Builders<IdentifiedRisId>.Update.Set(x => x.LastSuccessfulQueried, date),
            cancellationToken: cancellationToken
        );
        
        // then insert the Trip
        if (result.Trip != null) await StoreTrip(result.Trip, cancellationToken);
    }

    private async Task<TripResult> CallApi(string risId, DateTime when, CancellationToken cancellationToken)
    {
        string formattedId = when.ToString("yyyyMMdd") + "-" + risId;

        _logger.LogDebug($"Calling Trip API for: {formattedId}");
        var response = await _httpClient.GetAsync(string.Format(_apiUrl, formattedId), cancellationToken);

        // 500 code is thrown if trip does not exist
        if (!response.IsSuccessStatusCode)
            return new TripResult()
            {
                Trip = null,
                ParsingError = false
            };

        var content = JObject.Parse(await response.Content.ReadAsStringAsync(cancellationToken));
        if (content["trip"] == null)
        {
            _logger.LogDebug($"No response from Trip API for: {formattedId}");
            return new TripResult()
            {
                Trip = null,
                ParsingError = true
            };
        }

        return new TripResult()
        {
            Trip = Trip.FromJson(content["trip"]!),
            ParsingError = false,
        };
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

class TripResult
{
    public Trip? Trip { get; set; }
    public bool ParsingError { get; set; } = false;
}