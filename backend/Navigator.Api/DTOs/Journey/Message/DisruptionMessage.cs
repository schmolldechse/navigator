using System.Text.Json.Serialization;
using Navigator.Data.Enums;

namespace Navigator.Api.DTOs.Journey.Message;

public class DisruptionMessage : JourneyMessage
{
    [JsonPropertyName("cause")]
    public string? Cause { get; set; }

    [JsonPropertyName("effect")]
    public string? Effect { get; set; }

    [JsonPropertyName("disruptionId")]
    public string? DisruptionId { get; set; }

    [JsonPropertyName("textShort")]
    public string? TextShort { get; set; }

    [JsonPropertyName("references")]
    public IEnumerable<JourneyMessageReference>? References { get; set; }

    internal override MessageType Type => MessageType.Disruption;
}