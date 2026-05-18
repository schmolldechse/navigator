using System.Text.Json.Serialization;
using Navigator.Data.Models.Statistics.DataPoint;

namespace Navigator.Data.Models.Statistics;

[JsonDerivedType(typeof(TimestampDataPoint), typeDiscriminator: "timestamp")]
[JsonDerivedType(typeof(TransportTypeDataPoint), typeDiscriminator: "transportType")]
[JsonDerivedType(typeof(TimestampTransportTypeDataPoint), typeDiscriminator: "timestampTransportType")]
[JsonDerivedType(typeof(TimestampStationTransportTypeDataPoint), typeDiscriminator: "timestampStationTransportType")]
[JsonDerivedType(typeof(StationDataPoint), typeDiscriminator: "station")]
[JsonDerivedType(typeof(AdministrationRankingDataPoint), typeDiscriminator: "administrationRanking")]
[JsonDerivedType(typeof(LineRankingDataPoint), typeDiscriminator: "lineRanking")]
[JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToBaseType)]
public abstract class BaseMetricDataPoint
{
    [JsonPropertyName("value")]
    public required decimal Value { get; set; }

    [JsonPropertyName("sample")]
    public MetricSample? Sample { get; set; }
}
