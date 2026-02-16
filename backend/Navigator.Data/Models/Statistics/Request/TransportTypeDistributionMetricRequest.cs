using Navigator.Data.Enums;
using Navigator.Data.Enums.Metric;

namespace Navigator.Data.Models.Statistics.Request;

public class TransportTypeDistributionMetricRequest : BaseMetricRequest
{
    public override MetricQueryType MetricQueryType => MetricQueryType.TransportTypeDistribution;
    public required DateTimeOffset End { get; set; }
    public TransportType[]? TransportTypes { get; set; }
}
