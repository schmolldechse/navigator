using Navigator.Data.Enums.Metric;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Statistics;

/// <summary>
/// Represents a request to query specific metrics within a time range.
/// </summary>
public class MetricQueryRequest
{
    [JsonPropertyName("start")]
    [Description("The start time of the range for which to retrieve metrics.")]
    public required DateTimeOffset Start { get; set; }

    [JsonPropertyName("end")]
    [Description("The start time of the range for which to retrieve metrics.")]
    public required DateTimeOffset End { get; set; }

    [JsonPropertyName("queryType")]
    [Description("The specific type of metric to retrieve.")]
    public required MetricQueryType MetricQueryType { get; set; }

    [JsonPropertyName("cumulativeValues")]
    public required bool CumulativeValues { get; set; }
}
