namespace Navigator.Data.Models.Statistics.Request;

public class MetricCategoricalRequest : BaseMetricRequest
{
    public DateTimeOffset? Start { get; set; }
    public required DateTimeOffset End { get; set; }
}
