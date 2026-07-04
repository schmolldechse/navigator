using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Navigator.Daemon;
using Navigator.Daemon.Infrastructure;
using Navigator.Daemon.Mapping;
using Navigator.Data;
using Navigator.Observability;
using Quartz;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddServices(builder.Configuration);

// logging
builder.AddNavigatorObservability("Navigator.Daemon");

// mappers
builder.Services.AddSingleton<JourneyMapper>();

// quartz
builder.Services.AddQuartz(options =>
{
    options.AddQuartzJobs<GatheringRisIdsJob>(builder.Configuration);
    options.AddQuartzJobs<GatheringJourneysJob>(builder.Configuration);
    options.AddQuartzJobs<DatabaseSizeEstimationJob>(builder.Configuration);
    options.AddQuartzJobs<RisIdSnapshotJob>(builder.Configuration);
    options.AddQuartzJobs<JourneySnapshotJob>(builder.Configuration);
    // options.AddQuartzJobs<StatisticsAggregateRefreshJob>(builder.Configuration);
    // options.AddQuartzJobs<StaleRisIdDeactivationJob>(builder.Configuration);
});
builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

builder.Services.Configure<HostOptions>(options => options.ShutdownTimeout = TimeSpan.FromMinutes(2));

var host = builder.Build();
await host.RunAsync();
