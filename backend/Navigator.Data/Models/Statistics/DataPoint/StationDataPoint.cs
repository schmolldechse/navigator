using System.Text.Json.Serialization;
using Navigator.Data.Models.Statistics.Subject;

namespace Navigator.Data.Models.Statistics.DataPoint;

public class StationDataPoint : BaseMetricDataPoint
{
    [JsonPropertyName("station")]
    public required StationMetricSubject Station { get; set; }
}
