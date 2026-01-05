using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Statistics;

public class MetricSummary
{
    [JsonPropertyName("startValue")]
    public decimal? StartValue { get; set; }

    [JsonPropertyName("endValue")]
    public decimal? EndValue { get; set; }

    [JsonPropertyName("absoluteChange")]
    public decimal? AbsoluteChange { get; set; }

    [JsonPropertyName("minValue")]
    public decimal? MinValue { get; set; }

    [JsonPropertyName("maxValue")]
    public decimal? MaxValue { get; set; }
}
