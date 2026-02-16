namespace Navigator.Data.Models.Statistics;

public class MetricSummary
{
    public decimal? StartValue { get; set; }
    public decimal? EndValue { get; set; }
    public required decimal MinValue { get; set; }
    public required decimal MaxValue { get; set; }
    public required decimal AbsoluteChange { get; set; }
}
