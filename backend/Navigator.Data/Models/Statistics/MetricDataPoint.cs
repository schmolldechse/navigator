namespace Navigator.Data.Models.Statistics;

public class MetricDataPoint
{
    public required DateTimeOffset Timestamp { get; set; }
    public required decimal Value { get; set; }
}
