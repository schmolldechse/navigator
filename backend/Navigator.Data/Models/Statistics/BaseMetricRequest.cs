using Navigator.Data.Enums.Metric;

namespace Navigator.Data.Models.Statistics;

public abstract class BaseMetricRequest
{
    public abstract MetricQueryType MetricQueryType { get; }
}
