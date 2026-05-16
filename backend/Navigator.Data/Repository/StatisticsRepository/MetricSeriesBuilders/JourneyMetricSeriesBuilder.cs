using Microsoft.EntityFrameworkCore;
using Navigator.Data.Enums.Metric;
using Navigator.Data.Models.Statistics;
using Navigator.Data.Models.Statistics.DataPoint;
using Navigator.Data.Models.Statistics.Request;

namespace Navigator.Data.Repository.StatisticsRepository.MetricSeriesBuilders;

public sealed class JourneyMetricSeriesBuilder(
    DataContext dataContext,
    Estimator estimator
) : MetricSeriesBuilder<JourneySnapshotMetricRequest>
{
    protected override async Task<MetricSeries> BuildAsync(JourneySnapshotMetricRequest request)
    {
        var dataPoints = await dataContext.JourneySnapshots
            .AsNoTracking()
            .Where(snapshot => snapshot.MeasuredAt >= request.Start.UtcDateTime && snapshot.MeasuredAt <= request.End.UtcDateTime)
            .OrderBy(snapshot => snapshot.MeasuredAt)
            .Select(snapshot => new TimestampDataPoint
            {
                Timestamp = snapshot.MeasuredAt,
                Value = snapshot.Total
            })
            .ToListAsync();

        DateTimeOffset now = DateTime.UtcNow;
        if (request.Start <= now && now <= request.End)
        {
            var currentEstimate = await estimator.EstimateCurrentJourneysAsync() ?? 0;
            dataPoints.Add(new TimestampDataPoint
            {
                Timestamp = now,
                Value = currentEstimate
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
