using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace daemon.Models.Database;

public class Ril100
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("eva_number")] public int EvaNumber { get; set; }

    [ForeignKey(nameof(EvaNumber))] public virtual Station Station { get; set; } = null!;

    [Column("ril100")] [MaxLength(32)] public required string Ril100Identifier { get; set; }
}
