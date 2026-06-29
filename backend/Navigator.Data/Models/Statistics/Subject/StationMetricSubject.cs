using System.Text.Json.Serialization;

namespace Navigator.Data.Models.Statistics.Subject;

public class StationMetricSubject
{
    [JsonPropertyName("evaNumber")]
    public required int EvaNumber { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
