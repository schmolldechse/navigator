using Microsoft.EntityFrameworkCore;
using Navigator.Data.Enums;
using Navigator.Data.Enums.Metric;
using Navigator.Data.Models.Statistics;
using Navigator.Data.Models.Statistics.DataPoint;
using Navigator.Data.Models.Statistics.Request;

namespace Navigator.Data.Repository.StatisticsRepository.MetricSeriesBuilders;

public sealed class TransportTypeDistributionMetricSeriesBuilder(
    DataContext dataContext
) : MetricSeriesBuilder<TransportTypeDistributionMetricRequest>
{
    protected override async Task<MetricSeries> BuildAsync(TransportTypeDistributionMetricRequest request)
    {
        request.TransportTypes = (request.TransportTypes is null || request.TransportTypes.Length == 0)
            ? Enum.GetValues<TransportType>()
            : request.TransportTypes;

        var query = dataContext.JourneyTransports
            .Include(transport => transport.Journey)
            .AsNoTracking()
            .Where(transport => transport.Journey!.Date <= DateOnly.FromDateTime(request.End.DateTime))
            .Where(transport => request.TransportTypes.Contains(transport.TransportType));

        var results = await query
            .GroupBy(query => query.TransportType)
            .Select(group => new { TransportType = group.Key, Value = group.Count() })
            .ToListAsync();

        var dataPoints = results
            .Select(dataPoint => new TransportTypeDataPoint
            {
                TransportType = dataPoint.TransportType,
                Value = dataPoint.Value
            })
            .OrderByDescending(dataPoint => dataPoint.Value)
            .ToList();

        return new()
        {
            SeriesType = request.MetricSeriesType,
            DataPoints = dataPoints,
            Unit = MetricUnit.Count
        };
    }
}
