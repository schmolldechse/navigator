using Navigator.Api.DTOs.Statistics.DataPoint;
using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Statistics;

[JsonDerivedType(typeof(TimestampDataPoint), typeDiscriminator: "timestamp")]
[JsonDerivedType(typeof(TransportTypeDataPoint), typeDiscriminator: "transportType")]
[JsonDerivedType(typeof(TimestampTransportTypeDataPoint), typeDiscriminator: "timestampTransportType")]
[JsonDerivedType(typeof(StationDataPoint), typeDiscriminator: "evaNumber")]
[JsonDerivedType(typeof(CategoryDataPoint), typeDiscriminator: "category")]
[JsonDerivedType(typeof(TimestampCategoryDataPoint), typeDiscriminator: "timestampCategory")]
[JsonDerivedType(typeof(TimestampStationTransportTypeDataPoint), typeDiscriminator: "timestampStationTransportType")]
[JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToBaseType)]
public class BaseMetricDataPoint
{
    [JsonPropertyName("value")]
    public required decimal Value { get; set; }
}
