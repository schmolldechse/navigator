using Navigator.Data.Enums;

namespace Navigator.Data.Models.Statistics.DataPoint;

public class TransportTypeMetricDataPoint : BaseMetricDataPoint
{
    public required TransportType TransportType { get; set; }
}
