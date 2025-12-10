using System.Text.Json.Serialization;

namespace Navigator.Data.Models.Station;

public class VendoStationsBySearchRequest
{
    [JsonPropertyName("searchTerm")]
    public required string SearchTerm { get; set; }
    [JsonPropertyName("maxResults")]
    public int? MaxResults { get; set; } = 10;
    [JsonPropertyName("locationTypes")]
    public string[]? LocationTypes { get; set; } = ["ALL"];
}
