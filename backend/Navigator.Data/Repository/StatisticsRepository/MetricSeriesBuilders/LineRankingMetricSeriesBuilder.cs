using Microsoft.EntityFrameworkCore;
using Navigator.Data.Enums;
using Navigator.Data.Enums.Metric;
using Navigator.Data.Models.Statistics;
using Navigator.Data.Models.Statistics.DataPoint;
using Navigator.Data.Models.Statistics.Request;
using Navigator.Data.Models.Statistics.Subject;
using System.Text.RegularExpressions;

namespace Navigator.Data.Repository.StatisticsRepository.MetricSeriesBuilders;

public sealed class LineRankingMetricSeriesBuilder(
    DataContext dataContext
) : MetricSeriesBuilder<LineRankingMetricRequest>
{
    private static readonly IReadOnlyDictionary<MetricSeriesType, MetricDefinition> Definitions =
        new Dictionary<MetricSeriesType, MetricDefinition>
        {
            [MetricSeriesType.LineRankingCount] = new(
                MetricUnit.Count,
                query => query.OrderByDescending(row => row.Count),
                row => row.Count),
            [MetricSeriesType.LineRankingCancellationCount] = new(
                MetricUnit.Count,
                query => query.OrderByDescending(row => row.CancellationCount),
                row => row.CancellationCount),
            [MetricSeriesType.LineRankingCancellationRate] = new(
                MetricUnit.Percent,
                query => query.OrderByDescending(row => row.Count == 0 ? 0 : 100m * row.CancellationCount / row.Count),
                row => row.Count == 0 ? 0 : 100m * row.CancellationCount / row.Count,
                row => new() { Numerator = row.CancellationCount, Denominator = row.Count }),
            [MetricSeriesType.LineRankingAverageDelay] = new(
                MetricUnit.Seconds,
                query => query.OrderByDescending(row => row.DelaySampleCount == 0 ? 0 : (decimal)row.DelaySumSeconds / row.DelaySampleCount),
                row => row.DelaySampleCount == 0 ? 0 : (decimal)row.DelaySumSeconds / row.DelaySampleCount,
                row => new() { Numerator = row.DelaySumSeconds, Denominator = row.DelaySampleCount }),
            [MetricSeriesType.LineRankingPunctuality5Rate] = new(
                MetricUnit.Percent,
                query => query.OrderByDescending(row => row.DelaySampleCount == 0 ? 0 : 100m * row.Punctual5Count / row.DelaySampleCount),
                row => row.DelaySampleCount == 0 ? 0 : 100m * row.Punctual5Count / row.DelaySampleCount,
                row => new() { Numerator = row.Punctual5Count, Denominator = row.DelaySampleCount }),
            [MetricSeriesType.LineRankingPunctuality15Rate] = new(
                MetricUnit.Percent,
                query => query.OrderByDescending(row => row.DelaySampleCount == 0 ? 0 : 100m * row.Punctual15Count / row.DelaySampleCount),
                row => row.DelaySampleCount == 0 ? 0 : 100m * row.Punctual15Count / row.DelaySampleCount,
                row => new() { Numerator = row.Punctual15Count, Denominator = row.DelaySampleCount })
        };

    protected override async Task<MetricSeries> BuildAsync(LineRankingMetricRequest request)
    {
        var definition = GetDefinition(request.SeriesType);
        var limit = Math.Clamp(request.Limit, 1, 500);
        var offset = Math.Max(request.Offset, 0);

        var ranking = request.EvaNumbers.Any()
            ? BuildStationLineRankingQuery(request)
            : BuildGlobalLineRankingQuery(request);

        var totalItems = await ranking.CountAsync();
        var rows = await definition.Order(ranking)
            .ThenByDescending(row => row.Count)
            .ThenBy(row => row.JourneyDescription)
            .ThenBy(row => row.Number)
            .ThenBy(row => row.TransportType)
            .ThenBy(row => row.AdministrationId)
            .ThenBy(row => row.OriginEvaNumber)
            .ThenBy(row => row.DestinationEvaNumber)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

        var administrations = await LoadAdministrationsAsync(rows.Select(row => row.AdministrationId));
        var stations = await LoadStationsAsync(rows.SelectMany(row => new[] { row.OriginEvaNumber, row.DestinationEvaNumber }));
        var routeTimes = request.EvaNumbers.Any()
            ? await LoadStationScopedRouteTimesAsync(request, rows)
            : await LoadGlobalRouteTimesAsync(request, rows);

        var dataPoints = rows
            .Where(row => administrations.ContainsKey(row.AdministrationId))
            .Select(row =>
            {
                routeTimes.TryGetValue(CreateKey(row), out var routeTime);

                return new LineRankingDataPoint
                {
                    Line = new LineMetricSubject
                    {
                        Number = row.Number,
                        JourneyDescription = row.JourneyDescription,
                        TransportType = row.TransportType
                    },
                    Administration = administrations[row.AdministrationId],
                    StartStation = GetStationSubject(stations, row.OriginEvaNumber),
                    EndStation = GetStationSubject(stations, row.DestinationEvaNumber),
                    RouteStartTime = ToDateTimeOffset(routeTime?.RouteStartTime),
                    RouteEndTime = ToDateTimeOffset(routeTime?.RouteEndTime),
                    Value = definition.GetValue(row),
                    Sample = definition.CreateSample?.Invoke(row)
                };
            })
            .ToList();

        return new()
        {
            SeriesType = request.SeriesType,
            Unit = definition.Unit,
            DataPoints = dataPoints,
            Page = MetricPage.Create(offset, limit, totalItems)
        };
    }

    private IQueryable<LineRankingRow> BuildGlobalLineRankingQuery(LineRankingMetricRequest request)
    {
        var start = request.Start.UtcDateTime;
        var end = request.End.UtcDateTime;
        var transportTypes = StationEventQualityMetricDefinitions.NormalizeTransportTypes(request.TransportTypes);

        var query = dataContext.JourneyRouteQualities
            .AsNoTracking()
            .Where(summary => summary.BucketHour >= start && summary.BucketHour < end)
            .Where(summary => transportTypes.Contains(summary.TransportType))
            .Where(summary => request.IncludeReplacementTransport || !summary.IsReplacementTransport);

        var journeyDescriptionRegex = request.JourneyDescription;
        if (!string.IsNullOrWhiteSpace(journeyDescriptionRegex))
            query = query.Where(summary => Regex.IsMatch(summary.JourneyDescription, journeyDescriptionRegex));
        if (!string.IsNullOrWhiteSpace(request.Number))
            query = query.Where(summary => Regex.IsMatch(summary.Number.ToString(), request.Number));

        return query
            .GroupBy(summary => new
            {
                summary.Number,
                summary.JourneyDescription,
                summary.TransportType,
                summary.AdministrationId,
                summary.OriginEvaNumber,
                summary.DestinationEvaNumber
            })
            .Select(group => new LineRankingRow
            {
                Number = group.Key.Number,
                JourneyDescription = group.Key.JourneyDescription,
                TransportType = group.Key.TransportType,
                AdministrationId = group.Key.AdministrationId,
                OriginEvaNumber = group.Key.OriginEvaNumber,
                DestinationEvaNumber = group.Key.DestinationEvaNumber,
                Count = group.Sum(summary => summary.JourneyCount),
                CancellationCount = group.Sum(summary => summary.JourneyCancelledCount),
                DelaySampleCount = group.Sum(summary => summary.DelaySampleCount),
                DelaySumSeconds = group.Sum(summary => summary.DelaySumSeconds),
                Punctual5Count = group.Sum(summary => summary.Punctual5Count),
                Punctual15Count = group.Sum(summary => summary.Punctual15Count)
            });
    }

    private async Task<Dictionary<LineRankingKey, RouteTimeWindow>> LoadStationScopedRouteTimesAsync(
        LineRankingMetricRequest request,
        IReadOnlyCollection<LineRankingRow> rows
    )
    {
        if (!rows.Any()) return new();

        var start = request.Start.UtcDateTime;
        var end = request.End.UtcDateTime;
        var transportTypes = StationEventQualityMetricDefinitions.NormalizeTransportTypes(request.TransportTypes);
        var keys = rows.Select(CreateKey).ToHashSet();
        var numbers = rows.Select(row => row.Number).Distinct().ToArray();
        var journeyDescriptions = rows.Select(row => row.JourneyDescription).Distinct().ToArray();
        var administrationIds = rows.Select(row => row.AdministrationId).Distinct().ToArray();
        var originEvaNumbers = rows.Select(row => row.OriginEvaNumber).Distinct().ToArray();
        var destinationEvaNumbers = rows.Select(row => row.DestinationEvaNumber).Distinct().ToArray();

        var query = dataContext.JourneyEventQualityFacts
            .AsNoTracking()
            .Where(fact => fact.PlannedTime >= start && fact.PlannedTime < end)
            .Where(fact => request.EvaNumbers.Contains(fact.StationEvaNumber))
            .Where(fact => request.ScheduleType == null || fact.ScheduleType == request.ScheduleType)
            .Where(fact => transportTypes.Contains(fact.TransportType))
            .Where(fact => request.IncludeReplacementTransport || !fact.IsReplacementTransport)
            .Where(fact => numbers.Contains(fact.Number))
            .Where(fact => journeyDescriptions.Contains(fact.JourneyDescription))
            .Where(fact => administrationIds.Contains(fact.AdministrationId))
            .Where(fact => originEvaNumbers.Contains(fact.OriginEvaNumber))
            .Where(fact => destinationEvaNumbers.Contains(fact.DestinationEvaNumber));

        var journeyDescriptionRegex = request.JourneyDescription;
        if (!string.IsNullOrWhiteSpace(journeyDescriptionRegex))
            query = query.Where(fact => Regex.IsMatch(fact.JourneyDescription, journeyDescriptionRegex));
        if (!string.IsNullOrWhiteSpace(request.Number))
            query = query.Where(fact => Regex.IsMatch(fact.Number.ToString(), request.Number));

        var candidates = await query
            .Select(fact => new RouteTimeCandidate
            {
                Key = new(
                    fact.Number,
                    fact.JourneyDescription,
                    fact.TransportType,
                    fact.AdministrationId,
                    fact.OriginEvaNumber,
                    fact.DestinationEvaNumber),
                MatchingTime = fact.PlannedTime,
                JourneyId = fact.JourneyId,
                Date = fact.Date,
                RouteStartTime = fact.JourneyStartTime,
                RouteEndTime = fact.JourneyEndTime
            })
            .ToListAsync();

        return candidates
            .Where(candidate => keys.Contains(candidate.Key))
            .GroupBy(candidate => candidate.Key)
            .ToDictionary(
                group => group.Key,
                group => group
                    .OrderBy(candidate => candidate.MatchingTime)
                    .ThenBy(candidate => candidate.JourneyId)
                    .ThenBy(candidate => candidate.Date)
                    .Select(candidate => new RouteTimeWindow(candidate.RouteStartTime, candidate.RouteEndTime))
                    .First());
    }

    private async Task<Dictionary<LineRankingKey, RouteTimeWindow>> LoadGlobalRouteTimesAsync(
        LineRankingMetricRequest request,
        IReadOnlyCollection<LineRankingRow> rows
    )
    {
        if (!rows.Any()) return new();

        var start = request.Start.UtcDateTime;
        var end = request.End.UtcDateTime;
        var transportTypes = StationEventQualityMetricDefinitions.NormalizeTransportTypes(request.TransportTypes);
        var keys = rows.Select(CreateKey).ToHashSet();
        var numbers = rows.Select(row => row.Number).Distinct().ToArray();
        var journeyDescriptions = rows.Select(row => row.JourneyDescription).Distinct().ToArray();
        var administrationIds = rows.Select(row => row.AdministrationId).Distinct().ToArray();
        var originEvaNumbers = rows.Select(row => row.OriginEvaNumber).Distinct().ToArray();
        var destinationEvaNumbers = rows.Select(row => row.DestinationEvaNumber).Distinct().ToArray();

        var query = dataContext.JourneyRouteQualityFacts
            .AsNoTracking()
            .Where(fact => fact.JourneyStartTime >= start && fact.JourneyStartTime < end)
            .Where(fact => transportTypes.Contains(fact.TransportType))
            .Where(fact => request.IncludeReplacementTransport || !fact.IsReplacementTransport)
            .Where(fact => numbers.Contains(fact.Number))
            .Where(fact => journeyDescriptions.Contains(fact.JourneyDescription))
            .Where(fact => administrationIds.Contains(fact.AdministrationId))
            .Where(fact => originEvaNumbers.Contains(fact.OriginEvaNumber))
            .Where(fact => destinationEvaNumbers.Contains(fact.DestinationEvaNumber));

        var journeyDescriptionRegex = request.JourneyDescription;
        if (!string.IsNullOrWhiteSpace(journeyDescriptionRegex))
            query = query.Where(fact => Regex.IsMatch(fact.JourneyDescription, journeyDescriptionRegex));
        if (!string.IsNullOrWhiteSpace(request.Number))
            query = query.Where(fact => Regex.IsMatch(fact.Number.ToString(), request.Number));

        var candidates = await query
            .Select(fact => new RouteTimeCandidate
            {
                Key = new(
                    fact.Number,
                    fact.JourneyDescription,
                    fact.TransportType,
                    fact.AdministrationId,
                    fact.OriginEvaNumber,
                    fact.DestinationEvaNumber),
                MatchingTime = fact.JourneyStartTime,
                JourneyId = fact.JourneyId,
                Date = fact.Date,
                RouteStartTime = fact.JourneyStartTime,
                RouteEndTime = fact.JourneyEndTime
            })
            .ToListAsync();

        return candidates
            .Where(candidate => keys.Contains(candidate.Key))
            .GroupBy(candidate => candidate.Key)
            .ToDictionary(
                group => group.Key,
                group => group
                    .OrderBy(candidate => candidate.MatchingTime)
                    .ThenBy(candidate => candidate.JourneyId)
                    .ThenBy(candidate => candidate.Date)
                    .Select(candidate => new RouteTimeWindow(candidate.RouteStartTime, candidate.RouteEndTime))
                    .First());
    }

    private IQueryable<LineRankingRow> BuildStationLineRankingQuery(LineRankingMetricRequest request)
    {
        var start = request.Start.UtcDateTime;
        var end = request.End.UtcDateTime;
        var transportTypes = StationEventQualityMetricDefinitions.NormalizeTransportTypes(request.TransportTypes);

        var query = dataContext.StationLineRouteQualities
            .AsNoTracking()
            .Where(summary => summary.BucketHour >= start && summary.BucketHour < end)
            .Where(summary => request.EvaNumbers.Contains(summary.StationEvaNumber))
            .Where(summary => request.ScheduleType == null || summary.ScheduleType == request.ScheduleType)
            .Where(summary => transportTypes.Contains(summary.TransportType))
            .Where(summary => request.IncludeReplacementTransport || !summary.IsReplacementTransport);

        var journeyDescriptionRegex = request.JourneyDescription;
        if (!string.IsNullOrWhiteSpace(journeyDescriptionRegex))
            query = query.Where(summary => Regex.IsMatch(summary.JourneyDescription, journeyDescriptionRegex));
        if (!string.IsNullOrWhiteSpace(request.Number))
            query = query.Where(summary => Regex.IsMatch(summary.Number.ToString(), request.Number));

        return query
            .GroupBy(summary => new
            {
                summary.Number,
                summary.JourneyDescription,
                summary.TransportType,
                summary.AdministrationId,
                summary.OriginEvaNumber,
                summary.DestinationEvaNumber
            })
            .Select(group => new LineRankingRow
            {
                Number = group.Key.Number,
                JourneyDescription = group.Key.JourneyDescription,
                TransportType = group.Key.TransportType,
                AdministrationId = group.Key.AdministrationId,
                OriginEvaNumber = group.Key.OriginEvaNumber,
                DestinationEvaNumber = group.Key.DestinationEvaNumber,
                Count = group.Sum(summary => summary.EventCount),
                CancellationCount = group.Sum(summary => summary.CancelledCount),
                DelaySampleCount = group.Sum(summary => summary.DelaySampleCount),
                DelaySumSeconds = group.Sum(summary => summary.DelaySumSeconds),
                Punctual5Count = group.Sum(summary => summary.Punctual5Count),
                Punctual15Count = group.Sum(summary => summary.Punctual15Count)
            });
    }

    private async Task<Dictionary<Guid, AdministrationMetricSubject>> LoadAdministrationsAsync(IEnumerable<Guid> administrationIds)
    {
        var ids = administrationIds.Distinct().ToArray();
        return await dataContext.Administrations
            .AsNoTracking()
            .Where(administration => ids.Contains(administration.Id))
            .ToDictionaryAsync(
                administration => administration.Id,
                administration => new AdministrationMetricSubject
                {
                    AdministrationId = administration.AdministrationId,
                    OperatorCode = administration.OperatorCode,
                    OperatorName = administration.OperatorName
                });
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

    private static DateTimeOffset? ToDateTimeOffset(DateTime? dateTime) =>
        dateTime is null
            ? null
            : new DateTimeOffset(DateTime.SpecifyKind(dateTime.Value, DateTimeKind.Utc));

    private static LineRankingKey CreateKey(LineRankingRow row) => new(
        row.Number,
        row.JourneyDescription,
        row.TransportType,
        row.AdministrationId,
        row.OriginEvaNumber,
        row.DestinationEvaNumber);

    private static MetricDefinition GetDefinition(MetricSeriesType seriesType) =>
        Definitions.TryGetValue(seriesType, out var definition)
            ? definition
            : throw new NotSupportedException($"Unsupported line ranking metric series type: {seriesType}");

    private sealed record MetricDefinition(
        MetricUnit Unit,
        Func<IQueryable<LineRankingRow>, IOrderedQueryable<LineRankingRow>> Order,
        Func<LineRankingRow, decimal> GetValue,
        Func<LineRankingRow, MetricSample?>? CreateSample = null);

    private sealed class LineRankingRow
    {
        public int Number { get; set; }
        public string JourneyDescription { get; set; } = string.Empty;
        public TransportType TransportType { get; set; }
        public Guid AdministrationId { get; set; }
        public int OriginEvaNumber { get; set; }
        public int DestinationEvaNumber { get; set; }
        public long Count { get; set; }
        public long CancellationCount { get; set; }
        public long DelaySampleCount { get; set; }
        public long DelaySumSeconds { get; set; }
        public long Punctual5Count { get; set; }
        public long Punctual15Count { get; set; }
    }

    private sealed record LineRankingKey(
        int Number,
        string JourneyDescription,
        TransportType TransportType,
        Guid AdministrationId,
        int OriginEvaNumber,
        int DestinationEvaNumber);

    private sealed record RouteTimeWindow(DateTime RouteStartTime, DateTime RouteEndTime);

    private sealed class RouteTimeCandidate
    {
        public required LineRankingKey Key { get; init; }
        public required DateTime MatchingTime { get; init; }
        public required string JourneyId { get; init; }
        public required DateOnly Date { get; init; }
        public required DateTime RouteStartTime { get; init; }
        public required DateTime RouteEndTime { get; init; }
    }
}
