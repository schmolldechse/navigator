using CommandLine;
using daemon.Database;
using daemon.Manager;
using daemon.Models;
using daemon.Service;
using daemon.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace daemon;

class Program
{
    static async Task Main(string[] args)
    {
        // parse command line arguments
        Options cmdOptions = new();
        CommandLine.Parser.Default.ParseArguments<Options>(args)
            .WithParsed(parsed => cmdOptions = parsed);
        
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddSimpleConsole(options =>
        {
            options.TimestampFormat = "[HH:mm:ss] ";
            options.IncludeScopes = false;
        }).SetMinimumLevel(cmdOptions.Debug ? LogLevel.Debug : LogLevel.Information));

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
        await dbContext.Database.MigrateAsync();
        logger!.LogInformation("Database migrations applied successfully.");
        await dbContext.Database.ExecuteSqlRawAsync("CREATE EXTENSION IF NOT EXISTS pg_trgm;");
        logger!.LogInformation("Created pg_trgm extension if it did not exist.");

        // parse command line arguments
        if (cmdOptions.SkipGathering != GatheringMode.Full)
            await RunStationGathering(serviceProvider, skipApi: cmdOptions.SkipGathering == GatheringMode.Api);
        
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
        var proxies = Environment.GetEnvironmentVariable("PROXIES");
        var proxyEnabled = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROXY_ENABLED")) && 
                           bool.TryParse(Environment.GetEnvironmentVariable("PROXY_ENABLED"), out var enabled) && enabled;
        services.AddSingleton(new AppConfiguration(dbClientId, dbClientSecret, proxies, proxyEnabled));

        // common
        services.AddHttpClient();
        services.AddSingleton(new ManualResetEventSlim(false));
        services.AddSingleton<ProxyRotator>();

        // station gathering services
        services.AddSingleton<ApiService>();
        services.AddSingleton<StationDiscoveryService>();
        services.AddSingleton<StationMergingService>();

        // database
        services.AddDbContext<NavigatorDbContext>(ServiceLifetime.Scoped);

        // daemons
        services.AddSingleton<DaemonManager>();
        services.AddSingleton<GatheringRisIdsDaemon>();
        services.AddSingleton<GatheringJourneyDaemon>();
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
        
        // merge files
        var stationMergingService = serviceProvider.GetRequiredService<StationMergingService>();
        var stadaStations = await stationMergingService.LoadStadaStationsAsync(cancellationToken: CancellationToken.None);
        
        var stations = await stationMergingService.MergeFiles(stadaStations, CancellationToken.None);
        logger.LogInformation("Merged {Count} stations.", stations.Count);
        
        // calculate weights
        stations = stations.Select(station =>
        {
            station.Weight = stationMergingService.CalculateWeight(station, stadaStations.GetValueOrDefault(station.EvaNumber)?.PriceCategory ?? -1);
            return station;
        }).ToList();
        logger.LogInformation("Calculated weights for stations.");

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
        manager.AddDaemon(serviceProvider.GetRequiredService<GatheringJourneyDaemon>());
        manager.StartAll();

        shutdownEvent.Wait();
        // await manager.StopAll();
    }
}

public enum GatheringMode
{
    None,
    Api, // skip Api calls
    Full, // skips Api calls + inserting ~297k stations into PostgresSQL
}

public class Options
{
    [Option('s', "skipGathering", Required = false, HelpText = "Skip gathering of stations. Options: 'None', 'Api', 'Full'")]
    public string SkipGatheringStr { get; set; } = "None";
    
    public GatheringMode SkipGathering 
    { 
        get 
        {
            if (Enum.TryParse<GatheringMode>(SkipGatheringStr, true, out var result))
                return result;
            return GatheringMode.None;
        }
    }
    
    [Option("debug", Required = false, HelpText = "Enable debug logging.")]
    public bool Debug { get; set; } = false;
}