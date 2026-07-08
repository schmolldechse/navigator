namespace Navigator.Data.StatisticsRefresh;

public sealed record StatisticsRefreshWindowPlan(
    DateTime EndExclusiveUtc,
    IReadOnlyList<StatisticsRefreshWindow> Windows,
    IReadOnlySet<StatisticsRefreshWindow> QueuedWindows);
