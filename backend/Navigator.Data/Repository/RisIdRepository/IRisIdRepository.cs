using Navigator.Data.Entities.RisId;
using Navigator.Data.Models.RisId;

namespace Navigator.Data.Repository.RisIdRepository;

public interface IRisIdRepository
{
    Task<RisId?> GetRisIdAsync(Guid id);
    Task<IEnumerable<RisId>> GetRisIdsBatchAsync(ShuffledRisIdRequest request);
    Task SaveRisIdsBatchAsync(IEnumerable<RisId> risIds);
}
