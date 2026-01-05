using Navigator.Api.Enums;
using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Statistics;

/// <summary>
/// Represents a series of measured data points for a specific metric over a time range.
/// </summary>
public class MetricSeries
{
    [JsonPropertyName("unit")]
    public required MetricUnit Unit { get; set; }

    [JsonPropertyName("type")]
    public required MetricType Type { get; set; }

    [JsonPropertyName("timerange")]
    public required Timerange Timerange { get; set; }

    /// <summary>
    /// Aggregated statistics about this series (e.g. totals, deltas).
    /// </summary>
    [JsonPropertyName("summary")]
    public required MetricSummary Summary { get; set; }

    [JsonPropertyName("dataPoints")]
    public required IEnumerable<MetricDataPoints> DataPoints { get; set; }
}
