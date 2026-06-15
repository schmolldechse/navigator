using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Navigator.Data.Enums;
using Navigator.Data.Enums.Metric;
using Navigator.Data.Infrastructure;

namespace Navigator.Data.Models.Statistics.Api;

#pragma warning disable CS0108

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(LineSummaryRequest), "SUMMARY")]
[JsonDerivedType(typeof(LineJourneyKpisRequest), "JOURNEY_KPIS")]
[JsonDerivedType(typeof(LineEventKpisRequest), "EVENT_KPIS")]
[JsonDerivedType(typeof(LineTimeSeriesRequest), "TIME_SERIES")]
[JsonDerivedType(typeof(LineRouteVariantsRequest), "ROUTE_VARIANTS")]
[JsonDerivedType(typeof(LineStationPerformanceRequest), "STATION_PERFORMANCE")]
[JsonDerivedType(typeof(LineJourneyNumberRankingRequest), "JOURNEY_NUMBER_RANKING")]
[JsonDerivedType(typeof(LineWeekdayHourHeatmapRequest), "WEEKDAY_HOUR_HEATMAP")]
[JsonDerivedType(typeof(LineProblemStationsRequest), "PROBLEM_STATIONS")]
public abstract class LineStatisticsMetricRequest : StatisticsMetricRequest
{
    [JsonPropertyName("lineName")]
    [Required]
    [Regex(@"^[\p{L}\p{N}\s_\-/]+$", Exception = "LineName may only contain letters, digits, spaces, underscores, dashes and slashes.", MaxLength = 64, ValidateRegexSyntax = false)]
    [Description("Line or journey description to evaluate.")]
    public required string LineName { get; init; }

    [JsonIgnore]
    public abstract LineStatisticsMetricType MetricType { get; }

    [JsonIgnore]
    public override string MetricName => MetricType.ToString();

    public override string? GetLineNameFilter() => LineName;
}

public abstract class LineJourneyFilterRequest : LineStatisticsMetricRequest
{
    [JsonPropertyName("transportTypes")]
    [Description("Optional transport types to include. Empty means all transport types.")]
    public TransportType[] TransportTypes { get; init; } = [];

    [JsonPropertyName("administrationIds")]
    [Description("Optional external administration identifiers to include.")]
    public string[] AdministrationIds { get; init; } = [];

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

    public override TransportType[] GetTransportTypes() => TransportTypes;

    public override string[] GetAdministrationIds() => AdministrationIds;

    public override int? GetJourneyNumberFilter() => JourneyNumber;

    public override int? GetOriginEvaNumber() => OriginEvaNumber;

    public override int? GetDestinationEvaNumber() => DestinationEvaNumber;

    public override bool GetIncludeReplacement() => IncludeReplacement;
}

public abstract class LineEventFilterRequest : LineJourneyFilterRequest
{
    [JsonPropertyName("scheduleType")]
    [Description("Optional filter for arrival or departure stop events.")]
    public ScheduleType? ScheduleType { get; init; }

    public override ScheduleType? GetScheduleType() => ScheduleType;
}

public sealed class LineSummaryRequest : LineJourneyFilterRequest
{
    [JsonIgnore]
    public override LineStatisticsMetricType MetricType => LineStatisticsMetricType.Summary;
}

public sealed class LineJourneyKpisRequest : LineJourneyFilterRequest
{
    [JsonIgnore]
    public override LineStatisticsMetricType MetricType => LineStatisticsMetricType.JourneyKpis;
}

public sealed class LineEventKpisRequest : LineEventFilterRequest
{
    [JsonIgnore]
    public override LineStatisticsMetricType MetricType => LineStatisticsMetricType.EventKpis;
}

public sealed class LineTimeSeriesRequest : LineEventFilterRequest
{
    [JsonPropertyName("bucket")]
    [Description("Aggregation bucket used for returned time series.")]
    public StatisticsBucket Bucket { get; init; } = StatisticsBucket.Day;

    [JsonIgnore]
    public override LineStatisticsMetricType MetricType => LineStatisticsMetricType.TimeSeries;

    public override StatisticsBucket GetBucket() => Bucket;
}

public sealed class LineRouteVariantsRequest : LineJourneyFilterRequest
{
    [JsonPropertyName("minVolume")]
    [Range(0, int.MaxValue)]
    [Description("Minimum planned journey count required for a route variant to be included.")]
    public int MinVolume { get; init; } = 0;

    [JsonIgnore]
    public override LineStatisticsMetricType MetricType => LineStatisticsMetricType.RouteVariants;

    public override int GetMinVolume() => MinVolume;
}

public class LineStationPerformanceRequest : LineEventFilterRequest
{
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
    public override LineStatisticsMetricType MetricType => LineStatisticsMetricType.StationPerformance;

    public override int GetMinVolume() => MinVolume;

    public override int GetLimit() => Limit;

    public override int GetOffset() => Offset;
}

public sealed class LineJourneyNumberRankingRequest : LineJourneyFilterRequest
{
    [JsonPropertyName("minVolume")]
    [Range(0, int.MaxValue)]
    [Description("Minimum planned journey count required for a journey number to be included.")]
    public int MinVolume { get; init; } = 0;

    [JsonPropertyName("limit")]
    [Range(1, 500)]
    [Description("Maximum number of journey numbers to return.")]
    public int Limit { get; init; } = 100;

    [JsonPropertyName("offset")]
    [Range(0, int.MaxValue)]
    [Description("Number of journey numbers to skip.")]
    public int Offset { get; init; } = 0;

    [JsonIgnore]
    public override LineStatisticsMetricType MetricType => LineStatisticsMetricType.JourneyNumberRanking;

    public override int GetMinVolume() => MinVolume;

    public override int GetLimit() => Limit;

    public override int GetOffset() => Offset;
}

public sealed class LineWeekdayHourHeatmapRequest : LineJourneyFilterRequest
{
    [JsonIgnore]
    public override LineStatisticsMetricType MetricType => LineStatisticsMetricType.WeekdayHourHeatmap;
}

public sealed class LineProblemStationsRequest : LineStationPerformanceRequest
{
    [JsonIgnore]
    public override LineStatisticsMetricType MetricType => LineStatisticsMetricType.ProblemStations;
}

#pragma warning restore CS0108
