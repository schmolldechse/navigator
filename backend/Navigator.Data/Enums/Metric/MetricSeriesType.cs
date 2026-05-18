namespace Navigator.Data.Enums.Metric;

public enum MetricSeriesType
{
    DatabaseSizeBytes,

    RisIdActiveCount,
    RisIdInactiveCount,

    JourneyTotalCount,

    StationEventCount,
    StationEventCancellationCount,
    StationEventCancellationRate,
    StationEventDelayAverage,
    StationEventPunctuality5Rate,
    StationEventPunctuality15Rate,

    AdministrationRankingCount,
    AdministrationRankingCancellationCount,
    AdministrationRankingCancellationRate,
    AdministrationRankingAverageDelay,
    AdministrationRankingPunctuality5Rate,
    AdministrationRankingPunctuality15Rate,

    LineRankingCount,
    LineRankingCancellationCount,
    LineRankingCancellationRate,
    LineRankingAverageDelay,
    LineRankingPunctuality5Rate,
    LineRankingPunctuality15Rate,
}
