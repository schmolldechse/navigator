using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Timetable;

public class TimetableEntryStopPlace
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("evaNumber")]
    public required int EvaNumber { get; set; }
}
