using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Navigator.Data.Enums;
using Navigator.Data.Enums.Metric;

namespace Navigator.Data.Models.Statistics.Api;

#pragma warning disable CS0108

public abstract class StatisticsMetricRequest : IValidatableObject
{
    [JsonPropertyName("from")]
    [Description("First local service day to include. The date is interpreted in Europe/Berlin.")]
    public required DateOnly From { get; init; }

    [JsonPropertyName("to")]
    [Description("First local service day to exclude. The date is interpreted in Europe/Berlin.")]
    public required DateOnly To { get; init; }

    [JsonIgnore]
    public DateOnly FromDate => From;

    [JsonIgnore]
    public DateOnly ToDate => To;

    [JsonIgnore]
    public DateTime FromUtc => ConvertLocalDateToUtc(FromDate);

    [JsonIgnore]
    public DateTime ToUtc => ConvertLocalDateToUtc(ToDate);

    [JsonIgnore]
    public abstract string MetricName { get; }

    [JsonIgnore]
    public StatisticsBucket Bucket => GetBucket();

    [JsonIgnore]
    public ScheduleType? ScheduleType => GetScheduleType();

    [JsonIgnore]
    public TransportType[] TransportTypes => GetTransportTypes();

    [JsonIgnore]
    public string[] AdministrationIds => GetAdministrationIds();

    [JsonIgnore]
    public int? OriginEvaNumber => GetOriginEvaNumber();

    [JsonIgnore]
    public int? DestinationEvaNumber => GetDestinationEvaNumber();

    [JsonIgnore]
    public int? DirectionEvaNumber => GetDirectionEvaNumber();

    [JsonIgnore]
    public int? JourneyNumber => GetJourneyNumberFilter();

    [JsonIgnore]
    public bool IncludeReplacement => GetIncludeReplacement();

    [JsonIgnore]
    public int MinVolume => GetMinVolume();

    [JsonIgnore]
    public int Limit => GetLimit();

    [JsonIgnore]
    public int Offset => GetOffset();

    public virtual StatisticsBucket GetBucket() => StatisticsBucket.Day;

    public virtual ScheduleType? GetScheduleType() => null;

    public virtual TransportType[] GetTransportTypes() => [];

    public virtual string[] GetAdministrationIds() => [];

    public virtual int? GetOriginEvaNumber() => null;

    public virtual int? GetDestinationEvaNumber() => null;

    public virtual int? GetDirectionEvaNumber() => null;

    public virtual int? GetJourneyNumberFilter() => null;

    public virtual string? GetLineNameFilter() => null;

    public virtual bool GetIncludeReplacement() => true;

    public virtual int GetMinVolume() => 0;

    public virtual int GetLimit() => 100;

    public virtual int GetOffset() => 0;

    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (From == default)
        {
            yield return new ValidationResult(
                "From is required.",
                [nameof(From)]);
        }

        if (To == default)
        {
            yield return new ValidationResult(
                "To is required.",
                [nameof(To)]);
        }

        if (From != default && To != default && From >= To)
        {
            yield return new ValidationResult(
                "From must be earlier than To. To is exclusive.",
                [nameof(From), nameof(To)]);
        }
    }

    private static DateTime ConvertLocalDateToUtc(DateOnly date)
    {
        var local = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Unspecified);
        return TimeZoneInfo.ConvertTimeToUtc(local, ResolveBerlinTimeZone());
    }

    private static TimeZoneInfo ResolveBerlinTimeZone()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin");
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
        }
    }
}

public abstract class BucketedStatisticsMetricRequest : StatisticsMetricRequest
{
    [JsonPropertyName("bucket")]
    [Description("Aggregation bucket used for returned time series.")]
    public StatisticsBucket Bucket { get; init; } = StatisticsBucket.Day;

    public override StatisticsBucket GetBucket() => Bucket;
}

public abstract class EventFilterStatisticsMetricRequest : StatisticsMetricRequest
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

public abstract class BucketedEventFilterStatisticsMetricRequest : EventFilterStatisticsMetricRequest
{
    [JsonPropertyName("bucket")]
    [Description("Aggregation bucket used for returned time series.")]
    public StatisticsBucket Bucket { get; init; } = StatisticsBucket.Day;

    public override StatisticsBucket GetBucket() => Bucket;
}

public abstract class JourneyFilterStatisticsMetricRequest : StatisticsMetricRequest
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

public abstract class BucketedJourneyFilterStatisticsMetricRequest : JourneyFilterStatisticsMetricRequest
{
    [JsonPropertyName("bucket")]
    [Description("Aggregation bucket used for returned time series.")]
    public StatisticsBucket Bucket { get; init; } = StatisticsBucket.Day;

    public override StatisticsBucket GetBucket() => Bucket;
}

public abstract class RouteJourneyFilterStatisticsMetricRequest : JourneyFilterStatisticsMetricRequest
{
    [JsonPropertyName("originEvaNumber")]
    [Description("Optional origin station EVA number filter.")]
    public int? OriginEvaNumber { get; init; }

    [JsonPropertyName("destinationEvaNumber")]
    [Description("Optional destination station EVA number filter.")]
    public int? DestinationEvaNumber { get; init; }

    public override int? GetOriginEvaNumber() => OriginEvaNumber;

    public override int? GetDestinationEvaNumber() => DestinationEvaNumber;
}

public abstract class BucketedRouteJourneyFilterStatisticsMetricRequest : RouteJourneyFilterStatisticsMetricRequest
{
    [JsonPropertyName("bucket")]
    [Description("Aggregation bucket used for returned time series.")]
    public StatisticsBucket Bucket { get; init; } = StatisticsBucket.Day;

    public override StatisticsBucket GetBucket() => Bucket;
}

public abstract class PagedStatisticsMetricRequest : StatisticsMetricRequest
{
    [JsonPropertyName("minVolume")]
    [Range(0, int.MaxValue)]
    [Description("Minimum planned event or journey count required for an item to be returned.")]
    public int MinVolume { get; init; } = 0;

    [JsonPropertyName("limit")]
    [Range(1, 500)]
    [Description("Maximum number of items to return.")]
    public int Limit { get; init; } = 100;

    [JsonPropertyName("offset")]
    [Range(0, int.MaxValue)]
    [Description("Number of ranked items to skip.")]
    public int Offset { get; init; } = 0;

    public override int GetMinVolume() => MinVolume;

    public override int GetLimit() => Limit;

    public override int GetOffset() => Offset;
}

#pragma warning restore CS0108
