using Microsoft.Extensions.Logging;
using Navigator.Data.Entities.Statistics;
using Navigator.Data.Repository.StatisticsRepository;
using Navigator.Observability;
using Quartz;

namespace Navigator.Daemon.Infrastructure;

[DisallowConcurrentExecution]
public class RisIdSnapshotJob(
    ILogger<RisIdSnapshotJob> logger,
    IStatisticsRepository statisticsRepository
) : IJob
{
    public async Task Execute(IJobExecutionContext context) => await logger.RunJobAsync(nameof(RisIdSnapshotJob), context.FireInstanceId, ExecuteCoreAsync);

    private async Task ExecuteCoreAsync()
    {
        var result = await statisticsRepository.EstimateCurrentRisIdsAsync();
        if (!result.HasValue) return;

        await statisticsRepository.SaveRisIdSnapshotAsync(new RisIdSnapshot()
        {
            MeasuredAt = DateTime.UtcNow,
            Active = result.Value.Active,
            Inactive = result.Value.Inactive
        });
        logger.LogInformation("Estimated active RisIds: {ActiveCount}, inactive RisIds: {InactiveCount}.", result.Value.Active, result.Value.Inactive);
    }
}
