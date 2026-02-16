using Navigator.Data.Enums.Metric;

namespace Navigator.Data.Models.Statistics.Request;

public class JourneySnapshotMetricRequest : BaseMetricRequest
{
    public override MetricQueryType MetricQueryType => MetricQueryType.JourneySnapshot;
    public required DateTimeOffset Start { get; set; }
    public required DateTimeOffset End { get; set; }
}
