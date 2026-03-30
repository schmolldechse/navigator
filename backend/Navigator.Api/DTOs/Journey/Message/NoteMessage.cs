using System.Text.Json.Serialization;
using Navigator.Data.Enums;

namespace Navigator.Api.DTOs.Journey.Message;

public class NoteMessage : JourneyMessage
{
    [JsonPropertyName("category")]
    public string? Category { get; set; }

    [JsonPropertyName("textShort")]
    public string? TextShort { get; set; }

    [JsonPropertyName("references")]
    public IEnumerable<JourneyMessageReference>? References { get; set; } = [];

    internal override MessageType Type => MessageType.Note;
}