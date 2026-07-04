namespace Navigator.Data.StatisticsRefresh;

public interface IStatisticsRefreshWindowPlanner
{
    Task<StatisticsRefreshWindowPlan> PlanDaemonWindowsAsync(
        DateTime endExclusiveUtc,
        CancellationToken cancellationToken);
}
