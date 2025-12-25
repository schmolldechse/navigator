using RisIdEntity = Navigator.Data.Entities.RisId.RisId;

namespace Navigator.Data.Models.Statistics;

public class RisIdEstimationResult
{
    public required int StartedWith { get; set; }

    public required int Total { get; set; }

    public required IEnumerable<RisIdEntity> RisIds { get; set; }
}
