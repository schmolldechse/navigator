using System.ComponentModel;
using System.Text.Json.Serialization;
using Navigator.Data.Enums;

namespace Navigator.Data.Models.Statistics.Api;

public sealed record StationBenchmarkResult(
    [property: JsonPropertyName("station")]
    [property: Description("Event metrics for the requested station.")]
    EventMetrics Station,

    [property: JsonPropertyName("network")]
    [property: Description("Comparable event metrics for the filtered network.")]
    EventMetrics Network,

    [property: JsonPropertyName("similarStations")]
    [property: Description("Optional benchmark stations with similar characteristics.")]
    IReadOnlyList<StationEventRankingItem>? SimilarStations
) : StatisticsMetricResult;

public sealed record StationTimeSeriesResult(
    [property: JsonPropertyName("items")]
    [property: Description("Station event metric points grouped by the requested bucket.")]
    IReadOnlyList<EventTimeSeriesPoint> Items
) : StatisticsMetricResult;

public sealed record ArrivalDepartureComparisonResult(
    [property: JsonPropertyName("items")]
    [property: Description("Event metrics grouped by arrival and departure schedule type.")]
    IReadOnlyList<ArrivalDepartureComparisonItem> Items
) : StatisticsMetricResult;

public sealed record ArrivalDepartureComparisonItem(
    [property: JsonPropertyName("scheduleType")]
    [property: Description("Arrival or departure schedule type.")]
    ScheduleType ScheduleType,

    [property: JsonPropertyName("eventMetrics")]
    [property: Description("Aggregated event metrics for the schedule type.")]
    EventMetrics EventMetrics
);

public sealed record StationLineRankingResult(
    [property: JsonPropertyName("items")]
    [property: Description("Ranked lines observed at the station.")]
    IReadOnlyList<StationLineRankingItem> Items,

    [property: JsonPropertyName("page")]
    [property: Description("Paging metadata for the ranking.")]
    MetricPage Page
) : StatisticsMetricResult;

public sealed record StationDirectionsResult(
    [property: JsonPropertyName("items")]
    [property: Description("Connected origin and destination directions for the station.")]
    IReadOnlyList<StationDirectionItem> Items
) : StatisticsMetricResult;

public sealed record StationDirectionItem(
    [property: JsonPropertyName("directionType")]
    [property: Description("Whether the connected station is treated as origin or destination.")]
    string DirectionType,

    [property: JsonPropertyName("station")]
    [property: Description("Connected station.")]
    StationReference Station,

    [property: JsonPropertyName("eventMetrics")]
    [property: Description("Aggregated event metrics for the direction.")]
    EventMetrics EventMetrics
);

public sealed record TransportTypeMixResult(
    [property: JsonPropertyName("items")]
    [property: Description("Station event metrics grouped by transport type.")]
    IReadOnlyList<TransportTypeMixItem> Items
) : StatisticsMetricResult;

public sealed record TransportTypeMixItem(
    [property: JsonPropertyName("transportType")]
    [property: Description("Transport type represented by the item.")]
    TransportType TransportType,

    [property: JsonPropertyName("share")]
    [property: Description("Share of station events represented by this transport type.")]
    decimal? Share,

    [property: JsonPropertyName("eventMetrics")]
    [property: Description("Aggregated event metrics for the transport type.")]
    EventMetrics EventMetrics
);

public sealed record LineHourMatrixResult(
    [property: JsonPropertyName("items")]
    [property: Description("Station-line event metrics grouped by hour.")]
    IReadOnlyList<LineHourMatrixItem> Items
) : StatisticsMetricResult;

public sealed record LineHourMatrixItem(
    [property: JsonPropertyName("lineName")]
    [property: Description("Line or journey description.")]
    string LineName,

    [property: JsonPropertyName("transportType")]
    [property: Description("Transport type of the line.")]
    TransportType TransportType,

    [property: JsonPropertyName("hour")]
    [property: Description("Hour of day in Europe/Berlin.")]
    int Hour,

    [property: JsonPropertyName("eventMetrics")]
    [property: Description("Aggregated event metrics for the line-hour cell.")]
    EventMetrics EventMetrics
);

public sealed record StationEventDetailsResult(
    [property: JsonPropertyName("items")]
    [property: Description("Detailed stop events at the station.")]
    IReadOnlyList<StationEventDetailItem> Items,

    [property: JsonPropertyName("page")]
    [property: Description("Paging metadata for the detail rows.")]
    MetricPage Page
) : StatisticsMetricResult;

public sealed record StationEventDetailItem(
    [property: JsonPropertyName("stopPlaceId")]
    [property: Description("Internal stop-place identifier.")]
    Guid StopPlaceId,

    [property: JsonPropertyName("journeyId")]
    [property: Description("External journey identifier from the source data.")]
    string JourneyId,

    [property: JsonPropertyName("journeyDate")]
    [property: Description("Service date of the journey.")]
    DateOnly JourneyDate,

    [property: JsonPropertyName("stationEvaNumber")]
    [property: Description("Station EVA number of the stop event.")]
    int StationEvaNumber,

    [property: JsonPropertyName("plannedTime")]
    [property: Description("Planned stop-event time in UTC.")]
    DateTime PlannedTime,

    [property: JsonPropertyName("scheduleType")]
    [property: Description("Arrival or departure schedule type of the stop event.")]
    ScheduleType ScheduleType,

    [property: JsonPropertyName("journeyNumber")]
    [property: Description("Journey number.")]
    int JourneyNumber,

    [property: JsonPropertyName("lineName")]
    [property: Description("Line or journey description.")]
    string LineName,

    [property: JsonPropertyName("originEvaNumber")]
    [property: Description("Origin station EVA number of the journey.")]
    int OriginEvaNumber,

    [property: JsonPropertyName("journeyStartTime")]
    [property: Description("Planned journey start time in UTC.")]
    DateTime JourneyStartTime,

    [property: JsonPropertyName("destinationEvaNumber")]
    [property: Description("Destination station EVA number of the journey.")]
    int DestinationEvaNumber,

    [property: JsonPropertyName("journeyEndTime")]
    [property: Description("Planned journey end time in UTC.")]
    DateTime JourneyEndTime,

    [property: JsonPropertyName("stopCancelled")]
    [property: Description("Whether this stop event was cancelled.")]
    bool StopCancelled,

    [property: JsonPropertyName("transportType")]
    [property: Description("Transport type of the journey.")]
    TransportType TransportType,

    [property: JsonPropertyName("eventDelaySeconds")]
    [property: Description("Stop-event delay in seconds.")]
    int EventDelaySeconds,

    [property: JsonPropertyName("isReplacement")]
    [property: Description("Whether the journey is replacement transport.")]
    bool IsReplacement
);
