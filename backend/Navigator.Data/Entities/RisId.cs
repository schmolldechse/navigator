using Navigator.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities;

public class RisId
{
    // The format of a RIS ID is a UUID. In some cases, two UUIDs are concatenated together, separated by a hyphen.
    [Column("id")]
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
