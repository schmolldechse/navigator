using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace stations.Models.Database;

public class Station
{
    [Key]
    public required int EvaNumber { get; set; }
    [MaxLength(512)]
    public required string Name { get; set; }
    public List<string>? Ril100 { get; set; }
    public required List<string> Products { get; set; }
    
    private double _latitude { get; set; }
    private double _longitude { get; set; }

    [NotMapped]
    public Coordinates? Coordinates
    {
        get => new() { Latitude = _latitude, Longitude = _longitude };
        set
        {
            if (value == null) return;
            _latitude = value.Latitude;
            _longitude = value.Longitude;
        }
    }
    
    // Parameters used for the Daemon
    public bool? QueryingEnabled { get; set; }
    public DateTime? LastQueried { get; set; }
}

public class Coordinates
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

