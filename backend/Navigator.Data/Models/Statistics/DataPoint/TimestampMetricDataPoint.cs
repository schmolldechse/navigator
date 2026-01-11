namespace Navigator.Data.Models.Statistics.DataPoint;

public class TimestampMetricDataPoint : MetricDataPoint
{
    public required DateTimeOffset Timestamp { get; set; }
}
