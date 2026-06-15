using Navigator.Data.Entities.Statistics;
using Navigator.Data.Models.Statistics.Api;
using Navigator.Data.Repository.StatisticsRepository.MetricSeriesBuilders;

namespace Navigator.Data.Repository.StatisticsRepository;

public class StatisticsRepository(
    DataContext dataContext,
    Estimator estimator,
    IEnumerable<IStatisticsMetricBuilder> metricBuilders
) : IStatisticsRepository
{
    private readonly IReadOnlyList<IStatisticsMetricBuilder> metricBuilders = metricBuilders.ToList();

    public Task<long?> EstimateCurrentDatabaseSizeAsync() => estimator.EstimateCurrentDatabaseSizeAsync();

    public Task<(int Active, int Inactive)?> EstimateCurrentRisIdsAsync() => estimator.EstimateCurrentRisIdsAsync();

    public Task<int?> EstimateCurrentJourneysAsync() => estimator.EstimateCurrentJourneysAsync();

    public Task<StatisticsMetricResponse> GetNetworkMetricAsync(
        NetworkStatisticsMetricRequest request,
        CancellationToken cancellationToken = default
    ) => BuildMetricAsync(request, cancellationToken);

    public Task<StatisticsMetricResponse> GetStationMetricAsync(
        StationStatisticsMetricRequest request,
        CancellationToken cancellationToken = default
    ) => BuildMetricAsync(request, cancellationToken);

    public Task<StatisticsMetricResponse> GetLineMetricAsync(
        LineStatisticsMetricRequest request,
        CancellationToken cancellationToken = default
    ) => BuildMetricAsync(request, cancellationToken);

    public Task<StatisticsMetricResponse> GetJourneyMetricAsync(
        JourneyStatisticsMetricRequest request,
        CancellationToken cancellationToken = default
    ) => BuildMetricAsync(request, cancellationToken);

    private async Task<StatisticsMetricResponse> BuildMetricAsync(
        StatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var builder = metricBuilders.FirstOrDefault(builder => builder.CanBuild(request));
        if (builder is null)
            throw new NotSupportedException($"No statistics metric builder supports request type '{request.GetType().Name}'.");

        return await builder.BuildAsync(request, cancellationToken);
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
