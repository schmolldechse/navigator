using Navigator.Data.Enums.Metric;
using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Statistics;

/// <summary>
/// Represents a series of measured data points for a specific metric over a time range.
/// </summary>
public class MetricSeries
{
    [JsonPropertyName("seriesType")]
    public required MetricSeriesType SeriesType { get; set; }

    [JsonPropertyName("unit")]
    public required MetricUnit Unit { get; set; }

    [JsonPropertyName("timerange")]
    public required Timerange Timerange { get; set; }

    [JsonPropertyName("isCumulative")]
    public bool IsCumulative { get; set; }

    /// <summary>
    /// Aggregated statistics about this series (e.g. totals, deltas).
    /// </summary>
    [JsonPropertyName("summary")]
    public required MetricSummary Summary { get; set; }

    [JsonPropertyName("dataPoints")]
    public required IEnumerable<MetricDataPoint> DataPoints { get; set; }
}
