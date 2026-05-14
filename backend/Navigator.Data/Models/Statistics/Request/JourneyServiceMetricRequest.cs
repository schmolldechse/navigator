using Navigator.Data.Enums;
using Navigator.Data.Enums.Metric;

namespace Navigator.Data.Models.Statistics.Request;

public class JourneyServiceMetricRequest(MetricSeriesType metricSeriesType) : BaseMetricRequest
{
    public override MetricSeriesType MetricSeriesType { get; } = metricSeriesType;

    public required DateTimeOffset Start { get; set; }
    public required DateTimeOffset End { get; set; }
    public TransportType[]? TransportTypes { get; set; }
    public JourneyType[]? JourneyTypes { get; set; }
    public string[]? OperatorCodes { get; set; }
}
