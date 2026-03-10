using Navigator.Data.Repository.StatisticsRepository;
using Quartz;

namespace Navigator.Daemon.Infrastructure.MaterializedViews;

public class RefreshTransportViewJob(IStatisticsRepository statisticsRepository) : IJob
{
    public async Task Execute(IJobExecutionContext context) => await statisticsRepository.RefreshHourlyTransportView();
}
