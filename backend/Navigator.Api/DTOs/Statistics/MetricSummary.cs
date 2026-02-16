using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Statistics;

public class MetricSummary
{
    [JsonPropertyName("startValue")]
    public decimal? StartValue { get; set; }

    [JsonPropertyName("endValue")]
    public decimal? EndValue { get; set; }

    [JsonPropertyName("absoluteChange")]
    public required decimal AbsoluteChange { get; set; }

    [JsonPropertyName("minValue")]
    public required decimal MinValue { get; set; }

    [JsonPropertyName("maxValue")]
    public required decimal MaxValue { get; set; }
}
