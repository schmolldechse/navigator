using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Navigator.Data.Enums;
using Navigator.Data.Enums.Metric;

namespace Navigator.Data.Models.Statistics.Api;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(StationEventSummaryRequest), "EVENT_SUMMARY")]
[JsonDerivedType(typeof(StationBenchmarkRequest), "BENCHMARK")]
[JsonDerivedType(typeof(StationTimeSeriesRequest), "TIME_SERIES")]
[JsonDerivedType(typeof(StationArrivalDepartureComparisonRequest), "ARRIVAL_DEPARTURE_COMPARISON")]
[JsonDerivedType(typeof(StationWeekdayHourHeatmapRequest), "WEEKDAY_HOUR_HEATMAP")]
[JsonDerivedType(typeof(StationLineRankingRequest), "LINE_RANKING")]
[JsonDerivedType(typeof(StationDirectionsRequest), "DIRECTIONS")]
[JsonDerivedType(typeof(StationTransportTypeMixRequest), "TRANSPORT_TYPE_MIX")]
[JsonDerivedType(typeof(StationLineHourMatrixRequest), "LINE_HOUR_MATRIX")]
[JsonDerivedType(typeof(StationEventDetailsRequest), "EVENT_DETAILS")]
public abstract class StationStatisticsMetricRequest : StatisticsMetricRequest
{
    [JsonIgnore]
    public abstract StationStatisticsMetricType MetricType { get; }

    [JsonPropertyName("stationEvaNumber")]
    [Required]
    [Range(1, int.MaxValue)]
    [Description("Station EVA number to evaluate.")]
    public int StationEvaNumber { get; init; }
}

public abstract class StationEventFilterRequest : StationStatisticsMetricRequest,
    IHasScheduleType,
    IHasTransportTypes,
    IHasAdministrationIds,
    IHasReplacementFilter
{
    [JsonPropertyName("scheduleType")]
    [Description("Optional filter for arrival or departure stop events.")]
    public ScheduleType? ScheduleType { get; init; }

    [JsonPropertyName("transportTypes")]
    [Description("Optional transport types to include. Empty means all transport types.")]
    public TransportType[] TransportTypes { get; init; } = [];

    [JsonPropertyName("administrationIds")]
    [Description("Optional external administration identifiers to include.")]
    public string[] AdministrationIds { get; init; } = [];

    [JsonPropertyName("includeReplacement")]
    [Description("Whether replacement transport should be included.")]
    public bool IncludeReplacement { get; init; } = true;
}

public sealed class StationEventSummaryRequest : StationEventFilterRequest
{
    [JsonIgnore]
    public override StationStatisticsMetricType MetricType => StationStatisticsMetricType.EventSummary;
}

public sealed class StationBenchmarkRequest : StationEventFilterRequest
{
    [JsonIgnore]
    public override StationStatisticsMetricType MetricType => StationStatisticsMetricType.Benchmark;
}

public sealed class StationTimeSeriesRequest : StationEventFilterRequest, IHasBucket
{
    [JsonIgnore]
    public override StationStatisticsMetricType MetricType => StationStatisticsMetricType.TimeSeries;

    [JsonPropertyName("bucket")]
    [Description("Aggregation bucket used for returned time series.")]
    public StatisticsBucket Bucket { get; init; } = StatisticsBucket.Day;
}

public sealed class StationArrivalDepartureComparisonRequest : StationEventFilterRequest
{
    [JsonIgnore]
    public override StationStatisticsMetricType MetricType => StationStatisticsMetricType.ArrivalDepartureComparison;
}

public sealed class StationWeekdayHourHeatmapRequest : StationEventFilterRequest
{
    [JsonIgnore]
    public override StationStatisticsMetricType MetricType => StationStatisticsMetricType.WeekdayHourHeatmap;
}

public sealed class StationLineRankingRequest : StationEventFilterRequest,
    IHasOriginEvaNumber,
    IHasDestinationEvaNumber,
    IHasDirectionEvaNumber,
    IHasMinimumVolume,
    IHasPagination
{
    [JsonIgnore]
    public override StationStatisticsMetricType MetricType => StationStatisticsMetricType.LineRanking;

    [JsonPropertyName("originEvaNumber")]
    [Description("Optional origin station EVA number filter.")]
    public int? OriginEvaNumber { get; init; }

    [JsonPropertyName("destinationEvaNumber")]
    [Description("Optional destination station EVA number filter.")]
    public int? DestinationEvaNumber { get; init; }

    [JsonPropertyName("directionEvaNumber")]
    [Description("Optional connected station EVA number filter.")]
    public int? DirectionEvaNumber { get; init; }

    [JsonPropertyName("minVolume")]
    [Range(0, int.MaxValue)]
    [Description("Minimum planned event count required for a line to be included.")]
    public int MinVolume { get; init; } = 0;

    [JsonPropertyName("limit")]
    [Range(1, 500)]
    [Description("Maximum number of ranked lines to return.")]
    public int Limit { get; init; } = 100;

    [JsonPropertyName("offset")]
    [Range(0, int.MaxValue)]
    [Description("Number of ranked lines to skip.")]
    public int Offset { get; init; } = 0;
}

public sealed class StationDirectionsRequest : StationEventFilterRequest, IHasDirectionEvaNumber
{
    [JsonIgnore]
    public override StationStatisticsMetricType MetricType => StationStatisticsMetricType.Directions;

    [JsonPropertyName("directionEvaNumber")]
    [Description("Optional connected station EVA number filter.")]
    public int? DirectionEvaNumber { get; init; }
}

public sealed class StationTransportTypeMixRequest : StationEventFilterRequest
{
    [JsonIgnore]
    public override StationStatisticsMetricType MetricType => StationStatisticsMetricType.TransportTypeMix;
}

public sealed class StationLineHourMatrixRequest : StationEventFilterRequest,
    IHasOriginEvaNumber,
    IHasDestinationEvaNumber,
    IHasDirectionEvaNumber
{
    [JsonIgnore]
    public override StationStatisticsMetricType MetricType => StationStatisticsMetricType.LineHourMatrix;

    [JsonPropertyName("originEvaNumber")]
    [Description("Optional origin station EVA number filter.")]
    public int? OriginEvaNumber { get; init; }

    [JsonPropertyName("destinationEvaNumber")]
    [Description("Optional destination station EVA number filter.")]
    public int? DestinationEvaNumber { get; init; }

    [JsonPropertyName("directionEvaNumber")]
    [Description("Optional connected station EVA number filter.")]
    public int? DirectionEvaNumber { get; init; }
}

public sealed class StationEventDetailsRequest : StationStatisticsMetricRequest,
    IHasScheduleType,
    IHasTransportTypes,
    IHasOptionalJourneyNumber,
    IHasOriginEvaNumber,
    IHasDestinationEvaNumber,
    IHasReplacementFilter,
    IHasPagination
{
    [JsonIgnore]
    public override StationStatisticsMetricType MetricType => StationStatisticsMetricType.EventDetails;

    [JsonPropertyName("scheduleType")]
    [Description("Optional filter for arrival or departure stop events.")]
    public ScheduleType? ScheduleType { get; init; }

    [JsonPropertyName("transportTypes")]
    [Description("Optional transport types to include. Empty means all transport types.")]
    public TransportType[] TransportTypes { get; init; } = [];

    [JsonPropertyName("journeyNumber")]
    [Description("Optional journey number filter.")]
    public int? JourneyNumber { get; init; }

    [JsonPropertyName("originEvaNumber")]
    [Description("Optional origin station EVA number filter.")]
    public int? OriginEvaNumber { get; init; }

    [JsonPropertyName("destinationEvaNumber")]
    [Description("Optional destination station EVA number filter.")]
    public int? DestinationEvaNumber { get; init; }

    [JsonPropertyName("includeReplacement")]
    [Description("Whether replacement transport should be included.")]
    public bool IncludeReplacement { get; init; } = true;

    [JsonPropertyName("limit")]
    [Range(1, 500)]
    [Description("Maximum number of detail rows to return.")]
    public int Limit { get; init; } = 100;

    [JsonPropertyName("offset")]
    [Range(0, int.MaxValue)]
    [Description("Number of detail rows to skip.")]
    public int Offset { get; init; } = 0;
}
