using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Navigator.Api.DTOs.Journey.Message;
using Navigator.Data.Enums;

namespace Navigator.Api.DTOs.Journey;

public class Journey
{
    [MaxLength(82)]
    [JsonPropertyName("journeyId")]
    public required string JourneyId { get; set; }

    [JsonPropertyName("administration")]
    public required JourneyAdministration Administration { get; set; }

    [JsonPropertyName("type")]
    public required JourneyType Type { get; set; }

    [JsonPropertyName("transport")]
    public required JourneyTransport Transport { get; set; }

    [JsonPropertyName("continuationBy")]
    public IEnumerable<string>? ContinuationBy { get; set; }

    [JsonPropertyName("continuationFor")]
    public IEnumerable<string>? ContinuationFor { get; set; }

    [JsonPropertyName("cancelled")]
    public required bool Cancelled { get; set; }

    [JsonPropertyName("destination")]
    public required JourneyRichStopPlace Destination { get; set; }

    [JsonPropertyName("differingDestination")]
    public JourneyStopPlace? DifferingDestination { get; set; }

    [JsonPropertyName("origin")]
    public required JourneyRichStopPlace Origin { get; set; }

    [JsonPropertyName("differingOrigin")]
    public JourneyStopPlace? DifferingOrigin { get; set; }

    [JsonPropertyName("scheduledEvents")]
    public required IEnumerable<JourneyScheduledEvent> ScheduledEvents { get; set; } = [];

    [JsonPropertyName("messages")]
    public required ICollection<JourneyMessage> Messages { get; set; } = [];
}