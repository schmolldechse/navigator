using Microsoft.EntityFrameworkCore;
using Navigator.Data.Models.Statistics;
using Navigator.Data.Models.Statistics.DataPoint;
using Navigator.Data.Models.Statistics.Request;

namespace Navigator.Data.Repository.StatisticsRepository.MetricSeriesBuilders;

public sealed class StationEventQualityTimeSeriesMetricSeriesBuilder(
    DataContext dataContext
) : MetricSeriesBuilder<StationEventQualityTimeSeriesMetricRequest>
{
    protected override async Task<MetricSeries> BuildAsync(StationEventQualityTimeSeriesMetricRequest request)
    {
        var definition = StationEventQualityMetricDefinitions.GetDefinition(request.SeriesType);
        var transportTypes = StationEventQualityMetricDefinitions.NormalizeTransportTypes(request.TransportTypes);

        var query = dataContext.StationLineRouteQualities
            .AsNoTracking()
            .Where(summary => summary.BucketHour >= request.Start.UtcDateTime && summary.BucketHour <= request.End.UtcDateTime)
            .Where(summary => summary.ScheduleType == request.ScheduleType)
            .Where(summary => transportTypes.Contains(summary.TransportType))
            .Where(summary => request.IncludeReplacementTransport || !summary.IsReplacementTransport);

        if (request.EvaNumbers is { Length: > 0 })
            query = query.Where(summary => request.EvaNumbers.Contains(summary.StationEvaNumber));

        var dataPoints = (await query.ToListAsync())
            .GroupBy(summary => new { summary.BucketHour, summary.StationEvaNumber, summary.TransportType })
            .Select(group => new
            {
                group.Key.BucketHour,
                EvaNumber = group.Key.StationEvaNumber,
                group.Key.TransportType,
                Aggregate = StationEventQualityMetricDefinitions.Aggregate(group)
            })
            .OrderBy(element => element.BucketHour)
            .ThenBy(element => element.EvaNumber)
            .ThenBy(element => element.TransportType)
            .Select(element => new TimestampStationTransportTypeDataPoint
            {
                Timestamp = element.BucketHour,
                EvaNumber = element.EvaNumber,
                TransportType = element.TransportType,
                Value = definition.GetValue(element.Aggregate),
                Sample = definition.CreateSample?.Invoke(element.Aggregate)
            })
            .ToList();

        return new MetricSeries()
        {
            SeriesType = request.SeriesType,
            Unit = definition.Unit,
            DataPoints = dataPoints
        };
    }
}
