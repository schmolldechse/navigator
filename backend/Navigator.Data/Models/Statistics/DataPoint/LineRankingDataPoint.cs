using Navigator.Data.Models.Statistics.Subject;

namespace Navigator.Data.Models.Statistics.DataPoint;

public class LineRankingDataPoint : BaseMetricDataPoint
{
    public required LineMetricSubject Line { get; set; }
    public required AdministrationMetricSubject Administration { get; set; }
    public required StationMetricSubject StartStation { get; set; }
    public required StationMetricSubject EndStation { get; set; }
}
