using Navigator.Data.Entities.Statistics;
using Navigator.Data.Models.Statistics;
using Navigator.Data.Repository.StationRil100Repository;
using Navigator.Data.Repository.StatisticsRepository.MetricSeriesBuilders;

namespace Navigator.Data.Repository.StatisticsRepository;

public class StatisticsRepository(
    DataContext dataContext,
    Estimator estimator,
    IStationRil100Repository stationRil100Repository,
    IEnumerable<IMetricSeriesBuilder> metricSeriesBuilders
) : IStatisticsRepository
{
    private readonly IReadOnlyDictionary<Type, IMetricSeriesBuilder> buildersByRequestType =
        metricSeriesBuilders.ToDictionary(builder => builder.RequestType);

    public Task<long?> EstimateCurrentDatabaseSizeAsync() => estimator.EstimateCurrentDatabaseSizeAsync();

    public Task<(int Active, int Inactive)?> EstimateCurrentRisIdsAsync() => estimator.EstimateCurrentRisIdsAsync();

    public Task<int?> EstimateCurrentJourneysAsync() => estimator.EstimateCurrentJourneysAsync();

    public async Task<MetricSeries> GetMetricAsync(BaseMetricRequest baseRequest)
    {
        await ExpandEvaNumbersByRil100Async(baseRequest);

        if (buildersByRequestType.TryGetValue(baseRequest.GetType(), out var builder))
            return await builder.BuildAsync(baseRequest);

        throw new NotSupportedException($"Metric type '{baseRequest.SeriesType}' is not supported.");
    }

    private async Task ExpandEvaNumbersByRil100Async(BaseMetricRequest request)
    {
        if (request is not IEvaNumberMetricRequest { IncludeRil100: true, EvaNumbers.Length: > 0 } evaNumberRequest)
            return;

        evaNumberRequest.EvaNumbers = await stationRil100Repository.ExpandEvaNumbersByRil100Async(evaNumberRequest.EvaNumbers);
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
