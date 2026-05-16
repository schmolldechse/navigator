namespace Navigator.Data.Enums.Metric;

public enum MetricSeriesType
{
    DatabaseSizeBytes,

    RisIdActiveCount,
    RisIdInactiveCount,

    JourneyTotalCount,

    JourneyTransportTypeDistribution,

    GlobalStationEventArrivalCount,
    GlobalStationEventArrivalCancellationCount,
    GlobalStationEventArrivalCancellationRate,
    GlobalStationEventArrivalDelaySum,
    GlobalStationEventArrivalPunctuality5Rate,
    GlobalStationEventArrivalPunctuality15Rate,
    GlobalStationEventDepartureCount,
    GlobalStationEventDepartureCancellationCount,
    GlobalStationEventDepartureCancellationRate,
    GlobalStationEventDepartureDelaySum,
    GlobalStationEventDeparturePunctuality5Rate,
    GlobalStationEventDeparturePunctuality15Rate,

    StationEventArrivalCount,
    StationEventArrivalCancellationCount,
    StationEventArrivalCancellationRate,
    StationEventArrivalDelayAverage,
    StationEventArrivalPunctuality5Rate,
    StationEventArrivalPunctuality15Rate,
    StationEventDepartureCount,
    StationEventDepartureCancellationCount,
    StationEventDepartureCancellationRate,
    StationEventDepartureDelayAverage,
    StationEventDeparturePunctuality5Rate,
    StationEventDeparturePunctuality15Rate,

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
