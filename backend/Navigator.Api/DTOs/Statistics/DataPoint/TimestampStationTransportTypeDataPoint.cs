using Navigator.Data.Enums;
using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Statistics.DataPoint;

public class TimestampStationTransportTypeDataPoint : BaseMetricDataPoint
{
    [JsonPropertyName("timestamp")]
    public required DateTimeOffset Timestamp { get; set; }

    [JsonPropertyName("evaNumber")]
    public required int EvaNumber { get; set; }

    [JsonPropertyName("transportType")]
    public required TransportType TransportType { get; set; }
}
