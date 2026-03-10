using Navigator.Data.Enums;
using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Statistics.DataPoint;

public class TransportTypeDataPoint : BaseMetricDataPoint
{
    [JsonPropertyName("transportType")]
    public required TransportType TransportType { get; set; }
}
