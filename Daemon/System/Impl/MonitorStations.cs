using System.Globalization;
using Daemon.Models;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace Daemon.System.Impl;

public class MonitorStations : Daemon
{
    private readonly HttpClient _httpClient;
    private readonly string _apiUrl = "https://vendo-prof-db.voldechse.wtf/stops/{0}/arrivals?when={1}&duration={2}";

    public MonitorStations() : base("MonitorStations", TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30))
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

            await Task.WhenAll(
                CallApi(station.EvaNumber, start.Date, 720),
                CallApi(station.EvaNumber, start.Date.AddHours(12), 720)
            );

            Console.WriteLine($"Updated 'LastQueried' for station {station.Name} to 7 days ago: {start}");
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

            await Task.WhenAll(
                CallApi(station.EvaNumber, newLastQueried.Date, 720),
                CallApi(station.EvaNumber, newLastQueried.Date.AddHours(12), 720)
            );

            Console.WriteLine($"Incremented 'LastQueried' for station {station.Name} to {newLastQueried}");
        }
    }

    private async Task CallApi(int evaNumber, DateTime when, int duration)
    {
        string formattedDateTime = when.ToString("yyyy-MM-ddTHH:mm:sszzz");
        var response = await _httpClient.GetAsync(string.Format(_apiUrl, evaNumber,
            Uri.EscapeDataString(formattedDateTime),
            duration));
        response.EnsureSuccessStatusCode();

        var content = JObject.Parse(await response.Content.ReadAsStringAsync());
        List<JourneyDocument> journeys = content["arrivals"]!
            .Select(arrival =>
            {
                string fullTripId = arrival["tripId"]!.ToString();
                string datePart = fullTripId.Substring(0, 8);

                if (DateTime.TryParseExact(datePart, "yyyyMMdd", null, DateTimeStyles.None, out DateTime _))
                {
                    fullTripId = fullTripId.Substring(9);
                }

                return new JourneyDocument() { risId = fullTripId, FirstQueried = DateTime.Now.ToLocalTime() };
            })
            .ToList();
        if (!journeys.Any()) return;

        var collection = await MongoDriver.GetCollectionAsync<JourneyDocument>("ris-rids");
        var bulkOps = journeys.Select(journey => new UpdateOneModel<JourneyDocument>(
                Builders<JourneyDocument>.Filter.Eq(j => j.risId, journey.risId),
                Builders<JourneyDocument>.Update.SetOnInsert(j => j.risId, journey.risId)
                    .SetOnInsert(j => j.FirstQueried, journey.FirstQueried))
            { IsUpsert = true }).ToList<WriteModel<JourneyDocument>>();
        await collection.BulkWriteAsync(bulkOps);
    }

    public override void Dispose()
    {
        _httpClient?.Dispose();
        base.Dispose();
    }
}