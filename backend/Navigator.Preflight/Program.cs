using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Navigator.Data;
using Navigator.Data.Repository.StationRepository;
using Navigator.Preflight.Infrastructure.Discovery;
using Navigator.Preflight.Infrastructure.Merging;
using Serilog;
using Serilog.Exceptions;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddServices(builder.Configuration);

// logging
builder.Services.AddSerilog((services, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithExceptionDetails()
    .Enrich.WithProperty("service_name", "Navigator.Preflight")
    .Enrich.WithProperty("env", builder.Environment.EnvironmentName));

builder.Services.AddTransient<IStationDiscovery, StationDiscovery>()
    .AddTransient<IStationMerging, StationMerging>();

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
    await dataContext.Database.MigrateAsync();

    var stationDiscovery = scope.ServiceProvider.GetRequiredService<IStationDiscovery>();
    await stationDiscovery.StartDiscoveringAsync();

    var stationMering = scope.ServiceProvider.GetRequiredService<IStationMerging>();
    var mappedStations = await stationMering.StartMergingAsync();

    var stationRepository = scope.ServiceProvider.GetRequiredService<IStationRepository>();
    await stationRepository.SaveStationsAsync(mappedStations);
}

host.Run();