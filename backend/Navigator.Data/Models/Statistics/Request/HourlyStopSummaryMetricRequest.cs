using Navigator.Data.Enums;
using Navigator.Data.Enums.Metric;

namespace Navigator.Data.Models.Statistics.Request;

public class HourlyStopSummaryMetricRequest : BaseMetricRequest
{
    public override MetricQueryType MetricQueryType => MetricQueryType.HourlyStopSummary;
    public required DateTimeOffset Start { get; set; }
    public required DateTimeOffset End { get; set; }
    public TransportType[]? TransportTypes { get; set; }
}
