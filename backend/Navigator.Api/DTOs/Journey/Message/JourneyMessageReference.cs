using System.Text.Json.Serialization;
using Navigator.Api.Enums;

namespace Navigator.Api.DTOs.Journey.Message;

public class JourneyMessageReference
{
    [JsonPropertyName("referenceType")]
    public required MessageReferenceType ReferenceType { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("label")]
    public string? Label { get; set; }
}