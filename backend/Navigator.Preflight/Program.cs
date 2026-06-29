using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Navigator.Data;
using Navigator.Data.Repository.StationRepository;
using Navigator.Observability;
using Navigator.Preflight.Infrastructure.Discovery;
using Navigator.Preflight.Infrastructure.Merging;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddServices(builder.Configuration);

// logging
builder.AddNavigatorObservability("Navigator.Preflight");

builder.Services.AddTransient<IStationDiscovery, StationDiscovery>()
    .AddTransient<IStationMerging, StationMerging>();

var host = builder.Build();
var skipStationBootstrap = string.Equals(
    builder.Configuration["Preflight:SkipStationBootstrap"],
    "true",
    StringComparison.OrdinalIgnoreCase);

var preflightLogger = host.Services
    .GetRequiredService<ILoggerFactory>()
    .CreateLogger("Navigator.Preflight");
using var preflightActivity = NavigatorLogging.StartJobActivity("StationBootstrap");
using var preflightScope = preflightLogger.BeginJobScope("StationBootstrap");
preflightLogger.LogInformation("Starting preflight station bootstrap.");

using (var scope = host.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
    await dataContext.Database.MigrateAsync();

    if (skipStationBootstrap)
    {
        Log.Information("Skipping station bootstrap process.");
        return;
    }

    var stationDiscovery = scope.ServiceProvider.GetRequiredService<IStationDiscovery>();
    await stationDiscovery.StartDiscoveringAsync();

    var stationMering = scope.ServiceProvider.GetRequiredService<IStationMerging>();
    var mappedStations = await stationMering.StartMergingAsync();

    var stationRepository = scope.ServiceProvider.GetRequiredService<IStationRepository>();
    await stationRepository.SaveStationsAsync(mappedStations);
}

preflightLogger.LogInformation("Completed preflight station bootstrap.");

host.Run();
