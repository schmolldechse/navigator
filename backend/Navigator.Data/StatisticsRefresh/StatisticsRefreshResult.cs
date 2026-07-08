namespace Navigator.Data.StatisticsRefresh;

public sealed record StatisticsRefreshResult(
    StatisticsRefreshWindow Window,
    long EventRowsAffected,
    long JourneyRowsAffected,
    int CaggRefreshCount,
    TimeSpan Duration);
