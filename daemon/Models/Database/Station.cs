using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace daemon.Models.Database;

[Index(nameof(EvaNumber), IsUnique = true)]
public class Station
{
    [Key] [Column("eva_number")] public required int EvaNumber { get; set; }

    [MaxLength(512)] [Column("name")] public required string Name { get; set; }

    public virtual ICollection<Ril100> Ril100 { get; set; } = new List<Ril100>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public Coordinates? Coordinates { get; set; }

    [Column("querying_enabled")] public bool? QueryingEnabled { get; set; }

    [Column("last_queried")] public DateTime? LastQueried { get; set; }
}

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

public class Product
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("eva_number")] public int EvaNumber { get; set; }

    [ForeignKey(nameof(EvaNumber))] public virtual Station Station { get; set; } = null!;

    [Column("name")] [MaxLength(32)] public required string ProductName { get; set; }

    [Column("querying_enabled")] public bool QueryingEnabled { get; set; }
    
    private static readonly Dictionary<string, string[]> ProductDict = new()
    {
        { "Hochgeschwindigkeitszuege", new[] { "HIGH_SPEED_TRAIN", "nationalExpress" } },
        { "IntercityUndEurocityZuege", new[] { "INTERCITY_TRAIN", "national" } },
        { "InterregioUndSchnellzuege", new[] { "INTER_REGIONAL_TRAIN", "regionalExpress" } },
        { "NahverkehrsonstigeZuege", new[] { "REGIONAL_TRAIN", "regional" } },
        { "Sbahnen", new[] { "CITY_TRAIN", "suburban" } },
        { "Busse", new[] { "BUS" } },
        { "Schiffe", new[] { "FERRY" } },
        { "UBahn", new[] { "SUBWAY" } },
        { "Strassenbahn", new[] { "TRAM" } },
        { "AnrufpflichtigeVerkehre", new[] { "SHUTTLE", "taxi" } },
        { "Unknown", new string[] { } }
    };
    
    public static string MapProduct(string? transport)
    {
        if (string.IsNullOrEmpty(transport)) return "Unknown";
        foreach (var pair in ProductDict)
        {
            if (pair.Value.Any(value => string.Equals(value, transport, StringComparison.OrdinalIgnoreCase)))
                return pair.Key;
        }
        return "Unknown";
    }
}

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