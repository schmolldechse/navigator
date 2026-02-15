using Navigator.Data.Entities.Statistics;
using Navigator.Data.Models.Statistics;
using Navigator.Data.Models.Statistics.Request;

namespace Navigator.Data.Repository.StatisticsRepository;

public interface IStatisticsRepository
{
    Task<long?> EstimateCurrentDatabaseSizeAsync();
    Task<(int Active, int Inactive)?> EstimateCurrentRisIdsAsync();
    Task<int?> EstimateCurrentJourneysAsync();

    Task<IEnumerable<MetricDataSet>> GetMetricAsync(BaseMetricRequest request);

    Task SaveDatabaseSizeAsync(DatabaseSize databaseSize);
    Task SaveRisIdSnapshotAsync(RisIdSnapshot risIdSnapshot);
    Task SaveJourneySnapshotAsync(JourneySnapshot journeySnapshot);
}
