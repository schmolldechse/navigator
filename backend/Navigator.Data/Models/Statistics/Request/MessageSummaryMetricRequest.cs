using Navigator.Data.Enums;
using Navigator.Data.Enums.Metric;

namespace Navigator.Data.Models.Statistics.Request;

public class MessageSummaryMetricRequest(MetricSeriesType metricSeriesType) : BaseMetricRequest
{
    public override MetricSeriesType MetricSeriesType { get; } = metricSeriesType;

    public required DateTimeOffset Start { get; set; }
    public required DateTimeOffset End { get; set; }
    public TransportType[]? TransportTypes { get; set; }
    public MessageType[]? MessageTypes { get; set; }
    public int[]? EvaNumbers { get; set; }
    public int Limit { get; set; } = 15;
}
