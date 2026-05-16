using Navigator.Data.Enums;
using Navigator.Data.Enums.Metric;

namespace Navigator.Data.Models.Statistics.Request;

public class GlobalTransportQualityMetricRequest : BaseMetricRequest
{
    public override MetricSeriesType MetricSeriesType { get; }

    public required DateTimeOffset Start { get; set; }
    public required DateTimeOffset End { get; set; }
    public TransportType[]? TransportTypes { get; set; }
    public int Stepping { get; set; } = 1;

    public GlobalTransportQualityMetricRequest(MetricSeriesType metricSeriesType)
    {
        MetricSeriesType = metricSeriesType;
    }
}
