using Navigator.Api.DTOs.Statistics.DataPoint;
using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Statistics;

[JsonDerivedType(typeof(TimestampDataPoint), typeDiscriminator: "timestamp")]
[JsonDerivedType(typeof(TransportTypeDataPoint), typeDiscriminator: "transportType")]
[JsonDerivedType(typeof(TimestampTransportTypeDataPoint), typeDiscriminator: "timestampTransportType")]
[JsonDerivedType(typeof(StationDataPoint), typeDiscriminator: "evaNumber")]
[JsonDerivedType(typeof(AdministrationRankingDataPoint), typeDiscriminator: "administrationRanking")]
[JsonDerivedType(typeof(LineRankingDataPoint), typeDiscriminator: "lineRanking")]
[JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToBaseType)]
public class BaseMetricDataPoint
{
    [JsonPropertyName("value")]
    public required decimal Value { get; set; }

    [JsonPropertyName("sample")]
    public MetricSample? Sample { get; set; }
}
