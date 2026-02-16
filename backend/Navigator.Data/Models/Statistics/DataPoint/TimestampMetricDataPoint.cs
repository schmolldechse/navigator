namespace Navigator.Data.Models.Statistics.DataPoint;

public class TimestampMetricDataPoint : BaseMetricDataPoint
{
    public required DateTimeOffset Timestamp { get; set; }
}
