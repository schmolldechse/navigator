using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Navigator.Data.Enums;
using Navigator.Data.Enums.Metric;

namespace Navigator.Data.Models.Statistics.Api;

public abstract class StatisticsMetricRequest : IValidatableObject
{
    [JsonPropertyName("from")]
    [Description("First included instant.")]
    public required DateTimeOffset From { get; init; }

    [JsonPropertyName("to")]
    [Description("First excluded instant.")]
    public required DateTimeOffset To { get; init; }

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
}

public interface IHasBucket
{
    StatisticsBucket Bucket { get; }
}

public interface IHasScheduleType
{
    ScheduleType? ScheduleType { get; }
}

public interface IHasTransportTypes
{
    TransportType[] TransportTypes { get; }
}

public interface IHasAdministrationIds
{
    string[] AdministrationIds { get; }
}

public interface IHasReplacementFilter
{
    bool IncludeReplacement { get; }
}

public interface IHasOriginEvaNumber
{
    int? OriginEvaNumber { get; }
}

public interface IHasDestinationEvaNumber
{
    int? DestinationEvaNumber { get; }
}

public interface IHasDirectionEvaNumber
{
    int? DirectionEvaNumber { get; }
}

public interface IHasJourneyNumber
{
    int JourneyNumber { get; }
}

public interface IHasOptionalJourneyNumber
{
    int? JourneyNumber { get; }
}

public interface IHasLineName
{
    string? LineName { get; }
}

public interface IHasMinimumVolume
{
    int MinVolume { get; }
}

public interface IHasLimit
{
    int Limit { get; }
}

public interface IHasPagination
{
    int Limit { get; }

    int Offset { get; }
}
