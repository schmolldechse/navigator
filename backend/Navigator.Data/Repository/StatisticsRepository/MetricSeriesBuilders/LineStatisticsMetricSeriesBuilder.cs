using Microsoft.EntityFrameworkCore;
using Navigator.Data.Entities.Views;
using Navigator.Data.Models.Statistics.Api;

namespace Navigator.Data.Repository.StatisticsRepository.MetricSeriesBuilders;

public sealed class LineStatisticsMetricSeriesBuilder(
    DataContext dataContext
) : StatisticsMetricBuilder<LineStatisticsMetricRequest>
{
    protected override async Task<StatisticsMetricResponse> BuildAsync(
        LineStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        StatisticsMetricResult result = request.MetricType switch
        {
            Navigator.Data.Enums.Metric.LineStatisticsMetricType.Summary => await BuildSummaryAsync(request, cancellationToken),
            Navigator.Data.Enums.Metric.LineStatisticsMetricType.JourneyKpis => new JourneyKpisResult(StatisticsMetricBuilderHelpers.ToJourneyMetrics(
                StatisticsMetricBuilderHelpers.AggregateJourneyRows(await LoadJourneyRowsAsync(request, cancellationToken)))),
            Navigator.Data.Enums.Metric.LineStatisticsMetricType.EventKpis => new EventKpisResult(StatisticsMetricBuilderHelpers.ToEventMetrics(
                StatisticsMetricBuilderHelpers.AggregateEventRows(await LoadEventRowsAsync(request, cancellationToken)))),
            Navigator.Data.Enums.Metric.LineStatisticsMetricType.TimeSeries => await BuildTimeSeriesAsync(request, cancellationToken),
            Navigator.Data.Enums.Metric.LineStatisticsMetricType.RouteVariants => await BuildRouteVariantsAsync(request, cancellationToken),
            Navigator.Data.Enums.Metric.LineStatisticsMetricType.StationPerformance => await BuildStationPerformanceAsync(request, cancellationToken),
            Navigator.Data.Enums.Metric.LineStatisticsMetricType.JourneyNumberRanking => await BuildJourneyNumberRankingAsync(request, cancellationToken),
            Navigator.Data.Enums.Metric.LineStatisticsMetricType.WeekdayHourHeatmap => new JourneyWeekdayHourHeatmapResult(BuildWeekdayHourHeatmap(await LoadJourneyRowsAsync(request, cancellationToken))),
            Navigator.Data.Enums.Metric.LineStatisticsMetricType.ProblemStations => await BuildProblemStationsAsync(request, cancellationToken),
            _ => throw new NotSupportedException($"Unsupported line statistics metric: {request.MetricType}")
        };

        return new StatisticsMetricResponse(CreateMeta(request), result);
    }

    private async Task<List<IJourneyQualityHourly>> LoadJourneyRowsAsync(
        LineStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var administrationIds = await StatisticsMetricBuilderHelpers.ResolveAdministrationIdsAsync(dataContext, request, cancellationToken);
        var transportTypes = request.TransportTypes;

        if (request.JourneyNumber is int journeyNumber)
        {
            var numberQuery = dataContext.JourneyNumberQualities
                .AsNoTracking()
                .Where(row => row.JourneyNumber == journeyNumber)
                .Where(row => row.JourneyDescription == request.LineName)
                .Where(row => row.BucketHour >= request.FromUtc && row.BucketHour < request.ToUtc)
                .Where(row => transportTypes.Length == 0 || transportTypes.Contains(row.TransportType))
                .Where(row => request.IncludeReplacement || !row.IsReplacement)
                .Where(row => request.OriginEvaNumber == null || row.OriginEvaNumber == request.OriginEvaNumber)
                .Where(row => request.DestinationEvaNumber == null || row.DestinationEvaNumber == request.DestinationEvaNumber)
                .Where(row => administrationIds.Length == 0 || administrationIds.Contains(row.AdministrationId));

            return (await numberQuery.ToListAsync(cancellationToken)).Cast<IJourneyQualityHourly>().ToList();
        }

        var query = dataContext.LineJourneyQualities
            .AsNoTracking()
            .Where(row => row.JourneyDescription == request.LineName)
            .Where(row => row.BucketHour >= request.FromUtc && row.BucketHour < request.ToUtc)
            .Where(row => transportTypes.Length == 0 || transportTypes.Contains(row.TransportType))
            .Where(row => request.IncludeReplacement || !row.IsReplacement)
            .Where(row => request.OriginEvaNumber == null || row.OriginEvaNumber == request.OriginEvaNumber)
            .Where(row => request.DestinationEvaNumber == null || row.DestinationEvaNumber == request.DestinationEvaNumber)
            .Where(row => administrationIds.Length == 0 || administrationIds.Contains(row.AdministrationId));

        return (await query.ToListAsync(cancellationToken)).Cast<IJourneyQualityHourly>().ToList();
    }

    private async Task<List<LineEventQualityHourly>> LoadEventRowsAsync(
        LineStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var administrationIds = await StatisticsMetricBuilderHelpers.ResolveAdministrationIdsAsync(dataContext, request, cancellationToken);
        var transportTypes = request.TransportTypes;

        return await dataContext.LineEventQualities
            .AsNoTracking()
            .Where(row => row.JourneyDescription == request.LineName)
            .Where(row => row.BucketHour >= request.FromUtc && row.BucketHour < request.ToUtc)
            .Where(row => request.ScheduleType == null || row.ScheduleType == request.ScheduleType)
            .Where(row => transportTypes.Length == 0 || transportTypes.Contains(row.TransportType))
            .Where(row => request.IncludeReplacement || !row.IsReplacement)
            .Where(row => request.OriginEvaNumber == null || row.OriginEvaNumber == request.OriginEvaNumber)
            .Where(row => request.DestinationEvaNumber == null || row.DestinationEvaNumber == request.DestinationEvaNumber)
            .Where(row => administrationIds.Length == 0 || administrationIds.Contains(row.AdministrationId))
            .ToListAsync(cancellationToken);
    }

    private async Task<LineSummaryResult> BuildSummaryAsync(
        LineStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var rows = await LoadJourneyNumberRowsAsync(request, cancellationToken);
        var grouped = rows
            .GroupBy(row => new
            {
                row.OriginEvaNumber,
                row.DestinationEvaNumber,
                row.TransportType
            })
            .Select(group => new
            {
                group.Key.OriginEvaNumber,
                group.Key.DestinationEvaNumber,
                group.Key.TransportType,
                JourneyCount = group.Sum(row => row.JourneyCount)
            })
            .OrderByDescending(row => row.JourneyCount)
            .ToList();

        var main = grouped.FirstOrDefault();
        var stations = await LoadStationsAsync(grouped.SelectMany(row => new[] { row.OriginEvaNumber, row.DestinationEvaNumber }), cancellationToken);

        return new LineSummaryResult(
            request.LineName,
            main?.TransportType,
            rows
                .GroupBy(row => row.JourneyNumber)
                .OrderByDescending(group => group.Sum(row => row.JourneyCount))
                .Take(5)
                .Select(group => group.Key)
                .ToArray(),
            grouped.Count,
            main is null ? null : ToStationReference(stations, main.OriginEvaNumber),
            main is null ? null : ToStationReference(stations, main.DestinationEvaNumber));
    }

    private async Task<LineTimeSeriesResult> BuildTimeSeriesAsync(
        LineStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var journeyRows = await LoadJourneyRowsAsync(request, cancellationToken);
        var eventRows = await LoadEventRowsAsync(request, cancellationToken);
        var buckets = journeyRows.Select(row => StatisticsMetricBuilderHelpers.BucketStart(row.BucketHour, request.Bucket))
            .Concat(eventRows.Select(row => StatisticsMetricBuilderHelpers.BucketStart(row.BucketHour, request.Bucket)))
            .Distinct()
            .OrderBy(bucket => bucket);

        var items = buckets.Select(bucket => new LineTimeSeriesPoint(
            bucket,
            StatisticsMetricBuilderHelpers.ToJourneyMetrics(
                StatisticsMetricBuilderHelpers.AggregateJourneyRows(journeyRows.Where(row => StatisticsMetricBuilderHelpers.BucketStart(row.BucketHour, request.Bucket) == bucket))),
            StatisticsMetricBuilderHelpers.ToEventMetrics(
                StatisticsMetricBuilderHelpers.AggregateEventRows(eventRows.Where(row => StatisticsMetricBuilderHelpers.BucketStart(row.BucketHour, request.Bucket) == bucket)))))
            .ToList();

        return new LineTimeSeriesResult(items);
    }

    private async Task<LineRouteVariantsResult> BuildRouteVariantsAsync(
        LineStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var rows = await LoadLineJourneyRowsAsync(request, cancellationToken);
        var grouped = rows
            .GroupBy(row => new { row.OriginEvaNumber, row.DestinationEvaNumber, row.TransportType })
            .Select(group => new
            {
                group.Key.OriginEvaNumber,
                group.Key.DestinationEvaNumber,
                group.Key.TransportType,
                Aggregate = StatisticsMetricBuilderHelpers.AggregateJourneyRows(group)
            })
            .Where(row => row.Aggregate.JourneyCount >= request.MinVolume)
            .OrderByDescending(row => row.Aggregate.JourneyCount)
            .ToList();

        var stations = await LoadStationsAsync(grouped.SelectMany(row => new[] { row.OriginEvaNumber, row.DestinationEvaNumber }), cancellationToken);
        var items = grouped.Select(row => new RouteVariantItem(
            $"{request.LineName}:{row.OriginEvaNumber}:{row.DestinationEvaNumber}",
            ToStationReference(stations, row.OriginEvaNumber),
            ToStationReference(stations, row.DestinationEvaNumber),
            row.TransportType,
            StatisticsMetricBuilderHelpers.ToJourneyMetrics(row.Aggregate)))
            .ToList();

        return new LineRouteVariantsResult(items);
    }

    private async Task<LineStationPerformanceResult> BuildStationPerformanceAsync(
        LineStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var rows = await LoadStationLineRowsAsync(request, cancellationToken);
        var grouped = rows
            .GroupBy(row => row.StationEvaNumber)
            .Select(group => new
            {
                StationEvaNumber = group.Key,
                Aggregate = StatisticsMetricBuilderHelpers.AggregateEventRows(group)
            })
            .Where(row => row.Aggregate.EventCount >= request.MinVolume)
            .OrderByDescending(row => row.Aggregate.EventCount)
            .ToList();

        var stations = await LoadStationsAsync(grouped.Select(row => row.StationEvaNumber), cancellationToken);
        var items = grouped
            .Skip(request.Offset)
            .Take(request.Limit)
            .Select(row => new LineStationPerformanceItem(
                ToStationReference(stations, row.StationEvaNumber),
                StatisticsMetricBuilderHelpers.ToEventMetrics(row.Aggregate)))
            .ToList();

        return new LineStationPerformanceResult(items, new MetricPage(request.Offset, request.Limit, grouped.Count));
    }

    private async Task<LineJourneyNumberRankingResult> BuildJourneyNumberRankingAsync(
        LineStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var rows = await LoadJourneyNumberRowsAsync(request, cancellationToken);
        var grouped = rows
            .GroupBy(row => new
            {
                row.JourneyNumber,
                row.OriginEvaNumber,
                row.DestinationEvaNumber,
                row.TransportType
            })
            .Select(group => new
            {
                group.Key.JourneyNumber,
                group.Key.OriginEvaNumber,
                group.Key.DestinationEvaNumber,
                group.Key.TransportType,
                Aggregate = StatisticsMetricBuilderHelpers.AggregateJourneyRows(group)
            })
            .Where(row => row.Aggregate.JourneyCount >= request.MinVolume)
            .OrderByDescending(row => row.Aggregate.DestinationPositiveDelaySumSeconds)
            .ThenByDescending(row => row.Aggregate.JourneyCount)
            .ToList();

        var stations = await LoadStationsAsync(grouped.SelectMany(row => new[] { row.OriginEvaNumber, row.DestinationEvaNumber }), cancellationToken);
        var items = grouped
            .Skip(request.Offset)
            .Take(request.Limit)
            .Select(row => new JourneyNumberRankingItem(
                row.JourneyNumber,
                new LineReference(
                    request.LineName,
                    row.JourneyNumber,
                    row.TransportType,
                    ToStationReference(stations, row.OriginEvaNumber),
                    ToStationReference(stations, row.DestinationEvaNumber)),
                StatisticsMetricBuilderHelpers.ToJourneyMetrics(row.Aggregate)))
            .ToList();

        return new LineJourneyNumberRankingResult(items, new MetricPage(request.Offset, request.Limit, grouped.Count));
    }

    private static IReadOnlyList<JourneyHeatmapCell> BuildWeekdayHourHeatmap(IReadOnlyCollection<IJourneyQualityHourly> rows) => rows
        .GroupBy(row =>
        {
            var local = StatisticsMetricBuilderHelpers.ToBerlinTime(row.BucketHour);
            return new
            {
                Weekday = local.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)local.DayOfWeek,
                local.Hour
            };
        })
        .OrderBy(group => group.Key.Weekday)
        .ThenBy(group => group.Key.Hour)
        .Select(group => new JourneyHeatmapCell(
            group.Key.Weekday,
            group.Key.Hour,
            StatisticsMetricBuilderHelpers.ToJourneyMetrics(StatisticsMetricBuilderHelpers.AggregateJourneyRows(group))))
        .ToList();

    private async Task<LineProblemStationsResult> BuildProblemStationsAsync(
        LineStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await BuildStationPerformanceAsync(request, cancellationToken);
        return new LineProblemStationsResult(
            result.Items
                .OrderByDescending(item => item.EventMetrics.DelayDebtMinutes)
                .ToList(),
            result.Page);
    }

    private async Task<List<LineJourneyQualityHourly>> LoadLineJourneyRowsAsync(
        LineStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var administrationIds = await StatisticsMetricBuilderHelpers.ResolveAdministrationIdsAsync(dataContext, request, cancellationToken);
        var transportTypes = request.TransportTypes;
        return await dataContext.LineJourneyQualities
            .AsNoTracking()
            .Where(row => row.JourneyDescription == request.LineName)
            .Where(row => row.BucketHour >= request.FromUtc && row.BucketHour < request.ToUtc)
            .Where(row => transportTypes.Length == 0 || transportTypes.Contains(row.TransportType))
            .Where(row => request.IncludeReplacement || !row.IsReplacement)
            .Where(row => request.OriginEvaNumber == null || row.OriginEvaNumber == request.OriginEvaNumber)
            .Where(row => request.DestinationEvaNumber == null || row.DestinationEvaNumber == request.DestinationEvaNumber)
            .Where(row => administrationIds.Length == 0 || administrationIds.Contains(row.AdministrationId))
            .ToListAsync(cancellationToken);
    }

    private async Task<List<JourneyNumberQualityHourly>> LoadJourneyNumberRowsAsync(
        LineStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var administrationIds = await StatisticsMetricBuilderHelpers.ResolveAdministrationIdsAsync(dataContext, request, cancellationToken);
        var transportTypes = request.TransportTypes;
        return await dataContext.JourneyNumberQualities
            .AsNoTracking()
            .Where(row => row.JourneyDescription == request.LineName)
            .Where(row => row.BucketHour >= request.FromUtc && row.BucketHour < request.ToUtc)
            .Where(row => request.JourneyNumber == null || row.JourneyNumber == request.JourneyNumber)
            .Where(row => transportTypes.Length == 0 || transportTypes.Contains(row.TransportType))
            .Where(row => request.IncludeReplacement || !row.IsReplacement)
            .Where(row => request.OriginEvaNumber == null || row.OriginEvaNumber == request.OriginEvaNumber)
            .Where(row => request.DestinationEvaNumber == null || row.DestinationEvaNumber == request.DestinationEvaNumber)
            .Where(row => administrationIds.Length == 0 || administrationIds.Contains(row.AdministrationId))
            .ToListAsync(cancellationToken);
    }

    private async Task<List<StationLineQualityHourly>> LoadStationLineRowsAsync(
        LineStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var administrationIds = await StatisticsMetricBuilderHelpers.ResolveAdministrationIdsAsync(dataContext, request, cancellationToken);
        var transportTypes = request.TransportTypes;
        return await dataContext.StationLineQualities
            .AsNoTracking()
            .Where(row => row.JourneyDescription == request.LineName)
            .Where(row => row.BucketHour >= request.FromUtc && row.BucketHour < request.ToUtc)
            .Where(row => request.ScheduleType == null || row.ScheduleType == request.ScheduleType)
            .Where(row => transportTypes.Length == 0 || transportTypes.Contains(row.TransportType))
            .Where(row => request.IncludeReplacement || !row.IsReplacement)
            .Where(row => request.OriginEvaNumber == null || row.OriginEvaNumber == request.OriginEvaNumber)
            .Where(row => request.DestinationEvaNumber == null || row.DestinationEvaNumber == request.DestinationEvaNumber)
            .Where(row => administrationIds.Length == 0 || administrationIds.Contains(row.AdministrationId))
            .ToListAsync(cancellationToken);
    }

    private async Task<Dictionary<int, StationReference>> LoadStationsAsync(
        IEnumerable<int> evaNumbers,
        CancellationToken cancellationToken
    )
    {
        var numbers = evaNumbers.Distinct().ToArray();
        if (numbers.Length == 0) return new();

        return await dataContext.Stations
            .AsNoTracking()
            .Where(station => numbers.Contains(station.EvaNumber))
            .ToDictionaryAsync(
                station => station.EvaNumber,
                station => new StationReference(station.EvaNumber, station.Name, station.Latitude, station.Longitude),
                cancellationToken);
    }

    private static StationReference ToStationReference(IReadOnlyDictionary<int, StationReference> stations, int evaNumber) =>
        stations.TryGetValue(evaNumber, out var station)
            ? station
            : new StationReference(evaNumber);

    private static StatisticsResponseMeta CreateMeta(LineStatisticsMetricRequest request) =>
        new(
            "LINE",
            request.MetricType.ToString(),
            request.From,
            request.To,
            request.Bucket,
            StatisticsMetricBuilderHelpers.Timezone,
            StatisticsMetricBuilderHelpers.CreateFilters(request)
                .Concat(new[] { new KeyValuePair<string, object?>("lineName", request.LineName) })
                .ToDictionary(pair => pair.Key, pair => pair.Value));
}
