using daemon.Database;
using daemon.Manager;
using daemon.Models;
using daemon.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace daemon;

class Program
{
    static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddSimpleConsole(options =>
        {
            options.SingleLine = true;
            options.TimestampFormat = "[HH:mm:ss] ";
            options.IncludeScopes = false;
        }).SetMinimumLevel(LogLevel.Information));

        ConfigureServices(services);

        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetService<ILogger<Program>>();
        logger!.LogInformation("Navigator daemon application is starting...");

        Console.CancelKeyPress += (sender, eventArgs) =>
        {
            eventArgs.Cancel = true;
            logger!.LogInformation("Stopping navigator daemon application...");

            var shutdownEvent = serviceProvider.GetRequiredService<ManualResetEventSlim>();
            shutdownEvent.Set();
        };

        // apply migrations
        var dbContext = serviceProvider.GetRequiredService<NavigatorDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
        await dbContext.Database.MigrateAsync();
        logger!.LogInformation("Database migrations applied successfully.");
        await dbContext.Database.ExecuteSqlRawAsync("CREATE EXTENSION IF NOT EXISTS pg_trgm;");
        logger!.LogInformation("Created pg_trgm extension if it did not exist.");

        // parse command line arguments
        var gatheringMode = ParseGatheringMode(args);
        if (gatheringMode != GatheringMode.Full)
            await RunStationGathering(serviceProvider, skipApi: gatheringMode == GatheringMode.Api);
        
        RunDaemon(serviceProvider);
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Environment Variables
        var dbClientId = Environment.GetEnvironmentVariable("DB_CLIENT_ID") ??
                         throw new ArgumentNullException("DB_CLIENT_ID", "DeutscheBahn ClientId not configured");
        var dbClientSecret = Environment.GetEnvironmentVariable("DB_CLIENT_SECRET") ??
                             throw new ArgumentNullException("DB_CLIENT_SECRET",
                                 "DeutscheBahn ClientSecret not configured");
        services.AddSingleton(new AppConfiguration(dbClientId, dbClientSecret));

        // common
        services.AddHttpClient();
        services.AddSingleton(new ManualResetEventSlim(false));

        // station gathering services
        services.AddSingleton<ApiService>();
        services.AddSingleton<StationDiscoveryService>();
        services.AddSingleton<StationMergingService>();

        // database
        services.AddDbContext<NavigatorDbContext>();

        // daemons
        services.AddSingleton<DaemonManager>();
        services.AddSingleton<GatheringRisIdsDaemon>();
    }

    private static async Task RunStationGathering(ServiceProvider serviceProvider, bool skipApi = false)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        if (!skipApi)
        {
            logger.LogInformation("Starting gathering of stations from Deutsche Bahn API...");
            var stationDiscoveryService = serviceProvider.GetRequiredService<StationDiscoveryService>();
            await stationDiscoveryService.DiscoverStations();
        }
        else logger.LogInformation("Skipping gathering of stations as requested.");

        // merge stations
        var stationMergingService = serviceProvider.GetRequiredService<StationMergingService>();
        var stations = await stationMergingService.MergeStationsAsync();
        logger.LogInformation("Merged {Count} stations.", stations.Count);

        // filter out stations already in the database
        var dbContext = serviceProvider.GetRequiredService<NavigatorDbContext>();
        var existingEvaNumbers = dbContext.Stations.Select(s => s.EvaNumber).ToHashSet();
        var newStations = stations.Where(s => !existingEvaNumbers.Contains(s.EvaNumber)).ToList();
        logger.LogInformation("Filtered to {Count} new stations (not already in database)", newStations.Count);

        dbContext.Stations.AddRange(newStations);
        var amount = await dbContext.SaveChangesAsync();
        logger.LogInformation("Inserted {Amount} entries", amount);
        
        logger.LogWarning("Please change --skipGathering to 'full' to avoid calling this process again.");
    }

    private static void RunDaemon(ServiceProvider serviceProvider)
    {
        var shutdownEvent = serviceProvider.GetRequiredService<ManualResetEventSlim>();
        using var manager = serviceProvider.GetRequiredService<DaemonManager>();

        manager.AddDaemon(serviceProvider.GetRequiredService<GatheringRisIdsDaemon>());
        manager.StartAll();

        shutdownEvent.Wait();
        // await manager.StopAll();
    }

    private static GatheringMode ParseGatheringMode(string[] args)
    {
        foreach (var arg in args)
        {
            if (arg.StartsWith("--skipGathering=", StringComparison.OrdinalIgnoreCase))
            {
                var value = arg.Substring("--skipGathering=".Length).ToLower();
                if (value == "full") return GatheringMode.Full;
                if (value == "api") return GatheringMode.Api;
            }
        }

        return GatheringMode.None;
    }
}

public enum GatheringMode
{
    None,
    Api, // skip Api calls
    Full, // skips Api calls + inserting ~297k stations into PostgresSQL
}