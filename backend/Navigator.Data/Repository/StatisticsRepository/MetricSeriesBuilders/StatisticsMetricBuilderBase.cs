using Microsoft.EntityFrameworkCore;
using Navigator.Data.Entities.Views;
using Navigator.Data.Enums.Metric;
using Navigator.Data.Models.Statistics.Api;

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
    public const string Timezone = "Europe/Berlin";

    private static readonly TimeZoneInfo BerlinTimeZone = ResolveBerlinTimeZone();

    public static DateTimeOffset ToBerlinTime(DateTime utc)
    {
        var utcOffset = new DateTimeOffset(DateTime.SpecifyKind(utc, DateTimeKind.Utc));
        return TimeZoneInfo.ConvertTime(utcOffset, BerlinTimeZone);
    }

    public static DateTimeOffset BucketStart(DateTime utc, StatisticsBucket bucket)
    {
        var local = ToBerlinTime(utc);
        var dateTime = local.DateTime;

        var bucketLocal = bucket switch
        {
            StatisticsBucket.Hour => new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0),
            StatisticsBucket.Day => dateTime.Date,
            StatisticsBucket.Week => dateTime.Date.AddDays(-((7 + (int)dateTime.DayOfWeek - (int)DayOfWeek.Monday) % 7)),
            StatisticsBucket.Month => new DateTime(dateTime.Year, dateTime.Month, 1),
            _ => dateTime.Date
        };

        return new DateTimeOffset(bucketLocal, BerlinTimeZone.GetUtcOffset(bucketLocal));
    }

    public static DateTime ConvertLocalDateToUtc(DateOnly date)
    {
        var local = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Unspecified);
        return TimeZoneInfo.ConvertTimeToUtc(local, BerlinTimeZone);
    }

    public static DateTime FromUtc(StatisticsMetricRequest request) => ConvertLocalDateToUtc(request.FromDate);

    public static DateTime ToUtc(StatisticsMetricRequest request) => ConvertLocalDateToUtc(request.ToDate);

    public static IReadOnlyDictionary<string, object?> CreateFilters(StatisticsMetricRequest request)
    {
        var filters = new Dictionary<string, object?>();

        if (request.GetScheduleType() is { } scheduleType) filters["scheduleType"] = scheduleType;
        if (request.GetTransportTypes().Length > 0) filters["transportTypes"] = request.GetTransportTypes();
        if (request.GetAdministrationIds().Length > 0) filters["administrationIds"] = request.GetAdministrationIds();
        if (request.GetLineNameFilter() is { } lineName) filters["lineName"] = lineName;
        if (request.GetOriginEvaNumber() is { } originEvaNumber) filters["originEvaNumber"] = originEvaNumber;
        if (request.GetDestinationEvaNumber() is { } destinationEvaNumber) filters["destinationEvaNumber"] = destinationEvaNumber;
        if (request.GetDirectionEvaNumber() is { } directionEvaNumber) filters["directionEvaNumber"] = directionEvaNumber;
        if (request.GetJourneyNumberFilter() is { } journeyNumber) filters["journeyNumber"] = journeyNumber;
        if (!request.GetIncludeReplacement()) filters["includeReplacement"] = false;
        if (request.GetMinVolume() > 0) filters["minVolume"] = request.GetMinVolume();
        if (request.GetLimit() != 100) filters["limit"] = request.GetLimit();
        if (request.GetOffset() > 0) filters["offset"] = request.GetOffset();

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
        var completed = aggregate.JourneyCount - aggregate.FullyCancelledCount - aggregate.DestinationNotReachedCount;
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
        var administrationIds = request.GetAdministrationIds();
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

    private static TimeZoneInfo ResolveBerlinTimeZone()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin");
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
        }
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
