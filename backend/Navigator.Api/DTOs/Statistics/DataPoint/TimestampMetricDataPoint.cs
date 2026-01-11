using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Statistics.DataPoint;

public class TimestampMetricDataPoint : MetricDataPoint
{
    [JsonPropertyName("timestamp")]
    public required DateTimeOffset Timestamp { get; set; }
}
