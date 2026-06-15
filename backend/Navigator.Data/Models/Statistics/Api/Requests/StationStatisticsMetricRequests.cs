using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Navigator.Data.Enums;
using Navigator.Data.Enums.Metric;

namespace Navigator.Data.Models.Statistics.Api;

#pragma warning disable CS0108

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(StationEventKpisRequest), "EVENT_KPIS")]
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
    [JsonPropertyName("stationEvaNumber")]
    [Required]
    [Range(1, int.MaxValue)]
    [Description("Station EVA number to evaluate.")]
    public int StationEvaNumber { get; init; }

    [JsonIgnore]
    public abstract StationStatisticsMetricType MetricType { get; }

    [JsonIgnore]
    public override string MetricName => MetricType.ToString();
}

public abstract class StationEventFilterRequest : StationStatisticsMetricRequest
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

    public override ScheduleType? GetScheduleType() => ScheduleType;

    public override TransportType[] GetTransportTypes() => TransportTypes;

    public override string[] GetAdministrationIds() => AdministrationIds;

    public override bool GetIncludeReplacement() => IncludeReplacement;
}

public sealed class StationEventKpisRequest : StationEventFilterRequest
{
    [JsonIgnore]
    public override StationStatisticsMetricType MetricType => StationStatisticsMetricType.EventKpis;
}

public sealed class StationBenchmarkRequest : StationEventFilterRequest
{
    [JsonIgnore]
    public override StationStatisticsMetricType MetricType => StationStatisticsMetricType.Benchmark;
}

public sealed class StationTimeSeriesRequest : StationEventFilterRequest
{
    [JsonPropertyName("bucket")]
    [Description("Aggregation bucket used for returned time series.")]
    public StatisticsBucket Bucket { get; init; } = StatisticsBucket.Day;

    [JsonIgnore]
    public override StationStatisticsMetricType MetricType => StationStatisticsMetricType.TimeSeries;

    public override StatisticsBucket GetBucket() => Bucket;
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

public sealed class StationLineRankingRequest : StationEventFilterRequest
{
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

    [JsonIgnore]
    public override StationStatisticsMetricType MetricType => StationStatisticsMetricType.LineRanking;

    public override int? GetOriginEvaNumber() => OriginEvaNumber;

    public override int? GetDestinationEvaNumber() => DestinationEvaNumber;

    public override int? GetDirectionEvaNumber() => DirectionEvaNumber;

    public override int GetMinVolume() => MinVolume;

    public override int GetLimit() => Limit;

    public override int GetOffset() => Offset;
}

public sealed class StationDirectionsRequest : StationEventFilterRequest
{
    [JsonPropertyName("directionEvaNumber")]
    [Description("Optional connected station EVA number filter.")]
    public int? DirectionEvaNumber { get; init; }

    [JsonIgnore]
    public override StationStatisticsMetricType MetricType => StationStatisticsMetricType.Directions;

    public override int? GetDirectionEvaNumber() => DirectionEvaNumber;
}

public sealed class StationTransportTypeMixRequest : StationEventFilterRequest
{
    [JsonIgnore]
    public override StationStatisticsMetricType MetricType => StationStatisticsMetricType.TransportTypeMix;
}

public sealed class StationLineHourMatrixRequest : StationEventFilterRequest
{
    [JsonPropertyName("originEvaNumber")]
    [Description("Optional origin station EVA number filter.")]
    public int? OriginEvaNumber { get; init; }

    [JsonPropertyName("destinationEvaNumber")]
    [Description("Optional destination station EVA number filter.")]
    public int? DestinationEvaNumber { get; init; }

    [JsonPropertyName("directionEvaNumber")]
    [Description("Optional connected station EVA number filter.")]
    public int? DirectionEvaNumber { get; init; }

    [JsonIgnore]
    public override StationStatisticsMetricType MetricType => StationStatisticsMetricType.LineHourMatrix;

    public override int? GetOriginEvaNumber() => OriginEvaNumber;

    public override int? GetDestinationEvaNumber() => DestinationEvaNumber;

    public override int? GetDirectionEvaNumber() => DirectionEvaNumber;
}

public sealed class StationEventDetailsRequest : StationStatisticsMetricRequest
{
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

    [JsonIgnore]
    public override StationStatisticsMetricType MetricType => StationStatisticsMetricType.EventDetails;

    public override ScheduleType? GetScheduleType() => ScheduleType;

    public override TransportType[] GetTransportTypes() => TransportTypes;

    public override int? GetJourneyNumberFilter() => JourneyNumber;

    public override int? GetOriginEvaNumber() => OriginEvaNumber;

    public override int? GetDestinationEvaNumber() => DestinationEvaNumber;

    public override bool GetIncludeReplacement() => IncludeReplacement;

    public override int GetLimit() => Limit;

    public override int GetOffset() => Offset;
}

#pragma warning restore CS0108
