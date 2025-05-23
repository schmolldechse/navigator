using Daemon.Models;
using Daemon.Models.Station;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace Daemon.Manager.Impl;

public class MonitorStations : Daemon
{
    private readonly HttpClient _httpClient;
    private readonly string _apiUrl = "https://vendo-prof-db.voldechse.wtf/stops/{0}/arrivals?when={1}&duration={2}";

    public MonitorStations(ILogger<MonitorStations> logger) : base("MonitorStations", TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(30), logger)
    {
        _httpClient = new HttpClient();
    }

    protected override async Task ExecuteCoreAsync(CancellationToken cancellationToken)
    {
        var date = DateTime.Now;

        var collection = await MongoDriver.GetCollectionAsync<StationDocument>("stations");

        var filter = Builders<StationDocument>.Filter.And(
            Builders<StationDocument>.Filter.Eq(x => x.QueryingEnabled, true), // queryingEnabled is true
            Builders<StationDocument>.Filter
                .Or( // lastQueried is null or lastQueried is older than current date at Midnight
                    Builders<StationDocument>.Filter.Exists(x => x.LastQueried, false),
                    Builders<StationDocument>.Filter.Lt(x => x.LastQueried, date.Date.AddDays(-1)))
        );

        var station = await collection
            .Aggregate()
            .Match(filter)
            .Sample(1)
            .FirstOrDefaultAsync(cancellationToken);
        if (station == null) return;

        if (station.LastQueried == null)
        {
            // RIS saves it up to 7 days retroactively
            var start = date.AddDays(-7);
            _logger.LogInformation(
                $"{station.Name} had no saved result's yet! Set 'LastQueried' for station {station.Name}: {start} (7 days ago)");

            await ProcessStation(station, start, cancellationToken);
        }
        else if (station.LastQueried.Value.Date < date.Date)
        {
            var newLastQueried = new DateTime(
                DateOnly.FromDateTime(station.LastQueried.Value.Date.AddDays(1)),
                TimeOnly.FromTimeSpan(date.TimeOfDay)
            );
            await ProcessStation(station, newLastQueried, cancellationToken);
        }
    }

    private async Task ProcessStation(Station station, DateTime date, CancellationToken cancellationToken)
    {
        var update = Builders<StationDocument>.Update.Set(x => x.LastQueried, date);

        var collection = await MongoDriver.GetCollectionAsync<StationDocument>("stations");
        await collection.UpdateOneAsync(
            Builders<StationDocument>.Filter.Eq(x => x.EvaNumber, station.EvaNumber),
            update,
            cancellationToken: cancellationToken
        );

        int upsertCount = Task.WhenAll(
            CallApi(station.EvaNumber, date.Date, 720),
            CallApi(station.EvaNumber, date.Date.AddHours(12), 720)
        ).GetAwaiter().GetResult().Sum();
        _logger.LogInformation(
            $"Queried {upsertCount} new RIS id's & incremented 'LastQueried' for station {station.Name}: {date}");
    }

    private async Task<int> CallApi(int evaNumber, DateTime when, int duration)
    {
        string formattedDateTime = when.ToString("yyyy-MM-ddTHH:mm:sszzz");
        var response = await _httpClient.GetAsync(string.Format(_apiUrl, evaNumber,
            Uri.EscapeDataString(formattedDateTime),
            duration));
        response.EnsureSuccessStatusCode();

        var content = JObject.Parse(await response.Content.ReadAsStringAsync());
        List<IdentifiedRisId> journeys = content["arrivals"]!.Select(IdentifiedRisId.FromJson).ToList();
        if (!journeys.Any()) return 0;

        var collection = await MongoDriver.GetCollectionAsync<IdentifiedRisId>("ris-ids");
        var bulkOps = journeys.Select(journey => new UpdateOneModel<IdentifiedRisId>(
                Builders<IdentifiedRisId>.Filter.Eq(j => j.RisId, journey.RisId),
                Builders<IdentifiedRisId>.Update
                    .SetOnInsert(j => j.RisId, journey.RisId)
                    .SetOnInsert(j => j.DiscoveredAt, journey.DiscoveredAt))
            { IsUpsert = true }).ToList<WriteModel<IdentifiedRisId>>();

        var result = await collection.BulkWriteAsync(bulkOps);
        return result.Upserts.Count;
    }

    public override void Dispose()
    {
        _httpClient?.Dispose();
        base.Dispose();
    }
}