using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Station;

/// <summary>
/// Represents a request to search for stations using a search term, with optional filters for result count and location
/// types.
/// </summary>
[Description("Represents a request to search for stations using a search term, with optional filters for result count and location types.")]
public class StationBySerchtermRequest
{
    [JsonPropertyName("searchTerm")]
    [Required(ErrorMessage = "SearchTerm is required")]
    [Description("The term to search for in station names or codes.")]
    public required string SearchTerm { get; set; }

    [JsonPropertyName("maxResults")]
    [DefaultValue(10)]
    [Description("The maximum number of results to return. Defaults to 10 if not specified.")]
    public int? MaxResults { get; set; } = 10;

    [JsonPropertyName("locationTypes")]
    [DefaultValue(new[] { "ALL" })]
    [Description("An array of location types to filter the search results. Defaults to ['ALL'] if not specified.")]
    public string[]? LocationTypes { get; set; } = ["ALL"];
}
