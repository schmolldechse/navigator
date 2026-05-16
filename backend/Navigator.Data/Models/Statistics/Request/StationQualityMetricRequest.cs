using Navigator.Data.Enums;
using Navigator.Data.Enums.Metric;

namespace Navigator.Data.Models.Statistics.Request;

public class StationQualityMetricRequest : BaseMetricRequest
{
    public override MetricSeriesType MetricSeriesType { get; }

    public DateTimeOffset? Start { get; set; }
    public required DateTimeOffset End { get; set; }
    public TransportType[]? TransportTypes { get; set; }
    public int[]? EvaNumbers { get; set; }

    public StationQualityMetricRequest(MetricSeriesType metricSeriesType)
    {
        MetricSeriesType = metricSeriesType;
    }

    public MetricUnit GetMetricUnit() => MetricSeriesType switch
    {
        MetricSeriesType.StationEventArrivalDelayAverage => MetricUnit.Seconds,
        MetricSeriesType.StationEventArrivalCancellationRate => MetricUnit.Percent,
        MetricSeriesType.StationEventArrivalPunctuality5Rate => MetricUnit.Percent,
        MetricSeriesType.StationEventArrivalPunctuality15Rate => MetricUnit.Percent,
        MetricSeriesType.StationEventDepartureDelayAverage => MetricUnit.Seconds,
        MetricSeriesType.StationEventDepartureCancellationRate => MetricUnit.Percent,
        MetricSeriesType.StationEventDeparturePunctuality5Rate => MetricUnit.Percent,
        MetricSeriesType.StationEventDeparturePunctuality15Rate => MetricUnit.Percent,
        _ => MetricUnit.Count,
    };
}
