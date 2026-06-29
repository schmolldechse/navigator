using Navigator.Data.Entities.Statistics;
using Navigator.Data.Models.Statistics;

namespace Navigator.Data.Repository.StatisticsRepository;

public interface IStatisticsRepository
{
    Task<long?> EstimateCurrentDatabaseSizeAsync();
    Task<(int Active, int Inactive)?> EstimateCurrentRisIdsAsync();
    Task<int?> EstimateCurrentJourneysAsync();

    Task<MetricSeries> GetMetricAsync(BaseMetricRequest request);

    Task SaveDatabaseSizeAsync(DatabaseSizeSnapshot snapshot);
    Task SaveRisIdSnapshotAsync(RisIdSnapshot snapshot);
    Task SaveJourneySnapshotAsync(JourneySnapshot snapshot);
}
