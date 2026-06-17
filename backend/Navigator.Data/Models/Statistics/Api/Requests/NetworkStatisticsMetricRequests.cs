using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Navigator.Data.Enums;
using Navigator.Data.Enums.Metric;

namespace Navigator.Data.Models.Statistics.Api;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(NetworkEventSummaryRequest), "EVENT_SUMMARY")]
[JsonDerivedType(typeof(NetworkJourneySummaryRequest), "JOURNEY_SUMMARY")]
[JsonDerivedType(typeof(NetworkEventTimeSeriesRequest), "EVENT_TIME_SERIES")]
[JsonDerivedType(typeof(NetworkJourneyTimeSeriesRequest), "JOURNEY_TIME_SERIES")]
[JsonDerivedType(typeof(NetworkWeekdayHourHeatmapRequest), "WEEKDAY_HOUR_HEATMAP")]
[JsonDerivedType(typeof(NetworkTransportTypeComparisonRequest), "TRANSPORT_TYPE_COMPARISON")]
[JsonDerivedType(typeof(NetworkStationRankingRequest), "STATION_RANKING")]
[JsonDerivedType(typeof(NetworkLineRankingRequest), "LINE_RANKING")]
[JsonDerivedType(typeof(NetworkEventDelayDistributionRequest), "EVENT_DELAY_DISTRIBUTION")]
public abstract class NetworkStatisticsMetricRequest : StatisticsMetricRequest
{
    [JsonIgnore]
    public abstract NetworkStatisticsMetricType MetricType { get; }
}

public abstract class NetworkEventFilterRequest : NetworkStatisticsMetricRequest,
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

public abstract class NetworkJourneyFilterRequest : NetworkStatisticsMetricRequest,
    IHasTransportTypes,
    IHasAdministrationIds,
    IHasReplacementFilter
{
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

public sealed class NetworkEventSummaryRequest : NetworkEventFilterRequest
{
    [JsonIgnore]
    public override NetworkStatisticsMetricType MetricType => NetworkStatisticsMetricType.EventSummary;
}

public sealed class NetworkJourneySummaryRequest : NetworkJourneyFilterRequest
{
    [JsonIgnore]
    public override NetworkStatisticsMetricType MetricType => NetworkStatisticsMetricType.JourneySummary;
}

public sealed class NetworkEventTimeSeriesRequest : NetworkEventFilterRequest, IHasBucket
{
    [JsonIgnore]
    public override NetworkStatisticsMetricType MetricType => NetworkStatisticsMetricType.EventTimeSeries;

    [JsonPropertyName("bucket")]
    [Description("Aggregation bucket used for returned time series.")]
    public StatisticsBucket Bucket { get; init; } = StatisticsBucket.Day;
}

public sealed class NetworkJourneyTimeSeriesRequest : NetworkJourneyFilterRequest, IHasBucket
{
    [JsonIgnore]
    public override NetworkStatisticsMetricType MetricType => NetworkStatisticsMetricType.JourneyTimeSeries;

    [JsonPropertyName("bucket")]
    [Description("Aggregation bucket used for returned time series.")]
    public StatisticsBucket Bucket { get; init; } = StatisticsBucket.Day;
}

public sealed class NetworkWeekdayHourHeatmapRequest : NetworkEventFilterRequest
{
    [JsonIgnore]
    public override NetworkStatisticsMetricType MetricType => NetworkStatisticsMetricType.WeekdayHourHeatmap;
}

public sealed class NetworkTransportTypeComparisonRequest : NetworkJourneyFilterRequest, IHasScheduleType
{
    [JsonIgnore]
    public override NetworkStatisticsMetricType MetricType => NetworkStatisticsMetricType.TransportTypeComparison;

    [JsonPropertyName("scheduleType")]
    [Description("Optional filter for arrival or departure stop events in event-based comparison values.")]
    public ScheduleType? ScheduleType { get; init; }
}

public sealed class NetworkStationRankingRequest : NetworkStatisticsMetricRequest,
    IHasScheduleType,
    IHasTransportTypes,
    IHasReplacementFilter,
    IHasMinimumVolume,
    IHasPagination
{
    [JsonIgnore]
    public override NetworkStatisticsMetricType MetricType => NetworkStatisticsMetricType.StationRanking;

    [JsonPropertyName("scheduleType")]
    [Description("Optional filter for arrival or departure stop events.")]
    public ScheduleType? ScheduleType { get; init; }

    [JsonPropertyName("transportTypes")]
    [Description("Optional transport types to include. Empty means all transport types.")]
    public TransportType[] TransportTypes { get; init; } = [];

    [JsonPropertyName("includeReplacement")]
    [Description("Whether replacement transport should be included.")]
    public bool IncludeReplacement { get; init; } = true;

    [JsonPropertyName("minVolume")]
    [Range(0, int.MaxValue)]
    [Description("Minimum planned event count required for a station to be included.")]
    public int MinVolume { get; init; } = 0;

    [JsonPropertyName("limit")]
    [Range(1, 500)]
    [Description("Maximum number of stations to return.")]
    public int Limit { get; init; } = 100;

    [JsonPropertyName("offset")]
    [Range(0, int.MaxValue)]
    [Description("Number of stations to skip.")]
    public int Offset { get; init; } = 0;
}

public sealed class NetworkLineRankingRequest : NetworkStatisticsMetricRequest,
    IHasTransportTypes,
    IHasAdministrationIds,
    IHasOriginEvaNumber,
    IHasDestinationEvaNumber,
    IHasReplacementFilter,
    IHasMinimumVolume,
    IHasPagination
{
    [JsonIgnore]
    public override NetworkStatisticsMetricType MetricType => NetworkStatisticsMetricType.LineRanking;

    [JsonPropertyName("transportTypes")]
    [Description("Optional transport types to include. Empty means all transport types.")]
    public TransportType[] TransportTypes { get; init; } = [];

    [JsonPropertyName("administrationIds")]
    [Description("Optional external administration identifiers to include.")]
    public string[] AdministrationIds { get; init; } = [];

    [JsonPropertyName("originEvaNumber")]
    [Description("Optional origin station EVA number filter.")]
    public int? OriginEvaNumber { get; init; }

    [JsonPropertyName("destinationEvaNumber")]
    [Description("Optional destination station EVA number filter.")]
    public int? DestinationEvaNumber { get; init; }

    [JsonPropertyName("includeReplacement")]
    [Description("Whether replacement transport should be included.")]
    public bool IncludeReplacement { get; init; } = true;

    [JsonPropertyName("minVolume")]
    [Range(0, int.MaxValue)]
    [Description("Minimum planned journey count required for a line to be included.")]
    public int MinVolume { get; init; } = 0;

    [JsonPropertyName("limit")]
    [Range(1, 500)]
    [Description("Maximum number of lines to return.")]
    public int Limit { get; init; } = 100;

    [JsonPropertyName("offset")]
    [Range(0, int.MaxValue)]
    [Description("Number of lines to skip.")]
    public int Offset { get; init; } = 0;
}

public sealed class NetworkMapHotspotsRequest : NetworkStatisticsMetricRequest,
    IHasScheduleType,
    IHasTransportTypes,
    IHasReplacementFilter,
    IHasMinimumVolume
{
    [JsonIgnore]
    public override NetworkStatisticsMetricType MetricType => NetworkStatisticsMetricType.MapHotspots;

    [JsonPropertyName("scheduleType")]
    [Description("Optional filter for arrival or departure stop events.")]
    public ScheduleType? ScheduleType { get; init; }

    [JsonPropertyName("transportTypes")]
    [Description("Optional transport types to include. Empty means all transport types.")]
    public TransportType[] TransportTypes { get; init; } = [];

    [JsonPropertyName("includeReplacement")]
    [Description("Whether replacement transport should be included.")]
    public bool IncludeReplacement { get; init; } = true;

    [JsonPropertyName("minVolume")]
    [Range(0, int.MaxValue)]
    [Description("Minimum planned event count required for a station to be included.")]
    public int MinVolume { get; init; } = 0;
}

public sealed class NetworkEventDelayDistributionRequest : NetworkEventFilterRequest
{
    [JsonIgnore]
    public override NetworkStatisticsMetricType MetricType => NetworkStatisticsMetricType.EventDelayDistribution;
}
