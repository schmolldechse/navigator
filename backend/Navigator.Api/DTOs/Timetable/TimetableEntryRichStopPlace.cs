using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Timetable;

public class TimetableEntryRichStopPlace : TimetableEntryStopPlace
{
    [JsonPropertyName("cancelled")]
    public required bool Cancelled { get; set; }

    [JsonPropertyName("additional")]
    public bool? Additional { get; set; }
}
