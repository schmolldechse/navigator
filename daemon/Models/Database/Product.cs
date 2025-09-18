using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace daemon.Models.Database;

public class Product
{
	[Key]
	[Column("id")]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int Id { get; set; }

	[Column("eva_number")]
	public int EvaNumber { get; set; }

	[ForeignKey(nameof(EvaNumber))]
	public virtual Station Station { get; set; } = null!;

	[Column("name")]
	[MaxLength(32)]
	public required string ProductName { get; set; }

	[Column("querying_enabled")]
	public bool QueryingEnabled { get; set; }
}
