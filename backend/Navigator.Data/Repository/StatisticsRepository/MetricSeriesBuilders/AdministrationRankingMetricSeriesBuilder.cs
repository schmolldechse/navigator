using Microsoft.EntityFrameworkCore;
using Navigator.Data.Enums.Metric;
using Navigator.Data.Models.Statistics;
using Navigator.Data.Models.Statistics.DataPoint;
using Navigator.Data.Models.Statistics.Request;
using Navigator.Data.Models.Statistics.Subject;

namespace Navigator.Data.Repository.StatisticsRepository.MetricSeriesBuilders;

public sealed class AdministrationRankingMetricSeriesBuilder(
    DataContext dataContext
) : MetricSeriesBuilder<AdministrationRankingMetricRequest>
{
    private static readonly IReadOnlyDictionary<MetricSeriesType, MetricDefinition> Definitions =
        new Dictionary<MetricSeriesType, MetricDefinition>
        {
            [MetricSeriesType.AdministrationRankingCount] = new(
                MetricUnit.Count,
                query => query.OrderByDescending(row => row.Count),
                row => row.Count),
            [MetricSeriesType.AdministrationRankingCancellationCount] = new(
                MetricUnit.Count,
                query => query.OrderByDescending(row => row.CancellationCount),
                row => row.CancellationCount),
            [MetricSeriesType.AdministrationRankingCancellationRate] = new(
                MetricUnit.Percent,
                query => query.OrderByDescending(row => row.Count == 0 ? 0 : 100m * row.CancellationCount / row.Count),
                row => row.Count == 0 ? 0 : 100m * row.CancellationCount / row.Count,
                row => new() { Numerator = row.CancellationCount, Denominator = row.Count }),
            [MetricSeriesType.AdministrationRankingAverageDelay] = new(
                MetricUnit.Seconds,
                query => query.OrderByDescending(row => row.DelaySampleCount == 0 ? 0 : (decimal)row.DelaySumSeconds / row.DelaySampleCount),
                row => row.DelaySampleCount == 0 ? 0 : (decimal)row.DelaySumSeconds / row.DelaySampleCount,
                row => new() { Numerator = row.DelaySumSeconds, Denominator = row.DelaySampleCount }),
            [MetricSeriesType.AdministrationRankingPunctuality5Rate] = new(
                MetricUnit.Percent,
                query => query.OrderByDescending(row => row.DelaySampleCount == 0 ? 0 : 100m * row.Punctual5Count / row.DelaySampleCount),
                row => row.DelaySampleCount == 0 ? 0 : 100m * row.Punctual5Count / row.DelaySampleCount,
                row => new() { Numerator = row.Punctual5Count, Denominator = row.DelaySampleCount }),
            [MetricSeriesType.AdministrationRankingPunctuality15Rate] = new(
                MetricUnit.Percent,
                query => query.OrderByDescending(row => row.DelaySampleCount == 0 ? 0 : 100m * row.Punctual15Count / row.DelaySampleCount),
                row => row.DelaySampleCount == 0 ? 0 : 100m * row.Punctual15Count / row.DelaySampleCount,
                row => new() { Numerator = row.Punctual15Count, Denominator = row.DelaySampleCount })
        };

    protected override async Task<MetricSeries> BuildAsync(AdministrationRankingMetricRequest request)
    {
        var definition = GetDefinition(request.SeriesType);
        var limit = Math.Clamp(request.Limit, 1, 500);
        var offset = Math.Max(request.Offset, 0);
        var start = request.Start.UtcDateTime;
        var end = request.End.UtcDateTime;
        var transportTypes = StationEventQualityMetricDefinitions.NormalizeTransportTypes(request.TransportTypes);

        IQueryable<AdministrationRankingRow> ranking = request.EvaNumbers.Any()
            ? dataContext.StationEventQualities
                .AsNoTracking()
                .Where(summary => summary.BucketHour >= start && summary.BucketHour < end)
                .Where(summary => transportTypes.Contains(summary.TransportType))
                .Where(summary => request.IncludeReplacementTransport || !summary.IsReplacementTransport)
                .Where(summary => request.EvaNumbers.Contains(summary.StationEvaNumber))
                .GroupBy(summary => new { summary.AdministrationId })
                .Select(group => new AdministrationRankingRow
                {
                    AdministrationId = group.Key.AdministrationId,
                    Count = group.Sum(summary => summary.EventCount),
                    CancellationCount = group.Sum(summary => summary.CancelledCount),
                    DelaySampleCount = group.Sum(summary => summary.DelaySampleCount),
                    DelaySumSeconds = group.Sum(summary => summary.DelaySumSeconds),
                    Punctual5Count = group.Sum(summary => summary.Punctual5Count),
                    Punctual15Count = group.Sum(summary => summary.Punctual15Count)
                })
            : dataContext.JourneyRouteQualities
                .AsNoTracking()
                .Where(summary => summary.BucketHour >= start && summary.BucketHour < end)
                .Where(summary => transportTypes.Contains(summary.TransportType))
                .Where(summary => request.IncludeReplacementTransport || !summary.IsReplacementTransport)
                .GroupBy(summary => new { summary.AdministrationId })
                .Select(group => new AdministrationRankingRow
                {
                    AdministrationId = group.Key.AdministrationId,
                    Count = group.Sum(summary => summary.JourneyCount),
                    CancellationCount = group.Sum(summary => summary.JourneyCancelledCount),
                    DelaySampleCount = group.Sum(summary => summary.DelaySampleCount),
                    DelaySumSeconds = group.Sum(summary => summary.DelaySumSeconds),
                    Punctual5Count = group.Sum(summary => summary.Punctual5Count),
                    Punctual15Count = group.Sum(summary => summary.Punctual15Count)
                });

        var totalItems = await ranking.CountAsync();
        var rows = await definition.Order(ranking)
            .ThenByDescending(row => row.Count)
            .ThenBy(row => row.AdministrationId)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

        var administrations = await LoadAdministrationsAsync(rows.Select(row => row.AdministrationId));

        var dataPoints = rows
            .Where(row => administrations.ContainsKey(row.AdministrationId))
            .Select(row => new AdministrationRankingDataPoint
            {
                Administration = administrations[row.AdministrationId],
                Value = definition.GetValue(row),
                Sample = definition.CreateSample?.Invoke(row)
            })
            .ToList();

        return new MetricSeries()
        {
            SeriesType = request.SeriesType,
            Unit = definition.Unit,
            DataPoints = dataPoints,
            Page = MetricPage.Create(offset, limit, totalItems)
        };
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

    private static MetricDefinition GetDefinition(MetricSeriesType seriesType) =>
        Definitions.TryGetValue(seriesType, out var definition)
            ? definition
            : throw new NotSupportedException($"Unsupported administration ranking metric series type: {seriesType}");

    private sealed record MetricDefinition(
        MetricUnit Unit,
        Func<IQueryable<AdministrationRankingRow>, IOrderedQueryable<AdministrationRankingRow>> Order,
        Func<AdministrationRankingRow, decimal> GetValue,
        Func<AdministrationRankingRow, MetricSample?>? CreateSample = null);

    private sealed class AdministrationRankingRow
    {
        /// <summary>
        /// Uses `id`-column (Primary Key) from an administration saved in the database to a referenced journey, as `administration_id` is a Deutsche Bahn specific identifier. 
        /// They are not the same.
        /// </summary>
        public Guid AdministrationId { get; set; }
        public long Count { get; set; }
        public long CancellationCount { get; set; }
        public long DelaySampleCount { get; set; }
        public long DelaySumSeconds { get; set; }
        public long Punctual5Count { get; set; }
        public long Punctual15Count { get; set; }
    }
}
