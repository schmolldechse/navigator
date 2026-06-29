using Navigator.Data.Models.Statistics;

namespace Navigator.Data.Repository.StatisticsRepository.MetricSeriesBuilders;

public abstract class MetricSeriesBuilder<T> : IMetricSeriesBuilder
    where T : BaseMetricRequest
{
    public Type RequestType => typeof(T);

    public Task<MetricSeries> BuildAsync(BaseMetricRequest request)
    {
        if (request is not T typedRequest)
            throw new ArgumentException(
                $"Expected request type '{typeof(T).Name}' but received '{request.GetType().Name}'.",
                nameof(request));

        return BuildAsync(typedRequest);
    }

    protected abstract Task<MetricSeries> BuildAsync(T request);
}
