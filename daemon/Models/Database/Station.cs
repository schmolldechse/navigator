using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace daemon.Models.Database;

[Index(nameof(EvaNumber), IsUnique = true)]
public class Station
{
    [Key]
    [Column("eva_number")]
    public required int EvaNumber { get; set; }
    
    [MaxLength(512)]
    [Column("name")]
    public required string Name { get; set; }
    
    [Column("ril100")]
    public required List<string> Ril100 { get; set; } = new();
    
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    
    public Coordinates? Coordinates { get; set; }
    
    [Column("querying_enabled")]
    public bool? QueryingEnabled { get; set; }
    
    [Column("last_queried")]
    public DateTime? LastQueried { get; set; }
}

public class Coordinates
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Column("eva_number")]
    public int EvaNumber { get; set; }
    
    [ForeignKey(nameof(EvaNumber))]
    public virtual Station Station { get; set; } = null!;
    
    [Column("latitude")]
    public double? Latitude { get; set; }
    
    [Column("longitude")]
    public double? Longitude { get; set; }
}

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
    public required string ProductName { get; set; }
    
    [Column("querying_enabled")]
    public bool? QueryingEnabled { get; set; }
}