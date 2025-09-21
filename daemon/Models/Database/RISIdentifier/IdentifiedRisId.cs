using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace daemon.Models.Database.RISIdentifier;

[Index(nameof(Id), IsUnique = true)]
public class IdentifiedRisId
{
	[Key]
	[Column("id")]
	[MaxLength(128)]
	public required string Id { get; init; }

	[Column("transport_type")]
	public required TransportType TransportProduct { get; init; }
	
	[Column("replacement_transport_type")]
	public TransportType? ReplacementTransportProduct { get; init; }

	[Column("discovery_date")]
	public required DateTime DiscoveryDate { get; set; }

	[Column("last_seen")]
	public DateTime? LastSeen { get; set; }

	[Column("last_succeeded_at")]
	public DateTime? LastSucceededAt { get; set; }

	[Column("active")]
	public bool Active { get; set; } = true;

	[Column("is_locked")]
	public bool IsLocked { get; set; } = false;
}
