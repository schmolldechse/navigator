using System.ComponentModel.DataAnnotations;
using Navigator.Data.Enums;

namespace Navigator.Data.Models.RisId;

public class RisIdBatchRequest
{
    public required RisIdOrder OrderBy { get; set; } = RisIdOrder.Random;
    public bool OrderByDescending { get; set; } = false;

    public required bool IncludeNullDates { get; set; } = false;

    public required bool OnlyActive { get; set; } = true;

    public DateTime? CutoffLastSeen { get; set; }
    public DateTime? CutoffDiscovered { get; set; }
    public DateTime? CutoffLastInserted { get; set; }

    [Range(1, 500)]
    public int Limit { get; set; } = 384;
}
