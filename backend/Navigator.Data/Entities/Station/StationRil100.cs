using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Station;

public class StationRil100
{
    [Column("id")]
    public virtual Guid Id { get; set; }

    [Column("eva_number")]
    public required int EvaNumber { get; set; }

    [ForeignKey(nameof(EvaNumber))]
    public virtual Station? Station { get; set; }

    [Column("ril100")]
    public required string Ril100Code { get; set; }
}
