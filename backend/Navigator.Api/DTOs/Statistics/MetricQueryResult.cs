using Navigator.Api.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Statistics;

/// <summary>
/// Represents a request to query specific metrics within a time range.
/// </summary>
public class MetricQueryResult
{
    [JsonPropertyName("start")]
    [Description("The start time of the range for which to retrieve metrics.")]
    public required DateTimeOffset Start { get; set; }

    [JsonPropertyName("end")]
    [Description("The start time of the range for which to retrieve metrics.")]
    public required DateTimeOffset End { get; set; }

    [JsonPropertyName("metricType")]
    [Description("The specific type of metric to retrieve.")]
    [AllowedValues(
        MetricType.DatabaseSize,
        MetricType.RecordedRisIds,
        MetricType.RecordedJourneys,
        ErrorMessage = "The provided metric type is not supported."
    )]
    public required MetricType MetricType { get; set; }
}
