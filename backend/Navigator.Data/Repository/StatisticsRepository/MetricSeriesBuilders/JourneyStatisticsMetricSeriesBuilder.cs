using Microsoft.EntityFrameworkCore;
using Navigator.Data.Entities.Views;
using Navigator.Data.Models.Statistics.Api;

namespace Navigator.Data.Repository.StatisticsRepository.MetricSeriesBuilders;

public sealed class JourneyStatisticsMetricSeriesBuilder(
    DataContext dataContext
) : StatisticsMetricBuilder<JourneyStatisticsMetricRequest>
{
    protected override async Task<StatisticsMetricResponse> BuildAsync(
        JourneyStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        StatisticsMetricResult result = request.MetricType switch
        {
            Navigator.Data.Enums.Metric.JourneyStatisticsMetricType.Pattern => await BuildPatternAsync(request, cancellationToken),
            Navigator.Data.Enums.Metric.JourneyStatisticsMetricType.JourneyKpis => new JourneyKpisResult(StatisticsMetricBuilderHelpers.ToJourneyMetrics(
                StatisticsMetricBuilderHelpers.AggregateJourneyRows(await LoadJourneyNumberRowsAsync(request, cancellationToken)))),
            Navigator.Data.Enums.Metric.JourneyStatisticsMetricType.DailyOutcomes => await BuildDailyOutcomesAsync(request, cancellationToken),
            Navigator.Data.Enums.Metric.JourneyStatisticsMetricType.StopProfile => await BuildStopProfileAsync(request, cancellationToken),
            Navigator.Data.Enums.Metric.JourneyStatisticsMetricType.DelayBuildUp => await BuildDelayBuildUpAsync(request, cancellationToken),
            Navigator.Data.Enums.Metric.JourneyStatisticsMetricType.Calendar => await BuildCalendarAsync(request, cancellationToken),
            _ => throw new NotSupportedException($"Unsupported journey statistics metric: {request.MetricType}")
        };

        return new StatisticsMetricResponse(CreateMeta(request), result);
    }

    private async Task<List<JourneyNumberQualityHourly>> LoadJourneyNumberRowsAsync(
        JourneyStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var administrationIds = await StatisticsMetricBuilderHelpers.ResolveAdministrationIdsAsync(dataContext, request, cancellationToken);
        var transportTypes = request.TransportTypes;

        return await dataContext.JourneyNumberQualities
            .AsNoTracking()
            .Where(row => row.JourneyNumber == request.JourneyNumber)
            .Where(row => request.LineName == null || row.JourneyDescription == request.LineName)
            .Where(row => row.BucketHour >= request.FromUtc && row.BucketHour < request.ToUtc)
            .Where(row => transportTypes.Length == 0 || transportTypes.Contains(row.TransportType))
            .Where(row => request.IncludeReplacement || !row.IsReplacement)
            .Where(row => request.OriginEvaNumber == null || row.OriginEvaNumber == request.OriginEvaNumber)
            .Where(row => request.DestinationEvaNumber == null || row.DestinationEvaNumber == request.DestinationEvaNumber)
            .Where(row => administrationIds.Length == 0 || administrationIds.Contains(row.AdministrationId))
            .ToListAsync(cancellationToken);
    }

    private async Task<List<JourneyQualityDetail>> LoadJourneyDetailsAsync(
        JourneyStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var administrationIds = await StatisticsMetricBuilderHelpers.ResolveAdministrationIdsAsync(dataContext, request, cancellationToken);
        var transportTypes = request.TransportTypes;

        return await dataContext.JourneyQualityDetails
            .AsNoTracking()
            .Where(row => row.JourneyNumber == request.JourneyNumber)
            .Where(row => request.LineName == null || row.JourneyDescription == request.LineName)
            .Where(row => row.JourneyStartTime >= request.FromUtc && row.JourneyStartTime < request.ToUtc)
            .Where(row => transportTypes.Length == 0 || transportTypes.Contains(row.TransportType))
            .Where(row => request.IncludeReplacement || !row.IsReplacement)
            .Where(row => request.OriginEvaNumber == null || row.OriginEvaNumber == request.OriginEvaNumber)
            .Where(row => request.DestinationEvaNumber == null || row.DestinationEvaNumber == request.DestinationEvaNumber)
            .Where(row => administrationIds.Length == 0 || administrationIds.Contains(row.AdministrationId))
            .OrderBy(row => row.JourneyDate)
            .ThenBy(row => row.JourneyStartTime)
            .ToListAsync(cancellationToken);
    }

    private async Task<List<StationJourneyEventDetail>> LoadStopDetailsAsync(
        JourneyStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var transportTypes = request.TransportTypes;

        return await dataContext.StationJourneyEventDetails
            .AsNoTracking()
            .Where(row => row.JourneyNumber == request.JourneyNumber)
            .Where(row => request.LineName == null || row.JourneyDescription == request.LineName)
            .Where(row => row.PlannedTime >= request.FromUtc && row.PlannedTime < request.ToUtc)
            .Where(row => request.ScheduleType == null || row.ScheduleType == request.ScheduleType)
            .Where(row => transportTypes.Length == 0 || transportTypes.Contains(row.TransportType))
            .Where(row => request.IncludeReplacement || !row.IsReplacement)
            .Where(row => request.OriginEvaNumber == null || row.OriginEvaNumber == request.OriginEvaNumber)
            .Where(row => request.DestinationEvaNumber == null || row.DestinationEvaNumber == request.DestinationEvaNumber)
            .OrderBy(row => row.PlannedTime)
            .ThenBy(row => row.StationEvaNumber)
            .ToListAsync(cancellationToken);
    }

    private async Task<JourneyPatternResult> BuildPatternAsync(
        JourneyStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var rows = await LoadJourneyDetailsAsync(request, cancellationToken);
        var main = rows
            .GroupBy(row => new
            {
                row.JourneyDescription,
                row.TransportType,
                row.OriginEvaNumber,
                row.DestinationEvaNumber
            })
            .OrderByDescending(group => group.Count())
            .FirstOrDefault();

        var evaNumbers = main is null
            ? Array.Empty<int>()
            : [main.Key.OriginEvaNumber, main.Key.DestinationEvaNumber];
        var stations = await LoadStationsAsync(evaNumbers, cancellationToken);

        return new JourneyPatternResult(
            request.JourneyNumber,
            main?.Key.JourneyDescription ?? request.LineName,
            main?.Key.TransportType,
            rows
                .Select(row => StatisticsMetricBuilderHelpers.ToBerlinTime(row.JourneyStartTime).TimeOfDay)
                .OrderBy(time => time)
                .FirstOrDefault(),
            rows
                .Select(row => StatisticsMetricBuilderHelpers.ToBerlinTime(row.JourneyEndTime).TimeOfDay)
                .OrderBy(time => time)
                .LastOrDefault(),
            main is null ? null : ToStationReference(stations, main.Key.OriginEvaNumber),
            main is null ? null : ToStationReference(stations, main.Key.DestinationEvaNumber),
            rows.Select(row => row.JourneyDate).Distinct().Count(),
            rows
                .Select(row => new { row.OriginEvaNumber, row.DestinationEvaNumber })
                .Distinct()
                .Count());
    }

    private async Task<JourneyDailyOutcomesResult> BuildDailyOutcomesAsync(
        JourneyStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var rows = await LoadJourneyDetailsAsync(request, cancellationToken);
        var items = rows.Select(row => new JourneyDailyOutcomeItem(
            row.JourneyDate,
            ResolveOutcome(row),
            row.DestinationDelaySeconds,
            ResolveDestinationPunctualityClass(row.DestinationDelaySeconds),
            row.FullyCancelled,
            row.PartiallyCancelled,
            row.DestinationNotReached))
            .ToList();

        return new JourneyDailyOutcomesResult(items);
    }

    private async Task<JourneyStopProfileResult> BuildStopProfileAsync(
        JourneyStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var rows = await LoadStopDetailsAsync(request, cancellationToken);
        var grouped = rows
            .GroupBy(row => new
            {
                row.StationEvaNumber,
                row.ScheduleType,
                PlannedTime = StatisticsMetricBuilderHelpers.ToBerlinTime(row.PlannedTime).TimeOfDay
            })
            .OrderBy(group => group.Key.PlannedTime)
            .ThenBy(group => group.Key.StationEvaNumber)
            .ToList();

        var stations = await LoadStationsAsync(grouped.Select(group => group.Key.StationEvaNumber), cancellationToken);
        var items = grouped.Select(group =>
        {
            var plannedEvents = group.LongCount();
            var cancelledEvents = group.LongCount(row => row.StopCancelled);
            var delays = group.Where(row => !row.StopCancelled).Select(row => row.EventDelaySeconds).Order().ToArray();

            return new JourneyStopProfileItem(
                ToStationReference(stations, group.Key.StationEvaNumber),
                group.Key.PlannedTime,
                group.Key.ScheduleType,
                plannedEvents,
                cancelledEvents,
                StatisticsMetricBuilderHelpers.Rate(delays.LongCount(delay => delay < 360), plannedEvents),
                Percentile(delays, 0.5),
                Percentile(delays, 0.95));
        })
        .ToList();

        return new JourneyStopProfileResult(items);
    }

    private async Task<JourneyDelayBuildUpResult> BuildDelayBuildUpAsync(
        JourneyStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var rows = await LoadStopDetailsAsync(request, cancellationToken);
        var grouped = rows
            .Where(row => !row.StopCancelled)
            .GroupBy(row => row.StationEvaNumber)
            .OrderBy(group => group.Min(row => row.PlannedTime))
            .ToList();

        var stations = await LoadStationsAsync(grouped.Select(group => group.Key), cancellationToken);
        var items = grouped.Select(group =>
        {
            var delays = group.Select(row => row.EventDelaySeconds).Order().ToArray();
            return new JourneyDelayBuildUpItem(
                ToStationReference(stations, group.Key),
                Percentile(delays, 0.5),
                Percentile(delays, 0.95));
        })
        .ToList();

        return new JourneyDelayBuildUpResult(items);
    }

    private async Task<JourneyCalendarResult> BuildCalendarAsync(
        JourneyStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var rows = await LoadJourneyDetailsAsync(request, cancellationToken);
        var items = rows.Select(row => new JourneyCalendarItem(
            row.JourneyDate,
            ResolveDestinationPunctualityClass(row.DestinationDelaySeconds),
            row.DestinationDelaySeconds,
            ResolveOutcome(row)))
            .ToList();

        return new JourneyCalendarResult(items);
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

    private static string ResolveOutcome(JourneyQualityDetail row)
    {
        if (row.FullyCancelled) return "FULLY_CANCELLED";
        if (row.DestinationNotReached) return "DESTINATION_NOT_REACHED";
        if (row.PartiallyCancelled) return "PARTIALLY_CANCELLED";
        return "COMPLETED";
    }

    private static string ResolveDestinationPunctualityClass(int? delaySeconds) =>
        delaySeconds switch
        {
            null => "NO_SAMPLE",
            < 360 => "PUNCTUAL_5",
            < 900 => "LATE_5",
            _ => "LATE_15"
        };

    private static decimal? Percentile(IReadOnlyList<int> orderedValues, double percentile)
    {
        if (orderedValues.Count == 0) return null;
        if (orderedValues.Count == 1) return orderedValues[0];

        var position = (orderedValues.Count - 1) * percentile;
        var lower = (int)Math.Floor(position);
        var upper = (int)Math.Ceiling(position);
        if (lower == upper) return orderedValues[lower];

        var weight = (decimal)(position - lower);
        return orderedValues[lower] + (orderedValues[upper] - orderedValues[lower]) * weight;
    }

    private static StatisticsResponseMeta CreateMeta(JourneyStatisticsMetricRequest request) =>
        new(
            "JOURNEY_NUMBER",
            request.MetricType.ToString(),
            request.From,
            request.To,
            request.Bucket,
            StatisticsMetricBuilderHelpers.Timezone,
            StatisticsMetricBuilderHelpers.CreateFilters(request)
                .Concat(new[]
                {
                    new KeyValuePair<string, object?>("journeyNumber", request.JourneyNumber),
                    new KeyValuePair<string, object?>("lineName", request.LineName)
                })
                .ToDictionary(pair => pair.Key, pair => pair.Value));
}
