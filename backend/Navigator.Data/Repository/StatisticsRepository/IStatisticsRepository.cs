using Navigator.Data.Entities.Journey;
using Navigator.Data.Entities.RisId;
using Navigator.Data.Entities.Statistics;
using Navigator.Data.Models.Statistics;

namespace Navigator.Data.Repository.StatisticsRepository;

public interface IStatisticsRepository
{
    Task<IEnumerable<DatabaseSize>> GetSizesByTimerangeAsync(DateTimeOffset start, DateTimeOffset end);
    Task<DatabaseSizeQueryResult?> EstimateDatabaseSizeAsync();
    Task<BaseEstimationResult<RisId>> GetRisIdEstimationByTimerangeAsync(DateTimeOffset start, DateTimeOffset end);
    Task<BaseEstimationResult<Journey>> GetJourneyEstimationByTimerangeAsync(DateTimeOffset start, DateTimeOffset end);
    Task SaveDatabaseSizeAsync(DatabaseSize databaseSize);
}
