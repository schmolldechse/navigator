using Navigator.Data.Enums.Metric;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Statistics;

/// <summary>
/// Represents a request to query specific metrics within a time range.
/// </summary>
public class MetricQueryRequest : IValidatableObject
{
    [JsonPropertyName("start")]
    [Description("The start time of the range for which to retrieve metrics.")]
    public DateTimeOffset? Start { get; set; }

    [JsonPropertyName("end")]
    [Description("The start time of the range for which to retrieve metrics.")]
    public DateTimeOffset? End { get; set; }

    [JsonPropertyName("queryType")]
    [Description("The specific type of metric to retrieve.")]
    public required MetricQueryType MetricQueryType { get; set; }

    [JsonPropertyName("cumulativeValues")]
    public bool CumulativeValues { get; set; } = true;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var allTimeMetrics = new[] { MetricQueryType.TransportTypes };
        if (allTimeMetrics.Contains(MetricQueryType)) yield break;

        if (Start is null)
        {
            yield return new ValidationResult(
                $"Start time must be provided for metric type {MetricQueryType}.",
                new[] { nameof(Start) });
        }

        if (End is null)
        {
            yield return new ValidationResult(
                $"End time must be provided for metric type {MetricQueryType}.",
                new[] { nameof(End) });
        }

        if (Start.HasValue && End.HasValue && Start > End)
        {
            yield return new ValidationResult(
                "Start time must be earlier than or equal to end time.",
                new[] { nameof(Start), nameof(End) });
        }
    }
}
