using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Navigator.Data.Enums;
using Navigator.Data.Enums.Metric;
using Navigator.Data.Infrastructure;

namespace Navigator.Data.Models.Statistics.Api;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(LineProfileRequest), "PROFILE")]
[JsonDerivedType(typeof(LineJourneySummaryRequest), "JOURNEY_SUMMARY")]
[JsonDerivedType(typeof(LineEventSummaryRequest), "EVENT_SUMMARY")]
[JsonDerivedType(typeof(LineTimeSeriesRequest), "TIME_SERIES")]
[JsonDerivedType(typeof(LineRouteVariantsRequest), "ROUTE_VARIANTS")]
[JsonDerivedType(typeof(LineStationPerformanceRequest), "STATION_PERFORMANCE")]
[JsonDerivedType(typeof(LineJourneyNumberRankingRequest), "JOURNEY_NUMBER_RANKING")]
[JsonDerivedType(typeof(LineWeekdayHourHeatmapRequest), "WEEKDAY_HOUR_HEATMAP")]
[JsonDerivedType(typeof(LineProblemStationsRequest), "PROBLEM_STATIONS")]
public abstract class LineStatisticsMetricRequest : StatisticsMetricRequest, IHasLineName
{
    [JsonIgnore]
    public abstract LineStatisticsMetricType MetricType { get; }

    [JsonPropertyName("lineName")]
    [Required]
    [Regex(@"^[\p{L}\p{N}\s_\-/]+$", Exception = "LineName may only contain letters, digits, spaces, underscores, dashes and slashes.", MaxLength = 64, ValidateRegexSyntax = false)]
    [Description("Line or journey description to evaluate.")]
    public required string LineName { get; init; }
}

public abstract class LineJourneyFilterRequest : LineStatisticsMetricRequest,
    IHasTransportTypes,
    IHasAdministrationIds,
    IHasOptionalJourneyNumber,
    IHasOriginEvaNumber,
    IHasDestinationEvaNumber,
    IHasReplacementFilter
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
}

public abstract class LineEventFilterRequest : LineJourneyFilterRequest, IHasScheduleType
{
    [JsonPropertyName("scheduleType")]
    [Description("Optional filter for arrival or departure stop events.")]
    public ScheduleType? ScheduleType { get; init; }
}

public sealed class LineProfileRequest : LineJourneyFilterRequest
{
    [JsonIgnore]
    public override LineStatisticsMetricType MetricType => LineStatisticsMetricType.Profile;
}

public sealed class LineJourneySummaryRequest : LineJourneyFilterRequest
{
    [JsonIgnore]
    public override LineStatisticsMetricType MetricType => LineStatisticsMetricType.JourneySummary;
}

public sealed class LineEventSummaryRequest : LineEventFilterRequest
{
    [JsonIgnore]
    public override LineStatisticsMetricType MetricType => LineStatisticsMetricType.EventSummary;
}

public sealed class LineTimeSeriesRequest : LineEventFilterRequest, IHasBucket
{
    [JsonIgnore]
    public override LineStatisticsMetricType MetricType => LineStatisticsMetricType.TimeSeries;

    [JsonPropertyName("bucket")]
    [Description("Aggregation bucket used for returned time series.")]
    public StatisticsBucket Bucket { get; init; } = StatisticsBucket.Day;
}

public sealed class LineRouteVariantsRequest : LineJourneyFilterRequest, IHasMinimumVolume
{
    [JsonIgnore]
    public override LineStatisticsMetricType MetricType => LineStatisticsMetricType.RouteVariants;

    [JsonPropertyName("minVolume")]
    [Range(0, int.MaxValue)]
    [Description("Minimum planned journey count required for a route variant to be included.")]
    public int MinVolume { get; init; } = 0;
}

public class LineStationPerformanceRequest : LineEventFilterRequest, IHasMinimumVolume, IHasPagination
{
    [JsonIgnore]
    public override LineStatisticsMetricType MetricType => LineStatisticsMetricType.StationPerformance;

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

public sealed class LineJourneyNumberRankingRequest : LineJourneyFilterRequest, IHasMinimumVolume, IHasPagination
{
    [JsonIgnore]
    public override LineStatisticsMetricType MetricType => LineStatisticsMetricType.JourneyNumberRanking;

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
