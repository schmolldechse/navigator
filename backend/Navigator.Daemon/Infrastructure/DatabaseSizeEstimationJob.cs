using Navigator.Data.Entities.Statistics;
using Navigator.Data.Repository.StatisticsRepository;
using Quartz;

namespace Navigator.Daemon.Infrastructure;

public class DatabaseSizeEstimationJob(
    IStatisticsRepository statisticsRepository
) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var result = await statisticsRepository.EstimateDatabaseSizeAsync();
        if (result is null) return;

        await statisticsRepository.SaveDatabaseSizeAsync(new DatabaseSize()
        {
            MeasuredAt = DateTime.UtcNow,
            SizeInBytes = result.SizeInBytes
        });
    }
}
