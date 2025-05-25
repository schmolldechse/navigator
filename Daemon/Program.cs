using Daemon.Manager;
using Daemon.Manager.Impl;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization.Conventions;

namespace Daemon;

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

        services.AddSingleton<DaemonManager>()
            .AddSingleton<MonitorStations>()
            .AddSingleton<MonitorJourneys>();

        var serviceProvider = services.BuildServiceProvider();

        var logger = serviceProvider.GetService<ILogger<Program>>();
        logger.LogInformation("Application starting");

        var _shutdownEvent = new ManualResetEventSlim(false);
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            logger.LogInformation("Shutdown requested");
            _shutdownEvent.Set();
        };

        // ignore null values in MongoDB
        ConventionRegistry.Register("Ignore null values", new ConventionPack() { new IgnoreIfNullConvention(true) }, t => true);
        
        using (var manager = serviceProvider.GetRequiredService<DaemonManager>())
        {
            manager.AddDaemon(serviceProvider.GetRequiredService<MonitorStations>());
            manager.AddDaemon(serviceProvider.GetRequiredService<MonitorJourneys>());
            manager.StartAll();

            _shutdownEvent.Wait();

            await manager.StopAllAsync();
        }

        logger.LogInformation("Application shutdown complete");
    }
}