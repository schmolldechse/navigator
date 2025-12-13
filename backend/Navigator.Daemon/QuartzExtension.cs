using Microsoft.Extensions.Configuration;
using Quartz;

namespace Navigator.Daemon;

public static class QuartzExtension
{
    public static IServiceCollectionQuartzConfigurator AddQuartzJobs<T>(
        this IServiceCollectionQuartzConfigurator quartz,
        IConfiguration configuration
    ) where T : IJob
    {
        string jobName = typeof(T).Name;

        var configKey = $"JobConfigs:{jobName}:CronExpression";
        var cronSchedule = configuration[configKey];
        if (string.IsNullOrEmpty(cronSchedule)) throw new Exception($"No CronExpression found in appsettings for {jobName}");

        var jobKey = new JobKey(jobName);
        quartz.AddJob<T>(options => options.WithIdentity(jobKey));

        quartz.AddTrigger(options => options
            .ForJob(jobKey)
            .WithIdentity(jobName + "-trigger")
            .WithCronSchedule(cronSchedule));

        return quartz;
    }
}
