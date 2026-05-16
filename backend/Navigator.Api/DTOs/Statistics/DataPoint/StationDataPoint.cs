using System.Text.Json.Serialization;
using Navigator.Api.DTOs.Statistics.Subject;

namespace Navigator.Api.DTOs.Statistics.DataPoint;

public class StationDataPoint : BaseMetricDataPoint
{
    [JsonPropertyName("station")]
    public required StationMetricSubject Station { get; set; }
}