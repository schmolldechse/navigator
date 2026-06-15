using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Navigator.Data.Enums;
using Navigator.Data.Enums.Metric;
using Navigator.Data.Infrastructure;

namespace Navigator.Data.Models.Statistics.Api;

#pragma warning disable CS0108

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(JourneyPatternRequest), "PATTERN")]
[JsonDerivedType(typeof(JourneyKpisRequest), "JOURNEY_KPIS")]
[JsonDerivedType(typeof(JourneyDailyOutcomesRequest), "DAILY_OUTCOMES")]
[JsonDerivedType(typeof(JourneyStopProfileRequest), "STOP_PROFILE")]
[JsonDerivedType(typeof(JourneyDelayBuildUpRequest), "DELAY_BUILD_UP")]
[JsonDerivedType(typeof(JourneyCalendarRequest), "CALENDAR")]
public abstract class JourneyStatisticsMetricRequest : StatisticsMetricRequest
{
    [JsonPropertyName("journeyNumber")]
    [Required]
    [Range(1, int.MaxValue)]
    [Description("Journey number to evaluate.")]
    public int JourneyNumber { get; init; }

    [JsonPropertyName("lineName")]
    [Regex(@"^[\p{L}\p{N}\s_\-/]+$", Exception = "LineName may only contain letters, digits, spaces, underscores, dashes and slashes.", MaxLength = 64, ValidateRegexSyntax = false)]
    [Description("Optional line or journey description filter.")]
    public string? LineName { get; init; }

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

    [JsonIgnore]
    public abstract JourneyStatisticsMetricType MetricType { get; }

    [JsonIgnore]
    public override string MetricName => MetricType.ToString();

    public override TransportType[] GetTransportTypes() => TransportTypes;

    public override string[] GetAdministrationIds() => AdministrationIds;

    public override int? GetOriginEvaNumber() => OriginEvaNumber;

    public override int? GetDestinationEvaNumber() => DestinationEvaNumber;

    public override string? GetLineNameFilter() => LineName;

    public override bool GetIncludeReplacement() => IncludeReplacement;
}

public sealed class JourneyPatternRequest : JourneyStatisticsMetricRequest
{
    [JsonIgnore]
    public override JourneyStatisticsMetricType MetricType => JourneyStatisticsMetricType.Pattern;
}

public sealed class JourneyKpisRequest : JourneyStatisticsMetricRequest
{
    [JsonIgnore]
    public override JourneyStatisticsMetricType MetricType => JourneyStatisticsMetricType.JourneyKpis;
}

public sealed class JourneyDailyOutcomesRequest : JourneyStatisticsMetricRequest
{
    [JsonIgnore]
    public override JourneyStatisticsMetricType MetricType => JourneyStatisticsMetricType.DailyOutcomes;
}

public sealed class JourneyStopProfileRequest : JourneyStatisticsMetricRequest
{
    [JsonPropertyName("scheduleType")]
    [Description("Optional filter for arrival or departure stop events.")]
    public ScheduleType? ScheduleType { get; init; }

    [JsonIgnore]
    public override JourneyStatisticsMetricType MetricType => JourneyStatisticsMetricType.StopProfile;

    public override ScheduleType? GetScheduleType() => ScheduleType;
}

public sealed class JourneyDelayBuildUpRequest : JourneyStatisticsMetricRequest
{
    [JsonPropertyName("scheduleType")]
    [Description("Optional filter for arrival or departure stop events.")]
    public ScheduleType? ScheduleType { get; init; }

    [JsonIgnore]
    public override JourneyStatisticsMetricType MetricType => JourneyStatisticsMetricType.DelayBuildUp;

    public override ScheduleType? GetScheduleType() => ScheduleType;
}

public sealed class JourneyCalendarRequest : JourneyStatisticsMetricRequest
{
    [JsonIgnore]
    public override JourneyStatisticsMetricType MetricType => JourneyStatisticsMetricType.Calendar;
}

#pragma warning restore CS0108
