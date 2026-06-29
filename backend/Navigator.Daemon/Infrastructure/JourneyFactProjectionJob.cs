using Microsoft.Extensions.Logging;
using Navigator.Observability;
using Quartz;

namespace Navigator.Daemon.Infrastructure;

[DisallowConcurrentExecution]
public class JourneyFactProjectionJob(
    ILogger<JourneyFactProjectionJob> logger,
    JourneyFactProjectionService projectionService
) : IJob
{
    public async Task Execute(IJobExecutionContext context) => await logger.RunJobAsync(nameof(JourneyFactProjectionJob), context.FireInstanceId, ExecuteCoreAsync);

    private async Task ExecuteCoreAsync() => await projectionService.ProjectAvailableAsync();
}
