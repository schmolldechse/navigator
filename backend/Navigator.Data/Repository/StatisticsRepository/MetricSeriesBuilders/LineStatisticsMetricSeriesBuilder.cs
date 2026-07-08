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
        StatisticsMetricResult result = request switch
        {
            LineProfileRequest => await BuildProfileAsync(request, cancellationToken),
            LineJourneySummaryRequest => new JourneySummaryResult(StatisticsMetricBuilderHelpers.ToJourneyMetrics(
                StatisticsMetricBuilderHelpers.AggregateJourneyRows(await LoadJourneyRowsAsync(request, cancellationToken)))),
            LineEventSummaryRequest => new EventSummaryResult(StatisticsMetricBuilderHelpers.ToEventMetrics(
                StatisticsMetricBuilderHelpers.AggregateEventRows(await LoadEventRowsAsync(request, cancellationToken)))),
            LineTimeSeriesRequest => await BuildTimeSeriesAsync(request, cancellationToken),
            LineRouteVariantsRequest => await BuildRouteVariantsAsync(request, cancellationToken),
            LineProblemStationsRequest => await BuildProblemStationsAsync(request, cancellationToken),
            LineStationPerformanceRequest => await BuildStationPerformanceAsync(request, cancellationToken),
            LineJourneyNumberRankingRequest => await BuildJourneyNumberRankingAsync(request, cancellationToken),
            LineWeekdayHourHeatmapRequest => new JourneyWeekdayHourHeatmapResult(BuildWeekdayHourHeatmap(await LoadJourneyRowsAsync(request, cancellationToken), request)),
            _ => throw new NotSupportedException($"Unsupported line statistics metric: {request.GetType().Name}")
        };

        return new StatisticsMetricResponse(CreateMeta(request), result);
    }

    private async Task<List<IJourneyQualityHourly>> LoadJourneyRowsAsync(
        LineStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var administrationIds = await StatisticsMetricBuilderHelpers.ResolveAdministrationIdsAsync(dataContext, request, cancellationToken);
        var transportTypes = StatisticsMetricBuilderHelpers.TransportTypes(request);
        var journeyNumber = StatisticsMetricBuilderHelpers.JourneyNumber(request);
        var originEvaNumber = StatisticsMetricBuilderHelpers.OriginEvaNumber(request);
        var destinationEvaNumber = StatisticsMetricBuilderHelpers.DestinationEvaNumber(request);
        var fromUtc = StatisticsMetricBuilderHelpers.UtcFrom(request);
        var toUtc = StatisticsMetricBuilderHelpers.UtcTo(request);
        var includeReplacement = StatisticsMetricBuilderHelpers.IncludeReplacement(request);

        if (journeyNumber is int concreteJourneyNumber)
        {
            var numberQuery = dataContext.JourneyNumberQualities
                .AsNoTracking()
                .Where(row => row.JourneyNumber == concreteJourneyNumber)
                .Where(row => row.JourneyDescription == request.LineName)
                .Where(row => row.BucketHour >= fromUtc && row.BucketHour < toUtc)
                .Where(row => transportTypes.Length == 0 || transportTypes.Contains(row.TransportType))
                .Where(row => includeReplacement || !row.IsReplacement)
                .Where(row => originEvaNumber == null || row.OriginEvaNumber == originEvaNumber)
                .Where(row => destinationEvaNumber == null || row.DestinationEvaNumber == destinationEvaNumber)
                .Where(row => administrationIds.Length == 0 || administrationIds.Contains(row.AdministrationId));

            return (await numberQuery.ToListAsync(cancellationToken)).Cast<IJourneyQualityHourly>().ToList();
        }

        var query = dataContext.LineJourneyQualities
            .AsNoTracking()
            .Where(row => row.JourneyDescription == request.LineName)
            .Where(row => row.BucketHour >= fromUtc && row.BucketHour < toUtc)
            .Where(row => transportTypes.Length == 0 || transportTypes.Contains(row.TransportType))
            .Where(row => includeReplacement || !row.IsReplacement)
            .Where(row => originEvaNumber == null || row.OriginEvaNumber == originEvaNumber)
            .Where(row => destinationEvaNumber == null || row.DestinationEvaNumber == destinationEvaNumber)
            .Where(row => administrationIds.Length == 0 || administrationIds.Contains(row.AdministrationId));

        return (await query.ToListAsync(cancellationToken)).Cast<IJourneyQualityHourly>().ToList();
    }

    private async Task<List<LineEventQualityHourly>> LoadEventRowsAsync(
        LineStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var administrationIds = await StatisticsMetricBuilderHelpers.ResolveAdministrationIdsAsync(dataContext, request, cancellationToken);
        var transportTypes = StatisticsMetricBuilderHelpers.TransportTypes(request);
        var scheduleType = StatisticsMetricBuilderHelpers.ScheduleType(request);
        var originEvaNumber = StatisticsMetricBuilderHelpers.OriginEvaNumber(request);
        var destinationEvaNumber = StatisticsMetricBuilderHelpers.DestinationEvaNumber(request);
        var fromUtc = StatisticsMetricBuilderHelpers.UtcFrom(request);
        var toUtc = StatisticsMetricBuilderHelpers.UtcTo(request);
        var includeReplacement = StatisticsMetricBuilderHelpers.IncludeReplacement(request);

        return await dataContext.LineEventQualities
            .AsNoTracking()
            .Where(row => row.JourneyDescription == request.LineName)
            .Where(row => row.BucketHour >= fromUtc && row.BucketHour < toUtc)
            .Where(row => scheduleType == null || row.ScheduleType == scheduleType)
            .Where(row => transportTypes.Length == 0 || transportTypes.Contains(row.TransportType))
            .Where(row => includeReplacement || !row.IsReplacement)
            .Where(row => originEvaNumber == null || row.OriginEvaNumber == originEvaNumber)
            .Where(row => destinationEvaNumber == null || row.DestinationEvaNumber == destinationEvaNumber)
            .Where(row => administrationIds.Length == 0 || administrationIds.Contains(row.AdministrationId))
            .ToListAsync(cancellationToken);
    }

    private async Task<LineProfileResult> BuildProfileAsync(
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

        return new LineProfileResult(
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
        var bucketMode = ((IHasBucket)request).Bucket;
        var buckets = journeyRows.Select(row => StatisticsMetricBuilderHelpers.BucketStart(row.BucketHour, request, bucketMode))
            .Concat(eventRows.Select(row => StatisticsMetricBuilderHelpers.BucketStart(row.BucketHour, request, bucketMode)))
            .Distinct()
            .OrderBy(bucket => bucket);

        var items = buckets.Select(bucket => new LineTimeSeriesPoint(
            bucket,
            StatisticsMetricBuilderHelpers.ToJourneyMetrics(
                StatisticsMetricBuilderHelpers.AggregateJourneyRows(journeyRows.Where(row => StatisticsMetricBuilderHelpers.BucketStart(row.BucketHour, request, bucketMode) == bucket))),
            StatisticsMetricBuilderHelpers.ToEventMetrics(
                StatisticsMetricBuilderHelpers.AggregateEventRows(eventRows.Where(row => StatisticsMetricBuilderHelpers.BucketStart(row.BucketHour, request, bucketMode) == bucket)))))
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
            .Where(row => row.Aggregate.JourneyCount >= StatisticsMetricBuilderHelpers.MinVolume(request))
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
            .Where(row => row.Aggregate.EventCount >= StatisticsMetricBuilderHelpers.MinVolume(request))
            .OrderByDescending(row => row.Aggregate.EventCount)
            .ToList();

        var stations = await LoadStationsAsync(grouped.Select(row => row.StationEvaNumber), cancellationToken);
        var items = grouped
            .Skip(StatisticsMetricBuilderHelpers.Offset(request))
            .Take(StatisticsMetricBuilderHelpers.Limit(request))
            .Select(row => new LineStationPerformanceItem(
                ToStationReference(stations, row.StationEvaNumber),
                StatisticsMetricBuilderHelpers.ToEventMetrics(row.Aggregate)))
            .ToList();

        return new LineStationPerformanceResult(
            items,
            new MetricPage(StatisticsMetricBuilderHelpers.Offset(request), StatisticsMetricBuilderHelpers.Limit(request), grouped.Count));
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
            .Where(row => row.Aggregate.JourneyCount >= StatisticsMetricBuilderHelpers.MinVolume(request))
            .OrderByDescending(row => row.Aggregate.DestinationPositiveDelaySumSeconds)
            .ThenByDescending(row => row.Aggregate.JourneyCount)
            .ToList();

        var stations = await LoadStationsAsync(grouped.SelectMany(row => new[] { row.OriginEvaNumber, row.DestinationEvaNumber }), cancellationToken);
        var items = grouped
            .Skip(StatisticsMetricBuilderHelpers.Offset(request))
            .Take(StatisticsMetricBuilderHelpers.Limit(request))
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

        return new LineJourneyNumberRankingResult(
            items,
            new MetricPage(StatisticsMetricBuilderHelpers.Offset(request), StatisticsMetricBuilderHelpers.Limit(request), grouped.Count));
    }

    private static IReadOnlyList<JourneyHeatmapCell> BuildWeekdayHourHeatmap(
        IReadOnlyCollection<IJourneyQualityHourly> rows,
        LineStatisticsMetricRequest request
    ) => rows
        .GroupBy(row =>
        {
            var local = StatisticsMetricBuilderHelpers.ToRequestOffsetTime(row.BucketHour, request);
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
        var transportTypes = StatisticsMetricBuilderHelpers.TransportTypes(request);
        var originEvaNumber = StatisticsMetricBuilderHelpers.OriginEvaNumber(request);
        var destinationEvaNumber = StatisticsMetricBuilderHelpers.DestinationEvaNumber(request);
        var fromUtc = StatisticsMetricBuilderHelpers.UtcFrom(request);
        var toUtc = StatisticsMetricBuilderHelpers.UtcTo(request);
        var includeReplacement = StatisticsMetricBuilderHelpers.IncludeReplacement(request);
        return await dataContext.LineJourneyQualities
            .AsNoTracking()
            .Where(row => row.JourneyDescription == request.LineName)
            .Where(row => row.BucketHour >= fromUtc && row.BucketHour < toUtc)
            .Where(row => transportTypes.Length == 0 || transportTypes.Contains(row.TransportType))
            .Where(row => includeReplacement || !row.IsReplacement)
            .Where(row => originEvaNumber == null || row.OriginEvaNumber == originEvaNumber)
            .Where(row => destinationEvaNumber == null || row.DestinationEvaNumber == destinationEvaNumber)
            .Where(row => administrationIds.Length == 0 || administrationIds.Contains(row.AdministrationId))
            .ToListAsync(cancellationToken);
    }

    private async Task<List<JourneyNumberQualityHourly>> LoadJourneyNumberRowsAsync(
        LineStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var administrationIds = await StatisticsMetricBuilderHelpers.ResolveAdministrationIdsAsync(dataContext, request, cancellationToken);
        var transportTypes = StatisticsMetricBuilderHelpers.TransportTypes(request);
        var journeyNumber = StatisticsMetricBuilderHelpers.JourneyNumber(request);
        var originEvaNumber = StatisticsMetricBuilderHelpers.OriginEvaNumber(request);
        var destinationEvaNumber = StatisticsMetricBuilderHelpers.DestinationEvaNumber(request);
        var fromUtc = StatisticsMetricBuilderHelpers.UtcFrom(request);
        var toUtc = StatisticsMetricBuilderHelpers.UtcTo(request);
        var includeReplacement = StatisticsMetricBuilderHelpers.IncludeReplacement(request);
        return await dataContext.JourneyNumberQualities
            .AsNoTracking()
            .Where(row => row.JourneyDescription == request.LineName)
            .Where(row => row.BucketHour >= fromUtc && row.BucketHour < toUtc)
            .Where(row => journeyNumber == null || row.JourneyNumber == journeyNumber)
            .Where(row => transportTypes.Length == 0 || transportTypes.Contains(row.TransportType))
            .Where(row => includeReplacement || !row.IsReplacement)
            .Where(row => originEvaNumber == null || row.OriginEvaNumber == originEvaNumber)
            .Where(row => destinationEvaNumber == null || row.DestinationEvaNumber == destinationEvaNumber)
            .Where(row => administrationIds.Length == 0 || administrationIds.Contains(row.AdministrationId))
            .ToListAsync(cancellationToken);
    }

    private async Task<List<StationLineQualityHourly>> LoadStationLineRowsAsync(
        LineStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var administrationIds = await StatisticsMetricBuilderHelpers.ResolveAdministrationIdsAsync(dataContext, request, cancellationToken);
        var transportTypes = StatisticsMetricBuilderHelpers.TransportTypes(request);
        var scheduleType = StatisticsMetricBuilderHelpers.ScheduleType(request);
        var originEvaNumber = StatisticsMetricBuilderHelpers.OriginEvaNumber(request);
        var destinationEvaNumber = StatisticsMetricBuilderHelpers.DestinationEvaNumber(request);
        var fromUtc = StatisticsMetricBuilderHelpers.UtcFrom(request);
        var toUtc = StatisticsMetricBuilderHelpers.UtcTo(request);
        var includeReplacement = StatisticsMetricBuilderHelpers.IncludeReplacement(request);
        return await dataContext.StationLineQualities
            .AsNoTracking()
            .Where(row => row.JourneyDescription == request.LineName)
            .Where(row => row.BucketHour >= fromUtc && row.BucketHour < toUtc)
            .Where(row => scheduleType == null || row.ScheduleType == scheduleType)
            .Where(row => transportTypes.Length == 0 || transportTypes.Contains(row.TransportType))
            .Where(row => includeReplacement || !row.IsReplacement)
            .Where(row => originEvaNumber == null || row.OriginEvaNumber == originEvaNumber)
            .Where(row => destinationEvaNumber == null || row.DestinationEvaNumber == destinationEvaNumber)
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
            StatisticsMetricBuilderHelpers.Scope(request),
            StatisticsMetricBuilderHelpers.Metric(request),
            request.From,
            request.To,
            StatisticsMetricBuilderHelpers.Bucket(request),
            StatisticsMetricBuilderHelpers.CreateFilters(request)
                .Concat(new[] { new KeyValuePair<string, object?>("lineName", request.LineName) })
                .ToDictionary(pair => pair.Key, pair => pair.Value));
}
