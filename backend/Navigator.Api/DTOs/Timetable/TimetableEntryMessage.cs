using Navigator.Data.Enums;
using System.Text.Json.Serialization;
using Navigator.Api.Enums;

namespace Navigator.Api.DTOs.Timetable;

public class TimetableEntryMessage
{
    [JsonPropertyName("type")]
    public required MessageType Type { get; set; }

    [JsonPropertyName("key")]
    public required MessageKey Key { get; set; }

    [JsonPropertyName("text")]
    public required string Text { get; set; }

    [JsonPropertyName("textShort")]
    public string? TextShort { get; set; }
}
