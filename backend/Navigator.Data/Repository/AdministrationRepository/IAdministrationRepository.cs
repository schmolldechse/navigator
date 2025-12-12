using Navigator.Data.Entities.Journey;

namespace Navigator.Data.Repository.AdministrationRepository;

public interface IAdministrationRepository
{
    Task<Administration> GetOrCreateAdministrationAsync(string administrationId, string operatorCode, string operatorName);
}
