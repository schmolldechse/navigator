namespace Navigator.Data.Models.Station;

public class VendoStationsBySearchRequest
{
    public required string SearchTerm { get; set; }
    public int? MaxResults { get; set; } = 10;
    public string[]? LocationTypes { get; set; } = ["ALL"];
}
