using Microsoft.EntityFrameworkCore;
using Navigator.Data.Entities.Journey;

namespace Navigator.Data.Repository.AdministrationRepository;

public class AdministrationRepository(DataContext dataContext) : IAdministrationRepository
{
    public async Task<Administration?> GetAdministrationAsync(string administrationId, string operatorCode, string operatorName) => await dataContext.Administrations
        .FirstOrDefaultAsync(administration => administration.AdministrationId == administrationId
            && administration.OperatorCode == operatorCode
            && administration.OperatorName == operatorName);

    public async Task SaveAdministrationAsync(Administration administration)
    {
        if (await GetAdministrationAsync(administration.AdministrationId, administration.OperatorCode, administration.OperatorName) != null) return;
        await dataContext.Administrations.AddAsync(administration);
        await dataContext.SaveChangesAsync();
    }
}
