using Microsoft.EntityFrameworkCore;
using Navigator.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.RisId;

[Table("ris_ids", Schema = "core")]
[Index(nameof(TransportType))]
[Index(nameof(ReplacementTransportType))]
[Index(nameof(DiscoveredAt))]
[Index(nameof(LastSeen))]
[Index(nameof(LastInserted))]
[Index(nameof(Active))]
public class RisId
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public required Guid Id { get; set; }

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
