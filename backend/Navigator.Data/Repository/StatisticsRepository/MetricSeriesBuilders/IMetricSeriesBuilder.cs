using Navigator.Data.Models.Statistics;

namespace Navigator.Data.Repository.StatisticsRepository.MetricSeriesBuilders;

public interface IMetricSeriesBuilder
{
    Type RequestType { get; }
    Task<MetricSeries> BuildAsync(BaseMetricRequest request);
}
