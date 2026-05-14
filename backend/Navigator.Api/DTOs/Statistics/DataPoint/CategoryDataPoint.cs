using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Statistics.DataPoint;

public class CategoryDataPoint : BaseMetricDataPoint
{
    [JsonPropertyName("category")]
    public required string Category { get; set; }
}
