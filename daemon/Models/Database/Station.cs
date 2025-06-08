using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace daemon.Models.Database;

[Index(nameof(EvaNumber), IsUnique = true)]
public class Station
{
    [Key] [Column("eva_number")] public required int EvaNumber { get; set; }

    [MaxLength(512)] [Column("name")] public required string Name { get; set; }

    [Column("weight")] public double Weight { get; set; } = 0;

    public virtual ICollection<Ril100> Ril100 { get; set; } = new List<Ril100>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual required Coordinates Coordinates { get; set; }

    [Column("querying_enabled")] public bool? QueryingEnabled { get; set; }

    [Column("last_queried")] public DateTime? LastQueried { get; set; }
}
