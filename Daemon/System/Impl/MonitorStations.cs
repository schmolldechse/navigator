using Daemon.Models;
using MongoDB.Driver;

namespace Daemon.System.Impl;

public class MonitorStations : Daemon
{
    public MonitorStations() : base("MonitorStations", TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30))
    {
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

            Console.WriteLine($"Incremented 'LastQueried' for station {station.Name} to {newLastQueried}");
        }
    }
}