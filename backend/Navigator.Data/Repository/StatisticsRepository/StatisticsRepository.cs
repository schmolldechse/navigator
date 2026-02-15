using Microsoft.EntityFrameworkCore;
using Navigator.Data.Entities.Statistics;
using Navigator.Data.Enums.Metric;
using Navigator.Data.Models.Statistics;
using Navigator.Data.Models.Statistics.DataPoint;
using Navigator.Data.Models.Statistics.Request;

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

    public async Task<IEnumerable<MetricDataSet>> GetMetricAsync(BaseMetricRequest request)
    {
        var metrics = (request, request.MetricQueryType) switch
        {
            (MetricTimeRequest, MetricQueryType.DatabaseSize) => await GetDatabaseSizeMetricsAsync((MetricTimeRequest)request),
            (MetricTimeRequest, MetricQueryType.RisIds) => await GetRisIdMetricsAsync((MetricTimeRequest)request),
            (MetricTimeRequest, MetricQueryType.Journeys) => await GetJourneyMetricsAsync((MetricTimeRequest)request),
            
            (MetricCategoricalRequest, MetricQueryType.TransportTypes) => await GetTransportDistributionsAsync((MetricCategoricalRequest)request),

            _ => throw new NotSupportedException($"Metric type '{request.MetricQueryType}' is not supported.")
        };

        if (request is MetricTimeRequest {  CumulativeValues: false })
        {
            foreach (var metric in metrics)
            {
                var sortedDataPoints = metric.DataPoints.Cast<TimestampMetricDataPoint>()
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

                metric.DataPoints = deltaDataPoints;
            }
        }

        return metrics;
    }

    private async Task<IEnumerable<MetricDataSet>> GetDatabaseSizeMetricsAsync(MetricTimeRequest request)
    {
        var dataPoints = await dataContext.DatabaseSizes
            .AsNoTracking()
            .Where(snapshot => snapshot.MeasuredAt >= request.Start.UtcDateTime && snapshot.MeasuredAt <= request.End.UtcDateTime)
            .OrderBy(snapshot => snapshot.MeasuredAt)
            .Select(snapshot => new TimestampMetricDataPoint()
            {
                Timestamp = snapshot.MeasuredAt,
                Value = snapshot.SizeInBytes
            })
            .ToListAsync();

        DateTimeOffset now = DateTime.UtcNow;
        if (request.Start <= now && now <= request.End)
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
            IsCumulative = request.CumulativeValues,
            DataPoints = dataPoints,
            Summary = CalculateSummary(dataPoints, MetricDimension.Time, request.CumulativeValues)
        }];
    }

    private async Task<IEnumerable<MetricDataSet>> GetRisIdMetricsAsync(MetricTimeRequest request)
    {
        var snapshots = await dataContext.RisIdSnapshots
            .AsNoTracking()
            .Where(snapshot => snapshot.MeasuredAt >= request.Start.UtcDateTime && snapshot.MeasuredAt <= request.End.UtcDateTime)
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
        if (request.Start <= now && now <= request.End)
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
                IsCumulative = request.CumulativeValues,
                DataPoints = activePoints,
                Summary = CalculateSummary(activePoints, MetricDimension.Time, request.CumulativeValues)
            },
            new MetricDataSet() {
                SeriesType = MetricSeriesType.RisIdsInactive,
                Dimension = MetricDimension.Time,
                Unit = MetricUnit.Count,
                IsCumulative = true,
                DataPoints = inactivePoints,
                Summary = CalculateSummary(inactivePoints, MetricDimension.Time, request.CumulativeValues)
            }
        ];
    }

    private async Task<IEnumerable<MetricDataSet>> GetJourneyMetricsAsync(MetricTimeRequest request)
    {
        var snapshots = await dataContext.JourneySnapshots
            .AsNoTracking()
            .Where(snapshot => snapshot.MeasuredAt >= request.Start.UtcDateTime && snapshot.MeasuredAt <= request.End.UtcDateTime)
            .OrderBy(snapshot => snapshot.MeasuredAt)
            .Select(snapshot => new TimestampMetricDataPoint()
            {
                Timestamp = snapshot.MeasuredAt,
                Value = snapshot.Total
            })
            .ToListAsync();

        DateTimeOffset now = DateTime.UtcNow;
        if (request.Start <= now && now <= request.End)
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
            IsCumulative = request.CumulativeValues,
            DataPoints = snapshots,
            Summary = CalculateSummary(snapshots, MetricDimension.Time, request.CumulativeValues)
        }];
    }

    private async Task<IEnumerable<MetricDataSet>> GetTransportDistributionsAsync(MetricCategoricalRequest request)
    {
        var query = dataContext.JourneyTransports
            .Include(transport => transport.Journey)
            .AsNoTracking()
            .Where(transport => transport.Journey!.Date <= DateOnly.FromDateTime(request.End.DateTime));

        if (request.Start.HasValue) 
            query = query.Where(transport => transport.Journey!.Date >= DateOnly.FromDateTime(request.Start.Value.DateTime));

        var results = await query
            .GroupBy(query => query.TransportType)
            .Select(group => new { TransportType = group.Key, Value = group.Count() })
            .ToListAsync();
        var dataPoints = results
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
            Summary = CalculateSummary(dataPoints, dimension: MetricDimension.Categorical, true)
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

        if (dimension == MetricDimension.Time && points.FirstOrDefault() is TimestampMetricDataPoint)
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
