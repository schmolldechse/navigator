using Microsoft.EntityFrameworkCore;
using Navigator.Data.Enums;
using Navigator.Data.Enums.Metric;
using Navigator.Data.Models.Statistics;
using Navigator.Data.Models.Statistics.DataPoint;
using Navigator.Data.Models.Statistics.Request;

namespace Navigator.Data.Repository.StatisticsRepository.MetricSeriesBuilders;

public sealed class GlobalTransportQualityMetricSeriesBuilder(
    DataContext dataContext
) : MetricSeriesBuilder<GlobalTransportQualityMetricRequest>
{
    private static readonly IReadOnlyDictionary<MetricSeriesType, MetricDefinition> Definitions =
        new Dictionary<MetricSeriesType, MetricDefinition>
        {
            [MetricSeriesType.GlobalStationEventArrivalCount] = new(
                ScheduleType.Arrival,
                MetricUnit.Count,
                result => result.EventCount),
            [MetricSeriesType.GlobalStationEventArrivalCancellationCount] = new(
                ScheduleType.Arrival,
                MetricUnit.Count,
                result => result.CancellationCount),
            [MetricSeriesType.GlobalStationEventArrivalCancellationRate] = new(
                ScheduleType.Arrival,
                MetricUnit.Percent,
                result => result.EventCount == 0 ? 0 : 100m * result.CancellationCount / result.EventCount,
                result => new() { Numerator = result.CancellationCount, Denominator = result.EventCount }),
            [MetricSeriesType.GlobalStationEventArrivalDelaySum] = new(
                ScheduleType.Arrival,
                MetricUnit.Seconds,
                result => result.DelaySum),
            [MetricSeriesType.GlobalStationEventArrivalPunctuality5Rate] = new(
                ScheduleType.Arrival,
                MetricUnit.Percent,
                result => result.DelaySampleCount == 0 ? 0 : 100m * result.Punctual5Count / result.DelaySampleCount,
                result => new() { Numerator = result.Punctual5Count, Denominator = result.DelaySampleCount }),
            [MetricSeriesType.GlobalStationEventArrivalPunctuality15Rate] = new(
                ScheduleType.Arrival,
                MetricUnit.Percent,
                result => result.DelaySampleCount == 0 ? 0 : 100m * result.Punctual15Count / result.DelaySampleCount,
                result => new() { Numerator = result.Punctual15Count, Denominator = result.DelaySampleCount }),

            [MetricSeriesType.GlobalStationEventDepartureCount] = new(
                ScheduleType.Departure,
                MetricUnit.Count,
                result => result.EventCount),
            [MetricSeriesType.GlobalStationEventDepartureCancellationCount] = new(
                ScheduleType.Departure,
                MetricUnit.Count,
                result => result.CancellationCount),
            [MetricSeriesType.GlobalStationEventDepartureCancellationRate] = new(
                ScheduleType.Departure,
                MetricUnit.Percent,
                result => result.EventCount == 0 ? 0 : 100m * result.CancellationCount / result.EventCount,
                result => new() { Numerator = result.CancellationCount, Denominator = result.EventCount }),
            [MetricSeriesType.GlobalStationEventDepartureDelaySum] = new(
                ScheduleType.Departure,
                MetricUnit.Seconds,
                result => result.DelaySum),
            [MetricSeriesType.GlobalStationEventDeparturePunctuality5Rate] = new(
                ScheduleType.Departure,
                MetricUnit.Percent,
                result => result.DelaySampleCount == 0 ? 0 : 100m * result.Punctual5Count / result.DelaySampleCount,
                result => new() { Numerator = result.Punctual5Count, Denominator = result.DelaySampleCount }),
            [MetricSeriesType.GlobalStationEventDeparturePunctuality15Rate] = new(
                ScheduleType.Departure,
                MetricUnit.Percent,
                result => result.DelaySampleCount == 0 ? 0 : 100m * result.Punctual15Count / result.DelaySampleCount,
                result => new() { Numerator = result.Punctual15Count, Denominator = result.DelaySampleCount })
        };

    protected override async Task<MetricSeries> BuildAsync(GlobalTransportQualityMetricRequest request)
    {
        var definition = GetDefinition(request.MetricSeriesType);
        request.TransportTypes = (request.TransportTypes is null || request.TransportTypes.Length == 0)
            ? Enum.GetValues<TransportType>()
            : request.TransportTypes;

        var query = dataContext.StationEventQualities
            .AsNoTracking()
            .Where(summary => summary.BucketHour >= request.Start.UtcDateTime && summary.BucketHour <= request.End.UtcDateTime)
            .Where(summary => request.TransportTypes.Contains(summary.TransportType));

        var results = (await query.ToListAsync())
            .GroupBy(summary => new
            {
                BucketHour = summary.BucketHour.AddHours(-(summary.BucketHour.Hour % request.Stepping)),
                summary.TransportType,
                summary.ScheduleType
            })
            .Select(group => new GlobalTransportQualityBucket
            {
                BucketHour = group.Key.BucketHour,
                TransportType = group.Key.TransportType,
                ScheduleType = group.Key.ScheduleType,
                EventCount = group.Sum(summary => summary.EventCount),
                CancellationCount = group.Sum(summary => summary.CancelledCount),
                DelaySampleCount = group.Sum(summary => summary.DelaySampleCount),
                DelaySum = group.Sum(summary => summary.DelaySumSeconds),
                Punctual5Count = group.Sum(summary => summary.Punctual5Count),
                Punctual15Count = group.Sum(summary => summary.Punctual15Count)
            })
            .OrderBy(element => element.BucketHour)
            .ToList();

        var dataPoints = results
            .Where(result => result.ScheduleType == definition.ScheduleType)
            .Select(result => new TimestampTransportTypeDataPoint
            {
                Timestamp = result.BucketHour,
                TransportType = result.TransportType,
                Value = definition.GetValue(result),
                Sample = definition.CreateSample?.Invoke(result)
            })
            .ToList();

        return new()
        {
            SeriesType = request.MetricSeriesType,
            Unit = definition.Unit,
            DataPoints = dataPoints
        };
    }

    private static MetricDefinition GetDefinition(MetricSeriesType seriesType) =>
        Definitions.TryGetValue(seriesType, out var definition)
            ? definition
            : throw new NotSupportedException($"Unsupported hourly transport metric series type: {seriesType}");

    private sealed record MetricDefinition(
        ScheduleType ScheduleType,
        MetricUnit Unit,
        Func<GlobalTransportQualityBucket, decimal> GetValue,
        Func<GlobalTransportQualityBucket, MetricSample?>? CreateSample = null);

    private sealed class GlobalTransportQualityBucket
    {
        public required DateTime BucketHour { get; init; }
        public required TransportType TransportType { get; init; }
        public required ScheduleType ScheduleType { get; init; }
        public required long EventCount { get; init; }
        public required long CancellationCount { get; init; }
        public required long DelaySampleCount { get; init; }
        public required long DelaySum { get; init; }
        public required long Punctual5Count { get; init; }
        public required long Punctual15Count { get; init; }
    }
}
