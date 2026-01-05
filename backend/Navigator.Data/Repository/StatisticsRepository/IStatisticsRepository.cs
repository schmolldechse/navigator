using Navigator.Data.Entities.Statistics;
using Navigator.Data.Models.Statistics;

namespace Navigator.Data.Repository.StatisticsRepository;

public interface IStatisticsRepository
{
    Task<long?> EstimateCurrentDatabaseSizeAsync();
    Task<(int Active, int Inactive)?> EstimateCurrentRisIdsAsync();
    Task<int?> EstimateCurrentJourneysAsync();

    Task<BaseSnapshot<DatabaseSize>> GetDatabaseSizeSnapshotAsync(DateTimeOffset start, DateTimeOffset end);
    Task<BaseSnapshot<RisIdSnapshot>> GetRisIdSnapshotAsync(DateTimeOffset start, DateTimeOffset end);
    Task<BaseSnapshot<JourneySnapshot>> GetJourneySnapshotAsync(DateTimeOffset start, DateTimeOffset end);

    Task SaveDatabaseSizeAsync(DatabaseSize databaseSize);
    Task SaveRisIdSnapshotAsync(RisIdSnapshot risIdSnapshot);
    Task SaveJourneySnapshotAsync(JourneySnapshot journeySnapshot);
}
