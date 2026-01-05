namespace Navigator.Data.Models.Statistics;

public class MetricDataSummary
{
    public required decimal StartValue { get; set; }
    public required decimal EndValue { get; set; }
    public required decimal MinValue { get; set; }
    public required decimal MaxValue { get; set; }
    public required decimal AbsoluteChange { get; set; }
}
