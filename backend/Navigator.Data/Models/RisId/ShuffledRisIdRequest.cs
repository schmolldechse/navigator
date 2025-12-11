using System.ComponentModel.DataAnnotations;

namespace Navigator.Data.Models.RisId;

public class ShuffledRisIdRequest
{
    public required bool OnlyIncludeActive { get; set; } = true;
    public required DateTime LastSeen { get; set; } = DateTime.UtcNow;

    [Range(1, 500)]
    public required int Limit { get; set; } = 384;
}
