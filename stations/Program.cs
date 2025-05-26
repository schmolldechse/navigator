using Microsoft.EntityFrameworkCore;
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

        // Merge our stations from RIS and StaDa
        var stationMergeService = provider.GetRequiredService<StationMergeService>();
        var list = await stationMergeService.MergeStationsAsync();
        logger?.LogInformation("Merged {Count} stations", list.Count);
        
        // Apply migration and create extension
        await using var context = provider.GetRequiredService<StationDbContext>();
        await context.Database.MigrateAsync();
        logger?.LogInformation("Applied database migrations");
        await context.Database.ExecuteSqlRawAsync("CREATE EXTENSION IF NOT EXISTS pg_trgm;");
        logger?.LogInformation("Created `pg_trgm` extension");

        // Gather existing stations and filter out those already in the database
        var existingEvaNumbers = context.Stations.Select(s => s.EvaNumber).ToHashSet();
        var newStations = list.Where(s => !existingEvaNumbers.Contains(s.EvaNumber)).ToList();
        logger?.LogInformation("Filtered to {Count} new stations (not already in database)", newStations.Count);

        context.Stations.AddRange(newStations);
        var amount = await context.SaveChangesAsync();
        logger?.LogInformation("Inserted {Amount} entries", amount);
    }
}
