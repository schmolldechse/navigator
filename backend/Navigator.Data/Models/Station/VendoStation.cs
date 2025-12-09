using System.Text.Json.Serialization;

namespace Navigator.Data.Models.Station;

public class VendoStation
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("stationId")]
    public string? StationId { get; set; }

    [JsonPropertyName("locationId")]
    public required string LocationId { get; set; }

    [JsonPropertyName("evaNr")]
    public required string EvaNumber { get; set; }

    [JsonPropertyName("coordinates")]
    public required CoordinatesResponse Coordinates { get; set; }

    [JsonPropertyName("weight")]
    public int Weight { get; set; }

    [JsonPropertyName("products")]
    public required string[] Products { get; set; }

    [JsonPropertyName("locationType")]
    public required string LocationType { get; set; }

    public class CoordinatesResponse
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
    }
}