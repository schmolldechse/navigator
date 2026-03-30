using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Navigator.Data.Enums;

namespace Navigator.Api.DTOs.Journey;

public class JourneyScheduledEvent
{
    [JsonPropertyName("stopPlace")]
    public required JourneyStopPlace StopPlace { get; set; }

    [JsonPropertyName("differingStopPlace")]
    public required JourneyStopPlace DifferingStopPlace { get; set; }

    [JsonPropertyName("cancelled")]
    public required bool Cancelled { get; set; }

    [JsonPropertyName("additional")]
    public required bool Additional { get; set; }

    [JsonPropertyName("noPassengerChange")]
    public required bool NoPassengerChange { get; set; }

    [JsonPropertyName("demand")]
    public required bool Demand { get; set; }

    [JsonPropertyName("scheduleType")]
    public required ScheduleType ScheduleType { get; set; }

    [JsonPropertyName("plannedTime")]
    public required DateTimeOffset PlannedTime { get; set; }

    [JsonPropertyName("actualTime")]
    public required DateTimeOffset ActualTime { get; set; }

    [JsonPropertyName("delay")]
    public required int Delay { get; set; }

    [JsonPropertyName("plannedPlatform")]
    public string? PlannedPlatform { get; set; }

    [JsonPropertyName("actualPlatform")]
    public string? ActualPlatform { get; set; }

    [JsonPropertyName("timeType")]
    public required TimeType TimeType { get; set; }

    [JsonPropertyName("travelsWith")]
    public IEnumerable<string>? TravelsWith { get; set; }

    [JsonPropertyName("messageIds")]
    public IEnumerable<int>? MessageIds { get; set; }
}