using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Station;

public class BaseStation
{
    [JsonPropertyName("evaNumber")]
    public required int EvaNumber { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("position")]
    public required StationPosition Position { get; set; }
}
