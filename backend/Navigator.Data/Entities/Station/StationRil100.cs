using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Station;


[Table("station_ril100", Schema = "core")]
public class StationRil100
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("eva_number")]
    public required int EvaNumber { get; set; }

    [ForeignKey(nameof(EvaNumber))]
    public virtual Station? Station { get; set; }

    [Column("ril100")]
    public required string Ril100Code { get; set; }
}
