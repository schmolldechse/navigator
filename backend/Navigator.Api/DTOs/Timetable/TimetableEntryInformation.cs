using Navigator.Data.Enums;
using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Timetable;

public class TimetableEntryInformation
{
    [JsonPropertyName("type")]
    public required InformationType Type { get; set; }

    [JsonPropertyName("text")]
    public required string Text { get; set; }

    [JsonPropertyName("textShort")]
    public string? TextShort { get; set; }
}
