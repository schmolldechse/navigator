namespace Navigator.Data.StatisticsRefresh;

public sealed class StatisticsRefreshOptions
{
    public const string SectionName = "StatisticsRefresh";

    public int HotLookbackHours { get; init; } = 6;
    public int CatchupLookbackHours { get; init; } = 168;
    public int EndOffsetHours { get; init; } = 1;
    public int WindowHours { get; init; } = 1;
    public int MaxWindowsPerRun { get; init; } = 8;
    public int QueuedWindowMaxPerRun { get; init; } = 2;
    public int QueueRetryDelayMinutes { get; init; } = 15;
    public int CommandTimeoutSeconds { get; init; } = 900;
    public string AdvisoryLockName { get; init; } = "navigator.statistics.refresh";
}
