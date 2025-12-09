using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Station;

public class StationSummaryDTO
{
    [JsonPropertyName("evaNumber")]
    public required int EvaNumber { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("position")]
    public required PositionDTO Position { get; set; }
}
