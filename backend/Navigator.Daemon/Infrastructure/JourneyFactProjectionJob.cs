using Microsoft.Extensions.Logging;
using Navigator.Observability;
using Quartz;

namespace Navigator.Daemon.Infrastructure;

[DisallowConcurrentExecution]
public class JourneyFactProjectionJob(
    ILogger<JourneyFactProjectionJob> logger,
    JourneyFactProjectionService projectionService,
    JourneyFactProjectionCoordinator coordinator
) : IJob
{
    public async Task Execute(IJobExecutionContext context) =>
        await logger.RunJobAsync(
            nameof(JourneyFactProjectionJob),
            context.FireInstanceId,
            () => ExecuteCoreAsync(context.CancellationToken));

    private async Task ExecuteCoreAsync(CancellationToken cancellationToken) =>
        await coordinator.RunExclusiveAsync(
            token => projectionService.ProjectAvailableAsync(token),
            cancellationToken);
}
