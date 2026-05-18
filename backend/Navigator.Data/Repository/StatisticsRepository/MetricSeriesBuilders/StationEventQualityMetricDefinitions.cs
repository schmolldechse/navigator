using Navigator.Data.Entities.Views;
using Navigator.Data.Enums;
using Navigator.Data.Enums.Metric;
using Navigator.Data.Models.Statistics;

namespace Navigator.Data.Repository.StatisticsRepository.MetricSeriesBuilders;

internal static class StationEventQualityMetricDefinitions
{
    private static readonly IReadOnlyDictionary<MetricSeriesType, StationEventQualityMetricDefinition> Definitions =
        new Dictionary<MetricSeriesType, StationEventQualityMetricDefinition>
        {
            [MetricSeriesType.StationEventCount] = new(
                MetricUnit.Count,
                aggregate => aggregate.EventCount),
            [MetricSeriesType.StationEventCancellationCount] = new(
                MetricUnit.Count,
                aggregate => aggregate.CancellationCount),
            [MetricSeriesType.StationEventCancellationRate] = new(
                MetricUnit.Percent,
                aggregate => aggregate.EventCount == 0 ? 0 : 100m * aggregate.CancellationCount / aggregate.EventCount,
                aggregate => new() { Numerator = aggregate.CancellationCount, Denominator = aggregate.EventCount }),
            [MetricSeriesType.StationEventDelayAverage] = new(
                MetricUnit.Seconds,
                aggregate => aggregate.DelaySampleCount == 0 ? 0 : (decimal)aggregate.DelaySumSeconds / aggregate.DelaySampleCount,
                aggregate => new() { Numerator = aggregate.DelaySumSeconds, Denominator = aggregate.DelaySampleCount }),
            [MetricSeriesType.StationEventPunctuality5Rate] = new(
                MetricUnit.Percent,
                aggregate => aggregate.DelaySampleCount == 0 ? 0 : 100m * aggregate.Punctual5Count / aggregate.DelaySampleCount,
                aggregate => new() { Numerator = aggregate.Punctual5Count, Denominator = aggregate.DelaySampleCount }),
            [MetricSeriesType.StationEventPunctuality15Rate] = new(
                MetricUnit.Percent,
                aggregate => aggregate.DelaySampleCount == 0 ? 0 : 100m * aggregate.Punctual15Count / aggregate.DelaySampleCount,
                aggregate => new() { Numerator = aggregate.Punctual15Count, Denominator = aggregate.DelaySampleCount })
        };

    public static StationEventQualityMetricDefinition GetDefinition(MetricSeriesType seriesType) =>
        Definitions.TryGetValue(seriesType, out var definition)
            ? definition
            : throw new NotSupportedException($"Unsupported station event quality metric series type: {seriesType}");

    public static TransportType[] NormalizeTransportTypes(TransportType[]? transportTypes) =>
        transportTypes is null || transportTypes.Length == 0
            ? Enum.GetValues<TransportType>()
            : transportTypes;

    public static StationEventQualityMetricAggregate Aggregate(IEnumerable<StationEventQualityHourly> summaries) =>
        new()
        {
            EventCount = summaries.Sum(summary => summary.EventCount),
            CancellationCount = summaries.Sum(summary => summary.CancelledCount),
            DelaySampleCount = summaries.Sum(summary => summary.DelaySampleCount),
            DelaySumSeconds = summaries.Sum(summary => summary.DelaySumSeconds),
            Punctual5Count = summaries.Sum(summary => summary.Punctual5Count),
            Punctual15Count = summaries.Sum(summary => summary.Punctual15Count)
        };
}

internal sealed record StationEventQualityMetricDefinition(
    MetricUnit Unit,
    Func<StationEventQualityMetricAggregate, decimal> GetValue,
    Func<StationEventQualityMetricAggregate, MetricSample?>? CreateSample = null);

internal sealed class StationEventQualityMetricAggregate
{
    public required long EventCount { get; init; }
    public required long CancellationCount { get; init; }
    public required long DelaySampleCount { get; init; }
    public required long DelaySumSeconds { get; init; }
    public required long Punctual5Count { get; init; }
    public required long Punctual15Count { get; init; }
}
