using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Navigator.Data.Enums.Metric;
using Navigator.Data.Models.Statistics.Request;

namespace Navigator.Data.Models.Statistics;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "queryType")]
[JsonDerivedType(typeof(DatabaseSizeSnapshotMetricRequest), "DATABASE_SIZE_SNAPSHOT")]
[JsonDerivedType(typeof(RisIdSnapshotMetricRequest), "RIS_ID_SNAPSHOT")]
[JsonDerivedType(typeof(JourneySnapshotMetricRequest), "JOURNEY_SNAPSHOT")]
[JsonDerivedType(typeof(NetworkStationEventQualityTimeSeriesMetricRequest), "NETWORK_STATION_EVENT_QUALITY_TIME_SERIES")]
[JsonDerivedType(typeof(StationEventQualityTimeSeriesMetricRequest), "STATION_EVENT_QUALITY_TIME_SERIES")]
[JsonDerivedType(typeof(StationEventQualitySummaryMetricRequest), "STATION_EVENT_QUALITY_SUMMARY")]
[JsonDerivedType(typeof(AdministrationRankingMetricRequest), "ADMINISTRATION_RANKING")]
[JsonDerivedType(typeof(LineRankingMetricRequest), "LINE_RANKING")]
public abstract class BaseMetricRequest : IValidatableObject
{
    public abstract MetricSeriesType SeriesType { get; set; }

    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext) { yield break; }
}

public interface IEvaNumberMetricRequest
{
    int[] EvaNumbers { get; set; }
    bool IncludeRil100 { get; set; }
}
