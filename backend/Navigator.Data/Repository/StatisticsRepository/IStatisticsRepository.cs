using Navigator.Data.Entities.Statistics;
using Navigator.Data.Models.Statistics.Api;

namespace Navigator.Data.Repository.StatisticsRepository;

public interface IStatisticsRepository
{
    Task<long?> EstimateCurrentDatabaseSizeAsync();
    Task<(int Active, int Inactive)?> EstimateCurrentRisIdsAsync();
    Task<int?> EstimateCurrentJourneysAsync();

    Task<StatisticsMetricResponse> GetNetworkMetricAsync(NetworkStatisticsMetricRequest request, CancellationToken cancellationToken = default);
    Task<StatisticsMetricResponse> GetStationMetricAsync(StationStatisticsMetricRequest request, CancellationToken cancellationToken = default);
    Task<StatisticsMetricResponse> GetLineMetricAsync(LineStatisticsMetricRequest request, CancellationToken cancellationToken = default);
    Task<StatisticsMetricResponse> GetJourneyMetricAsync(JourneyStatisticsMetricRequest request, CancellationToken cancellationToken = default);

    Task SaveDatabaseSizeAsync(DatabaseSizeSnapshot snapshot);
    Task SaveRisIdSnapshotAsync(RisIdSnapshot snapshot);
    Task SaveJourneySnapshotAsync(JourneySnapshot snapshot);
}
