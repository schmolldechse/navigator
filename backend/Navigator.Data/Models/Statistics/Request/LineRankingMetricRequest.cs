using Navigator.Data.Enums.Metric;
using System.ComponentModel.DataAnnotations;

namespace Navigator.Data.Models.Statistics.Request;

public class LineRankingMetricRequest : BaseMetricRequest
{
    public override MetricSeriesType MetricSeriesType { get; }

    public required DateTimeOffset Start { get; set; }
    public required DateTimeOffset End { get; set; }
    public int[] EvaNumber { get; set; } = [];
    public string? LineRegex { get; set; }
    public string? NumberRegex { get; set; }

    [Range(1, 500)]
    public int Limit { get; set; } = 500;
    public int Offset { get; set; } = 0;

    public LineRankingMetricRequest(MetricSeriesType metricSeriesType)
    {
        MetricSeriesType = metricSeriesType;
    }
}
