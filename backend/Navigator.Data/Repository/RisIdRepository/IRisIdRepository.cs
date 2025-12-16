using Navigator.Data.Entities.RisId;
using Navigator.Data.Models.RisId;
using System.ComponentModel.DataAnnotations;

namespace Navigator.Data.Repository.RisIdRepository;

public interface IRisIdRepository
{
    Task<RisId?> GetRisIdAsync([MaxLength(82)] string id);
    Task<IEnumerable<RisId>> GetRisIdsBatchAsync(ShuffledRisIdRequest request);
    Task<IEnumerable<RisId>> GetRisIdsBatchAsync(IEnumerable<string> ids);
    Task SaveRisIdsBatchAsync(IEnumerable<RisId> risIds);
}
