namespace Navigator.Data.Models.Statistics.DataPoint;

public class TimestampCategoryDataPoint : BaseMetricDataPoint
{
    public required DateTimeOffset Timestamp { get; set; }
    public required string Category { get; set; }
}
