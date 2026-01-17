using Navigator.Data.Enums;

namespace Navigator.Data.Models.Statistics.DataPoint;

public class TransportTypeMetricDataPoint : MetricDataPoint
{
    public required TransportType TransportType { get; set; }
}
