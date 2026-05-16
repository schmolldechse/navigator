using Microsoft.EntityFrameworkCore;
using Navigator.Data.Enums.Metric;
using Navigator.Data.Models.Statistics;
using Navigator.Data.Models.Statistics.DataPoint;
using Navigator.Data.Models.Statistics.Request;

namespace Navigator.Data.Repository.StatisticsRepository.MetricSeriesBuilders;

public sealed class RisIdMetricSeriesBuilder(
    DataContext dataContext,
    Estimator estimator
) : MetricSeriesBuilder<RisIdSnapshotMetricRequest>
{
    protected override async Task<MetricSeries> BuildAsync(RisIdSnapshotMetricRequest request)
    {
        var snapshots = await dataContext.RisIdSnapshots
            .AsNoTracking()
            .Where(snapshot => snapshot.MeasuredAt >= request.Start.UtcDateTime && snapshot.MeasuredAt <= request.End.UtcDateTime)
            .OrderBy(snapshot => snapshot.MeasuredAt)
            .ToListAsync();

        var dataPoints = request.MetricSeriesType switch
        {
            MetricSeriesType.RisIdActiveCount => snapshots.Select(snapshot => new TimestampDataPoint
            {
                Timestamp = snapshot.MeasuredAt,
                Value = snapshot.Active
            }).ToList(),
            MetricSeriesType.RisIdInactiveCount => snapshots.Select(snapshot => new TimestampDataPoint
            {
                Timestamp = snapshot.MeasuredAt,
                Value = snapshot.Inactive
            }).ToList(),
            _ => throw new NotSupportedException($"Unsupported RisId metric series type: {request.MetricSeriesType}")
        };

        DateTimeOffset now = DateTime.UtcNow;
        if (request.Start <= now && now <= request.End)
        {
            var currentEstimate = await estimator.EstimateCurrentRisIdsAsync();
            if (currentEstimate is null) currentEstimate = (0, 0);

            var currentValue = request.MetricSeriesType switch
            {
                MetricSeriesType.RisIdActiveCount => currentEstimate.Value.Active,
                MetricSeriesType.RisIdInactiveCount => currentEstimate.Value.Inactive,
                _ => 0
            };

            dataPoints.Add(new TimestampDataPoint
            {
                Timestamp = now,
                Value = currentValue
            });
        }

        return new()
        {
            SeriesType = request.MetricSeriesType,
            Unit = MetricUnit.Count,
            DataPoints = dataPoints
        };
    }
}
