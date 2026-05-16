using Microsoft.EntityFrameworkCore;

namespace Navigator.Data.Repository;

public class Estimator(DataContext dataContext)
{
    public async Task<long?> EstimateCurrentDatabaseSizeAsync() => await dataContext.Database
        .SqlQuery<long>($@"
            SELECT COALESCE(SUM(pg_total_relation_size(c.oid))::bigint, 0) AS ""Value""
            FROM pg_class c
                JOIN pg_namespace n ON n.oid = c.relnamespace
            WHERE n.nspname = 'core'
            AND c.relkind = 'r'")
        .SingleOrDefaultAsync();

    public async Task<(int Active, int Inactive)?> EstimateCurrentRisIdsAsync()
    {
        var result = await dataContext.RisIds
            .AsNoTracking()
            .GroupBy(risId => risId.Active)
            .Select(group => new { IsActive = group.Key, Count = group.Count() })
            .ToListAsync();

        if (!result.Any()) return null;

        return (
            Active: result.FirstOrDefault(x => x.IsActive)?.Count ?? 0,
            Inactive: result.FirstOrDefault(x => !x.IsActive)?.Count ?? 0
        );
    }

    public async Task<int?> EstimateCurrentJourneysAsync() => await dataContext.Journeys
        .AsNoTracking()
        .CountAsync();
}