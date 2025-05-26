using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace stations.Models.Database;

[Index(nameof(EvaNumber), IsUnique = true)]
public class Station
{
    [Key]
    public required int EvaNumber { get; set; }
    [MaxLength(512)]
    public required string Name { get; set; }
    public List<string> Ril100 { get; set; } = new();
    public required List<string> Products { get; set; } = new();
    
    public Coordinates? Coordinates { get; set; }
    
    // Parameters used for the Daemon
    public bool? QueryingEnabled { get; set; }
    public DateTime? LastQueried { get; set; }
}

public class Coordinates
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    // Foreign key to Station
    [ForeignKey("EvaNumber")]
    public int EvaNumber { get; set; }
    public Station Station { get; set; } = null!;
    
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

