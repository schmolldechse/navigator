using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace daemon.Models.Database.Station;

public class TransportOccurence
{
	[Key]
	[Column("id")]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int Id { get; init; }

	[Column("eva_number")]
	public int EvaNumber { get; init; }

	[ForeignKey(nameof(EvaNumber))]
	public virtual Station Station { get; init; } = null!;

	[Column("transport_name")]
	public required TransportType TransportType { get; init; }

	[Column("querying_enabled")]
	public bool QueryingEnabled { get; init; }
}
