using Navigator.Api.DTOs.Statistics.DataPoint;
using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Statistics;

[JsonDerivedType(typeof(TimestampMetricDataPoint), typeDiscriminator: "timestamp")]
[JsonDerivedType(typeof(TransportTypeMetricDataPoint), typeDiscriminator: "transportType")]
[JsonDerivedType(typeof(TimestampTransportTypeMetricDataPoint), typeDiscriminator: "timestampTransportType")]
[JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToBaseType)]
public class BaseMetricDataPoint
{
    [JsonPropertyName("value")]
    public required decimal Value { get; set; }
}
