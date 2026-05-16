using Navigator.Data.Models.Statistics.Subject;

namespace Navigator.Data.Models.Statistics.DataPoint;

public class StationDataPoint : BaseMetricDataPoint
{
    public required StationMetricSubject Station { get; set; }
}