using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Statistics.DataPoint;

public class StationDataPoint : BaseMetricDataPoint
{
    [JsonPropertyName("evaNumber")]
    public required int EvaNumber { get; set; }
}