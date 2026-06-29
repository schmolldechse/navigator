using Microsoft.EntityFrameworkCore;
using Navigator.Data.Entities;
using Navigator.Data.Enums;
using Navigator.Data.Models.RisId;

namespace Navigator.Data.Repository.RisIdRepository;

public class RisIdRepository(DataContext dataContext) : IRisIdRepository
{
    public async Task<RisId?> GetRisIdAsync(string id) => await dataContext.RisIds
        .Where(risId => risId.Id == id)
        .FirstOrDefaultAsync();

    public async Task<IEnumerable<RisId>> GetRisIdsBatchAsync(RisIdBatchRequest request)
    {
        var query = dataContext.RisIds.AsQueryable();
        if (request.OnlyActive) query = query.Where(risId => risId.Active);
        if (request.CutoffLastSeen.HasValue)
        {
            if (request.IncludeNullDates) query = query.Where(risId => risId.LastSeen == null || risId.LastSeen < request.CutoffLastSeen.Value);
            else query = query.Where(risId => risId.LastSeen < request.CutoffLastSeen.Value);
        }
        if (request.CutoffDiscovered.HasValue) query = query.Where(risId => risId.DiscoveredAt < request.CutoffDiscovered.Value);
        if (request.CutoffLastInserted.HasValue)
        {
            if (request.IncludeNullDates) query = query.Where(risId => risId.LastInserted == null || risId.LastInserted < request.CutoffLastInserted.Value);
            else query = query.Where(risId => risId.LastInserted < request.CutoffLastInserted.Value);
        }

        query = (request.OrderBy, request.OrderByDescending) switch
        {
            (RisIdOrder.Random, _) => query.OrderBy(x => Guid.NewGuid()),

            (RisIdOrder.LastSeen, false) => query.OrderBy(risId => risId.LastSeen ?? DateTime.MinValue),
            (RisIdOrder.LastSeen, true) => query.OrderByDescending(risId => risId.LastSeen ?? DateTime.MinValue),

            (RisIdOrder.LastInserted, false) => query.OrderBy(risId => risId.LastInserted ?? DateTime.MinValue),
            (RisIdOrder.LastInserted, true) => query.OrderByDescending(risId => risId.LastInserted ?? DateTime.MinValue),

            (RisIdOrder.DiscoveryDate, false) => query.OrderBy(risId => risId.DiscoveredAt),
            (RisIdOrder.DiscoveryDate, true) => query.OrderByDescending(risId => risId.DiscoveredAt),

            (_, false) => query.OrderBy(risId => risId.LastSeen ?? DateTime.MinValue),
            (_, true) => query.OrderByDescending(risId => risId.LastSeen ?? DateTime.MinValue)
        };

        var risIds = await query
            .Take(15_000)
            .ToListAsync();
        return risIds.Take(request.Limit);
    }

    public async Task<IEnumerable<RisId>> GetRisIdsBatchAsync(IEnumerable<string> ids) => await dataContext.RisIds
        .Where(risId => ids.Contains(risId.Id))
        .ToListAsync();

    public async Task SaveRisIdsBatchAsync(IEnumerable<RisId> risIds)
    {
        var ids = risIds.Select(risId => risId.Id).ToList();
        var existingIds = await dataContext.RisIds
            .Where(risId => ids.Contains(risId.Id))
            .Select(risId => risId.Id)
            .ToHashSetAsync();

        var toUpdate = risIds.Where(risId => existingIds.Contains(risId.Id));
        var toInsert = risIds.Where(risId => !existingIds.Contains(risId.Id));

        if (toUpdate.Any()) dataContext.UpdateRange(toUpdate);
        if (toInsert.Any()) dataContext.AddRange(toInsert);

        await dataContext.SaveChangesAsync();
    }
}
