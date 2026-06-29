using System.Text.Json.Serialization;
using Navigator.Data.Enums;

namespace Navigator.Api.DTOs.Journey;

public class JourneyTransport
{
    [JsonPropertyName("type")]
    public required TransportType Type { get; set; }

    [JsonPropertyName("replacementType")]
    public TransportType? ReplacementType { get; set; }

    [JsonPropertyName("category")]
    public string? Category { get; set; }

    [JsonPropertyName("categoryInternal")]
    public string? CategoryInternal { get; set; }

    [JsonPropertyName("journeyDescription")]
    public required string JourneyDescription { get; set; }

    [JsonPropertyName("label")]
    public string? Label { get; set; }

    [JsonPropertyName("line")]
    public string? Line { get; set; }

    [JsonPropertyName("number")]
    public required int Number { get; set; }
}