namespace Navigator.Data.Models.Station;

public class StationsByCoordinateRequest
{
    public required double Latitude { get; set; }
    public required double Longitude { get; set; }
    public int? Limit { get; set; } = 100;
    public double? MaxDistance { get; set; } = 1000.0;
}
