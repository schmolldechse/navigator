namespace Navigator.Data.Models.Statistics.Request;

public class MetricTimeRequest : BaseMetricRequest
{
    public required DateTimeOffset Start { get; set; }
    public required DateTimeOffset End { get; set; }

    public bool CumulativeValues { get; set; } = true;
}
