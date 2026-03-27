using Navigator.Data.Enums.Metric;

namespace Navigator.Data.Models.Statistics.Request;

public class RisIdSnapshotMetricRequest : BaseMetricRequest
{
    public override MetricSeriesType MetricSeriesType { get; }

    public required DateTimeOffset Start { get; set; }
    public required DateTimeOffset End { get; set; }

    public RisIdSnapshotMetricRequest(MetricSeriesType metricSeriesType)
    {
        MetricSeriesType = metricSeriesType;
    }
}
