using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Station;

[Table("stations", Schema = "core")]
[Index(nameof(QueryingEnabled))]
[Index(nameof(LastQueried))]
public class Station
{
    [Key]
    [Column("eva_number")]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public required int EvaNumber { get; set; }

    [Column("name")]
    public required string Name { get; set; }

    [Column("weight")]
    public required double Weight { get; set; }

    [Column("latitude")]
    public required double Latitude { get; set; }

    [Column("longitude")]
    public required double Longitude { get; set; }

    [Column("querying_enabled")]
    public required bool QueryingEnabled { get; set; }

    [Column("last_queried")]
    public DateTime? LastQueried { get; set; }

    public virtual ICollection<StationRil100> Ril100 { get; set; } = new List<StationRil100>();

    public virtual ICollection<StationTransport> Transports { get; set; } = new List<StationTransport>();
}
