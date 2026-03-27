using Navigator.Api.DTOs.Statistics.Request;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Statistics;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "queryType")]
[JsonDerivedType(typeof(DatabaseSizeSnapshotMetricRequest), "DATABASE_SIZE_SNAPSHOT")]
[JsonDerivedType(typeof(RisIdSnapshotMetricRequest), "RIS_ID_SNAPSHOT")]
[JsonDerivedType(typeof(JourneySnapshotMetricRequest), "JOURNEY_SNAPSHOT")]
[JsonDerivedType(typeof(TransportTypeDistributionMetricRequest), "TRANSPORT_TYPE_DISTRIBUTION")]
[JsonDerivedType(typeof(HourlyTransportSnapshotMetricRequest), "HOURLY_TRANSPORT_SNAPSHOT")]
[JsonDerivedType(typeof(StationSummaryMetricRequest), "STATION_SUMMARY")]
public abstract class BaseMetricRequest : IValidatableObject
{
    public abstract Navigator.Data.Models.Statistics.BaseMetricRequest BuildRequest();

    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        yield break;
    }
}
