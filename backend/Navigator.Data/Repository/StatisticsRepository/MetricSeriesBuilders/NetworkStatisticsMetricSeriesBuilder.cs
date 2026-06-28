using Microsoft.EntityFrameworkCore;
using Navigator.Data.Entities.Views;
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
        StatisticsMetricResult result = request switch
        {
            NetworkEventSummaryRequest => new EventSummaryResult(StatisticsMetricBuilderHelpers.ToEventMetrics(
                StatisticsMetricBuilderHelpers.AggregateEventRows(await LoadEventRowsAsync(request, cancellationToken)))),
            NetworkJourneySummaryRequest => new JourneySummaryResult(StatisticsMetricBuilderHelpers.ToJourneyMetrics(
                StatisticsMetricBuilderHelpers.AggregateJourneyRows(await LoadJourneyRowsAsync(request, cancellationToken)))),
            NetworkEventTimeSeriesRequest => new NetworkEventTimeSeriesResult(BuildEventTimeSeries(await LoadEventRowsAsync(request, cancellationToken), request)),
            NetworkJourneyTimeSeriesRequest => new NetworkJourneyTimeSeriesResult(BuildJourneyTimeSeries(await LoadJourneyRowsAsync(request, cancellationToken), request)),
            NetworkJourneyOutcomeTimeSeriesRequest journeyOutcomeTimeSeriesRequest =>
                new NetworkJourneyOutcomeTimeSeriesResult(await BuildJourneyOutcomeTimeSeriesAsync(journeyOutcomeTimeSeriesRequest, cancellationToken)),
            NetworkWeekdayHourHeatmapRequest => new EventWeekdayHourHeatmapResult(BuildWeekdayHourHeatmap(await LoadEventRowsAsync(request, cancellationToken), request)),
            NetworkTransportTypeComparisonRequest => new TransportTypeComparisonResult(await BuildTransportTypeComparisonAsync(request, cancellationToken)),
            NetworkStationRankingRequest => await BuildStationRankingAsync(request, cancellationToken),
            NetworkLineRankingRequest => await BuildLineRankingAsync(request, cancellationToken),
            NetworkMapHotspotsRequest => new NetworkMapHotspotsResult(await BuildMapHotspotsAsync(request, cancellationToken)),
            NetworkEventDelayDistributionRequest => await BuildEventDelayDistributionAsync(request, cancellationToken),
            _ => throw new NotSupportedException($"Unsupported network statistics metric: {request.GetType().Name}")
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
        var transportTypes = StatisticsMetricBuilderHelpers.TransportTypes(request);
        var scheduleType = StatisticsMetricBuilderHelpers.ScheduleType(request);
        var fromUtc = StatisticsMetricBuilderHelpers.UtcFrom(request);
        var toUtc = StatisticsMetricBuilderHelpers.UtcTo(request);
        var includeReplacement = StatisticsMetricBuilderHelpers.IncludeReplacement(request);

        if (administrationIds.Length > 0)
        {
            var query = dataContext.StationAdministrationQualities
                .AsNoTracking()
                .Where(row => row.BucketHour >= fromUtc && row.BucketHour < toUtc)
                .Where(row => scheduleType == null || row.ScheduleType == scheduleType)
                .Where(row => transportTypes.Length == 0 || transportTypes.Contains(row.TransportType))
                .Where(row => includeReplacement || !row.IsReplacement)
                .Where(row => administrationIds.Contains(row.AdministrationId));

            return (await query.ToListAsync(cancellationToken)).Cast<IEventQualityHourly>().ToList();
        }

        var networkQuery = dataContext.NetworkEventQualities
            .AsNoTracking()
            .Where(row => row.BucketHour >= fromUtc && row.BucketHour < toUtc)
            .Where(row => scheduleType == null || row.ScheduleType == scheduleType)
            .Where(row => transportTypes.Length == 0 || transportTypes.Contains(row.TransportType))
            .Where(row => includeReplacement || !row.IsReplacement);

        return (await networkQuery.ToListAsync(cancellationToken)).Cast<IEventQualityHourly>().ToList();
    }

    private async Task<List<IJourneyQualityHourly>> LoadJourneyRowsAsync(
        NetworkStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var administrationIds = await StatisticsMetricBuilderHelpers.ResolveAdministrationIdsAsync(dataContext, request, cancellationToken);
        var transportTypes = StatisticsMetricBuilderHelpers.TransportTypes(request);
        var fromUtc = StatisticsMetricBuilderHelpers.UtcFrom(request);
        var toUtc = StatisticsMetricBuilderHelpers.UtcTo(request);
        var includeReplacement = StatisticsMetricBuilderHelpers.IncludeReplacement(request);

        if (administrationIds.Length > 0)
        {
            var query = dataContext.JourneyAdministrationQualities
                .AsNoTracking()
                .Where(row => row.BucketHour >= fromUtc && row.BucketHour < toUtc)
                .Where(row => transportTypes.Length == 0 || transportTypes.Contains(row.TransportType))
                .Where(row => includeReplacement || !row.IsReplacement)
                .Where(row => administrationIds.Contains(row.AdministrationId));

            return (await query.ToListAsync(cancellationToken)).Cast<IJourneyQualityHourly>().ToList();
        }

        var networkQuery = dataContext.NetworkJourneyQualities
            .AsNoTracking()
            .Where(row => row.BucketHour >= fromUtc && row.BucketHour < toUtc)
            .Where(row => transportTypes.Length == 0 || transportTypes.Contains(row.TransportType))
            .Where(row => includeReplacement || !row.IsReplacement);

        return (await networkQuery.ToListAsync(cancellationToken)).Cast<IJourneyQualityHourly>().ToList();
    }

    private static IReadOnlyList<EventTimeSeriesPoint> BuildEventTimeSeries(
        IReadOnlyCollection<IEventQualityHourly> rows,
        NetworkStatisticsMetricRequest request
    ) => rows
        .GroupBy(row => StatisticsMetricBuilderHelpers.BucketStart(row.BucketHour, request, ((IHasBucket)request).Bucket))
        .OrderBy(group => group.Key)
        .Select(group => new EventTimeSeriesPoint(
            group.Key,
            StatisticsMetricBuilderHelpers.ToEventMetrics(StatisticsMetricBuilderHelpers.AggregateEventRows(group))))
        .ToList();

    private static IReadOnlyList<JourneyTimeSeriesPoint> BuildJourneyTimeSeries(
        IReadOnlyCollection<IJourneyQualityHourly> rows,
        NetworkStatisticsMetricRequest request
    ) => rows
        .GroupBy(row => StatisticsMetricBuilderHelpers.BucketStart(row.BucketHour, request, ((IHasBucket)request).Bucket))
        .OrderBy(group => group.Key)
        .Select(group => new JourneyTimeSeriesPoint(
            group.Key,
            StatisticsMetricBuilderHelpers.ToJourneyMetrics(StatisticsMetricBuilderHelpers.AggregateJourneyRows(group))))
        .ToList();

    private async Task<IReadOnlyList<JourneyOutcomeTimeSeriesPoint>> BuildJourneyOutcomeTimeSeriesAsync(
        NetworkJourneyOutcomeTimeSeriesRequest request,
        CancellationToken cancellationToken
    )
    {
        var administrationIds = await StatisticsMetricBuilderHelpers.ResolveAdministrationIdsAsync(dataContext, request, cancellationToken);
        var transportTypes = StatisticsMetricBuilderHelpers.TransportTypes(request);
        var fromUtc = StatisticsMetricBuilderHelpers.UtcFrom(request);
        var toUtc = StatisticsMetricBuilderHelpers.UtcTo(request);
        var includeReplacement = StatisticsMetricBuilderHelpers.IncludeReplacement(request);

        var rows = await dataContext.JourneyQualityFacts
            .AsNoTracking()
            .Where(row => row.BucketHour >= fromUtc && row.BucketHour < toUtc)
            .Where(row => transportTypes.Length == 0 || transportTypes.Contains(row.TransportType))
            .Where(row => includeReplacement || !row.IsReplacement)
            .Where(row => administrationIds.Length == 0 || administrationIds.Contains(row.AdministrationId))
            .Select(row => new
            {
                row.BucketHour,
                row.FullyCancelled,
                row.DestinationNotReached,
                row.PartiallyCancelled
            })
            .ToListAsync(cancellationToken);

        return rows
            .GroupBy(row => StatisticsMetricBuilderHelpers.BucketStart(row.BucketHour, request, request.Bucket))
            .OrderBy(group => group.Key)
            .Select(group =>
            {
                var planned = group.LongCount();
                var fullyCancelled = group.LongCount(row => row.FullyCancelled);
                var destinationNotReached = group.LongCount(row => !row.FullyCancelled && row.DestinationNotReached);
                var partiallyCancelledDestinationReached = group.LongCount(row =>
                    !row.FullyCancelled && !row.DestinationNotReached && row.PartiallyCancelled);
                var completed = planned - fullyCancelled - destinationNotReached - partiallyCancelledDestinationReached;

                return new JourneyOutcomeTimeSeriesPoint(
                    group.Key,
                    new JourneyOutcomeMetrics(
                        planned,
                        completed,
                        partiallyCancelledDestinationReached,
                        destinationNotReached,
                        fullyCancelled));
            })
            .ToList();
    }

    private static IReadOnlyList<EventHeatmapCell> BuildWeekdayHourHeatmap(
        IReadOnlyCollection<IEventQualityHourly> rows,
        NetworkStatisticsMetricRequest request
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
        var transportTypes = StatisticsMetricBuilderHelpers.TransportTypes(request);
        var scheduleType = StatisticsMetricBuilderHelpers.ScheduleType(request);
        var fromUtc = StatisticsMetricBuilderHelpers.UtcFrom(request);
        var toUtc = StatisticsMetricBuilderHelpers.UtcTo(request);
        var includeReplacement = StatisticsMetricBuilderHelpers.IncludeReplacement(request);
        var minVolume = StatisticsMetricBuilderHelpers.MinVolume(request);
        var offset = StatisticsMetricBuilderHelpers.Offset(request);
        var limit = StatisticsMetricBuilderHelpers.Limit(request);
        var query = dataContext.StationEventQualities
            .AsNoTracking()
            .Where(row => row.BucketHour >= fromUtc && row.BucketHour < toUtc)
            .Where(row => scheduleType == null || row.ScheduleType == scheduleType)
            .Where(row => transportTypes.Length == 0 || transportTypes.Contains(row.TransportType))
            .Where(row => includeReplacement || !row.IsReplacement);

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
            .Where(row => row.EventCount >= minVolume)
            .ToListAsync(cancellationToken);

        var stations = await LoadStationsAsync(grouped.Select(row => row.StationEvaNumber), cancellationToken);
        var ordered = grouped
            .OrderByDescending(row => row.EventPositiveDelaySumSeconds)
            .ThenByDescending(row => row.EventCount)
            .Skip(offset)
            .Take(limit)
            .Select(row => new StationEventRankingItem(
                ToStationReference(stations, row.StationEvaNumber),
                StatisticsMetricBuilderHelpers.ToEventMetrics(row.ToAggregate())))
            .ToList();

        return new NetworkStationRankingResult(
            ordered,
            new MetricPage(offset, limit, grouped.Count));
    }

    private async Task<NetworkLineRankingResult> BuildLineRankingAsync(
        NetworkStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var administrationIds = await StatisticsMetricBuilderHelpers.ResolveAdministrationIdsAsync(dataContext, request, cancellationToken);
        var transportTypes = StatisticsMetricBuilderHelpers.TransportTypes(request);
        var fromUtc = StatisticsMetricBuilderHelpers.UtcFrom(request);
        var toUtc = StatisticsMetricBuilderHelpers.UtcTo(request);
        var includeReplacement = StatisticsMetricBuilderHelpers.IncludeReplacement(request);
        var originEvaNumber = StatisticsMetricBuilderHelpers.OriginEvaNumber(request);
        var destinationEvaNumber = StatisticsMetricBuilderHelpers.DestinationEvaNumber(request);
        var minVolume = StatisticsMetricBuilderHelpers.MinVolume(request);
        var offset = StatisticsMetricBuilderHelpers.Offset(request);
        var limit = StatisticsMetricBuilderHelpers.Limit(request);

        var query = dataContext.LineJourneyQualities
            .AsNoTracking()
            .Where(row => row.BucketHour >= fromUtc && row.BucketHour < toUtc)
            .Where(row => transportTypes.Length == 0 || transportTypes.Contains(row.TransportType))
            .Where(row => includeReplacement || !row.IsReplacement)
            .Where(row => originEvaNumber == null || row.OriginEvaNumber == originEvaNumber)
            .Where(row => destinationEvaNumber == null || row.DestinationEvaNumber == destinationEvaNumber)
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
            .Where(row => row.JourneyCount >= minVolume)
            .ToListAsync(cancellationToken);

        var stations = await LoadStationsAsync(grouped.SelectMany(row => new[] { row.OriginEvaNumber, row.DestinationEvaNumber }), cancellationToken);
        var ordered = grouped
            .OrderByDescending(row => row.DestinationPositiveDelaySumSeconds)
            .ThenByDescending(row => row.JourneyCount)
            .Skip(offset)
            .Take(limit)
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
            new MetricPage(offset, limit, grouped.Count));
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
                ["servedEvents"] = row.EventMetrics.ServedEvents,
                ["cancellationRate"] = row.EventMetrics.CancellationRate,
                ["operativePunctuality5Rate"] = row.EventMetrics.OperativePunctuality5Rate,
                ["customerReliability5Rate"] = row.EventMetrics.CustomerReliability5Rate,
                ["operativePunctuality15Rate"] = row.EventMetrics.OperativePunctuality15Rate,
                ["customerReliability15Rate"] = row.EventMetrics.CustomerReliability15Rate,
                ["delayDebtMinutes"] = row.EventMetrics.DelayDebtMinutes,
                ["score"] = row.EventMetrics.CustomerReliability5Rate
            }))
            .ToList();

        return new GeoJsonFeatureCollection("FeatureCollection", features);
    }

    private async Task<EventDelayDistributionResult> BuildEventDelayDistributionAsync(
        NetworkStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var administrationIds = await StatisticsMetricBuilderHelpers.ResolveAdministrationIdsAsync(dataContext, request, cancellationToken);
        var transportTypes = StatisticsMetricBuilderHelpers.TransportTypes(request);
        var scheduleType = StatisticsMetricBuilderHelpers.ScheduleType(request);
        var fromUtc = StatisticsMetricBuilderHelpers.UtcFrom(request);
        var toUtc = StatisticsMetricBuilderHelpers.UtcTo(request);
        var includeReplacement = StatisticsMetricBuilderHelpers.IncludeReplacement(request);

        var delayQuery = dataContext.JourneyEventQualityFacts
            .AsNoTracking()
            .Where(row => row.BucketHour >= fromUtc && row.BucketHour < toUtc)
            .Where(row => !row.StopCancelled)
            .Where(row => scheduleType == null || row.ScheduleType == scheduleType)
            .Where(row => transportTypes.Length == 0 || transportTypes.Contains(row.TransportType))
            .Where(row => includeReplacement || !row.IsReplacement)
            .Where(row => administrationIds.Length == 0 || administrationIds.Contains(row.AdministrationId))
            .Select(row => row.EventDelaySeconds);

        var sampleCount = await delayQuery.LongCountAsync(cancellationToken);

        var summary = new EventDelayDistributionSummary(
            sampleCount,
            await PercentileContAsync(delayQuery, sampleCount, 0.5, cancellationToken),
            await PercentileContAsync(delayQuery, sampleCount, 0.95, cancellationToken));

        if (sampleCount == 0) return new EventDelayDistributionResult(summary, []);

        var counts = await delayQuery
            .GroupBy(_ => 1)
            .Select(group => new EventDelayDistributionBinCounts
            {
                EarlierThanMinus5Minutes = group.LongCount(delay => delay < -300),
                Minus5To0Minutes = group.LongCount(delay => delay >= -300 && delay < 0),
                ZeroTo5Minutes = group.LongCount(delay => delay >= 0 && delay < 300),
                FiveTo10Minutes = group.LongCount(delay => delay >= 300 && delay < 600),
                TenTo15Minutes = group.LongCount(delay => delay >= 600 && delay < 900),
                FifteenTo30Minutes = group.LongCount(delay => delay >= 900 && delay < 1800),
                ThirtyTo60Minutes = group.LongCount(delay => delay >= 1800 && delay < 3600),
                SixtyTo120Minutes = group.LongCount(delay => delay >= 3600 && delay < 7200),
                LaterThan120Minutes = group.LongCount(delay => delay >= 7200)
            })
            .SingleAsync(cancellationToken);

        var binEdges = new[]
        {
            int.MinValue, -300, 0, 300, 600, 900, 1800, 3600, 7200, int.MaxValue
        };
        var binCounts = new[]
        {
            counts.EarlierThanMinus5Minutes,
            counts.Minus5To0Minutes,
            counts.ZeroTo5Minutes,
            counts.FiveTo10Minutes,
            counts.TenTo15Minutes,
            counts.FifteenTo30Minutes,
            counts.ThirtyTo60Minutes,
            counts.SixtyTo120Minutes,
            counts.LaterThan120Minutes
        };

        var bins = new List<EventDelayDistributionBin>(binEdges.Length - 1);
        long cumulativeCount = 0;
        var decimalSampleCount = (decimal)sampleCount;

        for (var index = 0; index < binEdges.Length - 1; index++)
        {
            var lower = binEdges[index];
            var upper = binEdges[index + 1];
            var count = binCounts[index];
            cumulativeCount += count;

            bins.Add(new EventDelayDistributionBin(
                lower,
                upper,
                count,
                cumulativeCount / decimalSampleCount));
        }

        return new EventDelayDistributionResult(summary, bins);
    }

    private static async Task<decimal?> PercentileContAsync(
        IQueryable<int> delayQuery,
        long sampleCount,
        double percentile,
        CancellationToken cancellationToken
    )
    {
        if (sampleCount == 0) return null;
        if (sampleCount > int.MaxValue) throw new InvalidOperationException("Delay distribution percentile offsets exceed supported query size.");

        var position = (sampleCount - 1) * percentile;
        var lowerIndex = (int)Math.Floor(position);
        var upperIndex = (int)Math.Ceiling(position);
        var values = await delayQuery
            .OrderBy(delay => delay)
            .Skip(lowerIndex)
            .Take(upperIndex - lowerIndex + 1)
            .ToArrayAsync(cancellationToken);

        if (values.Length == 0) return null;
        if (lowerIndex == upperIndex || values.Length == 1) return values[0];

        var weight = (decimal)(position - lowerIndex);
        return values[0] + (values[^1] - values[0]) * weight;
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
            StatisticsMetricBuilderHelpers.Scope(request),
            StatisticsMetricBuilderHelpers.Metric(request),
            request.From,
            request.To,
            StatisticsMetricBuilderHelpers.Bucket(request),
            StatisticsMetricBuilderHelpers.CreateFilters(request));

    private sealed class EventDelayDistributionBinCounts
    {
        public long EarlierThanMinus5Minutes { get; init; }
        public long Minus5To0Minutes { get; init; }
        public long ZeroTo5Minutes { get; init; }
        public long FiveTo10Minutes { get; init; }
        public long TenTo15Minutes { get; init; }
        public long FifteenTo30Minutes { get; init; }
        public long ThirtyTo60Minutes { get; init; }
        public long SixtyTo120Minutes { get; init; }
        public long LaterThan120Minutes { get; init; }
    }

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
