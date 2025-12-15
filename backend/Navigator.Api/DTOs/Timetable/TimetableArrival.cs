using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Timetable;

public class TimetableArrival
{
    [JsonPropertyName("journeyId")]
    public required string JourneyId { get; set; }

    [JsonPropertyName("administration")]
    public required TimetableEntryAdministration Administration { get; set; }

    [JsonPropertyName("transport")]
    public required TimetableEntryTransport Transport { get; set; }

    [JsonPropertyName("origin")]
    public required TimetableEntryRichStopPlace Origin { get; set; }

    [JsonPropertyName("differingOrigin")]
    public TimetableEntryRichStopPlace? DifferingOrigin { get; set; }

    [JsonPropertyName("direction")]
    public required IEnumerable<TimetableEntryStopPlace> Direction { get; set; } = [];

    [JsonPropertyName("viaStops")]
    public required IEnumerable<TimetableEntryRichStopPlace> ViaStops { get; set; } = [];

    [JsonPropertyName("schedule")]
    public required TimetableEntrySchedule Schedule { get; set; }

    [JsonPropertyName("informations")]
    public required IEnumerable<TimetableEntryInformation> Informations { get; set; } = [];

    [JsonPropertyName("cancelled")]
    public required bool Cancelled { get; set; }

    [JsonPropertyName("additional")]
    public bool? Additional { get; set; }

    [JsonPropertyName("demand")]
    public bool? Demand { get; set; }

    [JsonPropertyName("travelsWith")]
    public IEnumerable<string>? TravelsWith { get; set; }
}
