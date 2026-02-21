using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Navigator.Api.DTOs.Statistics.Request;
using Navigator.Data.Enums.Metric;

namespace Navigator.Api.DTOs.Statistics;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "queryType")]
[JsonDerivedType(typeof(DatabaseSizeSnapshotMetricRequest), "DATABASE_SIZE_SNAPSHOT")]
[JsonDerivedType(typeof(RisIdSnapshotMetricRequest), "RIS_ID_SNAPSHOT")]
[JsonDerivedType(typeof(JourneySnapshotMetricRequest), "JOURNEY_SNAPSHOT")]
[JsonDerivedType(typeof(TransportTypeDistributionMetricRequest), "TRANSPORT_TYPE_DISTRIBUTION")]
[JsonDerivedType(typeof(GlobalStopSummaryMetricRequest), "GLOBAL_STOP_SUMMARY")]
public abstract class BaseMetricRequest : IValidatableObject
{
    [JsonIgnore]
    public abstract MetricQueryType MetricQueryType { get; }

    public abstract Navigator.Data.Models.Statistics.BaseMetricRequest BuildRequest();

    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        yield break;
    }
}
