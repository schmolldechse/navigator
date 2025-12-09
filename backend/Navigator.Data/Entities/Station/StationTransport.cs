using Microsoft.EntityFrameworkCore;
using Navigator.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Station;

[Table("station_transports", Schema = "core")]
[Index(nameof(EvaNumber), nameof(TransportType), IsUnique = true)]
public class StationTransport
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("eva_number")]
    public required int EvaNumber { get; set; }

    [ForeignKey(nameof(EvaNumber))]
    public Station? Station { get; set; }

    [Column("transport")]
    public required TransportType TransportType { get; set; }

    [Column("enabled")]
    public required bool Enabled { get; set; }
}
