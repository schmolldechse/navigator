using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Navigator.Daemon;
using Navigator.Daemon.Infrastructure;
using Navigator.Daemon.Mapping;
using Navigator.Data;
using Quartz;
using Serilog;
using Serilog.Exceptions;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddServices(builder.Configuration);

// logging
builder.Services.AddSerilog((services, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithExceptionDetails()
    .Enrich.WithProperty("service_name", "Navigator.Daemon")
    .Enrich.WithProperty("env", builder.Environment.EnvironmentName));

// mappers
builder.Services.AddSingleton<JourneyMapper>();

builder.Services.AddQuartz(options =>
{
    options.AddQuartzJobs<GatheringRisIdsJob>(builder.Configuration);
    options.AddQuartzJobs<GatheringJourneysJob>(builder.Configuration);
    options.AddQuartzJobs<DatabaseSizeEstimationJob>(builder.Configuration);
    options.AddQuartzJobs<RisIdSnapshotJob>(builder.Configuration);
    options.AddQuartzJobs<JourneySnapshotJob>(builder.Configuration);
    options.AddQuartzJobs<StaleRisIdDeactivationJob>(builder.Configuration);
});
builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

builder.Services.Configure<HostOptions>(options => options.ShutdownTimeout = TimeSpan.FromMinutes(2));

var host = builder.Build();
host.Run();