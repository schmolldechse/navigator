namespace Navigator.Data.StatisticsRefresh;

public sealed record StatisticsCaggRefreshResult(
    StatisticsRefreshWindow Window,
    int CaggRefreshCount,
    TimeSpan Duration);
