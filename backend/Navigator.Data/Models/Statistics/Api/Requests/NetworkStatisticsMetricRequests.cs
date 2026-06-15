using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Navigator.Data.Enums;
using Navigator.Data.Enums.Metric;

namespace Navigator.Data.Models.Statistics.Api;

#pragma warning disable CS0108

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(NetworkEventKpisRequest), "EVENT_KPIS")]
[JsonDerivedType(typeof(NetworkJourneyKpisRequest), "JOURNEY_KPIS")]
[JsonDerivedType(typeof(NetworkEventTimeSeriesRequest), "EVENT_TIME_SERIES")]
[JsonDerivedType(typeof(NetworkJourneyTimeSeriesRequest), "JOURNEY_TIME_SERIES")]
[JsonDerivedType(typeof(NetworkWeekdayHourHeatmapRequest), "WEEKDAY_HOUR_HEATMAP")]
[JsonDerivedType(typeof(NetworkTransportTypeComparisonRequest), "TRANSPORT_TYPE_COMPARISON")]
[JsonDerivedType(typeof(NetworkStationRankingRequest), "STATION_RANKING")]
[JsonDerivedType(typeof(NetworkLineRankingRequest), "LINE_RANKING")]
public abstract class NetworkStatisticsMetricRequest : StatisticsMetricRequest
{
    [JsonIgnore]
    public abstract NetworkStatisticsMetricType MetricType { get; }

    [JsonIgnore]
    public override string MetricName => MetricType.ToString();
}

public abstract class NetworkEventFilterRequest : NetworkStatisticsMetricRequest
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

public abstract class NetworkJourneyFilterRequest : NetworkStatisticsMetricRequest
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

    public override TransportType[] GetTransportTypes() => TransportTypes;

    public override string[] GetAdministrationIds() => AdministrationIds;

    public override bool GetIncludeReplacement() => IncludeReplacement;
}

public sealed class NetworkEventKpisRequest : NetworkEventFilterRequest
{
    [JsonIgnore]
    public override NetworkStatisticsMetricType MetricType => NetworkStatisticsMetricType.EventKpis;
}

public sealed class NetworkJourneyKpisRequest : NetworkJourneyFilterRequest
{
    [JsonIgnore]
    public override NetworkStatisticsMetricType MetricType => NetworkStatisticsMetricType.JourneyKpis;
}

public sealed class NetworkEventTimeSeriesRequest : NetworkEventFilterRequest
{
    [JsonPropertyName("bucket")]
    [Description("Aggregation bucket used for returned time series.")]
    public StatisticsBucket Bucket { get; init; } = StatisticsBucket.Day;

    [JsonIgnore]
    public override NetworkStatisticsMetricType MetricType => NetworkStatisticsMetricType.EventTimeSeries;

    public override StatisticsBucket GetBucket() => Bucket;
}

public sealed class NetworkJourneyTimeSeriesRequest : NetworkJourneyFilterRequest
{
    [JsonPropertyName("bucket")]
    [Description("Aggregation bucket used for returned time series.")]
    public StatisticsBucket Bucket { get; init; } = StatisticsBucket.Day;

    [JsonIgnore]
    public override NetworkStatisticsMetricType MetricType => NetworkStatisticsMetricType.JourneyTimeSeries;

    public override StatisticsBucket GetBucket() => Bucket;
}

public sealed class NetworkWeekdayHourHeatmapRequest : NetworkEventFilterRequest
{
    [JsonIgnore]
    public override NetworkStatisticsMetricType MetricType => NetworkStatisticsMetricType.WeekdayHourHeatmap;
}

public sealed class NetworkTransportTypeComparisonRequest : NetworkJourneyFilterRequest
{
    [JsonPropertyName("scheduleType")]
    [Description("Optional filter for arrival or departure stop events in event-based comparison values.")]
    public ScheduleType? ScheduleType { get; init; }

    [JsonIgnore]
    public override NetworkStatisticsMetricType MetricType => NetworkStatisticsMetricType.TransportTypeComparison;

    public override ScheduleType? GetScheduleType() => ScheduleType;
}

public sealed class NetworkStationRankingRequest : NetworkStatisticsMetricRequest
{
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

    [JsonIgnore]
    public override NetworkStatisticsMetricType MetricType => NetworkStatisticsMetricType.StationRanking;

    public override ScheduleType? GetScheduleType() => ScheduleType;

    public override TransportType[] GetTransportTypes() => TransportTypes;

    public override bool GetIncludeReplacement() => IncludeReplacement;

    public override int GetMinVolume() => MinVolume;

    public override int GetLimit() => Limit;

    public override int GetOffset() => Offset;
}

public sealed class NetworkLineRankingRequest : NetworkStatisticsMetricRequest
{
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

    [JsonIgnore]
    public override NetworkStatisticsMetricType MetricType => NetworkStatisticsMetricType.LineRanking;

    public override TransportType[] GetTransportTypes() => TransportTypes;

    public override string[] GetAdministrationIds() => AdministrationIds;

    public override int? GetOriginEvaNumber() => OriginEvaNumber;

    public override int? GetDestinationEvaNumber() => DestinationEvaNumber;

    public override bool GetIncludeReplacement() => IncludeReplacement;

    public override int GetMinVolume() => MinVolume;

    public override int GetLimit() => Limit;

    public override int GetOffset() => Offset;
}

public sealed class NetworkMapHotspotsRequest : NetworkStatisticsMetricRequest
{
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

    [JsonIgnore]
    public override NetworkStatisticsMetricType MetricType => NetworkStatisticsMetricType.MapHotspots;

    public override ScheduleType? GetScheduleType() => ScheduleType;

    public override TransportType[] GetTransportTypes() => TransportTypes;

    public override bool GetIncludeReplacement() => IncludeReplacement;

    public override int GetMinVolume() => MinVolume;

    public override int GetLimit() => int.MaxValue;
}

#pragma warning restore CS0108
