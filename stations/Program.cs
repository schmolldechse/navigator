using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using stations.Api;
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

        var provider = services.BuildServiceProvider();

        var logger = provider.GetService<ILogger<Program>>();

        bool skipDiscovery = args.Any(arg => arg.Equals("skipDiscovery", StringComparison.OrdinalIgnoreCase));
        if (skipDiscovery)
        {
            var stationDiscovery = provider.GetRequiredService<StationDiscovery>();
            await stationDiscovery.DiscoverStations();
        }
        else logger?.LogInformation("Skipping station discovery as requested");

        logger?.LogInformation("Writing stations to database...");
    }
}