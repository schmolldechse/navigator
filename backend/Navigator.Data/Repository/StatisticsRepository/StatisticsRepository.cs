using Microsoft.EntityFrameworkCore;
using Navigator.Data.Entities.Statistics;
using Navigator.Data.Models.Statistics;

namespace Navigator.Data.Repository.StatisticsRepository;

public class StatisticsRepository(
    DataContext dataContext
) : IStatisticsRepository
{
    public async Task<IEnumerable<DatabaseSize>> GetSizesByTimerangeAsync(DateTimeOffset start, DateTimeOffset end) => await dataContext.DatabaseSizes
        .Where(size => size.MeasuredAt >= start.UtcDateTime && size.MeasuredAt <= end.UtcDateTime)
        .OrderBy(size => size.MeasuredAt)
        .ToListAsync();

    public async Task<DatabaseSizeQueryResult?> EstimateDatabaseSizeAsync() => await dataContext.Database
        .SqlQuery<DatabaseSizeQueryResult>($@"
            SELECT COALESCE(SUM(pg_total_relation_size(c.oid))::bigint, 0) AS ""SizeInBytes""
            FROM pg_class c
                JOIN pg_namespace n ON n.oid = c.relnamespace
            WHERE n.nspname = 'core'
            AND c.relkind = 'r'")
        .SingleOrDefaultAsync();

    public async Task<RisIdEstimationResult> GetRisIdEstimationByTimerangeAsync(DateTimeOffset start, DateTimeOffset end)
    {
        var startedWith = await dataContext.RisIds
            .CountAsync(risId => risId.DiscoveredAt < start.UtcDateTime);

        var risIds = await dataContext.RisIds
            .Where(risId => risId.DiscoveredAt >= start.UtcDateTime && risId.DiscoveredAt <= end.UtcDateTime)
            .ToListAsync();

        return new RisIdEstimationResult()
        {
            StartedWith = startedWith,
            Total = startedWith + risIds.Count,
            RisIds = risIds
        };
    }

    public async Task SaveDatabaseSizeAsync(DatabaseSize databaseSize)
    {
        await dataContext.DatabaseSizes.AddAsync(databaseSize);
        await dataContext.SaveChangesAsync();
    }
}
