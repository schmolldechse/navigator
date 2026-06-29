using System.Text.Json.Serialization;
using Navigator.Api.Enums;
using Navigator.Data.Enums;

namespace Navigator.Api.DTOs.Journey.Message;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(AttributeMessage), "ATTRIBUTE")]
[JsonDerivedType(typeof(DisruptionMessage), "DISRUPTION")]
[JsonDerivedType(typeof(NoteMessage), "NOTE")]
[JsonDerivedType(typeof(RisCauseMessage), "RIS_CAUSE")]
[JsonDerivedType(typeof(RisQualityDeviationMessage), "RIS_QUALITY_DEVIATION")]
public abstract class JourneyMessage
{
    [JsonPropertyName("messageId")]
    public required int MessageId { get; set; }

    [JsonPropertyName("key")]
    public required MessageKey Key { get; set; }

    [JsonPropertyName("text")]
    public required string Text { get; set; }

    [JsonIgnore]
    internal abstract MessageType Type { get; }
}