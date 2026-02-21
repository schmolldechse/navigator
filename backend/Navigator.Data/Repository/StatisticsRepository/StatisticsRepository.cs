using Microsoft.EntityFrameworkCore;
using Navigator.Data.Entities.Statistics;
using Navigator.Data.Enums;
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

    public async Task RefreshHourlyStopView() => await dataContext.Database
        .ExecuteSqlRawAsync("REFRESH MATERIALIZED VIEW CONCURRENTLY statistics.hourly_stop_summary;");

    public async Task<IEnumerable<MetricSeries>> GetMetricAsync(BaseMetricRequest request) => request switch
    {
        DatabaseSizeSnapshotMetricRequest => await GetDatabaseSizeMetricsAsync((DatabaseSizeSnapshotMetricRequest) request),
        JourneySnapshotMetricRequest => await GetJourneyMetricsAsync((JourneySnapshotMetricRequest)request),
        RisIdSnapshotMetricRequest => await GetRisIdMetricsAsync((RisIdSnapshotMetricRequest)request),
        
        TransportTypeDistributionMetricRequest => await GetTransportDistributionsAsync((TransportTypeDistributionMetricRequest)request),
        GlobalStopSummaryMetricRequest => await GetGlobalStopSummaryAsync((GlobalStopSummaryMetricRequest)request),
        
        _ => throw new NotSupportedException($"Metric type '{request.MetricQueryType}' is not supported.")
    };

    private async Task<IEnumerable<MetricSeries>> GetDatabaseSizeMetricsAsync(DatabaseSizeSnapshotMetricRequest request)
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

        return [new MetricSeries()
        {
            SeriesType = MetricSeriesType.DatabaseSize,
            Unit = MetricUnit.Bytes,
            DataPoints = dataPoints,
            Summary = CalculateSummary(dataPoints)
        }];
    }

    private async Task<IEnumerable<MetricSeries>> GetRisIdMetricsAsync(RisIdSnapshotMetricRequest request)
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
            new MetricSeries() {
                SeriesType = MetricSeriesType.RisIdsActive,
                Unit = MetricUnit.Count,
                DataPoints = activePoints,
                Summary = CalculateSummary(activePoints)
            },
            new MetricSeries() {
                SeriesType = MetricSeriesType.RisIdsInactive,
                Unit = MetricUnit.Count,
                DataPoints = inactivePoints,
                Summary = CalculateSummary(inactivePoints)
            }
        ];
    }

    private async Task<IEnumerable<MetricSeries>> GetJourneyMetricsAsync(JourneySnapshotMetricRequest request)
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

        return [new MetricSeries() {
            SeriesType = MetricSeriesType.JourneyTotal,
            Unit = MetricUnit.Count,
            DataPoints = snapshots,
            Summary = CalculateSummary(snapshots)
        }];
    }

    private async Task<IEnumerable<MetricSeries>> GetTransportDistributionsAsync(TransportTypeDistributionMetricRequest request)
    {
        request.TransportTypes = (request.TransportTypes is null || request.TransportTypes.Length == 0)
            ? Enum.GetValues<TransportType>()
            : request.TransportTypes;

        var query = dataContext.JourneyTransports
            .Include(transport => transport.Journey)
            .AsNoTracking()
            .Where(transport => transport.Journey!.Date <= DateOnly.FromDateTime(request.End.DateTime))
            .Where(transport => request.TransportTypes.Contains(transport.TransportType));

        /**
        if (request.Start.HasValue) 
            query = query.Where(transport => transport.Journey!.Date >= DateOnly.FromDateTime(request.Start.Value.DateTime));
        */

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

        return [new MetricSeries() {
            SeriesType = MetricSeriesType.TransportTypesTotal,
            Unit = MetricUnit.Count,
            DataPoints = dataPoints,
            Summary = CalculateSummary(dataPoints)
        }];
    }

    private async Task<IEnumerable<MetricSeries>> GetGlobalStopSummaryAsync(GlobalStopSummaryMetricRequest request)
    {
        request.TransportTypes = (request.TransportTypes is null || request.TransportTypes.Length == 0)
            ? Enum.GetValues<TransportType>()
            : request.TransportTypes;

        var query = dataContext.HourlyStopSummaries
            .AsNoTracking()
            .Where(summary => summary.BucketHour >= request.Start.UtcDateTime && summary.BucketHour <= request.End.UtcDateTime)
            .Where(summary => request.TransportTypes.Contains(summary.TransportType));

        var rawResults = await query.ToListAsync();

        var results = rawResults
            .GroupBy(summary => new 
            { 
                BucketHour = summary.BucketHour.AddHours(-(summary.BucketHour.Hour % request.Stepping)), 
                summary.TransportType 
            })
            .Select(group => new
            {
                group.Key.BucketHour,
                group.Key.TransportType,
                ArrivalsCount = group.Sum(x => x.ArrivalsCount),
                ArrivalCancellationCount = group.Sum(x => x.ArrivalCancellationCount),
                ArrivalDelayAvg = group.Sum(x => x.ArrivalsCount) > 0 ? group.Sum(x => x.ArrivalDelaySum) / group.Sum(x => x.ArrivalsCount) : 0,

                DeparturesCount = group.Sum(x => x.DeparturesCount),
                DepartureCancellationCount = group.Sum(x => x.DepartureCancellationCount),
                DepartureDelayAvg = group.Sum(x => x.DeparturesCount) > 0 ? group.Sum(x => x.DepartureDelaySum) / group.Sum(x => x.DeparturesCount) : 0,
            })
            .OrderBy(r => r.BucketHour)
            .ToList();

        var arrivalsCountPoints = results
            .Select(result => new TimestampTransportTypeMetricDataPoint { Timestamp = result.BucketHour, TransportType = result.TransportType, Value = result.ArrivalsCount })
            .ToList();
        var arrivalCancellationsPoints = results
            .Select(result => new TimestampTransportTypeMetricDataPoint { Timestamp = result.BucketHour, TransportType = result.TransportType, Value = result.ArrivalCancellationCount })
            .ToList();
        var arrivalDelayPoints = results
            .Select(result => new TimestampTransportTypeMetricDataPoint { Timestamp = result.BucketHour, TransportType = result.TransportType, Value = (decimal)result.ArrivalDelayAvg })
            .ToList();

        var departuresCountPoints = results
            .Select(result => new TimestampTransportTypeMetricDataPoint { Timestamp = result.BucketHour, TransportType = result.TransportType, Value = result.DeparturesCount })
            .ToList();
        var departureCancellationsPoints = results
            .Select(result => new TimestampTransportTypeMetricDataPoint { Timestamp = result.BucketHour, TransportType = result.TransportType, Value = result.DepartureCancellationCount })
            .ToList();
        var departureDelayPoints = results
            .Select(result => new TimestampTransportTypeMetricDataPoint { Timestamp = result.BucketHour, TransportType = result.TransportType, Value = (decimal)result.DepartureDelayAvg })
            .ToList();

        return [
            new MetricSeries {
                SeriesType = MetricSeriesType.GlobalStopArrivalsCount,
                Unit = MetricUnit.Count,
                DataPoints = arrivalsCountPoints,
                Summary = CalculateSummary(arrivalsCountPoints)
            },
            new MetricSeries {
                SeriesType = MetricSeriesType.GlobalStopArrivalCancellations,
                Unit = MetricUnit.Count,
                DataPoints = arrivalCancellationsPoints,
                Summary = CalculateSummary(arrivalCancellationsPoints)
            },
            new MetricSeries {
                SeriesType = MetricSeriesType.GlobalStopArrivalDelays,
                Unit = MetricUnit.Seconds,
                DataPoints = arrivalDelayPoints,
                Summary = CalculateSummary(arrivalDelayPoints)
            },
            new MetricSeries {
                SeriesType = MetricSeriesType.GlobalStopDeparturesCount,
                Unit = MetricUnit.Count,
                DataPoints = departuresCountPoints,
                Summary = CalculateSummary(departuresCountPoints)
            },
            new MetricSeries {
                SeriesType = MetricSeriesType.GlobalStopDepartureCancellations,
                Unit = MetricUnit.Count,
                DataPoints = departureCancellationsPoints,
                Summary = CalculateSummary(departureCancellationsPoints)
            },
            new MetricSeries {
                SeriesType = MetricSeriesType.GlobalStopDepartureDelays,
                Unit = MetricUnit.Seconds,
                DataPoints = departureDelayPoints,
                Summary = CalculateSummary(departureDelayPoints)
            }
        ];
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

    private MetricSummary CalculateSummary(IEnumerable<BaseMetricDataPoint> points)
    {
        if (!points.Any()) return new MetricSummary()
        {
            StartValue = 0, EndValue = 0, MinValue = 0, MaxValue = 0, AbsoluteChange = 0
        };

        var values = points.Select(point => point.Value).ToList();
        decimal startValue = 0, endValue = 0, absoluteChange = 0;

        if (points.First() is TimestampMetricDataPoint or TimestampTransportTypeMetricDataPoint)
        {
            var groupedByTime = points.GroupBy(point => point switch
            {
                TimestampMetricDataPoint timestampPoint => timestampPoint.Timestamp,
                TimestampTransportTypeMetricDataPoint timestampTransportTypePoint => timestampTransportTypePoint.Timestamp,
                _ => throw new InvalidOperationException()
            })
            .OrderBy(group => group.Key)
            .ToList();

            startValue = groupedByTime.First().Sum(point => point.Value);
            endValue = groupedByTime.Last().Sum(point => point.Value);
            absoluteChange = endValue - startValue;
        }
        else absoluteChange = values.Sum();

        return new MetricSummary()
        {
            StartValue = startValue,
            EndValue = endValue,
            MinValue = values.Min(),
            MaxValue = values.Max(),
            AbsoluteChange = absoluteChange
        };
    }
}
