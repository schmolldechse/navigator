using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Statistics;

public class MeasuredStatisticValue
{
    [JsonPropertyName("date")]
    public required DateTimeOffset Date { get; set; }

    [JsonPropertyName("value")]
    public required long Value { get; set; }
}
