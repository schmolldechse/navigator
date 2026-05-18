using System.Text.Json.Serialization;
using Navigator.Data.Enums;

namespace Navigator.Data.Models.Statistics.DataPoint;

public class TransportTypeDataPoint : BaseMetricDataPoint
{
    [JsonPropertyName("transportType")]
    public required TransportType TransportType { get; set; }
}
