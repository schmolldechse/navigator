using Navigator.Data.Enums.Metric;

namespace Navigator.Data.Models.Statistics.Request;

public class BaseMetricRequest
{
    public required MetricQueryType MetricQueryType { get; set; }
}
