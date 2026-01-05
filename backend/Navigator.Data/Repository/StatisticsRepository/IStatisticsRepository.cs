using Navigator.Data.Entities.Statistics;
using Navigator.Data.Enums.Metric;
using Navigator.Data.Models.Statistics;

namespace Navigator.Data.Repository.StatisticsRepository;

public interface IStatisticsRepository
{
    Task<long?> EstimateCurrentDatabaseSizeAsync();
    Task<(int Active, int Inactive)?> EstimateCurrentRisIdsAsync();
    Task<int?> EstimateCurrentJourneysAsync();

    Task<IEnumerable<MetricDataSet>> GetMetricAsync(MetricQueryType type, DateTimeOffset start, DateTimeOffset end, bool isCumulative);

    Task SaveDatabaseSizeAsync(DatabaseSize databaseSize);
    Task SaveRisIdSnapshotAsync(RisIdSnapshot risIdSnapshot);
    Task SaveJourneySnapshotAsync(JourneySnapshot journeySnapshot);
}
