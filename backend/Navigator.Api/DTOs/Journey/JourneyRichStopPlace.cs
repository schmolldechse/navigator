using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Journey;

public class JourneyRichStopPlace : JourneyStopPlace
{
    [JsonPropertyName("cancelled")]
    public required bool Cancelled { get; set; }
}