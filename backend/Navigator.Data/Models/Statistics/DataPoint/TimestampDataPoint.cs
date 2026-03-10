namespace Navigator.Data.Models.Statistics.DataPoint;

public class TimestampDataPoint : BaseMetricDataPoint
{
    public required DateTimeOffset Timestamp { get; set; }
}
