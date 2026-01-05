using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Statistics;

public class MetricDataPoint
{
    [JsonPropertyName("timestamp")]
    public required DateTimeOffset Timestamp { get; set; }

    [JsonPropertyName("value")]
    public required decimal Value { get; set; }
}
