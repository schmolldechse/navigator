using Navigator.Data.Entities.Journey;
using Navigator.Data.Models.Statistics;

namespace Navigator.Data.Repository.StatisticsRepository;

public class JourneyEstimationResult : BaseEstimationResult<Journey>
{
    public override required int StartedWith { get; set; }
    public override required int Total { get; set; }
    public override required IEnumerable<Journey> Values { get; set; }
}