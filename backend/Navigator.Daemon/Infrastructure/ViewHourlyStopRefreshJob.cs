using Navigator.Data.Repository.StatisticsRepository;
using Quartz;

namespace Navigator.Daemon.Infrastructure;

public class ViewHourlyStopRefreshJob(IStatisticsRepository statisticsRepository) : IJob
{
    public async Task Execute(IJobExecutionContext context) => await statisticsRepository.RefreshHourlyStopView();
}
