using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Statistics;

public class MetricPage
{
    [JsonPropertyName("offset")]
    public required int Offset { get; set; }

    [JsonPropertyName("limit")]
    public required int Limit { get; set; }

    [JsonPropertyName("hasMore")]
    public required bool HasMore { get; set; }

    [JsonPropertyName("totalItems")]
    public required int TotalItems { get; set; }

    [JsonPropertyName("totalPages")]
    public required int TotalPages { get; set; }
}
