using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Navigator.Daemon;
using Navigator.Daemon.Infrastructure;
using Navigator.Data;
using Quartz;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddServices(builder.Configuration);

builder.Services.AddQuartz(options =>
{
    options.AddQuartzJobs<GatheringRisIdsJob>(builder.Configuration);
});
builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{

}

host.Run();