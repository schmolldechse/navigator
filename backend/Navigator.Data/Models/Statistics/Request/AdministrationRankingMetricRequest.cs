using System.ComponentModel.DataAnnotations;
using Navigator.Data.Enums.Metric;

namespace Navigator.Data.Models.Statistics.Request;

public class AdministrationRankingMetricRequest : BaseMetricRequest
{
    public override MetricSeriesType MetricSeriesType { get; }

    public required DateTimeOffset Start { get; set; }
    public required DateTimeOffset End { get; set; }
    public int[] EvaNumber { get; set; } = [];

    [Range(1, 500)]
    public int Limit { get; set; } = 500;
    public int Offset { get; set; } = 0;

    public AdministrationRankingMetricRequest(MetricSeriesType metricSeriesType)
    {
        MetricSeriesType = metricSeriesType;
    }
}
