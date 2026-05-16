using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Statistics;

public class MetricSample
{
    [JsonPropertyName("numerator")]
    public required long Numerator { get; set; }

    [JsonPropertyName("denominator")]
    public required long Denominator { get; set; }
}
