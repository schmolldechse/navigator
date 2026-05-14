using Navigator.Data.Enums;

namespace Navigator.Data.Models.Statistics.DataPoint;

public class TimestampStationTransportTypeDataPoint : BaseMetricDataPoint
{
    public required DateTimeOffset Timestamp { get; set; }
    public required int EvaNumber { get; set; }
    public required TransportType TransportType { get; set; }
}
