using Microsoft.EntityFrameworkCore;
using Navigator.Data.Entities.Statistics;
using Navigator.Data.Enums.Metric;
using Navigator.Data.Models.Statistics;
using Navigator.Data.Models.Statistics.DataPoint;

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

    public async Task<IEnumerable<MetricDataSet>> GetMetricAsync(MetricQueryType type, DateTimeOffset? start, DateTimeOffset? end, bool isCumulative)
    {
        var allTimeMetrics = new[] { MetricQueryType.TransportTypes };
        if (!allTimeMetrics.Contains(type) && (start is null || end is null))
            throw new ArgumentNullException("Start and End dates are required for time-series metrics.");

        var metrics = type switch
        {
            MetricQueryType.DatabaseSize => await GetDatabaseSizeMetricsAsync(start!.Value, end!.Value),
            MetricQueryType.RisIds => await GetRisIdMetricsAsync(start!.Value, end!.Value),
            MetricQueryType.Journeys => await GetJourneyMetricsAsync(start!.Value, end!.Value),
            MetricQueryType.TransportTypes => await GetTransportDistributionsAsync(),
            _ => throw new NotSupportedException($"Metric type '{type}' is not supported.")
        };
        
        return metrics.Select(metric =>
        {
            if (metric.Dimension == MetricDimension.Categorical)
            {
                metric.Summary = CalculateSummary(metric.DataPoints, dimension: metric.Dimension);
                return metric;
            }

            var timeDataPoints = metric.DataPoints.Cast<TimestampMetricDataPoint>().ToList();
            if (isCumulative)
            {
                metric.IsCumulative = true;
                metric.Summary = CalculateSummary(timeDataPoints, dimension: MetricDimension.Time, isCumulative);
                return metric;
            }

            var sortedDataPoints = timeDataPoints
                .OrderBy(dataPoint => dataPoint.Timestamp)
                .ToList();
            var deltaDataPoints = new List<TimestampMetricDataPoint>();

            for (int i = 0; i < sortedDataPoints.Count; i++)
            {
                decimal deltaValue = (i == 0)
                    ? sortedDataPoints[i].Value
                    : sortedDataPoints[i].Value - sortedDataPoints[i - 1].Value;
                deltaDataPoints.Add(new TimestampMetricDataPoint()
                {
                    Timestamp = sortedDataPoints[i].Timestamp,
                    Value = deltaValue
                });
            }

            metric.IsCumulative = true;
            metric.DataPoints = deltaDataPoints;
            metric.Summary = CalculateSummary(metric.DataPoints, dimension: MetricDimension.Time, isCumulative: metric.IsCumulative);
            return metric;
        });
    }

    private async Task<IEnumerable<MetricDataSet>> GetDatabaseSizeMetricsAsync(DateTimeOffset start, DateTimeOffset end)
    {
        var dataPoints = await dataContext.DatabaseSizes
            .AsNoTracking()
            .Where(snapshot => snapshot.MeasuredAt >= start.UtcDateTime && snapshot.MeasuredAt <= end.UtcDateTime)
            .OrderBy(snapshot => snapshot.MeasuredAt)
            .Select(snapshot => new TimestampMetricDataPoint()
            {
                Timestamp = snapshot.MeasuredAt,
                Value = snapshot.SizeInBytes
            })
            .ToListAsync();

        DateTimeOffset now = DateTime.UtcNow;
        if (start <= now && now <= end)
        {
            var currentEstimate = await EstimateCurrentDatabaseSizeAsync() ?? 0;
            dataPoints.Add(new TimestampMetricDataPoint()
            {
                Timestamp = now,
                Value = currentEstimate
            });
        }

        return [new MetricDataSet()
        {
            SeriesType = MetricSeriesType.DatabaseSize,
            Dimension = MetricDimension.Time,
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

        var activePoints = snapshots.Select(snapshot => new TimestampMetricDataPoint()
        {
            Timestamp = snapshot.MeasuredAt,
            Value = snapshot.Active
        }).ToList();
        var inactivePoints = snapshots.Select(snapshot => new TimestampMetricDataPoint()
        {
            Timestamp = snapshot.MeasuredAt,
            Value = snapshot.Inactive
        }).ToList();

        DateTimeOffset now = DateTime.UtcNow;
        if (start <= now && now <= end)
        {
            var currentEstimate = await EstimateCurrentRisIdsAsync();
            if (currentEstimate is null) currentEstimate = (0, 0);

            activePoints.Add(new TimestampMetricDataPoint()
            {
                Timestamp = now,
                Value = currentEstimate.Value.Active
            });
            inactivePoints.Add(new TimestampMetricDataPoint()
            {
                Timestamp = now,
                Value = currentEstimate.Value.Inactive
            });
        }

        return [
            new MetricDataSet() {
                SeriesType = MetricSeriesType.RisIdsActive,
                Dimension = MetricDimension.Time,
                Unit = MetricUnit.Count,
                IsCumulative = true,
                DataPoints = activePoints,
                Summary = null!
            },
            new MetricDataSet() {
                SeriesType = MetricSeriesType.RisIdsInactive,
                Dimension = MetricDimension.Time,
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
            .OrderBy(snapshot => snapshot.MeasuredAt)
            .Select(snapshot => new TimestampMetricDataPoint()
            {
                Timestamp = snapshot.MeasuredAt,
                Value = snapshot.Total
            })
            .ToListAsync();

        DateTimeOffset now = DateTime.UtcNow;
        if (now >= start && now <= end)
        {
            var currentEstimate = await EstimateCurrentJourneysAsync() ?? 0;
            snapshots.Add(new TimestampMetricDataPoint()
            {
                Timestamp = now,
                Value = currentEstimate
            });
        }

        return [new MetricDataSet() {
            SeriesType = MetricSeriesType.JourneyTotal,
            Dimension = MetricDimension.Time,
            Unit = MetricUnit.Count,
            IsCumulative = true,
            DataPoints = snapshots,
            Summary = null!
        }];
    }

    public async Task<IEnumerable<MetricDataSet>> GetTransportDistributionsAsync()
    {
        var dataPoints = (await dataContext.JourneyTransports
            .AsNoTracking()
            .GroupBy(journey => journey.TransportType)
            .Select(group => new { TransportType = group.Key, Value = group.Count() })
            .ToListAsync())
            .Select(dataPoint => new TransportTypeMetricDataPoint()
            {
                TransportType = dataPoint.TransportType,
                Value = dataPoint.Value
            })
            .OrderByDescending(dataPoint => dataPoint.Value)
            .ToList();

        return [new MetricDataSet() {
            SeriesType = MetricSeriesType.TransportTypesTotal,
            Dimension = MetricDimension.Categorical,
            Unit = MetricUnit.Count,
            DataPoints = dataPoints,
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

    private MetricDataSummary CalculateSummary(IEnumerable<MetricDataPoint> points, MetricDimension dimension, bool isCumulative = false)
    {
        if (!points.Any()) return new MetricDataSummary()
        {
            StartValue = 0, EndValue = 0, MinValue = 0, MaxValue = 0, AbsoluteChange = 0
        };

        var values = points
            .Select(point => point.Value)
            .ToList();

        decimal absoluteChange = 0, startValue = 0, endValue = 0;
        if (dimension == MetricDimension.Time)
        {
            var timestampDataPoints = points.Cast<TimestampMetricDataPoint>()
                .OrderBy(dataPoint => dataPoint.Timestamp)
                .ToList();
            startValue = timestampDataPoints.First().Value;
            endValue = timestampDataPoints.Last().Value;

            absoluteChange = isCumulative ? (endValue - startValue) : values.Sum();
        }
        else absoluteChange = values.Sum();

        return new MetricDataSummary()
        {
            StartValue = startValue,
            EndValue = endValue,
            MinValue = values.Min(),
            MaxValue = values.Max(),
            AbsoluteChange = absoluteChange
        };
    }
}
