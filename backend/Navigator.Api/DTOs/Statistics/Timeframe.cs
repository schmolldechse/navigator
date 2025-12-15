using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Statistics;

public class Timeframe
{
    [JsonPropertyName("start")]
    public required DateTimeOffset Start { get; set; }

    [JsonPropertyName("end")]
    public required DateTimeOffset End { get; set; }
}
