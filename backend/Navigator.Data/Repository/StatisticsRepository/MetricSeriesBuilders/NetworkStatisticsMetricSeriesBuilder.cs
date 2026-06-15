using Microsoft.EntityFrameworkCore;
using Navigator.Data.Entities.Views;
using Navigator.Data.Enums.Metric;
using Navigator.Data.Models.Statistics.Api;

namespace Navigator.Data.Repository.StatisticsRepository.MetricSeriesBuilders;

public sealed class NetworkStatisticsMetricSeriesBuilder(
    DataContext dataContext
) : StatisticsMetricBuilder<NetworkStatisticsMetricRequest>
{
    protected override async Task<StatisticsMetricResponse> BuildAsync(
        NetworkStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        StatisticsMetricResult result = request.MetricType switch
        {
            NetworkStatisticsMetricType.EventKpis => new EventKpisResult(StatisticsMetricBuilderHelpers.ToEventMetrics(
                StatisticsMetricBuilderHelpers.AggregateEventRows(await LoadEventRowsAsync(request, cancellationToken)))),
            NetworkStatisticsMetricType.JourneyKpis => new JourneyKpisResult(StatisticsMetricBuilderHelpers.ToJourneyMetrics(
                StatisticsMetricBuilderHelpers.AggregateJourneyRows(await LoadJourneyRowsAsync(request, cancellationToken)))),
            NetworkStatisticsMetricType.EventTimeSeries => new NetworkEventTimeSeriesResult(BuildEventTimeSeries(await LoadEventRowsAsync(request, cancellationToken), request)),
            NetworkStatisticsMetricType.JourneyTimeSeries => new NetworkJourneyTimeSeriesResult(BuildJourneyTimeSeries(await LoadJourneyRowsAsync(request, cancellationToken), request)),
            NetworkStatisticsMetricType.WeekdayHourHeatmap => new EventWeekdayHourHeatmapResult(BuildWeekdayHourHeatmap(await LoadEventRowsAsync(request, cancellationToken))),
            NetworkStatisticsMetricType.TransportTypeComparison => new TransportTypeComparisonResult(await BuildTransportTypeComparisonAsync(request, cancellationToken)),
            NetworkStatisticsMetricType.StationRanking => await BuildStationRankingAsync(request, cancellationToken),
            NetworkStatisticsMetricType.LineRanking => await BuildLineRankingAsync(request, cancellationToken),
            NetworkStatisticsMetricType.MapHotspots => new NetworkMapHotspotsResult(await BuildMapHotspotsAsync(request, cancellationToken)),
            _ => throw new NotSupportedException($"Unsupported network statistics metric: {request.MetricType}")
        };

        return new StatisticsMetricResponse(
            CreateMeta(request),
            result);
    }

    private async Task<List<IEventQualityHourly>> LoadEventRowsAsync(
        NetworkStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var administrationIds = await StatisticsMetricBuilderHelpers.ResolveAdministrationIdsAsync(dataContext, request, cancellationToken);
        var transportTypes = request.TransportTypes;

        if (administrationIds.Length > 0)
        {
            var query = dataContext.StationAdministrationQualities
                .AsNoTracking()
                .Where(row => row.BucketHour >= request.FromUtc && row.BucketHour < request.ToUtc)
                .Where(row => request.ScheduleType == null || row.ScheduleType == request.ScheduleType)
                .Where(row => transportTypes.Length == 0 || transportTypes.Contains(row.TransportType))
                .Where(row => request.IncludeReplacement || !row.IsReplacement)
                .Where(row => administrationIds.Contains(row.AdministrationId));

            return (await query.ToListAsync(cancellationToken)).Cast<IEventQualityHourly>().ToList();
        }

        var networkQuery = dataContext.NetworkEventQualities
            .AsNoTracking()
            .Where(row => row.BucketHour >= request.FromUtc && row.BucketHour < request.ToUtc)
            .Where(row => request.ScheduleType == null || row.ScheduleType == request.ScheduleType)
            .Where(row => transportTypes.Length == 0 || transportTypes.Contains(row.TransportType))
            .Where(row => request.IncludeReplacement || !row.IsReplacement);

        return (await networkQuery.ToListAsync(cancellationToken)).Cast<IEventQualityHourly>().ToList();
    }

    private async Task<List<IJourneyQualityHourly>> LoadJourneyRowsAsync(
        NetworkStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var administrationIds = await StatisticsMetricBuilderHelpers.ResolveAdministrationIdsAsync(dataContext, request, cancellationToken);
        var transportTypes = request.TransportTypes;

        if (administrationIds.Length > 0)
        {
            var query = dataContext.JourneyAdministrationQualities
                .AsNoTracking()
                .Where(row => row.BucketHour >= request.FromUtc && row.BucketHour < request.ToUtc)
                .Where(row => transportTypes.Length == 0 || transportTypes.Contains(row.TransportType))
                .Where(row => request.IncludeReplacement || !row.IsReplacement)
                .Where(row => administrationIds.Contains(row.AdministrationId));

            return (await query.ToListAsync(cancellationToken)).Cast<IJourneyQualityHourly>().ToList();
        }

        var networkQuery = dataContext.NetworkJourneyQualities
            .AsNoTracking()
            .Where(row => row.BucketHour >= request.FromUtc && row.BucketHour < request.ToUtc)
            .Where(row => transportTypes.Length == 0 || transportTypes.Contains(row.TransportType))
            .Where(row => request.IncludeReplacement || !row.IsReplacement);

        return (await networkQuery.ToListAsync(cancellationToken)).Cast<IJourneyQualityHourly>().ToList();
    }

    private static IReadOnlyList<EventTimeSeriesPoint> BuildEventTimeSeries(
        IReadOnlyCollection<IEventQualityHourly> rows,
        NetworkStatisticsMetricRequest request
    ) => rows
        .GroupBy(row => StatisticsMetricBuilderHelpers.BucketStart(row.BucketHour, request.Bucket))
        .OrderBy(group => group.Key)
        .Select(group => new EventTimeSeriesPoint(
            group.Key,
            StatisticsMetricBuilderHelpers.ToEventMetrics(StatisticsMetricBuilderHelpers.AggregateEventRows(group))))
        .ToList();

    private static IReadOnlyList<JourneyTimeSeriesPoint> BuildJourneyTimeSeries(
        IReadOnlyCollection<IJourneyQualityHourly> rows,
        NetworkStatisticsMetricRequest request
    ) => rows
        .GroupBy(row => StatisticsMetricBuilderHelpers.BucketStart(row.BucketHour, request.Bucket))
        .OrderBy(group => group.Key)
        .Select(group => new JourneyTimeSeriesPoint(
            group.Key,
            StatisticsMetricBuilderHelpers.ToJourneyMetrics(StatisticsMetricBuilderHelpers.AggregateJourneyRows(group))))
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

    private async Task<IReadOnlyList<TransportTypeComparisonItem>> BuildTransportTypeComparisonAsync(
        NetworkStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var eventRows = await LoadEventRowsAsync(request, cancellationToken);
        var journeyRows = await LoadJourneyRowsAsync(request, cancellationToken);

        return eventRows.Select(row => row.TransportType)
            .Concat(journeyRows.Select(row => row.TransportType))
            .Distinct()
            .OrderBy(type => type)
            .Select(type => new TransportTypeComparisonItem(
                type,
                StatisticsMetricBuilderHelpers.ToEventMetrics(
                    StatisticsMetricBuilderHelpers.AggregateEventRows(eventRows.Where(row => row.TransportType == type))),
                StatisticsMetricBuilderHelpers.ToJourneyMetrics(
                    StatisticsMetricBuilderHelpers.AggregateJourneyRows(journeyRows.Where(row => row.TransportType == type)))))
            .ToList();
    }

    private async Task<NetworkStationRankingResult> BuildStationRankingAsync(
        NetworkStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var transportTypes = request.TransportTypes;
        var query = dataContext.StationEventQualities
            .AsNoTracking()
            .Where(row => row.BucketHour >= request.FromUtc && row.BucketHour < request.ToUtc)
            .Where(row => request.ScheduleType == null || row.ScheduleType == request.ScheduleType)
            .Where(row => transportTypes.Length == 0 || transportTypes.Contains(row.TransportType))
            .Where(row => request.IncludeReplacement || !row.IsReplacement);

        var grouped = await query
            .GroupBy(row => row.StationEvaNumber)
            .Select(group => new StationRankingRow
            {
                StationEvaNumber = group.Key,
                EventCount = group.Sum(row => row.EventCount),
                StopCancelledCount = group.Sum(row => row.StopCancelledCount),
                EventDelaySumSeconds = group.Sum(row => row.EventDelaySumSeconds),
                EventPositiveDelaySumSeconds = group.Sum(row => row.EventPositiveDelaySumSeconds),
                EventPunctual5Count = group.Sum(row => row.EventPunctual5Count),
                EventPunctual15Count = group.Sum(row => row.EventPunctual15Count),
                EventLate30Count = group.Sum(row => row.EventLate30Count),
                EventLate60Count = group.Sum(row => row.EventLate60Count)
            })
            .Where(row => row.EventCount >= request.MinVolume)
            .ToListAsync(cancellationToken);

        var stations = await LoadStationsAsync(grouped.Select(row => row.StationEvaNumber), cancellationToken);
        var ordered = grouped
            .OrderByDescending(row => row.EventPositiveDelaySumSeconds)
            .ThenByDescending(row => row.EventCount)
            .Skip(request.Offset)
            .Take(request.Limit)
            .Select(row => new StationEventRankingItem(
                ToStationReference(stations, row.StationEvaNumber),
                StatisticsMetricBuilderHelpers.ToEventMetrics(row.ToAggregate())))
            .ToList();

        return new NetworkStationRankingResult(
            ordered,
            new MetricPage(request.Offset, request.Limit, grouped.Count));
    }

    private async Task<NetworkLineRankingResult> BuildLineRankingAsync(
        NetworkStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var administrationIds = await StatisticsMetricBuilderHelpers.ResolveAdministrationIdsAsync(dataContext, request, cancellationToken);
        var transportTypes = request.TransportTypes;

        var query = dataContext.LineJourneyQualities
            .AsNoTracking()
            .Where(row => row.BucketHour >= request.FromUtc && row.BucketHour < request.ToUtc)
            .Where(row => transportTypes.Length == 0 || transportTypes.Contains(row.TransportType))
            .Where(row => request.IncludeReplacement || !row.IsReplacement)
            .Where(row => request.OriginEvaNumber == null || row.OriginEvaNumber == request.OriginEvaNumber)
            .Where(row => request.DestinationEvaNumber == null || row.DestinationEvaNumber == request.DestinationEvaNumber)
            .Where(row => administrationIds.Length == 0 || administrationIds.Contains(row.AdministrationId));

        var grouped = await query
            .GroupBy(row => new
            {
                row.JourneyDescription,
                row.TransportType,
                row.OriginEvaNumber,
                row.DestinationEvaNumber
            })
            .Select(group => new LineJourneyRankingRow
            {
                LineName = group.Key.JourneyDescription,
                TransportType = group.Key.TransportType,
                OriginEvaNumber = group.Key.OriginEvaNumber,
                DestinationEvaNumber = group.Key.DestinationEvaNumber,
                JourneyCount = group.Sum(row => row.JourneyCount),
                FullyCancelledCount = group.Sum(row => row.FullyCancelledCount),
                PartiallyCancelledCount = group.Sum(row => row.PartiallyCancelledCount),
                DestinationNotReachedCount = group.Sum(row => row.DestinationNotReachedCount),
                DestinationDelaySumSeconds = group.Sum(row => row.DestinationDelaySumSeconds),
                DestinationPositiveDelaySumSeconds = group.Sum(row => row.DestinationPositiveDelaySumSeconds),
                DestinationPunctual5Count = group.Sum(row => row.DestinationPunctual5Count),
                DestinationPunctual15Count = group.Sum(row => row.DestinationPunctual15Count),
                DestinationLate30Count = group.Sum(row => row.DestinationLate30Count),
                DestinationLate60Count = group.Sum(row => row.DestinationLate60Count)
            })
            .Where(row => row.JourneyCount >= request.MinVolume)
            .ToListAsync(cancellationToken);

        var stations = await LoadStationsAsync(grouped.SelectMany(row => new[] { row.OriginEvaNumber, row.DestinationEvaNumber }), cancellationToken);
        var ordered = grouped
            .OrderByDescending(row => row.DestinationPositiveDelaySumSeconds)
            .ThenByDescending(row => row.JourneyCount)
            .Skip(request.Offset)
            .Take(request.Limit)
            .Select(row => new LineJourneyRankingItem(
                new LineReference(
                    row.LineName,
                    null,
                    row.TransportType,
                    ToStationReference(stations, row.OriginEvaNumber),
                    ToStationReference(stations, row.DestinationEvaNumber)),
                StatisticsMetricBuilderHelpers.ToJourneyMetrics(row.ToAggregate())))
            .ToList();

        return new NetworkLineRankingResult(
            ordered,
            new MetricPage(request.Offset, request.Limit, grouped.Count));
    }

    private async Task<GeoJsonFeatureCollection> BuildMapHotspotsAsync(
        NetworkStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var ranking = await BuildStationRankingAsync(request, cancellationToken);
        var stationRows = ranking.Items
            .Where(row => row.Station.Latitude is not null && row.Station.Longitude is not null)
            .ToArray();

        var features = stationRows.Select(row => new GeoJsonFeature(
            "Feature",
            row.Station.StationEvaNumber.ToString(),
            new GeoJsonPoint("Point", [row.Station.Longitude!.Value, row.Station.Latitude!.Value]),
            new Dictionary<string, object?>
            {
                ["stationEvaNumber"] = row.Station.StationEvaNumber,
                ["stationName"] = row.Station.StationName,
                ["plannedEvents"] = row.EventMetrics.PlannedEvents,
                ["cancellationRate"] = row.EventMetrics.CancellationRate,
                ["customerReliability5Rate"] = row.EventMetrics.CustomerReliability5Rate,
                ["delayDebtMinutes"] = row.EventMetrics.DelayDebtMinutes,
                ["score"] = row.EventMetrics.CustomerReliability5Rate
            }))
            .ToList();

        return new GeoJsonFeatureCollection("FeatureCollection", features);
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
                station => new StationReference(
                    station.EvaNumber,
                    station.Name,
                    station.Latitude,
                    station.Longitude),
                cancellationToken);
    }

    private static StationReference ToStationReference(IReadOnlyDictionary<int, StationReference> stations, int evaNumber) =>
        stations.TryGetValue(evaNumber, out var station)
            ? station
            : new StationReference(evaNumber);

    private static StatisticsResponseMeta CreateMeta(NetworkStatisticsMetricRequest request) =>
        new(
            "NETWORK",
            request.MetricType.ToString(),
            request.From,
            request.To,
            request.Bucket,
            StatisticsMetricBuilderHelpers.Timezone,
            StatisticsMetricBuilderHelpers.CreateFilters(request));

    private sealed class StationRankingRow
    {
        public int StationEvaNumber { get; init; }
        public long EventCount { get; init; }
        public long StopCancelledCount { get; init; }
        public long EventDelaySumSeconds { get; init; }
        public long EventPositiveDelaySumSeconds { get; init; }
        public long EventPunctual5Count { get; init; }
        public long EventPunctual15Count { get; init; }
        public long EventLate30Count { get; init; }
        public long EventLate60Count { get; init; }

        public EventAggregate ToAggregate() => new()
        {
            EventCount = EventCount,
            StopCancelledCount = StopCancelledCount,
            EventDelaySumSeconds = EventDelaySumSeconds,
            EventPositiveDelaySumSeconds = EventPositiveDelaySumSeconds,
            EventPunctual5Count = EventPunctual5Count,
            EventPunctual15Count = EventPunctual15Count,
            EventLate30Count = EventLate30Count,
            EventLate60Count = EventLate60Count
        };
    }

    private sealed class LineJourneyRankingRow
    {
        public required string LineName { get; init; }
        public required Navigator.Data.Enums.TransportType TransportType { get; init; }
        public int OriginEvaNumber { get; init; }
        public int DestinationEvaNumber { get; init; }
        public long JourneyCount { get; init; }
        public long FullyCancelledCount { get; init; }
        public long PartiallyCancelledCount { get; init; }
        public long DestinationNotReachedCount { get; init; }
        public long DestinationDelaySumSeconds { get; init; }
        public long DestinationPositiveDelaySumSeconds { get; init; }
        public long DestinationPunctual5Count { get; init; }
        public long DestinationPunctual15Count { get; init; }
        public long DestinationLate30Count { get; init; }
        public long DestinationLate60Count { get; init; }

        public JourneyAggregate ToAggregate() => new()
        {
            JourneyCount = JourneyCount,
            FullyCancelledCount = FullyCancelledCount,
            PartiallyCancelledCount = PartiallyCancelledCount,
            DestinationNotReachedCount = DestinationNotReachedCount,
            DestinationDelaySumSeconds = DestinationDelaySumSeconds,
            DestinationPositiveDelaySumSeconds = DestinationPositiveDelaySumSeconds,
            DestinationPunctual5Count = DestinationPunctual5Count,
            DestinationPunctual15Count = DestinationPunctual15Count,
            DestinationLate30Count = DestinationLate30Count,
            DestinationLate60Count = DestinationLate60Count
        };
    }
}
