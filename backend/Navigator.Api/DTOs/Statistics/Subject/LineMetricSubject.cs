using Navigator.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Statistics.Subject;

public class LineMetricSubject
{
    [JsonPropertyName("number")]
    public required int Number { get; set; }

    [MaxLength(64)]
    [JsonPropertyName("journeyDescription")]
    public required string JourneyDescription { get; set; }

    [JsonPropertyName("transportType")]
    public required TransportType TransportType { get; set; }
}
