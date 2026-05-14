using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Statistics.DataPoint;

public class TimestampCategoryDataPoint : BaseMetricDataPoint
{
    [JsonPropertyName("timestamp")]
    public required DateTimeOffset Timestamp { get; set; }

    [JsonPropertyName("category")]
    public required string Category { get; set; }
}
