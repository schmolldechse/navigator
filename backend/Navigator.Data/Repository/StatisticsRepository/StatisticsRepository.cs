using Microsoft.EntityFrameworkCore;
using Navigator.Data.Entities.Statistics;
using Navigator.Data.Enums.Metric;
using Navigator.Data.Models.Statistics;

namespace Navigator.Data.Repository.StatisticsRepository;

public class StatisticsRepository(
    DataContext dataContext
) : IStatisticsRepository
{
    public async Task<long?> EstimateCurrentDatabaseSizeAsync() => await dataContext.Database
        .SqlQuery<long>($@"
            SELECT COALESCE(SUM(pg_total_relation_size(c.oid))::bigint, 0) AS ""Value""
            FROM pg_class c
                JOIN pg_namespace n ON n.oid = c.relnamespace
            WHERE n.nspname = 'core'
            AND c.relkind = 'r'")
        .SingleOrDefaultAsync();

    public async Task<(int Active, int Inactive)?> EstimateCurrentRisIdsAsync()
    {
        var result = await dataContext.RisIds
            .AsNoTracking()
            .GroupBy(risId => risId.Active)
            .Select(group => new { IsActive = group.Key, Count = group.Count() })
            .ToListAsync();
        if (!result.Any()) return null;
        return (
            Active: result.FirstOrDefault(x => x.IsActive)?.Count ?? 0,
            Inactive: result.FirstOrDefault(x => !x.IsActive)?.Count ?? 0
        );
    }

    public async Task<int?> EstimateCurrentJourneysAsync() => await dataContext.Journeys
        .AsNoTracking()
        .CountAsync();

    public async Task<IEnumerable<MetricDataSet>> GetMetricAsync(MetricQueryType type, DateTimeOffset start, DateTimeOffset end, bool isCumulative)
    {
        var metrics = type switch
        {
            MetricQueryType.DatabaseSize => await GetDatabaseSizeMetricsAsync(start, end),
            MetricQueryType.RisIds => await GetRisIdMetricsAsync(start, end),
            MetricQueryType.Journeys => await GetJourneyMetricsAsync(start, end),
            _ => throw new NotSupportedException($"Metric type '{type}' is not supported.")
        };

        if (isCumulative)
        {
            metrics = metrics.Select(metric =>
            {
                metric.IsCumulative = true;
                metric.Summary = CalculateSummary(metric.DataPoints, isCumulative: true);
                return metric;
            });
            return metrics;
        }

        metrics = metrics.Select(metric =>
        {
            var sortedDataPoints = metric.DataPoints
                .OrderBy(dataPoint => dataPoint.Timestamp)
                .ToList();
            var deltaDataPoints = new List<MetricDataPoint>();

            for (int i = 0; i < sortedDataPoints.Count; i++)
            {
                decimal deltaValue;

                if (i == 0) deltaValue = sortedDataPoints[i].Value;
                else deltaValue = sortedDataPoints[i].Value - sortedDataPoints[i - 1].Value;

                deltaDataPoints.Add(new MetricDataPoint()
                {
                    Timestamp = sortedDataPoints[i].Timestamp,
                    Value = deltaValue
                });
            }

            metric.IsCumulative = false;
            metric.DataPoints = deltaDataPoints;
            metric.Summary = CalculateSummary(deltaDataPoints, isCumulative: false);
            return metric;
        });
        return metrics;
    }

    private async Task<IEnumerable<MetricDataSet>> GetDatabaseSizeMetricsAsync(DateTimeOffset start, DateTimeOffset end)
    {
        var dataPoints = await dataContext.DatabaseSizes
            .AsNoTracking()
            .Where(snapshot => snapshot.MeasuredAt >= start.UtcDateTime && snapshot.MeasuredAt <= end.UtcDateTime)
            .OrderBy(snapshot => snapshot.MeasuredAt)
            .Select(snapshot => new MetricDataPoint()
            {
                Timestamp = snapshot.MeasuredAt,
                Value = snapshot.SizeInBytes
            })
            .ToListAsync();

        DateTimeOffset now = DateTime.UtcNow;
        if (start <= now && now <= end)
        {
            var currentEstimate = await EstimateCurrentDatabaseSizeAsync() ?? 0;
            dataPoints.Add(new MetricDataPoint()
            {
                Timestamp = now,
                Value = currentEstimate
            });
        }

        return [new MetricDataSet()
        {
            SeriesType = MetricSeriesType.DatabaseSize,
            Unit = MetricUnit.Bytes,
            DataPoints = dataPoints,
            Summary = null!
        }];
    }

    public async Task<IEnumerable<MetricDataSet>> GetRisIdMetricsAsync(DateTimeOffset start, DateTimeOffset end)
    {
        var snapshots = await dataContext.RisIdSnapshots
            .AsNoTracking()
            .Where(snapshot => snapshot.MeasuredAt >= start.UtcDateTime && snapshot.MeasuredAt <= end.UtcDateTime)
            .OrderBy(snapshot => snapshot.MeasuredAt)
            .ToListAsync();

        var activePoints = snapshots.Select(snapshot => new MetricDataPoint()
        {
            Timestamp = snapshot.MeasuredAt,
            Value = snapshot.Active
        }).ToList();
        var inactivePoints = snapshots.Select(snapshot => new MetricDataPoint()
        {
            Timestamp = snapshot.MeasuredAt,
            Value = snapshot.Inactive
        }).ToList();

        DateTimeOffset now = DateTime.UtcNow;
        if (start <= now && now <= end)
        {
            var currentEstimate = await EstimateCurrentRisIdsAsync();
            if (currentEstimate is null) currentEstimate = (0, 0);

            activePoints.Add(new MetricDataPoint()
            {
                Timestamp = now,
                Value = currentEstimate.Value.Active
            });
            inactivePoints.Add(new MetricDataPoint()
            {
                Timestamp = now,
                Value = currentEstimate.Value.Inactive
            });
        }

        return [
            new MetricDataSet() {
                SeriesType = MetricSeriesType.RisIdsActive,
                Unit = MetricUnit.Count,
                IsCumulative = true,
                DataPoints = activePoints,
                Summary = null!
            },
            new MetricDataSet() {
                SeriesType = MetricSeriesType.RisIdsInactive,
                Unit = MetricUnit.Count,
                IsCumulative = true,
                DataPoints = inactivePoints,
                Summary = null!
            }
        ];
    }

    public async Task<IEnumerable<MetricDataSet>> GetJourneyMetricsAsync(DateTimeOffset start, DateTimeOffset end)
    {
        var snapshots = await dataContext.JourneySnapshots
            .AsNoTracking()
            .Where(snapshot => snapshot.MeasuredAt >= start.UtcDateTime && snapshot.MeasuredAt <= end.UtcDateTime)
            .Select(snapshot => new MetricDataPoint()
            {
                Timestamp = snapshot.MeasuredAt,
                Value = snapshot.Total
            })
            .OrderBy(point => point.Timestamp)
            .ToListAsync();

        DateTimeOffset now = DateTime.UtcNow;
        if (now >= start && now <= end)
        {
            var currentEstimate = await EstimateCurrentJourneysAsync() ?? 0;
            snapshots.Add(new MetricDataPoint()
            {
                Timestamp = now,
                Value = currentEstimate
            });
        }

        return [new MetricDataSet() {
            SeriesType = MetricSeriesType.JourneyTotal,
            Unit = MetricUnit.Count,
            IsCumulative = true,
            DataPoints = snapshots,
            Summary = null!
        }];
    }

    public async Task SaveDatabaseSizeAsync(DatabaseSize databaseSize)
    {
        await dataContext.DatabaseSizes.AddAsync(databaseSize);
        await dataContext.SaveChangesAsync();
    }

    public async Task SaveRisIdSnapshotAsync(RisIdSnapshot risIdSnapshot)
    {
        await dataContext.RisIdSnapshots.AddAsync(risIdSnapshot);
        await dataContext.SaveChangesAsync();
    }

    public async Task SaveJourneySnapshotAsync(JourneySnapshot journeySnapshot)
    {
        await dataContext.JourneySnapshots.AddAsync(journeySnapshot);
        await dataContext.SaveChangesAsync();
    }

    private MetricDataSummary CalculateSummary(IEnumerable<MetricDataPoint> points, bool isCumulative)
    {
        if (!points.Any()) return new MetricDataSummary()
        {
            StartValue = 0,
            EndValue = 0,
            MinValue = 0,
            MaxValue = 0,
            AbsoluteChange = 0,
        };

        var values = points
            .OrderBy(point => point.Timestamp)
            .Select(point => point.Value)
            .ToList();
        var first = values.First();
        var last = values.Last();

        // Cumulative: The total change is the difference between the last and first values.
        // Non-cumulative (Deltas): The total change is the sum of all individual changes.
        decimal absoluteChange = isCumulative ? (last - first) : values.Sum();

        return new MetricDataSummary
        {
            StartValue = first,
            EndValue = last,
            MinValue = values.Min(),
            MaxValue = values.Max(),
            AbsoluteChange = absoluteChange,
        };
    }
}
