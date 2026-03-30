using System.Text.Json.Serialization;
using Navigator.Data.Enums;

namespace Navigator.Api.DTOs.Timetable;

public class TimetableEntrySchedule
{
    [JsonPropertyName("plannedTime")]
    public required DateTimeOffset PlannedTime { get; set; }

    [JsonPropertyName("actualTime")]
    public required DateTimeOffset ActualTime { get; set; }

    [JsonPropertyName("delay")]
    public int Delay
    {
        get => (int)(ActualTime - PlannedTime).TotalSeconds;
        private set => _ = value;
    }

    [JsonPropertyName("plannedPlatform")]
    public string? PlannedPlatform { get; set; }

    [JsonPropertyName("actualPlatform")]
    public string? ActualPlatform { get; set; }

    [JsonPropertyName("timeType")]
    public required TimeType TimeType { get; set; }
}
