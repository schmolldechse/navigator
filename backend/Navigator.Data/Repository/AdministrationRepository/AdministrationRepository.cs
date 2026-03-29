using Microsoft.EntityFrameworkCore;
using Navigator.Data.Entities.Journey;

namespace Navigator.Data.Repository.AdministrationRepository;

public class AdministrationRepository(DataContext dataContext) : IAdministrationRepository
{
    public async Task<IEnumerable<Administration>> GetOrCreateAdministrationsAsync(IEnumerable<Administration> administrations)
    {
        if (!administrations.Any()) return Enumerable.Empty<Administration>();

        var administrationIds = administrations
            .Select(administration => administration.AdministrationId)
            .Distinct()
            .ToList();

        var candidates = await dataContext.Administrations
            .Where(dbAdmin => administrationIds.Contains(dbAdmin.AdministrationId))
            .ToListAsync();

        // client-side matching
        // 3. Match against the full composite key in-memory (Client-side)
        var results = new List<Administration>();
        var distinctIncoming = administrations
            .DistinctBy(a => new { a.AdministrationId, a.OperatorCode, a.OperatorName })
            .ToList();

        foreach (var incoming in distinctIncoming)
        {
            var existing = candidates.FirstOrDefault(c =>
                c.AdministrationId == incoming.AdministrationId &&
                c.OperatorCode == incoming.OperatorCode &&
                c.OperatorName == incoming.OperatorName);

            if (existing != null) results.Add(existing);
            else
            {
                var entry = await dataContext.Administrations.AddAsync(incoming);
                results.Add(entry.Entity);
            }
        }

        await dataContext.SaveChangesAsync();
        return results;
    }
}
