using Navigator.Data.Enums;
using Navigator.Data.Enums.Metric;

namespace Navigator.Data.Models.Statistics.Request;

public class StationSummaryMetricRequest : BaseMetricRequest
{
    public override MetricSeriesType MetricSeriesType { get; }

    public DateTimeOffset? Start { get; set; }
    public required DateTimeOffset End { get; set; }
    public TransportType[]? TransportTypes { get; set; }
    public int[]? EvaNumbers { get; set; }

    public StationSummaryMetricRequest(MetricSeriesType metricSeriesType)
    {
        MetricSeriesType = metricSeriesType;
    }

    public MetricUnit GetMetricUnit() => MetricSeriesType switch
    {
        MetricSeriesType.StationArrivalDelayAvg => MetricUnit.Seconds,
        MetricSeriesType.StationDepartureDelayAvg => MetricUnit.Seconds,
        _ => MetricUnit.Count,
    };
}
