using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace daemon.Models.Database;

public class Coordinates
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("eva_number")] public int EvaNumber { get; set; }

    [ForeignKey(nameof(EvaNumber))] public virtual Station Station { get; set; } = null!;

    [Column("latitude")] public double? Latitude { get; set; }

    [Column("longitude")] public double? Longitude { get; set; }
}
