using Daemon.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Daemon.System.Impl;

public class MonitorStations : Daemon
{
    public MonitorStations() : base("MonitorStations", TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30))
    {
    }
    
    protected override async Task ExecuteCoreAsync(CancellationToken cancellationToken)
    {
        var collection = await MongoDriver.GetCollectionAsync<StationDocument>("stations");
        var filter = Builders<StationDocument>.Filter.Eq(x => x.QueryingEnabled, true);
        var station = await collection
            .Aggregate()
            .Match(filter)
            .Sample(1)
            .FirstAsync();

        Console.WriteLine("Monitoring station: " + station.ToJson());
    }
}