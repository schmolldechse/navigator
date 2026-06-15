using Microsoft.EntityFrameworkCore;
using Navigator.Data.Entities.Views;
using Navigator.Data.Enums;
using Navigator.Data.Enums.Metric;
using Navigator.Data.Models.Statistics.Api;

namespace Navigator.Data.Repository.StatisticsRepository.MetricSeriesBuilders;

public sealed class StationStatisticsMetricSeriesBuilder(
    DataContext dataContext
) : StatisticsMetricBuilder<StationStatisticsMetricRequest>
{
    protected override async Task<StatisticsMetricResponse> BuildAsync(
        StationStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        StatisticsMetricResult result = request.MetricType switch
        {
            StationStatisticsMetricType.EventKpis => new EventKpisResult(StatisticsMetricBuilderHelpers.ToEventMetrics(
                StatisticsMetricBuilderHelpers.AggregateEventRows(await LoadEventRowsAsync(request, cancellationToken)))),
            StationStatisticsMetricType.Benchmark => await BuildBenchmarkAsync(request, cancellationToken),
            StationStatisticsMetricType.TimeSeries => new StationTimeSeriesResult(BuildTimeSeries(await LoadEventRowsAsync(request, cancellationToken), request)),
            StationStatisticsMetricType.ArrivalDepartureComparison => new ArrivalDepartureComparisonResult(BuildArrivalDepartureComparison(await LoadStationEventRowsAsync(request, cancellationToken))),
            StationStatisticsMetricType.WeekdayHourHeatmap => new EventWeekdayHourHeatmapResult(BuildWeekdayHourHeatmap(await LoadEventRowsAsync(request, cancellationToken))),
            StationStatisticsMetricType.LineRanking => await BuildLineRankingAsync(request, cancellationToken),
            StationStatisticsMetricType.Directions => new StationDirectionsResult(await BuildDirectionsAsync(request, cancellationToken)),
            StationStatisticsMetricType.TransportTypeMix => new TransportTypeMixResult(BuildTransportTypeMix(await LoadEventRowsAsync(request, cancellationToken))),
            StationStatisticsMetricType.LineHourMatrix => new LineHourMatrixResult(await BuildLineHourMatrixAsync(request, cancellationToken)),
            StationStatisticsMetricType.EventDetails => await BuildEventDetailsAsync(request, cancellationToken),
            _ => throw new NotSupportedException($"Unsupported station statistics metric: {request.MetricType}")
        };

        return new StatisticsMetricResponse(CreateMeta(request), result);
    }

    private async Task<List<IEventQualityHourly>> LoadEventRowsAsync(
        StationStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var administrationIds = await StatisticsMetricBuilderHelpers.ResolveAdministrationIdsAsync(dataContext, request, cancellationToken);
        var transportTypes = request.TransportTypes;

        if (administrationIds.Length > 0)
        {
            var adminQuery = dataContext.StationAdministrationQualities
                .AsNoTracking()
                .Where(row => row.StationEvaNumber == request.StationEvaNumber)
                .Where(row => row.BucketHour >= request.FromUtc && row.BucketHour < request.ToUtc)
                .Where(row => request.ScheduleType == null || row.ScheduleType == request.ScheduleType)
                .Where(row => transportTypes.Length == 0 || transportTypes.Contains(row.TransportType))
                .Where(row => request.IncludeReplacement || !row.IsReplacement)
                .Where(row => administrationIds.Contains(row.AdministrationId));

            return (await adminQuery.ToListAsync(cancellationToken)).Cast<IEventQualityHourly>().ToList();
        }

        return (await LoadStationEventRowsAsync(request, cancellationToken)).Cast<IEventQualityHourly>().ToList();
    }

    private async Task<List<StationEventQualityHourly>> LoadStationEventRowsAsync(
        StationStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var transportTypes = request.TransportTypes;
        return await dataContext.StationEventQualities
            .AsNoTracking()
            .Where(row => row.StationEvaNumber == request.StationEvaNumber)
            .Where(row => row.BucketHour >= request.FromUtc && row.BucketHour < request.ToUtc)
            .Where(row => request.ScheduleType == null || row.ScheduleType == request.ScheduleType)
            .Where(row => transportTypes.Length == 0 || transportTypes.Contains(row.TransportType))
            .Where(row => request.IncludeReplacement || !row.IsReplacement)
            .ToListAsync(cancellationToken);
    }

    private async Task<StationBenchmarkResult> BuildBenchmarkAsync(
        StationStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var stationRows = await LoadEventRowsAsync(request, cancellationToken);
        var transportTypes = request.TransportTypes;
        var networkRows = await dataContext.NetworkEventQualities
            .AsNoTracking()
            .Where(row => row.BucketHour >= request.FromUtc && row.BucketHour < request.ToUtc)
            .Where(row => request.ScheduleType == null || row.ScheduleType == request.ScheduleType)
            .Where(row => transportTypes.Length == 0 || transportTypes.Contains(row.TransportType))
            .Where(row => request.IncludeReplacement || !row.IsReplacement)
            .ToListAsync(cancellationToken);

        return new StationBenchmarkResult(
            StatisticsMetricBuilderHelpers.ToEventMetrics(
                StatisticsMetricBuilderHelpers.AggregateEventRows(stationRows)),
            StatisticsMetricBuilderHelpers.ToEventMetrics(
                StatisticsMetricBuilderHelpers.AggregateEventRows(networkRows)),
            null);
    }

    private static IReadOnlyList<EventTimeSeriesPoint> BuildTimeSeries(
        IReadOnlyCollection<IEventQualityHourly> rows,
        StationStatisticsMetricRequest request
    ) => rows
        .GroupBy(row => StatisticsMetricBuilderHelpers.BucketStart(row.BucketHour, request.Bucket))
        .OrderBy(group => group.Key)
        .Select(group => new EventTimeSeriesPoint(
            group.Key,
            StatisticsMetricBuilderHelpers.ToEventMetrics(StatisticsMetricBuilderHelpers.AggregateEventRows(group))))
        .ToList();

    private static IReadOnlyList<ArrivalDepartureComparisonItem> BuildArrivalDepartureComparison(IReadOnlyCollection<StationEventQualityHourly> rows) => rows
        .GroupBy(row => row.ScheduleType)
        .OrderBy(group => group.Key)
        .Select(group => new ArrivalDepartureComparisonItem(
            group.Key,
            StatisticsMetricBuilderHelpers.ToEventMetrics(StatisticsMetricBuilderHelpers.AggregateEventRows(group))))
        .ToList();

    private static IReadOnlyList<EventHeatmapCell> BuildWeekdayHourHeatmap(IReadOnlyCollection<IEventQualityHourly> rows) => rows
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
        .Select(group => new EventHeatmapCell(
            group.Key.Weekday,
            group.Key.Hour,
            StatisticsMetricBuilderHelpers.ToEventMetrics(StatisticsMetricBuilderHelpers.AggregateEventRows(group))))
        .ToList();

    private async Task<StationLineRankingResult> BuildLineRankingAsync(
        StationStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var rows = await LoadStationLineRowsAsync(request, cancellationToken);
        var grouped = rows
            .GroupBy(row => new
            {
                row.JourneyDescription,
                row.TransportType,
                row.OriginEvaNumber,
                row.DestinationEvaNumber
            })
            .Select(group => new
            {
                group.Key.JourneyDescription,
                group.Key.TransportType,
                group.Key.OriginEvaNumber,
                group.Key.DestinationEvaNumber,
                Aggregate = StatisticsMetricBuilderHelpers.AggregateEventRows(group)
            })
            .Where(row => row.Aggregate.EventCount >= request.MinVolume)
            .OrderByDescending(row => row.Aggregate.EventPositiveDelaySumSeconds)
            .ThenByDescending(row => row.Aggregate.EventCount)
            .ToList();

        var stations = await LoadStationsAsync(grouped.SelectMany(row => new[] { row.OriginEvaNumber, row.DestinationEvaNumber }), cancellationToken);
        var items = grouped
            .Skip(request.Offset)
            .Take(request.Limit)
            .Select(row => new StationLineRankingItem(
                new LineReference(
                    row.JourneyDescription,
                    null,
                    row.TransportType,
                    ToStationReference(stations, row.OriginEvaNumber),
                    ToStationReference(stations, row.DestinationEvaNumber)),
                StatisticsMetricBuilderHelpers.ToEventMetrics(row.Aggregate)))
            .ToList();

        return new StationLineRankingResult(
            items,
            new MetricPage(request.Offset, request.Limit, grouped.Count));
    }

    private async Task<IReadOnlyList<StationDirectionItem>> BuildDirectionsAsync(
        StationStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var rows = await LoadStationLineRowsAsync(request, cancellationToken);
        var grouped = rows
            .Select(row => new
            {
                DirectionType = row.OriginEvaNumber == request.StationEvaNumber ? "DESTINATION" : "ORIGIN",
                EvaNumber = row.OriginEvaNumber == request.StationEvaNumber ? row.DestinationEvaNumber : row.OriginEvaNumber,
                Row = row
            })
            .Where(row => request.DirectionEvaNumber == null || row.EvaNumber == request.DirectionEvaNumber)
            .GroupBy(row => new { row.DirectionType, row.EvaNumber })
            .Select(group => new
            {
                group.Key.DirectionType,
                group.Key.EvaNumber,
                Aggregate = StatisticsMetricBuilderHelpers.AggregateEventRows(group.Select(row => row.Row))
            })
            .OrderByDescending(row => row.Aggregate.EventPositiveDelaySumSeconds)
            .ToList();

        var stations = await LoadStationsAsync(grouped.Select(row => row.EvaNumber), cancellationToken);
        return grouped
            .Select(row => new StationDirectionItem(
                row.DirectionType,
                ToStationReference(stations, row.EvaNumber),
                StatisticsMetricBuilderHelpers.ToEventMetrics(row.Aggregate)))
            .ToList();
    }

    private static IReadOnlyList<TransportTypeMixItem> BuildTransportTypeMix(IReadOnlyCollection<IEventQualityHourly> rows)
    {
        var total = rows.Sum(row => row.EventCount);
        return rows
            .GroupBy(row => row.TransportType)
            .OrderByDescending(group => group.Sum(row => row.EventCount))
            .Select(group =>
            {
                var aggregate = StatisticsMetricBuilderHelpers.AggregateEventRows(group);
                return new TransportTypeMixItem(
                    group.Key,
                    StatisticsMetricBuilderHelpers.Rate(aggregate.EventCount, total),
                    StatisticsMetricBuilderHelpers.ToEventMetrics(aggregate));
            })
            .ToList();
    }

    private async Task<IReadOnlyList<LineHourMatrixItem>> BuildLineHourMatrixAsync(
        StationStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var rows = await LoadStationLineRowsAsync(request, cancellationToken);
        return rows
            .GroupBy(row => new
            {
                row.JourneyDescription,
                row.TransportType,
                StatisticsMetricBuilderHelpers.ToBerlinTime(row.BucketHour).Hour
            })
            .OrderBy(group => group.Key.JourneyDescription)
            .ThenBy(group => group.Key.Hour)
            .Select(group => new LineHourMatrixItem(
                group.Key.JourneyDescription,
                group.Key.TransportType,
                group.Key.Hour,
                StatisticsMetricBuilderHelpers.ToEventMetrics(StatisticsMetricBuilderHelpers.AggregateEventRows(group))))
            .ToList();
    }

    private async Task<StationEventDetailsResult> BuildEventDetailsAsync(
        StationStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var transportTypes = request.TransportTypes;
        var query = dataContext.StationJourneyEventDetails
            .AsNoTracking()
            .Where(row => row.StationEvaNumber == request.StationEvaNumber)
            .Where(row => row.PlannedTime >= request.FromUtc && row.PlannedTime < request.ToUtc)
            .Where(row => request.ScheduleType == null || row.ScheduleType == request.ScheduleType)
            .Where(row => transportTypes.Length == 0 || transportTypes.Contains(row.TransportType))
            .Where(row => request.IncludeReplacement || !row.IsReplacement)
            .Where(row => request.JourneyNumber == null || row.JourneyNumber == request.JourneyNumber)
            .Where(row => request.OriginEvaNumber == null || row.OriginEvaNumber == request.OriginEvaNumber)
            .Where(row => request.DestinationEvaNumber == null || row.DestinationEvaNumber == request.DestinationEvaNumber);

        var total = await query.CountAsync(cancellationToken);
        var rows = await query
            .OrderBy(row => row.PlannedTime)
            .Skip(request.Offset)
            .Take(request.Limit)
            .Select(row => new StationEventDetailItem(
                row.StopPlaceId,
                row.JourneyId,
                row.JourneyDate,
                row.StationEvaNumber,
                row.PlannedTime,
                row.ScheduleType,
                row.JourneyNumber,
                row.JourneyDescription,
                row.OriginEvaNumber,
                row.JourneyStartTime,
                row.DestinationEvaNumber,
                row.JourneyEndTime,
                row.StopCancelled,
                row.TransportType,
                row.EventDelaySeconds,
                row.IsReplacement))
            .ToListAsync(cancellationToken);

        return new StationEventDetailsResult(
            rows,
            new MetricPage(request.Offset, request.Limit, total));
    }

    private async Task<List<StationLineQualityHourly>> LoadStationLineRowsAsync(
        StationStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var administrationIds = await StatisticsMetricBuilderHelpers.ResolveAdministrationIdsAsync(dataContext, request, cancellationToken);
        var transportTypes = request.TransportTypes;

        return await dataContext.StationLineQualities
            .AsNoTracking()
            .Where(row => row.StationEvaNumber == request.StationEvaNumber)
            .Where(row => row.BucketHour >= request.FromUtc && row.BucketHour < request.ToUtc)
            .Where(row => request.ScheduleType == null || row.ScheduleType == request.ScheduleType)
            .Where(row => transportTypes.Length == 0 || transportTypes.Contains(row.TransportType))
            .Where(row => request.IncludeReplacement || !row.IsReplacement)
            .Where(row => request.OriginEvaNumber == null || row.OriginEvaNumber == request.OriginEvaNumber)
            .Where(row => request.DestinationEvaNumber == null || row.DestinationEvaNumber == request.DestinationEvaNumber)
            .Where(row => request.DirectionEvaNumber == null || row.OriginEvaNumber == request.DirectionEvaNumber || row.DestinationEvaNumber == request.DirectionEvaNumber)
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

    private static StatisticsResponseMeta CreateMeta(StationStatisticsMetricRequest request) =>
        new(
            "STATION",
            request.MetricType.ToString(),
            request.From,
            request.To,
            request.Bucket,
            StatisticsMetricBuilderHelpers.Timezone,
            StatisticsMetricBuilderHelpers.CreateFilters(request)
                .Concat(new[] { new KeyValuePair<string, object?>("stationEvaNumber", request.StationEvaNumber) })
                .ToDictionary(pair => pair.Key, pair => pair.Value));
}
