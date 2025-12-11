using Microsoft.EntityFrameworkCore;

namespace Navigator.Data.Repository.StationRilRepository;

public class StationRilRepository(DataContext dataContext) : IStationRilRepository
{
    public async Task<ILookup<int, string>> GetRilByEvaNumbersAsync(int[] evaNumbers)
    {
        var ril100 = await dataContext.StationRil
            .Where(ril => evaNumbers.Contains(ril.EvaNumber))
            .Select(ril => new { ril.EvaNumber, ril.Ril100Code })
            .ToListAsync();
        return ril100.ToLookup(key => key.EvaNumber, value => value.Ril100Code);
    }
}