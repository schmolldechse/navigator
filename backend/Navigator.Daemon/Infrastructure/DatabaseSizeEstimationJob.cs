using Navigator.Data.Entities.Statistics;
using Navigator.Data.Repository.StatisticsRepository;
using Microsoft.Extensions.Logging;
using Navigator.Observability;
using Quartz;

namespace Navigator.Daemon.Infrastructure;

[DisallowConcurrentExecution]
public class DatabaseSizeEstimationJob(
    ILogger<DatabaseSizeEstimationJob> logger,
    IStatisticsRepository statisticsRepository
) : IJob
{
    public async Task Execute(IJobExecutionContext context) => await logger.RunJobAsync(nameof(DatabaseSizeEstimationJob), context.FireInstanceId, ExecuteCoreAsync);

    private async Task ExecuteCoreAsync()
    {
        var result = await statisticsRepository.EstimateCurrentDatabaseSizeAsync();
        if (!result.HasValue) return;

        await statisticsRepository.SaveDatabaseSizeAsync(new DatabaseSizeSnapshot()
        {
            MeasuredAt = DateTime.UtcNow,
            SizeInBytes = result.Value
        });

        logger.LogInformation("Estimated database size: {DatabaseSizeBytes} bytes.", result.Value);
    }
}
