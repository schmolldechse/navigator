using System.Text.Json.Serialization;

namespace Navigator.Data.Models.Statistics;

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

    public static MetricPage Create(int offset, int limit, int totalItems) => new()
    {
        Offset = offset,
        Limit = limit,
        HasMore = offset + limit < totalItems,
        TotalItems = totalItems,
        TotalPages = (int)Math.Ceiling((decimal)totalItems / limit)
    };
}
