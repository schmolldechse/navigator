using Daemon.System;
using Daemon.System.Impl;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

        services.AddSingleton<DaemonManager>();
        services.AddSingleton<MonitorStations>();

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

        using (var manager = serviceProvider.GetRequiredService<DaemonManager>())
        {
            manager.AddDaemon(serviceProvider.GetRequiredService<MonitorStations>());
            manager.StartAll();

            _shutdownEvent.Wait();

            await manager.StopAllAsync();
        }

        logger.LogInformation("Application shutdown complete");
    }
}