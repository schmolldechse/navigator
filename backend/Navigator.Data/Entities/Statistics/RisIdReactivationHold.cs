using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Statistics;

public sealed class RisIdReactivationHold
{
    [Column("ris_id")]
    public required string RisId { get; set; }

    [Column("reactivated_at")]
    public required DateTime ReactivatedAt { get; set; }

    [Column("protect_until")]
    public required DateTime ProtectUntil { get; set; }

    [Column("activation_count")]
    public required int ActivationCount { get; set; }

    [Column("last_seen_at_reactivation")]
    public DateTime? LastSeenAtReactivation { get; set; }

    [Column("last_inserted_at_reactivation")]
    public DateTime? LastInsertedAtReactivation { get; set; }
}
