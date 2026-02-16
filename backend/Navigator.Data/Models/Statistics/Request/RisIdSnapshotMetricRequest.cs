using Navigator.Data.Enums.Metric;

namespace Navigator.Data.Models.Statistics.Request;

public class RisIdSnapshotMetricRequest : BaseMetricRequest
{
    public override MetricQueryType MetricQueryType => MetricQueryType.RisIdSnapshot;
    public required DateTimeOffset Start { get; set; }
    public required DateTimeOffset End { get; set; }
}
