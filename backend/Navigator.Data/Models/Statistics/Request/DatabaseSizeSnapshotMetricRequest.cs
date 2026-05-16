using Navigator.Data.Enums.Metric;

namespace Navigator.Data.Models.Statistics.Request;

public class DatabaseSizeSnapshotMetricRequest : BaseMetricRequest
{
    public override MetricSeriesType MetricSeriesType => MetricSeriesType.DatabaseSizeBytes;
    public required DateTimeOffset Start { get; set; }
    public required DateTimeOffset End { get; set; }
}
