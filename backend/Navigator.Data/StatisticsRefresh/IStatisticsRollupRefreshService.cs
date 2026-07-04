namespace Navigator.Data.StatisticsRefresh;

public interface IStatisticsRollupRefreshService
{
    Task<bool> HasSuccessfulWindowAsync(
        StatisticsRefreshWindow window,
        string operation,
        CancellationToken cancellationToken);

    Task<StatisticsRefreshResult> RefreshWindowAsync(
        StatisticsRefreshWindow window,
        string operation,
        bool refreshContinuousAggregates,
        CancellationToken cancellationToken);

    Task<StatisticsCaggRefreshResult> RefreshContinuousAggregatesAsync(
        StatisticsRefreshWindow window,
        string operation,
        CancellationToken cancellationToken);
}
