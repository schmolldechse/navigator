using Microsoft.EntityFrameworkCore;
using Navigator.Data.Models.Statistics;
using Navigator.Data.Models.Statistics.DataPoint;
using Navigator.Data.Models.Statistics.Request;
using Navigator.Data.Models.Statistics.Subject;

namespace Navigator.Data.Repository.StatisticsRepository.MetricSeriesBuilders;

public sealed class StationEventQualitySummaryMetricSeriesBuilder(
    DataContext dataContext
) : MetricSeriesBuilder<StationEventQualitySummaryMetricRequest>
{
    protected override async Task<MetricSeries> BuildAsync(StationEventQualitySummaryMetricRequest request)
    {
        var definition = StationEventQualityMetricDefinitions.GetDefinition(request.SeriesType);
        var transportTypes = StationEventQualityMetricDefinitions.NormalizeTransportTypes(request.TransportTypes);

        var query = dataContext.StationLineRouteQualities
            .AsNoTracking()
            .Where(summary => summary.BucketHour >= request.Start.UtcDateTime && summary.BucketHour <= request.End.UtcDateTime)
            .Where(summary => summary.ScheduleType == request.ScheduleType)
            .Where(summary => transportTypes.Contains(summary.TransportType))
            .Where(summary => request.IncludeReplacementTransport || !summary.IsReplacementTransport);

        if (request.EvaNumbers is { Length: > 0 })
            query = query.Where(summary => request.EvaNumbers.Contains(summary.StationEvaNumber));

        var rows = await query
            .GroupBy(summary => summary.StationEvaNumber)
            .Select(group => new StationEventQualitySummaryRow
            {
                EvaNumber = group.Key,
                EventCount = group.Sum(summary => summary.EventCount),
                CancellationCount = group.Sum(summary => summary.CancelledCount),
                DelaySampleCount = group.Sum(summary => summary.DelaySampleCount),
                DelaySumSeconds = group.Sum(summary => summary.DelaySumSeconds),
                Punctual5Count = group.Sum(summary => summary.Punctual5Count),
                Punctual15Count = group.Sum(summary => summary.Punctual15Count)
            })
            .ToListAsync();

        var stations = await LoadStationsAsync(rows.Select(row => row.EvaNumber));

        var dataPoints = rows
            .Select(row =>
            {
                var aggregate = row.ToAggregate();
                return new StationDataPoint
                {
                    Station = GetStationSubject(stations, row.EvaNumber),
                    Value = definition.GetValue(aggregate),
                    Sample = definition.CreateSample?.Invoke(aggregate)
                };
            })
            .ToList();

        return new()
        {
            SeriesType = request.SeriesType,
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

    private sealed class StationEventQualitySummaryRow
    {
        public required int EvaNumber { get; init; }
        public required long EventCount { get; init; }
        public required long CancellationCount { get; init; }
        public required long DelaySampleCount { get; init; }
        public required long DelaySumSeconds { get; init; }
        public required long Punctual5Count { get; init; }
        public required long Punctual15Count { get; init; }

        public StationEventQualityMetricAggregate ToAggregate() => new()
        {
            EventCount = EventCount,
            CancellationCount = CancellationCount,
            DelaySampleCount = DelaySampleCount,
            DelaySumSeconds = DelaySumSeconds,
            Punctual5Count = Punctual5Count,
            Punctual15Count = Punctual15Count
        };
    }
}
