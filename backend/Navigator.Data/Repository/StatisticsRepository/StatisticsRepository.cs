using Microsoft.EntityFrameworkCore;
using Navigator.Data.Entities.Statistics;
using Navigator.Data.Entities.Views;
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
        StationTimeSeriesMetricRequest request => await GetStationTimeSeriesAsync(request),
        JourneyServiceMetricRequest request => await GetJourneyServiceMetricsAsync(request),
        MessageSummaryMetricRequest request => await GetMessageSummaryMetricsAsync(request),

        _ => throw new NotSupportedException($"Metric type '{baseRequest.MetricSeriesType}' is not supported.")
    };

    private async Task<MetricSeries> GetDatabaseSizeMetricsAsync(DatabaseSizeSnapshotMetricRequest request)
    {
        var dataPoints = await dataContext.DatabaseSizeSnapshots
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
                ArrivalDelaySampleCount = group.Sum(summary => summary.ArrivalDelaySampleCount),
                ArrivalPunctualCount = group.Sum(summary => summary.ArrivalPunctualCount),
                ArrivalDelayMinorCount = group.Sum(summary => summary.ArrivalDelayMinorCount),
                ArrivalDelayMajorCount = group.Sum(summary => summary.ArrivalDelayMajorCount),
                ArrivalDelaySevereCount = group.Sum(summary => summary.ArrivalDelaySevereCount),
                ArrivalPlatformChangeCount = group.Sum(summary => summary.ArrivalPlatformChangeCount),

                DeparturesCount = group.Sum(summary => summary.DepartureCount),
                DepartureCancellationCount = group.Sum(summary => summary.DepartureCancellationCount),
                DepartureDelaySum = group.Sum(summary => summary.DepartureDelaySum),
                DepartureDelaySampleCount = group.Sum(summary => summary.DepartureDelaySampleCount),
                DeparturePunctualCount = group.Sum(summary => summary.DeparturePunctualCount),
                DepartureDelayMinorCount = group.Sum(summary => summary.DepartureDelayMinorCount),
                DepartureDelayMajorCount = group.Sum(summary => summary.DepartureDelayMajorCount),
                DepartureDelaySevereCount = group.Sum(summary => summary.DepartureDelaySevereCount),
                DeparturePlatformChangeCount = group.Sum(summary => summary.DeparturePlatformChangeCount),
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
            MetricSeriesType.HourlyGlobalArrivalDelaySampleCount => (
                results.Select(result => new TimestampTransportTypeDataPoint { Timestamp = result.BucketHour, TransportType = result.TransportType, Value = result.ArrivalDelaySampleCount }).ToList(),
                MetricUnit.Count
            ),
            MetricSeriesType.HourlyGlobalArrivalPunctualCount => (
                results.Select(result => new TimestampTransportTypeDataPoint { Timestamp = result.BucketHour, TransportType = result.TransportType, Value = result.ArrivalPunctualCount }).ToList(),
                MetricUnit.Count
            ),
            MetricSeriesType.HourlyGlobalArrivalDelayMinorCount => (
                results.Select(result => new TimestampTransportTypeDataPoint { Timestamp = result.BucketHour, TransportType = result.TransportType, Value = result.ArrivalDelayMinorCount }).ToList(),
                MetricUnit.Count
            ),
            MetricSeriesType.HourlyGlobalArrivalDelayMajorCount => (
                results.Select(result => new TimestampTransportTypeDataPoint { Timestamp = result.BucketHour, TransportType = result.TransportType, Value = result.ArrivalDelayMajorCount }).ToList(),
                MetricUnit.Count
            ),
            MetricSeriesType.HourlyGlobalArrivalDelaySevereCount => (
                results.Select(result => new TimestampTransportTypeDataPoint { Timestamp = result.BucketHour, TransportType = result.TransportType, Value = result.ArrivalDelaySevereCount }).ToList(),
                MetricUnit.Count
            ),
            MetricSeriesType.HourlyGlobalArrivalPlatformChanges => (
                results.Select(result => new TimestampTransportTypeDataPoint { Timestamp = result.BucketHour, TransportType = result.TransportType, Value = result.ArrivalPlatformChangeCount }).ToList(),
                MetricUnit.Count
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
            MetricSeriesType.HourlyGlobalDepartureDelaySampleCount => (
                results.Select(result => new TimestampTransportTypeDataPoint { Timestamp = result.BucketHour, TransportType = result.TransportType, Value = result.DepartureDelaySampleCount }).ToList(),
                MetricUnit.Count
            ),
            MetricSeriesType.HourlyGlobalDeparturePunctualCount => (
                results.Select(result => new TimestampTransportTypeDataPoint { Timestamp = result.BucketHour, TransportType = result.TransportType, Value = result.DeparturePunctualCount }).ToList(),
                MetricUnit.Count
            ),
            MetricSeriesType.HourlyGlobalDepartureDelayMinorCount => (
                results.Select(result => new TimestampTransportTypeDataPoint { Timestamp = result.BucketHour, TransportType = result.TransportType, Value = result.DepartureDelayMinorCount }).ToList(),
                MetricUnit.Count
            ),
            MetricSeriesType.HourlyGlobalDepartureDelayMajorCount => (
                results.Select(result => new TimestampTransportTypeDataPoint { Timestamp = result.BucketHour, TransportType = result.TransportType, Value = result.DepartureDelayMajorCount }).ToList(),
                MetricUnit.Count
            ),
            MetricSeriesType.HourlyGlobalDepartureDelaySevereCount => (
                results.Select(result => new TimestampTransportTypeDataPoint { Timestamp = result.BucketHour, TransportType = result.TransportType, Value = result.DepartureDelaySevereCount }).ToList(),
                MetricUnit.Count
            ),
            MetricSeriesType.HourlyGlobalDeparturePlatformChanges => (
                results.Select(result => new TimestampTransportTypeDataPoint { Timestamp = result.BucketHour, TransportType = result.TransportType, Value = result.DeparturePlatformChangeCount }).ToList(),
                MetricUnit.Count
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
                    ValidArrivals = group.Sum(snapshot => snapshot.ArrivalDelaySampleCount),
                    DelaySum = group.Sum(snapshot => snapshot.ArrivalDelaySum)
                })
                .Select(element => new StationDataPoint
                {
                    EvaNumber = element.EvaNumber,
                    Value = element.ValidArrivals == 0 ? 0 : (decimal)element.DelaySum / element.ValidArrivals
                })
                .ToListAsync(),
            MetricSeriesType.StationArrivalPunctualityRate => await groupedQuery
                .Select(group => new
                {
                    EvaNumber = group.Key,
                    ValidArrivals = group.Sum(snapshot => snapshot.ArrivalDelaySampleCount),
                    PunctualArrivals = group.Sum(snapshot => snapshot.ArrivalPunctualCount)
                })
                .Select(element => new StationDataPoint
                {
                    EvaNumber = element.EvaNumber,
                    Value = element.ValidArrivals == 0 ? 0 : (decimal)element.PunctualArrivals / element.ValidArrivals * 100
                })
                .ToListAsync(),
            MetricSeriesType.StationArrivalDelayMinor => await groupedQuery
                .Select(group => new StationDataPoint { EvaNumber = group.Key, Value = group.Sum(snapshot => snapshot.ArrivalDelayMinorCount) })
                .ToListAsync(),
            MetricSeriesType.StationArrivalDelayMajor => await groupedQuery
                .Select(group => new StationDataPoint { EvaNumber = group.Key, Value = group.Sum(snapshot => snapshot.ArrivalDelayMajorCount) })
                .ToListAsync(),
            MetricSeriesType.StationArrivalDelaySevere => await groupedQuery
                .Select(group => new StationDataPoint { EvaNumber = group.Key, Value = group.Sum(snapshot => snapshot.ArrivalDelaySevereCount) })
                .ToListAsync(),
            MetricSeriesType.StationArrivalPlatformChanges => await groupedQuery
                .Select(group => new StationDataPoint { EvaNumber = group.Key, Value = group.Sum(snapshot => snapshot.ArrivalPlatformChangeCount) })
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
                    ValidDepartures = group.Sum(snapshot => snapshot.DepartureDelaySampleCount),
                    DelaySum = group.Sum(snapshot => snapshot.DepartureDelaySum)
                })
                .Select(element => new StationDataPoint
                {
                    EvaNumber = element.EvaNumber,
                    Value = element.ValidDepartures == 0 ? 0 : (decimal)element.DelaySum / element.ValidDepartures
                })
                .ToListAsync(),
            MetricSeriesType.StationDeparturePunctualityRate => await groupedQuery
                .Select(group => new
                {
                    EvaNumber = group.Key,
                    ValidDepartures = group.Sum(snapshot => snapshot.DepartureDelaySampleCount),
                    PunctualDepartures = group.Sum(snapshot => snapshot.DeparturePunctualCount)
                })
                .Select(element => new StationDataPoint
                {
                    EvaNumber = element.EvaNumber,
                    Value = element.ValidDepartures == 0 ? 0 : (decimal)element.PunctualDepartures / element.ValidDepartures * 100
                })
                .ToListAsync(),
            MetricSeriesType.StationDepartureDelayMinor => await groupedQuery
                .Select(group => new StationDataPoint { EvaNumber = group.Key, Value = group.Sum(snapshot => snapshot.DepartureDelayMinorCount) })
                .ToListAsync(),
            MetricSeriesType.StationDepartureDelayMajor => await groupedQuery
                .Select(group => new StationDataPoint { EvaNumber = group.Key, Value = group.Sum(snapshot => snapshot.DepartureDelayMajorCount) })
                .ToListAsync(),
            MetricSeriesType.StationDepartureDelaySevere => await groupedQuery
                .Select(group => new StationDataPoint { EvaNumber = group.Key, Value = group.Sum(snapshot => snapshot.DepartureDelaySevereCount) })
                .ToListAsync(),
            MetricSeriesType.StationDeparturePlatformChanges => await groupedQuery
                .Select(group => new StationDataPoint { EvaNumber = group.Key, Value = group.Sum(snapshot => snapshot.DeparturePlatformChangeCount) })
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

    private async Task<MetricSeries> GetStationTimeSeriesAsync(StationTimeSeriesMetricRequest request)
    {
        request.TransportTypes = (request.TransportTypes is null || request.TransportTypes.Length == 0)
            ? Enum.GetValues<TransportType>()
            : request.TransportTypes;

        var query = dataContext.HourlyStationSnapshots
            .AsNoTracking()
            .Where(summary => summary.BucketHour >= request.Start.UtcDateTime && summary.BucketHour <= request.End.UtcDateTime)
            .Where(summary => request.TransportTypes.Contains(summary.TransportType))
            .Where(summary => request.EvaNumbers.Contains(summary.EvaNumber));

        var results = (await query.ToListAsync())
            .GroupBy(summary => new
            {
                BucketHour = summary.BucketHour.AddHours(-(summary.BucketHour.Hour % request.Stepping)),
                summary.EvaNumber,
                summary.TransportType
            })
            .Select(group => new
            {
                group.Key.BucketHour,
                group.Key.EvaNumber,
                group.Key.TransportType,
                ArrivalCount = group.Sum(summary => summary.ArrivalCount),
                ArrivalCancellationCount = group.Sum(summary => summary.ArrivalCancellationCount),
                ArrivalDelaySum = group.Sum(summary => summary.ArrivalDelaySum),
                ArrivalDelaySampleCount = group.Sum(summary => summary.ArrivalDelaySampleCount),
                ArrivalPunctualCount = group.Sum(summary => summary.ArrivalPunctualCount),
                ArrivalPlatformChangeCount = group.Sum(summary => summary.ArrivalPlatformChangeCount),
                DepartureCount = group.Sum(summary => summary.DepartureCount),
                DepartureCancellationCount = group.Sum(summary => summary.DepartureCancellationCount),
                DepartureDelaySum = group.Sum(summary => summary.DepartureDelaySum),
                DepartureDelaySampleCount = group.Sum(summary => summary.DepartureDelaySampleCount),
                DeparturePunctualCount = group.Sum(summary => summary.DeparturePunctualCount),
                DeparturePlatformChangeCount = group.Sum(summary => summary.DeparturePlatformChangeCount),
            })
            .OrderBy(element => element.BucketHour)
            .ToList();

        static TimestampStationTransportTypeDataPoint DataPoint(DateTime timestamp, int evaNumber, TransportType transportType, decimal value) => new()
        {
            Timestamp = timestamp,
            EvaNumber = evaNumber,
            TransportType = transportType,
            Value = value
        };

        var (dataPoints, unit) = request.MetricSeriesType switch
        {
            MetricSeriesType.StationTimelineArrivals => (
                results.Select(result => DataPoint(result.BucketHour, result.EvaNumber, result.TransportType, result.ArrivalCount)).ToList(),
                MetricUnit.Count
            ),
            MetricSeriesType.StationTimelineArrivalCancellations => (
                results.Select(result => DataPoint(result.BucketHour, result.EvaNumber, result.TransportType, result.ArrivalCancellationCount)).ToList(),
                MetricUnit.Count
            ),
            MetricSeriesType.StationTimelineArrivalDelayAvg => (
                results.Select(result => DataPoint(result.BucketHour, result.EvaNumber, result.TransportType, result.ArrivalDelaySampleCount == 0 ? 0 : (decimal)result.ArrivalDelaySum / result.ArrivalDelaySampleCount)).ToList(),
                MetricUnit.Seconds
            ),
            MetricSeriesType.StationTimelineArrivalPunctualityRate => (
                results.Select(result => DataPoint(result.BucketHour, result.EvaNumber, result.TransportType, result.ArrivalDelaySampleCount == 0 ? 0 : (decimal)result.ArrivalPunctualCount / result.ArrivalDelaySampleCount * 100)).ToList(),
                MetricUnit.Percent
            ),
            MetricSeriesType.StationTimelineArrivalPlatformChanges => (
                results.Select(result => DataPoint(result.BucketHour, result.EvaNumber, result.TransportType, result.ArrivalPlatformChangeCount)).ToList(),
                MetricUnit.Count
            ),
            MetricSeriesType.StationTimelineDepartures => (
                results.Select(result => DataPoint(result.BucketHour, result.EvaNumber, result.TransportType, result.DepartureCount)).ToList(),
                MetricUnit.Count
            ),
            MetricSeriesType.StationTimelineDepartureCancellations => (
                results.Select(result => DataPoint(result.BucketHour, result.EvaNumber, result.TransportType, result.DepartureCancellationCount)).ToList(),
                MetricUnit.Count
            ),
            MetricSeriesType.StationTimelineDepartureDelayAvg => (
                results.Select(result => DataPoint(result.BucketHour, result.EvaNumber, result.TransportType, result.DepartureDelaySampleCount == 0 ? 0 : (decimal)result.DepartureDelaySum / result.DepartureDelaySampleCount)).ToList(),
                MetricUnit.Seconds
            ),
            MetricSeriesType.StationTimelineDeparturePunctualityRate => (
                results.Select(result => DataPoint(result.BucketHour, result.EvaNumber, result.TransportType, result.DepartureDelaySampleCount == 0 ? 0 : (decimal)result.DeparturePunctualCount / result.DepartureDelaySampleCount * 100)).ToList(),
                MetricUnit.Percent
            ),
            MetricSeriesType.StationTimelineDeparturePlatformChanges => (
                results.Select(result => DataPoint(result.BucketHour, result.EvaNumber, result.TransportType, result.DeparturePlatformChangeCount)).ToList(),
                MetricUnit.Count
            ),
            _ => throw new NotSupportedException($"Unsupported station time-series metric series type: {request.MetricSeriesType}")
        };

        return new MetricSeries
        {
            SeriesType = request.MetricSeriesType,
            Unit = unit,
            DataPoints = dataPoints,
        };
    }

    private async Task<MetricSeries> GetJourneyServiceMetricsAsync(JourneyServiceMetricRequest request)
    {
        request.TransportTypes = (request.TransportTypes is null || request.TransportTypes.Length == 0)
            ? Enum.GetValues<TransportType>()
            : request.TransportTypes;
        request.JourneyTypes = (request.JourneyTypes is null || request.JourneyTypes.Length == 0)
            ? Enum.GetValues<JourneyType>()
            : request.JourneyTypes;

        var query = dataContext.DailyJourneyServiceSnapshots
            .AsNoTracking()
            .Where(snapshot => snapshot.BucketDay >= request.Start.UtcDateTime.Date && snapshot.BucketDay <= request.End.UtcDateTime.Date)
            .Where(snapshot => request.TransportTypes.Contains(snapshot.TransportType))
            .Where(snapshot => request.JourneyTypes.Contains(snapshot.JourneyType));

        if (request.OperatorCodes is { Length: > 0 })
            query = query.Where(snapshot => request.OperatorCodes.Contains(snapshot.OperatorCode));

        var snapshots = await query.ToListAsync();

        var dataPoints = request.MetricSeriesType switch
        {
            MetricSeriesType.JourneyServiceOperatorJourneys => snapshots
                .GroupBy(snapshot => new { snapshot.OperatorCode, snapshot.OperatorName })
                .Select(group => new CategoryDataPoint
                {
                    Category = $"{group.Key.OperatorName} ({group.Key.OperatorCode})",
                    Value = group.Sum(snapshot => snapshot.JourneyCount)
                })
                .OrderByDescending(dataPoint => dataPoint.Value)
                .ToList<BaseMetricDataPoint>(),
            MetricSeriesType.JourneyServiceOperatorCancellations => snapshots
                .GroupBy(snapshot => new { snapshot.OperatorCode, snapshot.OperatorName })
                .Select(group => new CategoryDataPoint
                {
                    Category = $"{group.Key.OperatorName} ({group.Key.OperatorCode})",
                    Value = group.Sum(snapshot => snapshot.JourneyCancellationCount)
                })
                .OrderByDescending(dataPoint => dataPoint.Value)
                .ToList<BaseMetricDataPoint>(),
            MetricSeriesType.JourneyServiceDailyOperatorJourneys => snapshots
                .GroupBy(snapshot => new { snapshot.BucketDay, snapshot.OperatorCode, snapshot.OperatorName })
                .Select(group => new TimestampCategoryDataPoint
                {
                    Timestamp = group.Key.BucketDay,
                    Category = $"{group.Key.OperatorName} ({group.Key.OperatorCode})",
                    Value = group.Sum(snapshot => snapshot.JourneyCount)
                })
                .OrderBy(dataPoint => dataPoint.Timestamp)
                .ToList<BaseMetricDataPoint>(),
            MetricSeriesType.JourneyServiceDailyOperatorCancellations => snapshots
                .GroupBy(snapshot => new { snapshot.BucketDay, snapshot.OperatorCode, snapshot.OperatorName })
                .Select(group => new TimestampCategoryDataPoint
                {
                    Timestamp = group.Key.BucketDay,
                    Category = $"{group.Key.OperatorName} ({group.Key.OperatorCode})",
                    Value = group.Sum(snapshot => snapshot.JourneyCancellationCount)
                })
                .OrderBy(dataPoint => dataPoint.Timestamp)
                .ToList<BaseMetricDataPoint>(),
            MetricSeriesType.JourneyServiceJourneyTypeJourneys => snapshots
                .GroupBy(snapshot => new { snapshot.BucketDay, snapshot.JourneyType })
                .Select(group => new TimestampCategoryDataPoint
                {
                    Timestamp = group.Key.BucketDay,
                    Category = group.Key.JourneyType.ToString(),
                    Value = group.Sum(snapshot => snapshot.JourneyCount)
                })
                .OrderBy(dataPoint => dataPoint.Timestamp)
                .ToList<BaseMetricDataPoint>(),
            MetricSeriesType.JourneyServiceReplacementTransports => snapshots
                .GroupBy(snapshot => new { snapshot.BucketDay, snapshot.TransportType })
                .Select(group => new TimestampCategoryDataPoint
                {
                    Timestamp = group.Key.BucketDay,
                    Category = group.Key.TransportType.ToString(),
                    Value = group.Sum(snapshot => snapshot.ReplacementTransportCount)
                })
                .OrderBy(dataPoint => dataPoint.Timestamp)
                .ToList<BaseMetricDataPoint>(),
            _ => throw new NotSupportedException($"Unsupported journey service metric series type: {request.MetricSeriesType}")
        };

        return new MetricSeries
        {
            SeriesType = request.MetricSeriesType,
            Unit = MetricUnit.Count,
            DataPoints = dataPoints,
        };
    }

    private async Task<MetricSeries> GetMessageSummaryMetricsAsync(MessageSummaryMetricRequest request)
    {
        request.TransportTypes = (request.TransportTypes is null || request.TransportTypes.Length == 0)
            ? Enum.GetValues<TransportType>()
            : request.TransportTypes;
        request.MessageTypes = (request.MessageTypes is null || request.MessageTypes.Length == 0)
            ? request.MetricSeriesType is MetricSeriesType.MessageDisruptionCauses or MetricSeriesType.StationMessageDisruptions
                ? [MessageType.Disruption]
                : Enum.GetValues<MessageType>()
            : request.MessageTypes;

        var query = dataContext.DailyStationMessageSnapshots
            .AsNoTracking()
            .Where(snapshot => snapshot.BucketDay >= request.Start.UtcDateTime.Date && snapshot.BucketDay <= request.End.UtcDateTime.Date)
            .Where(snapshot => request.TransportTypes.Contains(snapshot.TransportType))
            .Where(snapshot => request.MessageTypes.Contains(snapshot.MessageType));

        if (request.EvaNumbers is { Length: > 0 })
            query = query.Where(snapshot => request.EvaNumbers.Contains(snapshot.EvaNumber));

        var snapshots = await query.ToListAsync();

        static string Category(DailyStationMessageSnapshot snapshot)
        {
            if (!string.IsNullOrWhiteSpace(snapshot.DisruptionCause)) return snapshot.DisruptionCause;
            if (!string.IsNullOrWhiteSpace(snapshot.MessageCode)) return snapshot.MessageCode;
            if (!string.IsNullOrWhiteSpace(snapshot.DisruptionEffect)) return snapshot.DisruptionEffect;
            if (!string.IsNullOrWhiteSpace(snapshot.NoteCategory)) return snapshot.NoteCategory;
            return snapshot.MessageType.ToString();
        }

        var dataPoints = request.MetricSeriesType switch
        {
            MetricSeriesType.MessageDisruptionCauses => snapshots
                .GroupBy(Category)
                .Select(group => new CategoryDataPoint
                {
                    Category = group.Key,
                    Value = group.Sum(snapshot => snapshot.MessageCount)
                })
                .OrderByDescending(dataPoint => dataPoint.Value)
                .Take(request.Limit)
                .ToList<BaseMetricDataPoint>(),
            MetricSeriesType.MessageDailyTypes => snapshots
                .GroupBy(snapshot => new { snapshot.BucketDay, snapshot.MessageType })
                .Select(group => new TimestampCategoryDataPoint
                {
                    Timestamp = group.Key.BucketDay,
                    Category = group.Key.MessageType.ToString(),
                    Value = group.Sum(snapshot => snapshot.MessageCount)
                })
                .OrderBy(dataPoint => dataPoint.Timestamp)
                .ToList<BaseMetricDataPoint>(),
            MetricSeriesType.StationMessageDisruptions => snapshots
                .GroupBy(snapshot => snapshot.EvaNumber)
                .Select(group => new StationDataPoint
                {
                    EvaNumber = group.Key,
                    Value = group.Sum(snapshot => snapshot.MessageCount)
                })
                .OrderByDescending(dataPoint => dataPoint.Value)
                .ToList<BaseMetricDataPoint>(),
            MetricSeriesType.StationMessageAffectedStops => snapshots
                .GroupBy(snapshot => snapshot.EvaNumber)
                .Select(group => new StationDataPoint
                {
                    EvaNumber = group.Key,
                    Value = group.Sum(snapshot => snapshot.AffectedStopPlaceCount)
                })
                .OrderByDescending(dataPoint => dataPoint.Value)
                .ToList<BaseMetricDataPoint>(),
            _ => throw new NotSupportedException($"Unsupported message metric series type: {request.MetricSeriesType}")
        };

        return new MetricSeries
        {
            SeriesType = request.MetricSeriesType,
            Unit = MetricUnit.Count,
            DataPoints = dataPoints,
        };
    }

    public async Task SaveDatabaseSizeAsync(DatabaseSizeSnapshot snapshot)
    {
        await dataContext.DatabaseSizeSnapshots.AddAsync(snapshot);
        await dataContext.SaveChangesAsync();
    }

    public async Task SaveRisIdSnapshotAsync(RisIdSnapshot snapshot)
    {
        await dataContext.RisIdSnapshots.AddAsync(snapshot);
        await dataContext.SaveChangesAsync();
    }

    public async Task SaveJourneySnapshotAsync(JourneySnapshot snapshot)
    {
        await dataContext.JourneySnapshots.AddAsync(snapshot);
        await dataContext.SaveChangesAsync();
    }
}
