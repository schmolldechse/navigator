using Microsoft.EntityFrameworkCore;
using Navigator.Data.Entities.Journey;

namespace Navigator.Data.Repository.AdministrationRepository;

public class AdministrationRepository(DataContext dataContext) : IAdministrationRepository
{
    public async Task<Administration> GetOrCreateAdministrationAsync(string administrationId, string operatorCode, string operatorName)
    {
        var administration = await dataContext.Administrations
            .FirstOrDefaultAsync(a => a.AdministrationId == administrationId && a.OperatorCode == operatorCode && a.OperatorName == operatorName);
        if (administration != null) return administration;

        administration = new Administration
        {
            AdministrationId = administrationId,
            OperatorCode = operatorCode,
            OperatorName = operatorName
        };

        await dataContext.Administrations.AddAsync(administration);
        await dataContext.SaveChangesAsync();
        return administration;
    }
}
