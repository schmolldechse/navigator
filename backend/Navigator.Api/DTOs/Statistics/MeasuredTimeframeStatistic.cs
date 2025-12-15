using Navigator.Api.Enums;
using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Statistics;

public class MeasuredTimeframeStatistic
{
    [JsonPropertyName("unit")]
    public required StatisticUnit Unit { get; set; }

    [JsonPropertyName("timeframe")]
    public required Timeframe Timeframe { get; set; }

    [JsonPropertyName("values")]
    public required IEnumerable<MeasuredStatisticValue> Values { get; set; }

    [JsonPropertyName("startedWith")]
    public required long StartedWith { get; set; }

    [JsonPropertyName("changedBy")]
    public required long ChangedBy { get; set; }

    [JsonPropertyName("Total")]
    public required long Total { get; set; }
}
