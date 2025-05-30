using System.ComponentModel.DataAnnotations.Schema;

namespace daemon.Models.Database;

public class Coordinates
{
    [Column("latitude")] public double? Latitude { get; set; }

    [Column("longitude")] public double? Longitude { get; set; }
}
