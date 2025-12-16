using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Timetable;

public class TimetableDeparture
{
    [JsonPropertyName("journeyId")]
    public required string JourneyId { get; set; }

    [JsonPropertyName("administration")]
    public required TimetableEntryAdministration Administration { get; set; }

    [JsonPropertyName("transport")]
    public required TimetableEntryTransport Transport { get; set; }

    [JsonPropertyName("destination")]
    public required TimetableEntryRichStopPlace Destination { get; set; }

    [JsonPropertyName("differingDestination")]
    public TimetableEntryRichStopPlace? DifferingDestination { get; set; }

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
    public IEnumerable<TimetableEntryCoupledTransport>? TravelsWith { get; set; }
}
