using Navigator.Api.DTOs.Statistics.Request;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Statistics;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "queryType")]
[JsonDerivedType(typeof(DatabaseSizeSnapshotMetricRequest), "DATABASE_SIZE_SNAPSHOT")]
[JsonDerivedType(typeof(RisIdSnapshotMetricRequest), "RIS_ID_SNAPSHOT")]
[JsonDerivedType(typeof(JourneySnapshotMetricRequest), "JOURNEY_SNAPSHOT")]
[JsonDerivedType(typeof(TransportTypeDistributionMetricRequest), "TRANSPORT_TYPE_DISTRIBUTION")]
[JsonDerivedType(typeof(GlobalTransportQualityMetricRequest), "GLOBAL_TRANSPORT_QUALITY")]
[JsonDerivedType(typeof(StationQualityMetricRequest), "STATION_QUALITY")]
[JsonDerivedType(typeof(AdministrationRankingMetricRequest), "ADMINISTRATION_RANKING")]
[JsonDerivedType(typeof(LineRankingMetricRequest), "LINE_RANKING")]
public abstract class BaseMetricRequest : IValidatableObject
{
    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext) { yield break; }
}
