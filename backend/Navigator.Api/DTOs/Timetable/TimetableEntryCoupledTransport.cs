using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Timetable;

public class TimetableEntryCoupledTransport
{
    [JsonPropertyName("journeyId")]
    public required string JourneyId { get; set; }

    [JsonPropertyName("separationAt")]
    public TimetableEntryStopPlace? SeparationAt { get; set; }
}
