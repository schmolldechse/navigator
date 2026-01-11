using Navigator.Data.Enums;
using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Statistics.DataPoint;

public class TransportTypeMetricDataPoint : MetricDataPoint
{
    [JsonPropertyName("transportType")]
    public required TransportType TransportType { get; set; }
}
