using Navigator.Data.Enums;
using System.Text.Json.Serialization;
using Navigator.Api.Enums;

namespace Navigator.Api.DTOs.Timetable;

public class TimetableEntryInformation
{
    [JsonPropertyName("type")]
    public required InformationType Type { get; set; }
    
    [JsonPropertyName("key")]
    public required MessageKey Key { get; set; }

    [JsonPropertyName("text")]
    public required string Text { get; set; }

    [JsonPropertyName("textShort")]
    public string? TextShort { get; set; }
}
