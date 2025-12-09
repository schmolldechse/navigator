using Navigator.Data.Entities.RisId;

namespace Navigator.Data.Repository.RisIdRepository;

public interface IRisIdRepository
{
    Task<RisId> GetRisIdByIdAsync(Guid id);
}
