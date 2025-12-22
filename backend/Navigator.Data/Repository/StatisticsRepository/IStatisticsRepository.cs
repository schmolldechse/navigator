using Navigator.Data.Entities.Statistics;
using Navigator.Data.Models.Statistics;

namespace Navigator.Data.Repository.StatisticsRepository;

public interface IStatisticsRepository
{
    Task<IEnumerable<DatabaseSize>> GetSizesByTimerangeAsync(DateTimeOffset start, DateTimeOffset end);
    Task<DatabaseSizeQueryResult?> EstimateDatabaseSizeAsync();
    Task SaveDatabaseSizeAsync(DatabaseSize databaseSize);
}
