using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Timetable;

/// <summary>
/// Represents a request to retrieve a timetable for a specific station, including the station's EVA number, the desired
/// date and time, and the duration for which to retrieve timetable data.
/// </summary>
[Description("Represents a request to search for stations using a search term, with optional filters for result count and location types.")]
public class TimetableRequest
{
    [JsonPropertyName("evaNumber")]
    [Required(ErrorMessage = "EvaNumber is required")]
    [Description("The evaNumber to lookup a timetable for")]
    public required int EvaNumber { get; set; }

    [JsonPropertyName("when")]
    [Description("The date and time for which to retrieve the timetable. Defaults to the current date and time if not specified.")]
    public DateTimeOffset? When { get; set; } = DateTimeOffset.UtcNow;

    [JsonPropertyName("duration")]
    [DefaultValue(60)]
    [Description("The duration in minutes for which to retrieve the timetable. Defaults to 60 minutes if not specified.")]
    public int? Duration { get; set; } = 60;
}
