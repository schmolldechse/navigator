using Navigator.Data.Entities.Statistics;
using Navigator.Data.Repository.StatisticsRepository;
using Microsoft.Extensions.Logging;
using Navigator.Observability;
using Quartz;

namespace Navigator.Daemon.Infrastructure;

[DisallowConcurrentExecution]
public class JourneySnapshotJob(
    ILogger<JourneySnapshotJob> logger,
    IStatisticsRepository statisticsRepository
) : IJob
{
    public async Task Execute(IJobExecutionContext context) => await logger.RunJobAsync(nameof(JourneySnapshotJob), context.FireInstanceId, ExecuteCoreAsync);

    private async Task ExecuteCoreAsync()
    {
        var result = await statisticsRepository.EstimateCurrentJourneysAsync();
        if (!result.HasValue) return;

        await statisticsRepository.SaveJourneySnapshotAsync(new JourneySnapshot()
        {
            MeasuredAt = DateTime.UtcNow,
            Total = result.Value
        });

        logger.LogInformation("Estimated journey count: {JourneyCount}.", result.Value);
    }
}
