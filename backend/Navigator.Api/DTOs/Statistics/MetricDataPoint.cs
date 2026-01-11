using Navigator.Api.DTOs.Statistics.DataPoint;
using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Statistics;

[JsonDerivedType(typeof(TimestampMetricDataPoint), typeDiscriminator: "timestamp")]
[JsonDerivedType(typeof(TransportTypeMetricDataPoint), typeDiscriminator: "transportType")]
[JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToBaseType)]
public class MetricDataPoint
{
    [JsonPropertyName("value")]
    public required decimal Value { get; set; }
}
