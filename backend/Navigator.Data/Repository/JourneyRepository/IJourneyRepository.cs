using System.ComponentModel.DataAnnotations;
using Navigator.Data.Entities.Journey;
using Navigator.Data.Models.Journey;
using Navigator.Data.Models.Ris;

namespace Navigator.Data.Repository.JourneyRepository;

public interface IJourneyRepository
{
    Task<RisJourneys.JourneyEventBased?> GetJourneyAsync([MaxLength(73)] string journeyId);
    Task<RisJourneys.JourneyBatchResponse?> GetJourneysBatchAsync(IEnumerable<JourneyOnDateRequest> request);
    Task SaveJourneysBatchAsync(IEnumerable<Journey> journeys);
}
