using System.Text.Json.Serialization;

namespace Navigator.Data.Models.Statistics.DataPoint;

public class TimestampDataPoint : BaseMetricDataPoint
{
    [JsonPropertyName("timestamp")]
    public required DateTimeOffset Timestamp { get; set; }
}
