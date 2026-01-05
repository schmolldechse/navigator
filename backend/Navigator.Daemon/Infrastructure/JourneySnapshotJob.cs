using Navigator.Data.Entities.Statistics;
using Navigator.Data.Repository.StatisticsRepository;
using Quartz;

namespace Navigator.Daemon.Infrastructure;

public class JourneySnapshotJob(IStatisticsRepository statisticsRepository) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var result = await statisticsRepository.EstimateCurrentJourneysAsync();
        if (!result.HasValue) return;

        await statisticsRepository.SaveJourneySnapshotAsync(new JourneySnapshot()
        {
            MeasuredAt = DateTime.UtcNow,
            Total = result.Value
        });
    }
}
