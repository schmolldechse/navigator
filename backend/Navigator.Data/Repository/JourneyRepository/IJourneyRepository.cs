using Navigator.Data.Entities.Journey;
using Navigator.Data.Models.Journey;
using Navigator.Data.Models.Ris;

namespace Navigator.Data.Repository.JourneyRepository;

public interface IJourneyRepository
{
    Task<RisJourneys.JourneyBatchResponse?> GetJourneysBatchAsync(IEnumerable<JourneyOnDateRequest> request);
    Task SaveJourneysBatchAsync(IEnumerable<Journey> journeys);
}
