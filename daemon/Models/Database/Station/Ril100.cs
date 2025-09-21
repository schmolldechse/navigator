using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace daemon.Models.Database.Station;

public class Ril100
{
	[Key]
	[Column("id")]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int Id { get; init; }

	[Column("eva_number")]
	public int EvaNumber { get; init; }

	[ForeignKey(nameof(EvaNumber))]
	public virtual Station Station { get; init; } = null!;

	[Column("ril100")]
	[MaxLength(64)]
	public required string Ril100Identifier { get; init; }
}
