using Navigator.Data.Enums;

namespace Navigator.Data.Models.Statistics.DataPoint;

public class TimestampTransportTypeMetricDataPoint : BaseMetricDataPoint
{
    public required DateTimeOffset Timestamp { get; set; }
    public required TransportType TransportType { get; set; }
}
