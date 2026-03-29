using Navigator.Data.Entities.Journey;

namespace Navigator.Data.Repository.AdministrationRepository;

public interface IAdministrationRepository
{
    Task<IEnumerable<Administration>> GetOrCreateAdministrationsAsync(IEnumerable<Administration> administrations);
}
