using Microsoft.EntityFrameworkCore;
using Navigator.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.RisId;

[Table("ris_ids", Schema = "core")]
[Index(nameof(TransportType))]
[Index(nameof(ReplacementTransportType))]
[Index(nameof(DiscoveredAt))]
[Index(nameof(LastInserted))]
[Index(nameof(Active), nameof(LastSeen))]
public class RisId
{
    /// This is because RIS IDs can have a UUID-UUID format.
    [Key]
    [Column("id")]
    [MaxLength(73)]
    public required string Id { get; set; }

    [Column("transport_type")]
    public required TransportType TransportType { get; set; }

    [Column("replacement_transport_type")]
    public TransportType? ReplacementTransportType { get; set; } = null;

    [Column("discovered_at")]
    public required DateTime DiscoveredAt { get; set; }

    [Column("last_seen")]
    public DateTime? LastSeen { get; set; }

    [Column("last_inserted")]
    public DateTime? LastInserted { get; set; }

    [Column("active")]
    public required bool Active { get; set; }
}
