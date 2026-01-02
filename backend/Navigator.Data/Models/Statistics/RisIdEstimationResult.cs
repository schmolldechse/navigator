using RisIdEntity = Navigator.Data.Entities.RisId.RisId;

namespace Navigator.Data.Models.Statistics;

public class RisIdEstimationResult : BaseEstimationResult<RisIdEntity>
{
    public override required int StartedWith { get; set; }
    public override required int Total { get; set; }
    public override required IEnumerable<RisIdEntity> Values { get; set; }
}
