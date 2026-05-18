using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Navigator.Data.Enums;

namespace Navigator.Data.Models.Statistics.Subject;

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
