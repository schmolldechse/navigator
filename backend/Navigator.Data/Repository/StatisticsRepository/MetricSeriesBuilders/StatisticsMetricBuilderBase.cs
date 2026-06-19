using Microsoft.EntityFrameworkCore;
using Navigator.Data.Entities.Views;
using Navigator.Data.Enums.Metric;
using Navigator.Data.Models.Statistics.Api;
using StatisticsBucket = Navigator.Data.Enums.Metric.StatisticsBucket;

namespace Navigator.Data.Repository.StatisticsRepository.MetricSeriesBuilders;

public interface IStatisticsMetricBuilder
{
    Type RequestType { get; }
    bool CanBuild(StatisticsMetricRequest request);
    Task<StatisticsMetricResponse> BuildAsync(StatisticsMetricRequest request, CancellationToken cancellationToken);
}

public abstract class StatisticsMetricBuilder<TRequest> : IStatisticsMetricBuilder
    where TRequest : StatisticsMetricRequest
{
    public Type RequestType => typeof(TRequest);

    public bool CanBuild(StatisticsMetricRequest request) => request is TRequest typedRequest && CanBuild(typedRequest);

    public Task<StatisticsMetricResponse> BuildAsync(
        StatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        if (request is not TRequest typedRequest)
            throw new ArgumentException(
                $"Expected request type '{typeof(TRequest).Name}' but received '{request.GetType().Name}'.",
                nameof(request));

        return BuildAsync(typedRequest, cancellationToken);
    }

    protected virtual bool CanBuild(TRequest request) => true;

    protected abstract Task<StatisticsMetricResponse> BuildAsync(
        TRequest request,
        CancellationToken cancellationToken
    );
}

internal static class StatisticsMetricBuilderHelpers
{
    public static DateTime UtcFrom(StatisticsMetricRequest request) => request.From.UtcDateTime;

    public static DateTime UtcTo(StatisticsMetricRequest request) => request.To.UtcDateTime;

    public static DateTimeOffset ToRequestOffsetTime(DateTime utc, StatisticsMetricRequest request)
    {
        var utcOffset = new DateTimeOffset(DateTime.SpecifyKind(utc, DateTimeKind.Utc));
        return utcOffset.ToOffset(request.From.Offset);
    }

    public static DateTimeOffset BucketStart(DateTime utc, StatisticsMetricRequest request, StatisticsBucket bucket)
    {
        var local = ToRequestOffsetTime(utc, request);
        var dateTime = local.DateTime;

        var bucketLocal = bucket switch
        {
            StatisticsBucket.Hour => new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0),
            StatisticsBucket.Day => dateTime.Date,
            StatisticsBucket.Week => dateTime.Date.AddDays(-((7 + (int)dateTime.DayOfWeek - (int)DayOfWeek.Monday) % 7)),
            StatisticsBucket.Month => new DateTime(dateTime.Year, dateTime.Month, 1),
            _ => dateTime.Date
        };

        return new DateTimeOffset(bucketLocal, request.From.Offset);
    }

    public static StatisticsBucket? Bucket(StatisticsMetricRequest request) =>
        request is IHasBucket bucketed ? bucketed.Bucket : null;

    public static Navigator.Data.Enums.ScheduleType? ScheduleType(StatisticsMetricRequest request) =>
        request is IHasScheduleType filter ? filter.ScheduleType : null;

    public static Navigator.Data.Enums.TransportType[] TransportTypes(StatisticsMetricRequest request) =>
        request is IHasTransportTypes filter ? filter.TransportTypes : [];

    public static string[] AdministrationIds(StatisticsMetricRequest request) =>
        request is IHasAdministrationIds filter ? filter.AdministrationIds : [];

    public static int? OriginEvaNumber(StatisticsMetricRequest request) =>
        request is IHasOriginEvaNumber filter ? filter.OriginEvaNumber : null;

    public static int? DestinationEvaNumber(StatisticsMetricRequest request) =>
        request is IHasDestinationEvaNumber filter ? filter.DestinationEvaNumber : null;

    public static int? DirectionEvaNumber(StatisticsMetricRequest request) =>
        request is IHasDirectionEvaNumber filter ? filter.DirectionEvaNumber : null;

    public static int? JourneyNumber(StatisticsMetricRequest request) =>
        request switch
        {
            IHasJourneyNumber required => required.JourneyNumber,
            IHasOptionalJourneyNumber optional => optional.JourneyNumber,
            _ => null
        };

    public static string? LineName(StatisticsMetricRequest request) =>
        request is IHasLineName filter ? filter.LineName : null;

    public static bool IncludeReplacement(StatisticsMetricRequest request) =>
        request is not IHasReplacementFilter filter || filter.IncludeReplacement;

    public static int MinVolume(StatisticsMetricRequest request) =>
        request is IHasMinimumVolume filter ? filter.MinVolume : 0;

    public static int Limit(StatisticsMetricRequest request) =>
        request switch
        {
            IHasPagination paged => paged.Limit,
            IHasLimit limited => limited.Limit,
            _ => int.MaxValue
        };

    public static int Offset(StatisticsMetricRequest request) =>
        request is IHasPagination paged ? paged.Offset : 0;

    public static StatisticsScope Scope(StatisticsMetricRequest request) =>
        request switch
        {
            NetworkStatisticsMetricRequest => StatisticsScope.Network,
            StationStatisticsMetricRequest => StatisticsScope.Station,
            LineStatisticsMetricRequest => StatisticsScope.Line,
            JourneyStatisticsMetricRequest => StatisticsScope.JourneyNumber,
            _ => throw new NotSupportedException($"Unsupported statistics request scope: {request.GetType().Name}")
        };

    public static StatisticsMetricType Metric(StatisticsMetricRequest request) =>
        request switch
        {
            NetworkStatisticsMetricRequest network => Metric(network.MetricType),
            StationStatisticsMetricRequest station => Metric(station.MetricType),
            LineStatisticsMetricRequest line => Metric(line.MetricType),
            JourneyStatisticsMetricRequest journey => Metric(journey.MetricType),
            _ => throw new NotSupportedException($"Unsupported statistics metric request: {request.GetType().Name}")
        };

    private static StatisticsMetricType Metric(NetworkStatisticsMetricType metricType) =>
        metricType switch
        {
            NetworkStatisticsMetricType.EventSummary => StatisticsMetricType.EventSummary,
            NetworkStatisticsMetricType.JourneySummary => StatisticsMetricType.JourneySummary,
            NetworkStatisticsMetricType.EventTimeSeries => StatisticsMetricType.EventTimeSeries,
            NetworkStatisticsMetricType.JourneyTimeSeries => StatisticsMetricType.JourneyTimeSeries,
            NetworkStatisticsMetricType.WeekdayHourHeatmap => StatisticsMetricType.WeekdayHourHeatmap,
            NetworkStatisticsMetricType.TransportTypeComparison => StatisticsMetricType.TransportTypeComparison,
            NetworkStatisticsMetricType.StationRanking => StatisticsMetricType.StationRanking,
            NetworkStatisticsMetricType.LineRanking => StatisticsMetricType.LineRanking,
            NetworkStatisticsMetricType.MapHotspots => StatisticsMetricType.MapHotspots,
            NetworkStatisticsMetricType.EventDelayDistribution => StatisticsMetricType.EventDelayDistribution,
            _ => throw new NotSupportedException($"Unsupported network statistics metric type: {metricType}")
        };

    private static StatisticsMetricType Metric(StationStatisticsMetricType metricType) =>
        metricType switch
        {
            StationStatisticsMetricType.EventSummary => StatisticsMetricType.EventSummary,
            StationStatisticsMetricType.Benchmark => StatisticsMetricType.Benchmark,
            StationStatisticsMetricType.TimeSeries => StatisticsMetricType.TimeSeries,
            StationStatisticsMetricType.ArrivalDepartureComparison => StatisticsMetricType.ArrivalDepartureComparison,
            StationStatisticsMetricType.WeekdayHourHeatmap => StatisticsMetricType.WeekdayHourHeatmap,
            StationStatisticsMetricType.LineRanking => StatisticsMetricType.LineRanking,
            StationStatisticsMetricType.Directions => StatisticsMetricType.Directions,
            StationStatisticsMetricType.TransportTypeMix => StatisticsMetricType.TransportTypeMix,
            StationStatisticsMetricType.LineHourMatrix => StatisticsMetricType.LineHourMatrix,
            StationStatisticsMetricType.EventDetails => StatisticsMetricType.EventDetails,
            _ => throw new NotSupportedException($"Unsupported station statistics metric type: {metricType}")
        };

    private static StatisticsMetricType Metric(LineStatisticsMetricType metricType) =>
        metricType switch
        {
            LineStatisticsMetricType.Profile => StatisticsMetricType.LineProfile,
            LineStatisticsMetricType.JourneySummary => StatisticsMetricType.JourneySummary,
            LineStatisticsMetricType.EventSummary => StatisticsMetricType.EventSummary,
            LineStatisticsMetricType.TimeSeries => StatisticsMetricType.TimeSeries,
            LineStatisticsMetricType.RouteVariants => StatisticsMetricType.RouteVariants,
            LineStatisticsMetricType.StationPerformance => StatisticsMetricType.StationPerformance,
            LineStatisticsMetricType.JourneyNumberRanking => StatisticsMetricType.JourneyNumberRanking,
            LineStatisticsMetricType.WeekdayHourHeatmap => StatisticsMetricType.WeekdayHourHeatmap,
            LineStatisticsMetricType.ProblemStations => StatisticsMetricType.ProblemStations,
            _ => throw new NotSupportedException($"Unsupported line statistics metric type: {metricType}")
        };

    private static StatisticsMetricType Metric(JourneyStatisticsMetricType metricType) =>
        metricType switch
        {
            JourneyStatisticsMetricType.Pattern => StatisticsMetricType.Pattern,
            JourneyStatisticsMetricType.JourneySummary => StatisticsMetricType.JourneySummary,
            JourneyStatisticsMetricType.DailyOutcomes => StatisticsMetricType.DailyOutcomes,
            JourneyStatisticsMetricType.StopProfile => StatisticsMetricType.StopProfile,
            JourneyStatisticsMetricType.DelayBuildUp => StatisticsMetricType.DelayBuildUp,
            JourneyStatisticsMetricType.Calendar => StatisticsMetricType.Calendar,
            _ => throw new NotSupportedException($"Unsupported journey statistics metric type: {metricType}")
        };

    public static IReadOnlyDictionary<string, object?> CreateFilters(StatisticsMetricRequest request)
    {
        var filters = new Dictionary<string, object?>();

        var transportTypes = TransportTypes(request);
        var administrationIds = AdministrationIds(request);

        if (ScheduleType(request) is { } scheduleType) filters["scheduleType"] = scheduleType;
        if (transportTypes.Length > 0) filters["transportTypes"] = transportTypes;
        if (administrationIds.Length > 0) filters["administrationIds"] = administrationIds;
        if (LineName(request) is { } lineName) filters["lineName"] = lineName;
        if (OriginEvaNumber(request) is { } originEvaNumber) filters["originEvaNumber"] = originEvaNumber;
        if (DestinationEvaNumber(request) is { } destinationEvaNumber) filters["destinationEvaNumber"] = destinationEvaNumber;
        if (DirectionEvaNumber(request) is { } directionEvaNumber) filters["directionEvaNumber"] = directionEvaNumber;
        if (JourneyNumber(request) is { } journeyNumber) filters["journeyNumber"] = journeyNumber;
        if (!IncludeReplacement(request)) filters["includeReplacement"] = false;
        if (MinVolume(request) > 0) filters["minVolume"] = MinVolume(request);
        if (Limit(request) != 100) filters["limit"] = Limit(request);
        if (Offset(request) > 0) filters["offset"] = Offset(request);

        return filters;
    }

    public static EventAggregate AggregateEventRows(IEnumerable<IEventQualityHourly> rows)
    {
        var aggregate = new EventAggregate();

        foreach (var row in rows)
        {
            aggregate.EventCount += row.EventCount;
            aggregate.StopCancelledCount += row.StopCancelledCount;
            aggregate.EventDelaySumSeconds += row.EventDelaySumSeconds;
            aggregate.EventPositiveDelaySumSeconds += row.EventPositiveDelaySumSeconds;
            aggregate.EventPunctual5Count += row.EventPunctual5Count;
            aggregate.EventPunctual15Count += row.EventPunctual15Count;
            aggregate.EventLate30Count += row.EventLate30Count;
            aggregate.EventLate60Count += row.EventLate60Count;
        }

        return aggregate;
    }

    public static JourneyAggregate AggregateJourneyRows(IEnumerable<IJourneyQualityHourly> rows)
    {
        var aggregate = new JourneyAggregate();

        foreach (var row in rows)
        {
            aggregate.JourneyCount += row.JourneyCount;
            aggregate.FullyCancelledCount += row.FullyCancelledCount;
            aggregate.PartiallyCancelledCount += row.PartiallyCancelledCount;
            aggregate.DestinationNotReachedCount += row.DestinationNotReachedCount;
            aggregate.DestinationDelaySumSeconds += row.DestinationDelaySumSeconds;
            aggregate.DestinationPositiveDelaySumSeconds += row.DestinationPositiveDelaySumSeconds;
            aggregate.DestinationPunctual5Count += row.DestinationPunctual5Count;
            aggregate.DestinationPunctual15Count += row.DestinationPunctual15Count;
            aggregate.DestinationLate30Count += row.DestinationLate30Count;
            aggregate.DestinationLate60Count += row.DestinationLate60Count;
        }

        return aggregate;
    }

    public static EventMetrics ToEventMetrics(EventAggregate aggregate)
    {
        var served = aggregate.EventCount - aggregate.StopCancelledCount;
        return new EventMetrics(
            PlannedEvents: aggregate.EventCount,
            CancelledEvents: aggregate.StopCancelledCount,
            ServedEvents: served,
            CancellationRate: Rate(aggregate.StopCancelledCount, aggregate.EventCount),
            OperativePunctuality5Rate: Rate(aggregate.EventPunctual5Count, served),
            CustomerReliability5Rate: Rate(aggregate.EventPunctual5Count, aggregate.EventCount),
            OperativePunctuality15Rate: Rate(aggregate.EventPunctual15Count, served),
            CustomerReliability15Rate: Rate(aggregate.EventPunctual15Count, aggregate.EventCount),
            AverageDelaySeconds: Average(aggregate.EventDelaySumSeconds, served),
            DelayDebtMinutes: aggregate.EventPositiveDelaySumSeconds / 60m,
            Late30Rate: Rate(aggregate.EventLate30Count, aggregate.EventCount),
            Late60Rate: Rate(aggregate.EventLate60Count, aggregate.EventCount));
    }

    public static JourneyMetrics ToJourneyMetrics(JourneyAggregate aggregate)
    {
        // Fully cancelled journeys are already included in DestinationNotReachedCount.
        var completed = aggregate.JourneyCount - aggregate.DestinationNotReachedCount;
        var delaySamples = aggregate.JourneyCount - aggregate.DestinationNotReachedCount;

        return new JourneyMetrics(
            PlannedJourneys: aggregate.JourneyCount,
            CompletedJourneys: completed,
            FullyCancelledJourneys: aggregate.FullyCancelledCount,
            PartiallyCancelledJourneys: aggregate.PartiallyCancelledCount,
            DestinationNotReachedJourneys: aggregate.DestinationNotReachedCount,
            FullCancellationRate: Rate(aggregate.FullyCancelledCount, aggregate.JourneyCount),
            PartialCancellationRate: Rate(aggregate.PartiallyCancelledCount, aggregate.JourneyCount),
            DestinationNotReachedRate: Rate(aggregate.DestinationNotReachedCount, aggregate.JourneyCount),
            JourneyCompletionRate: Rate(completed, aggregate.JourneyCount),
            DestinationPunctuality5Rate: Rate(aggregate.DestinationPunctual5Count, aggregate.JourneyCount),
            DestinationPunctuality15Rate: Rate(aggregate.DestinationPunctual15Count, aggregate.JourneyCount),
            AverageDestinationDelaySeconds: Average(aggregate.DestinationDelaySumSeconds, delaySamples),
            DestinationDelayDebtMinutes: aggregate.DestinationPositiveDelaySumSeconds / 60m);
    }

    public static async Task<Guid[]> ResolveAdministrationIdsAsync(
        DataContext dataContext,
        StatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var administrationIds = AdministrationIds(request);
        if (administrationIds.Length == 0) return [];

        return await dataContext.Administrations
            .AsNoTracking()
            .Where(administration => administrationIds.Contains(administration.AdministrationId))
            .Select(administration => administration.Id)
            .ToArrayAsync(cancellationToken);
    }

    public static decimal? Rate(long numerator, long denominator) =>
        denominator == 0 ? null : (decimal)numerator / denominator;

    public static decimal? Average(long numerator, long denominator) =>
        denominator == 0 ? null : (decimal)numerator / denominator;

    public static decimal? Percentile(IReadOnlyList<int> orderedValues, double percentile)
    {
        if (orderedValues.Count == 0) return null;
        if (orderedValues.Count == 1) return orderedValues[0];

        var position = (orderedValues.Count - 1) * percentile;
        var lowerIndex = (int)Math.Floor(position);
        var upperIndex = (int)Math.Ceiling(position);

        if (lowerIndex == upperIndex) return orderedValues[lowerIndex];

        var weight = (decimal)(position - lowerIndex);
        return orderedValues[lowerIndex] + (orderedValues[upperIndex] - orderedValues[lowerIndex]) * weight;
    }

}

internal sealed class EventAggregate
{
    public long EventCount { get; set; }
    public long StopCancelledCount { get; set; }
    public long EventDelaySumSeconds { get; set; }
    public long EventPositiveDelaySumSeconds { get; set; }
    public long EventPunctual5Count { get; set; }
    public long EventPunctual15Count { get; set; }
    public long EventLate30Count { get; set; }
    public long EventLate60Count { get; set; }
}

internal sealed class JourneyAggregate
{
    public long JourneyCount { get; set; }
    public long FullyCancelledCount { get; set; }
    public long PartiallyCancelledCount { get; set; }
    public long DestinationNotReachedCount { get; set; }
    public long DestinationDelaySumSeconds { get; set; }
    public long DestinationPositiveDelaySumSeconds { get; set; }
    public long DestinationPunctual5Count { get; set; }
    public long DestinationPunctual15Count { get; set; }
    public long DestinationLate30Count { get; set; }
    public long DestinationLate60Count { get; set; }
}
