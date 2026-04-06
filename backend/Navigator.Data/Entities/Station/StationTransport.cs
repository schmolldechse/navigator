using Navigator.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Station;

public class StationTransport
{
    [Column("id")]
    public virtual Guid Id { get; set; }

    [Column("eva_number")]
    public required int EvaNumber { get; set; }

    [ForeignKey(nameof(EvaNumber))]
    public Station? Station { get; set; }

    [Column("transport_type")]
    public required TransportType TransportType { get; set; }

    [Column("enabled")]
    public required bool Enabled { get; set; }
}
