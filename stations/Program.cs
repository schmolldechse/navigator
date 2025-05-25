using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using stations.Api;
using stations.Database;
using stations.Models;

namespace stations;

class Program
{
    static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        services.AddSingleton<AppConfiguration>(_ => new AppConfiguration()
        {
            ClientId = Environment.GetEnvironmentVariable("DB_CLIENT_ID") ??
                       throw new ArgumentNullException("DB_CLIENT_ID", "DeutscheBahn ClientId not configured"),
            ClientSecret = Environment.GetEnvironmentVariable("DB_CLIENT_SECRET") ??
                           throw new ArgumentNullException("DB_CLIENT_SECRET",
                               "DeutscheBahn ClientSecret not configured"),
        });

        services.AddHttpClient();
        services.AddSingleton<ApiService>();
        services.AddSingleton<StationDiscovery>();
        services.AddSingleton<StationMergeService>();
        services.AddDbContext<StationDbContext>();

        var provider = services.BuildServiceProvider();
        var logger = provider.GetService<ILogger<Program>>();

        bool skipDiscovery = args.Any(arg => arg.Equals("--skipDiscovery", StringComparison.OrdinalIgnoreCase));
        if (!skipDiscovery)
        {
            var stationDiscovery = provider.GetRequiredService<StationDiscovery>();
            await stationDiscovery.DiscoverStations();
        }
        else logger?.LogInformation("Skipping station discovery as requested");
        
        var stationMergeService = provider.GetRequiredService<StationMergeService>();
        var stations = await stationMergeService.MergeStationsAsync();
        logger?.LogInformation("Merged {Count} stations", stations.Count);
        
        // remove duplicates
        var uniqueStations = stations
            .GroupBy(s => s.EvaNumber)
            .Select(g => g.First())
            .ToList();
        logger?.LogInformation("Found {Count} unique stations", uniqueStations.Count);

        using (var context = provider.GetRequiredService<StationDbContext>())
        {
            context.Stations.AddRange(uniqueStations);
            var amount = await context.SaveChangesAsync();
                
            logger?.LogInformation("Inserted {Amount} stations", amount);
        }
    }
}