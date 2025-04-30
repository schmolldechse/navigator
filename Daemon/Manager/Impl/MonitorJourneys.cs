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
    private readonly DateTime _startTime = new(2024, 12, 17, 0, 0, 0);

    public MonitorJourneys(ILogger<MonitorJourneys> logger) : base("MonitorJourneys", TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(5), logger)
    {
        _httpClient = new HttpClient();
    }

    protected override async Task ExecuteCoreAsync(CancellationToken cancellationToken)
    {
        var date = DateTime.Now;

        var risCollection = await MongoDriver.GetCollectionAsync<IdentifiedRisId>("ris-ids");
        var lastSuccessfulQueriedFilter = Builders<IdentifiedRisId>.Filter.Or(
            Builders<IdentifiedRisId>.Filter.Exists(x => x.LastSuccessfulQueried, false),
            Builders<IdentifiedRisId>.Filter.Lt(x => x.LastSuccessfulQueried, date.Date.AddDays(-1))
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
                DateOnly.FromDateTime(_startTime),
                TimeOnly.FromTimeSpan(date.TimeOfDay)
            );
            await ProcessTrip(risDocument, date, risCollection, cancellationToken);
        }
        // do not fetch connections for the same day, as the connection may not reach their destination
        else if (risDocument.LastSuccessfulQueried.Value.Date < date.Date)
        {
            date = new DateTime(
                DateOnly.FromDateTime(risDocument.LastSuccessfulQueried!.Value.Date.AddDays(1)),
                TimeOnly.FromTimeSpan(date.TimeOfDay)
            );
            await ProcessTrip(risDocument, date, risCollection, cancellationToken);
        }
    }

    private async Task ProcessTrip(
        IdentifiedRisId risDocument,
        DateTime date,
        IMongoCollection<IdentifiedRisId> risCollection,
        CancellationToken cancellationToken)
    {
        TripResult result = await CallApi(risDocument.risId, date, cancellationToken);
        if (result.ParsingError) return;

        await risCollection.FindOneAndUpdateAsync(
            Builders<IdentifiedRisId>.Filter.Eq(x => x.RisId, risDocument.RisId),
            Builders<IdentifiedRisId>.Update.Set(x => x.LastSuccessfulQueried, date),
            cancellationToken: cancellationToken
        );
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