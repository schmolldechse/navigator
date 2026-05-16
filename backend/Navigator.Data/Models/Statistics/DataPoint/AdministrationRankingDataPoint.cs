using Navigator.Data.Models.Statistics.Subject;

namespace Navigator.Data.Models.Statistics.DataPoint;

public class AdministrationRankingDataPoint : BaseMetricDataPoint
{
    public required AdministrationMetricSubject Administration { get; set; }
}
