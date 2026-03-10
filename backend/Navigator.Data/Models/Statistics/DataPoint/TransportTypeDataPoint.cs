using Navigator.Data.Enums;

namespace Navigator.Data.Models.Statistics.DataPoint;

public class TransportTypeDataPoint : BaseMetricDataPoint
{
    public required TransportType TransportType { get; set; }
}
