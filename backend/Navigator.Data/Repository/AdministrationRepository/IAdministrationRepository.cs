using Navigator.Data.Entities.Journey;

namespace Navigator.Data.Repository.AdministrationRepository;

public interface IAdministrationRepository
{
    Task<Administration?> GetAdministrationAsync(string administrationId, string operatorCode, string operatorName);
    Task SaveAdministrationAsync(Administration administration);
}
