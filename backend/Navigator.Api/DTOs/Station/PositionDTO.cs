using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Station;

public class PositionDTO
{
    [JsonPropertyName("latitude")]
    public required double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public required double Longitude { get; set; }
}
