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

    public async Task<MetricSeries> GetMetricAsync(BaseMetricRequest baseRequest) => baseRequest switch
    {
        DatabaseSizeSnapshotMetricRequest request => await GetDatabaseSizeMetricsAsync(request),
        JourneySnapshotMetricRequest request => await GetJourneyMetricsAsync(request),
        RisIdSnapshotMetricRequest request => await GetRisIdMetricsAsync(request),

        TransportTypeDistributionMetricRequest request => await GetTransportDistributionsAsync(request),

        HourlyTransportSnapshotMetricRequest request => await GetHourlyTransportSnapshotAsync(request),
        StationSummaryMetricRequest request => await GetStationSummaryAsync(request),

        _ => throw new NotSupportedException($"Metric type '{baseRequest.MetricSeriesType}' is not supported.")
    };

    private async Task<MetricSeries> GetDatabaseSizeMetricsAsync(DatabaseSizeSnapshotMetricRequest request)
    {
        var dataPoints = await dataContext.DatabaseSizes
            .AsNoTracking()
            .Where(snapshot => snapshot.MeasuredAt >= request.Start.UtcDateTime && snapshot.MeasuredAt <= request.End.UtcDateTime)
            .OrderBy(snapshot => snapshot.MeasuredAt)
            .Select(snapshot => new TimestampDataPoint()
            {
                Timestamp = snapshot.MeasuredAt,
                Value = snapshot.SizeInBytes
            })
            .ToListAsync();

        DateTimeOffset now = DateTime.UtcNow;
        if (request.Start <= now && now <= request.End)
        {
            var currentEstimate = await EstimateCurrentDatabaseSizeAsync() ?? 0;
            dataPoints.Add(new TimestampDataPoint()
            {
                Timestamp = now,
                Value = currentEstimate
            });
        }

        return new MetricSeries()
        {
            SeriesType = MetricSeriesType.DatabaseSize,
            Unit = MetricUnit.Bytes,
            DataPoints = dataPoints,
        };
    }

    private async Task<MetricSeries> GetRisIdMetricsAsync(RisIdSnapshotMetricRequest request)
    {
        var snapshots = await dataContext.RisIdSnapshots
            .AsNoTracking()
            .Where(snapshot => snapshot.MeasuredAt >= request.Start.UtcDateTime && snapshot.MeasuredAt <= request.End.UtcDateTime)
            .OrderBy(snapshot => snapshot.MeasuredAt)
            .ToListAsync();

        var dataPoints = request.MetricSeriesType switch
        {
            MetricSeriesType.RisIdsActive => snapshots.Select(snapshot => new TimestampDataPoint()
            {
                Timestamp = snapshot.MeasuredAt,
                Value = snapshot.Active
            }).ToList(),
            MetricSeriesType.RisIdsInactive => snapshots.Select(snapshot => new TimestampDataPoint()
            {
                Timestamp = snapshot.MeasuredAt,
                Value = snapshot.Inactive
            }).ToList(),
            _ => throw new NotSupportedException($"Unsupported RisId metric series type: {request.MetricSeriesType}")
        };

        DateTimeOffset now = DateTime.UtcNow;
        if (request.Start <= now && now <= request.End)
        {
            var currentEstimate = await EstimateCurrentRisIdsAsync();
            if (currentEstimate is null) currentEstimate = (0, 0);

            var currentValue = request.MetricSeriesType switch
            {
                MetricSeriesType.RisIdsActive => currentEstimate.Value.Active,
                MetricSeriesType.RisIdsInactive => currentEstimate.Value.Inactive,
                _ => 0
            };

            dataPoints.Add(new TimestampDataPoint()
            {
                Timestamp = now,
                Value = currentValue
            });
        }

        return new MetricSeries()
        {
            SeriesType = request.MetricSeriesType,
            Unit = MetricUnit.Count,
            DataPoints = dataPoints,
        };
    }

    private async Task<MetricSeries> GetJourneyMetricsAsync(JourneySnapshotMetricRequest request)
    {
        var snapshots = await dataContext.JourneySnapshots
            .AsNoTracking()
            .Where(snapshot => snapshot.MeasuredAt >= request.Start.UtcDateTime && snapshot.MeasuredAt <= request.End.UtcDateTime)
            .OrderBy(snapshot => snapshot.MeasuredAt)
            .Select(snapshot => new TimestampDataPoint()
            {
                Timestamp = snapshot.MeasuredAt,
                Value = snapshot.Total
            })
            .ToListAsync();

        DateTimeOffset now = DateTime.UtcNow;
        if (request.Start <= now && now <= request.End)
        {
            var currentEstimate = await EstimateCurrentJourneysAsync() ?? 0;
            snapshots.Add(new TimestampDataPoint()
            {
                Timestamp = now,
                Value = currentEstimate
            });
        }

        return new MetricSeries()
        {
            SeriesType = MetricSeriesType.JourneyTotal,
            Unit = MetricUnit.Count,
            DataPoints = snapshots,
        };
    }

    private async Task<MetricSeries> GetTransportDistributionsAsync(TransportTypeDistributionMetricRequest request)
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
            .Select(dataPoint => new TransportTypeDataPoint()
            {
                TransportType = dataPoint.TransportType,
                Value = dataPoint.Value
            })
            .OrderByDescending(dataPoint => dataPoint.Value)
            .ToList();

        return new MetricSeries()
        {
            SeriesType = MetricSeriesType.TransportTypesTotal,
            Unit = MetricUnit.Count,
            DataPoints = dataPoints,
        };
    }

    private async Task<MetricSeries> GetHourlyTransportSnapshotAsync(HourlyTransportSnapshotMetricRequest request)
    {
        request.TransportTypes = (request.TransportTypes is null || request.TransportTypes.Length == 0)
            ? Enum.GetValues<TransportType>()
            : request.TransportTypes;

        var query = dataContext.HourlyStationSnapshots
            .AsNoTracking()
            .Where(summary => summary.BucketHour >= request.Start.UtcDateTime && summary.BucketHour <= request.End.UtcDateTime)
            .Where(summary => request.TransportTypes.Contains(summary.TransportType));

        var results = (await query.ToListAsync())
            .GroupBy(summary => new
            {
                BucketHour = summary.BucketHour.AddHours(-(summary.BucketHour.Hour % request.Stepping)),
                summary.TransportType
            })
            .Select(group => new
            {
                group.Key.BucketHour,
                group.Key.TransportType,
                ArrivalsCount = group.Sum(summary => summary.ArrivalCount),
                ArrivalCancellationCount = group.Sum(summary => summary.ArrivalCancellationCount),
                ArrivalDelaySum = group.Sum(summary => summary.ArrivalDelaySum),

                DeparturesCount = group.Sum(summary => summary.DepartureCount),
                DepartureCancellationCount = group.Sum(summary => summary.DepartureCancellationCount),
                DepartureDelaySum = group.Sum(summary => summary.DepartureDelaySum),
            })
            .OrderBy(element => element.BucketHour)
            .ToList();

        var (dataPoints, unit) = request.MetricSeriesType switch
        {
            MetricSeriesType.HourlyGlobalArrivals => (
                results.Select(result => new TimestampTransportTypeDataPoint { Timestamp = result.BucketHour, TransportType = result.TransportType, Value = result.ArrivalsCount }).ToList(),
                MetricUnit.Count
            ),
            MetricSeriesType.HourlyGlobalArrivalCancellations => (
                results.Select(result => new TimestampTransportTypeDataPoint { Timestamp = result.BucketHour, TransportType = result.TransportType, Value = result.ArrivalCancellationCount }).ToList(),
                MetricUnit.Count
            ),
            MetricSeriesType.HourlyGlobalArrivalDelaySum => (
                results.Select(result => new TimestampTransportTypeDataPoint { Timestamp = result.BucketHour, TransportType = result.TransportType, Value = (decimal)result.ArrivalDelaySum }).ToList(),
                MetricUnit.Seconds
            ),
            MetricSeriesType.HourlyGlobalDepartures => (
                results.Select(result => new TimestampTransportTypeDataPoint { Timestamp = result.BucketHour, TransportType = result.TransportType, Value = result.DeparturesCount }).ToList(),
                MetricUnit.Count
            ),
            MetricSeriesType.HourlyGlobalDepartureCancellations => (
                results.Select(result => new TimestampTransportTypeDataPoint { Timestamp = result.BucketHour, TransportType = result.TransportType, Value = result.DepartureCancellationCount }).ToList(),
                MetricUnit.Count
            ),
            MetricSeriesType.HourlyGlobalDepartureDelaySum => (
                results.Select(result => new TimestampTransportTypeDataPoint { Timestamp = result.BucketHour, TransportType = result.TransportType, Value = (decimal)result.DepartureDelaySum }).ToList(),
                MetricUnit.Seconds
            ),
            _ => throw new NotSupportedException($"Unsupported hourly transport metric series type: {request.MetricSeriesType}")
        };

        return new MetricSeries
        {
            SeriesType = request.MetricSeriesType,
            Unit = unit,
            DataPoints = dataPoints,
        };
    }

    private async Task<MetricSeries> GetStationSummaryAsync(StationSummaryMetricRequest request)
    {
        request.TransportTypes = (request.TransportTypes is null || request.TransportTypes.Length == 0)
            ? Enum.GetValues<TransportType>()
            : request.TransportTypes;

        var query = dataContext.HourlyStationSnapshots
            .AsNoTracking()
            .Where(summary => summary.BucketHour <= request.End.UtcDateTime)
            .Where(summary => request.TransportTypes.Contains(summary.TransportType));

        if (request.Start.HasValue)
            query = query.Where(summary => summary.BucketHour >= request.Start.Value.UtcDateTime);
        if (request.EvaNumbers != null && request.EvaNumbers.Length > 0)
            query = query.Where(summary => request.EvaNumbers.Contains(summary.EvaNumber));

        var groupedQuery = query.GroupBy(snapshot => snapshot.EvaNumber);
        var dataPoints = request.MetricSeriesType switch
        {
            MetricSeriesType.StationArrivals => await groupedQuery
                .Select(group => new StationDataPoint { EvaNumber = group.Key, Value = group.Sum(snapshot => snapshot.ArrivalCount) })
                .ToListAsync(),
            MetricSeriesType.StationArrivalCancellations => await groupedQuery
                .Select(group => new StationDataPoint { EvaNumber = group.Key, Value = group.Sum(snapshot => snapshot.ArrivalCancellationCount) })
                .ToListAsync(),
            MetricSeriesType.StationArrivalDelayAvg => await groupedQuery
                .Select(group => new
                {
                    EvaNumber = group.Key,
                    ValidArrivals = group.Sum(snapshot => snapshot.ArrivalCount - snapshot.ArrivalCancellationCount),
                    DelaySum = group.Sum(snapshot => snapshot.ArrivalDelaySum)
                })
                .Select(element => new StationDataPoint
                {
                    EvaNumber = element.EvaNumber,
                    Value = element.ValidArrivals == 0 ? 0 : (decimal)element.DelaySum / element.ValidArrivals
                })
                .ToListAsync(),
            MetricSeriesType.StationDepartures => await groupedQuery
                .Select(group => new StationDataPoint { EvaNumber = group.Key, Value = group.Sum(snapshot => snapshot.DepartureCount) })
                .ToListAsync(),
            MetricSeriesType.StationDepartureCancellations => await groupedQuery
                .Select(group => new StationDataPoint { EvaNumber = group.Key, Value = group.Sum(snapshot => snapshot.DepartureCancellationCount) })
                .ToListAsync(),
            MetricSeriesType.StationDepartureDelayAvg => await groupedQuery
                .Select(group => new
                {
                    EvaNumber = group.Key,
                    ValidDepartures = group.Sum(snapshot => snapshot.DepartureCount - snapshot.DepartureCancellationCount),
                    DelaySum = group.Sum(snapshot => snapshot.DepartureDelaySum)
                })
                .Select(element => new StationDataPoint
                {
                    EvaNumber = element.EvaNumber,
                    Value = element.ValidDepartures == 0 ? 0 : (decimal)element.DelaySum / element.ValidDepartures
                })
                .ToListAsync(),
            _ => throw new NotSupportedException($"Unsupported station metric series type: {request.MetricSeriesType}")
        };

        return new MetricSeries
        {
            SeriesType = request.MetricSeriesType,
            Unit = request.GetMetricUnit(),
            DataPoints = dataPoints,
        };
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
}
