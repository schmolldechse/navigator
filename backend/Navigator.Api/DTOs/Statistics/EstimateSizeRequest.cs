using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Statistics;

/// <summary>
/// Represents a request to estimate the size of data within a specified time range.
/// </summary>
public class EstimateSizeRequest
{
    [JsonPropertyName("start")]
    [Description("The start time of the range for which to estimate size.")]
    public required DateTimeOffset Start { get; set; }

    [JsonPropertyName("end")]
    [Description("The start time of the range for which to estimate size.")]
    public required DateTimeOffset End { get; set; }
}
