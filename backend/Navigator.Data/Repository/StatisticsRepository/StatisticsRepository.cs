using Microsoft.EntityFrameworkCore;
using Navigator.Data.Entities.Statistics;
using Navigator.Data.Models.Statistics;

namespace Navigator.Data.Repository.StatisticsRepository;

public class StatisticsRepository(
    DataContext dataContext
) : IStatisticsRepository
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

    public async Task<BaseSnapshot<DatabaseSize>> GetDatabaseSizeSnapshotAsync(DateTimeOffset start, DateTimeOffset end)
    {
        var now = DateTime.UtcNow;

        var snapshots = await dataContext.DatabaseSizes
            .AsNoTracking()
            .Where(snapshot => snapshot.MeasuredAt >= start.UtcDateTime && snapshot.MeasuredAt <= end.UtcDateTime)
            .OrderBy(snapshot => snapshot.MeasuredAt)
            .ToListAsync();

        var previousEntry = await dataContext.DatabaseSizes
            .AsNoTracking()
            .Where(snapshot => snapshot.MeasuredAt < start.UtcDateTime)
            .OrderByDescending(snapshot => snapshot.MeasuredAt)
            .FirstOrDefaultAsync();

        DatabaseSize? currentEstimate = null;
        if (start <= now && now <= end)
        {
            var estimate = await EstimateCurrentDatabaseSizeAsync();
            if (estimate.HasValue)
            {
                currentEstimate = new DatabaseSize()
                {
                    MeasuredAt = now,
                    SizeInBytes = estimate.Value
                };
                snapshots.Add(currentEstimate);
            }
        }

        long endingValue;
        if (currentEstimate is not null) endingValue = currentEstimate.SizeInBytes;
        else if (snapshots.Any()) endingValue = snapshots.Last().SizeInBytes;
        else
        {
            var lastEntry = await dataContext.DatabaseSizes
                .AsNoTracking()
                .Where(snapshot => snapshot.MeasuredAt <= end.UtcDateTime)
                .OrderByDescending(snapshot => snapshot.MeasuredAt)
                .FirstOrDefaultAsync();
            endingValue = lastEntry?.SizeInBytes ?? 0;
        }
        
        return new BaseSnapshot<DatabaseSize>()
        {
            StartedWith = previousEntry?.SizeInBytes ?? 0,
            Total = endingValue,
            Values = snapshots
        };
    }

    public async Task<BaseSnapshot<RisIdSnapshot>> GetRisIdSnapshotAsync(DateTimeOffset start, DateTimeOffset end)
    {
        var now = DateTime.UtcNow;

        var snapshots = await dataContext.RisIdSnapshots
            .AsNoTracking()
            .Where(snapshot => snapshot.MeasuredAt >= start.UtcDateTime && snapshot.MeasuredAt <= end.UtcDateTime)
            .OrderBy(snapshot => snapshot.MeasuredAt)
            .ToListAsync();

        var previousEntry = await dataContext.RisIdSnapshots
            .AsNoTracking()
            .Where(snapshot => snapshot.MeasuredAt < start.UtcDateTime)
            .OrderByDescending(snapshot => snapshot.MeasuredAt)
            .FirstOrDefaultAsync();

        RisIdSnapshot? currentEstimate = null;
        if (start <= now && now <= end)
        {
            var estimate = await EstimateCurrentRisIdsAsync();
            if (estimate.HasValue)
            {
                currentEstimate = new RisIdSnapshot()
                {
                    MeasuredAt = now,
                    Active = estimate.Value.Active,
                    Inactive = estimate.Value.Inactive
                };
                snapshots.Add(currentEstimate);
            }
        }

        long endingValue;
        if (currentEstimate is not null) endingValue = currentEstimate.Active + currentEstimate.Inactive;
        else if (snapshots.Any()) endingValue = snapshots.Last().Active + snapshots.Last().Inactive;
        else
        {
            var estimate = await EstimateCurrentRisIdsAsync();
            endingValue = (estimate?.Active ?? 0) + (estimate?.Inactive ?? 0);
        }

        return new BaseSnapshot<RisIdSnapshot>()
        {
            StartedWith = (previousEntry?.Active + previousEntry?.Inactive) ?? 0,
            Total = endingValue,
            Values = snapshots
        };
    }

    public async Task<BaseSnapshot<JourneySnapshot>> GetJourneySnapshotAsync(DateTimeOffset start, DateTimeOffset end)
    {
        var now = DateTime.UtcNow;

        var snapshots = await dataContext.JourneySnapshots
            .AsNoTracking()
            .Where(snapshot => snapshot.MeasuredAt >= start.UtcDateTime && snapshot.MeasuredAt <= end.UtcDateTime)
            .OrderBy(snapshot => snapshot.MeasuredAt)
            .ToListAsync();

        var previousEntry = await dataContext.JourneySnapshots
            .AsNoTracking()
            .Where(snapshot => snapshot.MeasuredAt < start.UtcDateTime)
            .OrderByDescending(snapshot => snapshot.MeasuredAt)
            .FirstOrDefaultAsync();

        JourneySnapshot? currentEstimate = null;
        if (start <= now && now <= end)
        {
            currentEstimate = new JourneySnapshot()
            {
                MeasuredAt = now,
                Total = await EstimateCurrentJourneysAsync() ?? 0
            };
            snapshots.Add(currentEstimate);
        }

        long endingValue;
        if (currentEstimate is not null) endingValue = currentEstimate.Total;
        else if (snapshots.Any()) endingValue = snapshots.Last().Total;
        else endingValue = await EstimateCurrentJourneysAsync() ?? 0;

        return new BaseSnapshot<JourneySnapshot>()
        {
            StartedWith = previousEntry?.Total ?? 0,
            Total = endingValue,
            Values = snapshots
        };
    }

    public async Task SaveDatabaseSizeAsync(DatabaseSize databaseSize)
    {
        await dataContext.DatabaseSizes.AddAsync(databaseSize);
        await dataContext.SaveChangesAsync();
    }

    public async Task SaveRisIdSnapshotAsync(RisIdSnapshot risIdSnapshot)
    {
        await dataContext.RisIdSnapshots.AddAsync(risIdSnapshot);
        await dataContext.SaveChangesAsync();
    }

    public async Task SaveJourneySnapshotAsync(JourneySnapshot journeySnapshot)
    {
        await dataContext.JourneySnapshots.AddAsync(journeySnapshot);
        await dataContext.SaveChangesAsync();
    }
}
