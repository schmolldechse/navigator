using Navigator.Data.Enums;
using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Station;

public class StationDTO
{
    [JsonPropertyName("evaNumber")]
    public required int EvaNumber { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("position")]
    public required PositionDTO Position { get; set; }

    [JsonPropertyName("transports")]
    public required TransportType[] Transports { get; set; }

    [JsonPropertyName("ril100")]
    public string[]? Ril100 { get; set; }
}
