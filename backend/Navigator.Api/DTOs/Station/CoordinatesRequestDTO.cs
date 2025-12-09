using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Station;


/// <summary>
/// Represents a request to find stations near specific geographic coordinates.
/// </summary>
[Description("Represents a request to find stations near specific geographic coordinates.")]
public class CoordinatesRequestDTO
{
    [JsonPropertyName("latitude")]
    [Required(ErrorMessage = "Latitude is required")]
    [Description("The latitude of the location.")]
    public required double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    [Required(ErrorMessage = "Longitude is required")]
    [Description("The longitude of the location.")]
    public required double Longitude { get; set; }

    [JsonPropertyName("limit")]
    [Description("The maximum number of stations to return. Defaults to 100.")]
    [DefaultValue(100)]
    public int? Limit { get; set; } = 100;

    [JsonPropertyName("maxDistance")]
    [Description("The maximum distance in meters to search for stations. Defaults to 1000.")]
    [DefaultValue(1000.0)]
    public double? MaxDistance { get; set; } = 1000.0;
}
