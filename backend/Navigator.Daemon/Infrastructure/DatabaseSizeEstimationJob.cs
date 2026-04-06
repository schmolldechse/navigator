using Navigator.Data.Entities.Statistics;
using Navigator.Data.Repository.StatisticsRepository;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Navigator.Daemon.Infrastructure;

[DisallowConcurrentExecution]
public class DatabaseSizeEstimationJob(
    ILogger<DatabaseSizeEstimationJob> logger,
    IStatisticsRepository statisticsRepository
) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var result = await statisticsRepository.EstimateCurrentDatabaseSizeAsync();

        await statisticsRepository.SaveDatabaseSizeAsync(new DatabaseSizeSnapshot()
        {
            MeasuredAt = DateTime.UtcNow,
            SizeInBytes = result.Value
        });

        logger.LogInformation("Estimated database size: {DatabaseSizeBytes} bytes.", result.Value);
    }
}
