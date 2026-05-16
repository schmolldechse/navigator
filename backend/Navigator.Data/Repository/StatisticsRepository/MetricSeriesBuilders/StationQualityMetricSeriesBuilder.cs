using Microsoft.EntityFrameworkCore;
using Navigator.Data.Enums;
using Navigator.Data.Enums.Metric;
using Navigator.Data.Models.Statistics;
using Navigator.Data.Models.Statistics.DataPoint;
using Navigator.Data.Models.Statistics.Request;
using Navigator.Data.Models.Statistics.Subject;

namespace Navigator.Data.Repository.StatisticsRepository.MetricSeriesBuilders;

public sealed class StationQualityMetricSeriesBuilder(
    DataContext dataContext
) : MetricSeriesBuilder<StationQualityMetricRequest>
{
    private static readonly IReadOnlyDictionary<MetricSeriesType, MetricDefinition> Definitions =
        new Dictionary<MetricSeriesType, MetricDefinition>
        {
            [MetricSeriesType.StationEventArrivalCount] = new(
                MetricUnit.Count,
                row => row.ArrivalEventCount),
            [MetricSeriesType.StationEventArrivalCancellationCount] = new(
                MetricUnit.Count,
                row => row.ArrivalCancellationCount),
            [MetricSeriesType.StationEventArrivalCancellationRate] = new(
                MetricUnit.Percent,
                row => row.ArrivalEventCount == 0 ? 0 : 100m * row.ArrivalCancellationCount / row.ArrivalEventCount,
                row => new() { Numerator = row.ArrivalCancellationCount, Denominator = row.ArrivalEventCount }),
            [MetricSeriesType.StationEventArrivalDelayAverage] = new(
                MetricUnit.Seconds,
                row => row.ArrivalDelaySampleCount == 0 ? 0 : (decimal)row.ArrivalDelaySumSeconds / row.ArrivalDelaySampleCount,
                row => new() { Numerator = row.ArrivalDelaySumSeconds, Denominator = row.ArrivalDelaySampleCount }),
            [MetricSeriesType.StationEventArrivalPunctuality5Rate] = new(
                MetricUnit.Percent,
                row => row.ArrivalDelaySampleCount == 0 ? 0 : 100m * row.ArrivalPunctual5Count / row.ArrivalDelaySampleCount,
                row => new() { Numerator = row.ArrivalPunctual5Count, Denominator = row.ArrivalDelaySampleCount }),
            [MetricSeriesType.StationEventArrivalPunctuality15Rate] = new(
                MetricUnit.Percent,
                row => row.ArrivalDelaySampleCount == 0 ? 0 : 100m * row.ArrivalPunctual15Count / row.ArrivalDelaySampleCount,
                row => new() { Numerator = row.ArrivalPunctual15Count, Denominator = row.ArrivalDelaySampleCount }),
            [MetricSeriesType.StationEventDepartureCount] = new(
                MetricUnit.Count,
                row => row.DepartureEventCount),
            [MetricSeriesType.StationEventDepartureCancellationCount] = new(
                MetricUnit.Count,
                row => row.DepartureCancellationCount),
            [MetricSeriesType.StationEventDepartureCancellationRate] = new(
                MetricUnit.Percent,
                row => row.DepartureEventCount == 0 ? 0 : 100m * row.DepartureCancellationCount / row.DepartureEventCount,
                row => new() { Numerator = row.DepartureCancellationCount, Denominator = row.DepartureEventCount }),
            [MetricSeriesType.StationEventDepartureDelayAverage] = new(
                MetricUnit.Seconds,
                row => row.DepartureDelaySampleCount == 0 ? 0 : (decimal)row.DepartureDelaySumSeconds / row.DepartureDelaySampleCount,
                row => new() { Numerator = row.DepartureDelaySumSeconds, Denominator = row.DepartureDelaySampleCount }),
            [MetricSeriesType.StationEventDeparturePunctuality5Rate] = new(
                MetricUnit.Percent,
                row => row.DepartureDelaySampleCount == 0 ? 0 : 100m * row.DeparturePunctual5Count / row.DepartureDelaySampleCount,
                row => new() { Numerator = row.DeparturePunctual5Count, Denominator = row.DepartureDelaySampleCount }),
            [MetricSeriesType.StationEventDeparturePunctuality15Rate] = new(
                MetricUnit.Percent,
                row => row.DepartureDelaySampleCount == 0 ? 0 : 100m * row.DeparturePunctual15Count / row.DepartureDelaySampleCount,
                row => new() { Numerator = row.DeparturePunctual15Count, Denominator = row.DepartureDelaySampleCount })
        };

    protected override async Task<MetricSeries> BuildAsync(StationQualityMetricRequest request)
    {
        var definition = GetDefinition(request.MetricSeriesType);
        request.TransportTypes = (request.TransportTypes is null || request.TransportTypes.Length == 0)
            ? Enum.GetValues<TransportType>()
            : request.TransportTypes;

        var query = dataContext.StationEventQualities
            .AsNoTracking()
            .Where(summary => summary.BucketHour <= request.End.UtcDateTime)
            .Where(summary => request.TransportTypes.Contains(summary.TransportType));

        if (request.Start.HasValue)
            query = query.Where(summary => summary.BucketHour >= request.Start.Value.UtcDateTime);
        if (request.EvaNumbers != null && request.EvaNumbers.Length > 0)
            query = query.Where(summary => request.EvaNumbers.Contains(summary.StationEvaNumber));

        var rows = await query
            .GroupBy(summary => summary.StationEvaNumber)
            .Select(group => new StationQualityRow
            {
                EvaNumber = group.Key,
                ArrivalEventCount = group
                    .Where(summary => summary.ScheduleType == ScheduleType.Arrival)
                    .Sum(summary => summary.EventCount),
                ArrivalCancellationCount = group
                    .Where(summary => summary.ScheduleType == ScheduleType.Arrival)
                    .Sum(summary => summary.CancelledCount),
                ArrivalDelaySampleCount = group
                    .Where(summary => summary.ScheduleType == ScheduleType.Arrival)
                    .Sum(summary => summary.DelaySampleCount),
                ArrivalDelaySumSeconds = group
                    .Where(summary => summary.ScheduleType == ScheduleType.Arrival)
                    .Sum(summary => summary.DelaySumSeconds),
                ArrivalPunctual5Count = group
                    .Where(summary => summary.ScheduleType == ScheduleType.Arrival)
                    .Sum(summary => summary.Punctual5Count),
                ArrivalPunctual15Count = group
                    .Where(summary => summary.ScheduleType == ScheduleType.Arrival)
                    .Sum(summary => summary.Punctual15Count),
                DepartureEventCount = group
                    .Where(summary => summary.ScheduleType == ScheduleType.Departure)
                    .Sum(summary => summary.EventCount),
                DepartureCancellationCount = group
                    .Where(summary => summary.ScheduleType == ScheduleType.Departure)
                    .Sum(summary => summary.CancelledCount),
                DepartureDelaySampleCount = group
                    .Where(summary => summary.ScheduleType == ScheduleType.Departure)
                    .Sum(summary => summary.DelaySampleCount),
                DepartureDelaySumSeconds = group
                    .Where(summary => summary.ScheduleType == ScheduleType.Departure)
                    .Sum(summary => summary.DelaySumSeconds),
                DeparturePunctual5Count = group
                    .Where(summary => summary.ScheduleType == ScheduleType.Departure)
                    .Sum(summary => summary.Punctual5Count),
                DeparturePunctual15Count = group
                    .Where(summary => summary.ScheduleType == ScheduleType.Departure)
                    .Sum(summary => summary.Punctual15Count)
            })
            .ToListAsync();

        var stations = await LoadStationsAsync(rows.Select(row => row.EvaNumber));

        var dataPoints = rows
            .Select(row => new StationDataPoint
            {
                Station = GetStationSubject(stations, row.EvaNumber),
                Value = definition.GetValue(row),
                Sample = definition.CreateSample?.Invoke(row)
            })
            .ToList();

        return new()
        {
            SeriesType = request.MetricSeriesType,
            Unit = definition.Unit,
            DataPoints = dataPoints
        };
    }

    private async Task<Dictionary<int, StationMetricSubject>> LoadStationsAsync(IEnumerable<int> evaNumbers)
    {
        var numbers = evaNumbers.Distinct().ToArray();
        return await dataContext.Stations
            .AsNoTracking()
            .Where(station => numbers.Contains(station.EvaNumber))
            .ToDictionaryAsync(
                station => station.EvaNumber,
                station => new StationMetricSubject
                {
                    EvaNumber = station.EvaNumber,
                    Name = station.Name,
                });
    }

    private static StationMetricSubject GetStationSubject(
        IReadOnlyDictionary<int, StationMetricSubject> stations,
        int evaNumber
    ) => stations.TryGetValue(evaNumber, out var station)
        ? station
        : new StationMetricSubject
        {
            EvaNumber = evaNumber
        };

    private static MetricDefinition GetDefinition(MetricSeriesType seriesType) =>
        Definitions.TryGetValue(seriesType, out var definition)
            ? definition
            : throw new NotSupportedException($"Unsupported station metric series type: {seriesType}");

    private sealed record MetricDefinition(
        MetricUnit Unit,
        Func<StationQualityRow, decimal> GetValue,
        Func<StationQualityRow, MetricSample?>? CreateSample = null);

    private sealed class StationQualityRow
    {
        public required int EvaNumber { get; init; }
        public required long ArrivalEventCount { get; init; }
        public required long ArrivalCancellationCount { get; init; }
        public required long ArrivalDelaySampleCount { get; init; }
        public required long ArrivalDelaySumSeconds { get; init; }
        public required long ArrivalPunctual5Count { get; init; }
        public required long ArrivalPunctual15Count { get; init; }
        public required long DepartureEventCount { get; init; }
        public required long DepartureCancellationCount { get; init; }
        public required long DepartureDelaySampleCount { get; init; }
        public required long DepartureDelaySumSeconds { get; init; }
        public required long DeparturePunctual5Count { get; init; }
        public required long DeparturePunctual15Count { get; init; }
    }
}
