using Navigator.Data.Enums;
using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Timetable;

public class TimetableEntryTransport
{
    [JsonPropertyName("type")]
    public required TransportType Type { get; set; }

    [JsonPropertyName("replacementType")]
    public TransportType? ReplacementType { get; set; }

    [JsonPropertyName("category")]
    public string? Category { get; set; }

    [JsonPropertyName("categoryInternal")]
    public string? CategoryInternal { get; set; }

    [JsonPropertyName("journeyType")]
    public required JourneyType JourneyType { get; set; }

    [JsonPropertyName("journeyDescription")]
    public required string JourneyDescription { get; set; }

    [JsonPropertyName("number")]
    public required int Number { get; set; }

    [JsonPropertyName("line")]
    public string? Line { get; set; }
}
