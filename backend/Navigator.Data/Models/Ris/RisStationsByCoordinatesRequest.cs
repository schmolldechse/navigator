using System.Text.Json.Serialization;

namespace Navigator.Data.Models.Ris;

public class RisStationsByCoordinatesRequest
{
    [JsonPropertyName("latitude")]
    public required double Latitude { get; set; }
    [JsonPropertyName("longitude")]
    public required double Longitude { get; set; }
    [JsonPropertyName("radius")]
    public required int Radius { get; set; }
    [JsonPropertyName("groupBy")]
    public required RisStations.StopPlaceSearchGroupByKey GroupBy { get; set; }
    [JsonPropertyName("limit")]
    public int? Limit { get; set; } = 10_000;
}
