using System.ComponentModel;
using System.Text.Json.Serialization;
using Navigator.Data.Enums.Metric;

namespace Navigator.Data.Models.Statistics;

[Description("Represents a series of measured data points for a specific metric.")]
public class MetricSeries
{
    [JsonPropertyName("seriesType")]
    public required MetricSeriesType SeriesType { get; set; }

    [JsonPropertyName("unit")]
    public required MetricUnit Unit { get; set; }

    [JsonPropertyName("page")]
    public MetricPage? Page { get; set; }

    [JsonPropertyName("dataPoints")]
    public required IEnumerable<BaseMetricDataPoint> DataPoints { get; set; }
}
