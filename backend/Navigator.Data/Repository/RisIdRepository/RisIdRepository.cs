using Microsoft.EntityFrameworkCore;
using Navigator.Data.Entities.RisId;
using Navigator.Data.Models.RisId;
using System.Runtime.InteropServices;

namespace Navigator.Data.Repository.RisIdRepository;

public class RisIdRepository(DataContext dataContext) : IRisIdRepository
{
    public async Task<RisId?> GetRisIdAsync(string id) => await dataContext.RisIds
        .Where(risId => risId.Id == id)
        .FirstOrDefaultAsync();

    public async Task<IEnumerable<RisId>> GetRisIdsBatchAsync(ShuffledRisIdRequest request)
    {
        var risIds = await dataContext.RisIds
            .Where(risId => request.OnlyIncludeActive ? risId.Active : true)
            .Where(risId => risId.LastSeen == null || risId.LastSeen < request.LastSeen)
            .OrderBy(risId => risId.LastSeen ?? DateTime.MinValue)
            .Take(15_000)
            .ToListAsync();

        Random.Shared.Shuffle(CollectionsMarshal.AsSpan(risIds));
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
