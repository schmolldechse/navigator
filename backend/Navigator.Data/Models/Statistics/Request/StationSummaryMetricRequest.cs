using Navigator.Data.Enums;
using Navigator.Data.Enums.Metric;

namespace Navigator.Data.Models.Statistics.Request;

public class StationSummaryMetricRequest : BaseMetricRequest
{
    public override MetricQueryType MetricQueryType => MetricQueryType.TotalStationSnapshot;
    public required StationSnapshotType SnapshotType { get; set; }
    public DateTimeOffset? Start { get; set; }
    public required DateTimeOffset End { get; set; }
    public TransportType[]? TransportTypes { get; set; }
    public int[]? EvaNumbers { get; set; }

    public (MetricSeriesType SeriesType, MetricUnit Unit) GetMetricMetadata() => SnapshotType switch
    {
        StationSnapshotType.Arrivals => (MetricSeriesType.StationArrivals, MetricUnit.Count),
        StationSnapshotType.ArrivalCancellations => (MetricSeriesType.StationArrivalCancellations, MetricUnit.Count),
        StationSnapshotType.ArrivalDelayAvg => (MetricSeriesType.StationArrivalDelayAvg, MetricUnit.Seconds),
        StationSnapshotType.Departures => (MetricSeriesType.StationDepartures, MetricUnit.Count),
        StationSnapshotType.DepartureCancellations => (MetricSeriesType.StationDepartureCancellations, MetricUnit.Count),
        StationSnapshotType.DepartureDelayAvg => (MetricSeriesType.StationDepartureDelayAvg, MetricUnit.Seconds),
        _ => throw new NotSupportedException($"Unsupported snapshot type: {SnapshotType}")
    };
}
