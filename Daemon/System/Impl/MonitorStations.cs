using System.Globalization;
using Daemon.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace Daemon.System.Impl;

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

        var queryingEnabledFilter = Builders<StationDocument>.Filter.Eq(x => x.QueryingEnabled, true);
        var lastQueriedFilter = Builders<StationDocument>.Filter.Or(
            Builders<StationDocument>.Filter.Exists(x => x.LastQueried, false),
            Builders<StationDocument>.Filter.Lt(x => x.LastQueried, date)
        );

        var station = await collection
            .Aggregate()
            .Match(Builders<StationDocument>.Filter.And(queryingEnabledFilter, lastQueriedFilter))
            .Sample(1)
            .FirstOrDefaultAsync(cancellationToken);
        if (station == null) return;

        if (station.LastQueried == null)
        {
            // RIS saves it up to 7 days retroactively
            var start = date.AddDays(-7);
            var update = Builders<StationDocument>.Update.Set(x => x.LastQueried, start);

            await collection.UpdateOneAsync(
                Builders<StationDocument>.Filter.Eq(x => x.EvaNumber, station.EvaNumber),
                update,
                cancellationToken: cancellationToken
            );

            int upsertCount = Task.WhenAll(
                CallApi(station.EvaNumber, start.Date, 720),
                CallApi(station.EvaNumber, start.Date.AddHours(12), 720)
            ).GetAwaiter().GetResult().Sum();

            _logger.LogInformation($"{station.Name} had no saved result's yet! Set 'LastQueried' for station {station.Name}: {start} (7 days ago)");
            _logger.LogInformation($"Queried {upsertCount} new RIS id's");
        }
        else if (station.LastQueried.Value.Date < date.Date)
        {
            var newLastQueried = new DateTime(
                DateOnly.FromDateTime(station.LastQueried.Value.Date.AddDays(1)),
                TimeOnly.FromTimeSpan(date.TimeOfDay)
            );

            var update = Builders<StationDocument>.Update.Set(x => x.LastQueried, newLastQueried);

            await collection.UpdateOneAsync(
                Builders<StationDocument>.Filter.Eq(x => x.EvaNumber, station.EvaNumber),
                update,
                cancellationToken: cancellationToken
            );

            int upsertCount = Task.WhenAll(
                CallApi(station.EvaNumber, newLastQueried.Date, 720),
                CallApi(station.EvaNumber, newLastQueried.Date.AddHours(12), 720)
            ).GetAwaiter().GetResult().Sum();

            _logger.LogInformation($"Queried {upsertCount} new RIS id's & incremented 'LastQueried' for station {station.Name}: {newLastQueried}");
        }
    }

    private async Task<int> CallApi(int evaNumber, DateTime when, int duration)
    {
        string formattedDateTime = when.ToString("yyyy-MM-ddTHH:mm:sszzz");
        var response = await _httpClient.GetAsync(string.Format(_apiUrl, evaNumber,
            Uri.EscapeDataString(formattedDateTime),
            duration));
        response.EnsureSuccessStatusCode();

        var content = JObject.Parse(await response.Content.ReadAsStringAsync());
        List<RisDocument> journeys = content["arrivals"]!
            .Select(arrival =>
            {
                string fullTripId = arrival["tripId"]!.ToString();
                string datePart = fullTripId.Substring(0, 8);

                if (DateTime.TryParseExact(datePart, "yyyyMMdd", null, DateTimeStyles.None, out DateTime _))
                {
                    fullTripId = fullTripId.Substring(9);
                }

                DateTime? discoveredAt = null;
                if (arrival["plannedWhen"] != null)
                {
                    discoveredAt = new DateTime(
                        DateOnly.FromDateTime(DateTime.Parse(arrival["plannedWhen"]!.ToString()).Date),
                        TimeOnly.FromTimeSpan(DateTime.Now.Date.TimeOfDay)
                    );
                }

                return new RisDocument() { risId = fullTripId, DiscoveredAt = discoveredAt };
            })
            .ToList();
        if (!journeys.Any()) return 0;

        var collection = await MongoDriver.GetCollectionAsync<RisDocument>("ris-ids");
        var bulkOps = journeys.Select(journey => new UpdateOneModel<RisDocument>(
                Builders<RisDocument>.Filter.Eq(j => j.risId, journey.risId),
                Builders<RisDocument>.Update.SetOnInsert(j => j.risId, journey.risId)
                    .SetOnInsert(j => j.DiscoveredAt, journey.DiscoveredAt))
            { IsUpsert = true }).ToList<WriteModel<RisDocument>>();

        var result = await collection.BulkWriteAsync(bulkOps);
        return result.Upserts.Count;
    }

    public override void Dispose()
    {
        _httpClient?.Dispose();
        base.Dispose();
    }
}