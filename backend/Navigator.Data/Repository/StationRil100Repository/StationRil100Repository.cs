using Microsoft.EntityFrameworkCore;

namespace Navigator.Data.Repository.StationRil100Repository;

public class StationRil100Repository(DataContext dataContext) : IStationRil100Repository
{
    public async Task<ILookup<int, string>> GetRilByEvaNumbersAsync(int[] evaNumbers)
    {
        var requestedEvaNumbers = evaNumbers.Distinct().ToArray();
        var ril100 = await dataContext.StationRil
            .AsNoTracking()
            .Where(ril => requestedEvaNumbers.Contains(ril.EvaNumber))
            .Select(ril => new { ril.EvaNumber, ril.Ril100Code })
            .ToListAsync();

        return ril100.ToLookup(key => key.EvaNumber, value => value.Ril100Code);
    }

    public async Task<int[]> ExpandEvaNumbersByRil100Async(int[] evaNumbers)
    {
        var requestedEvaNumbers = evaNumbers.Distinct().ToArray();
        if (requestedEvaNumbers.Length == 0) return requestedEvaNumbers;

        var ril100Codes = await dataContext.StationRil
            .AsNoTracking()
            .Where(ril => requestedEvaNumbers.Contains(ril.EvaNumber))
            .Select(ril => ril.Ril100Code)
            .Distinct()
            .ToArrayAsync();

        if (ril100Codes.Length == 0) return requestedEvaNumbers;

        var relatedEvaNumbers = await dataContext.StationRil
            .AsNoTracking()
            .Where(ril => ril100Codes.Contains(ril.Ril100Code))
            .Select(ril => ril.EvaNumber)
            .Distinct()
            .ToArrayAsync();

        return requestedEvaNumbers
            .Concat(relatedEvaNumbers)
            .Distinct()
            .ToArray();
    }
}
