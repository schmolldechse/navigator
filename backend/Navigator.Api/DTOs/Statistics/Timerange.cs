using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Statistics;

public class Timerange
{
    [JsonPropertyName("start")]
    public required DateTimeOffset Start { get; set; }

    [JsonPropertyName("end")]
    public required DateTimeOffset End { get; set; }
}
