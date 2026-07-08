namespace Navigator.Data.StatisticsRefresh;

public static class StatisticsCaggCatalog
{
    public static readonly IReadOnlyList<string> Names =
    [
        "network_event_quality_hourly",
        "network_event_delay_distribution_hourly",
        "station_event_quality_hourly",
        "station_administration_quality_hourly",
        "line_event_quality_hourly",
        "station_line_quality_hourly",
        "network_journey_quality_hourly",
        "journey_administration_quality_hourly",
        "line_journey_quality_hourly",
        "journey_number_quality_hourly",
        "network_journey_outcome_hourly"
    ];
}
