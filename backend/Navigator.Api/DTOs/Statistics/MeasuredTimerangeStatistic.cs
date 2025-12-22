using Navigator.Api.Enums;
using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Statistics;

public class MeasuredTimerangeStatistic
{
    [JsonPropertyName("unit")]
    public required StatisticUnit Unit { get; set; }

    [JsonPropertyName("timerange")]
    public required Timerange Timerange { get; set; }

    [JsonPropertyName("values")]
    public required IEnumerable<MeasuredStatisticValue> Values { get; set; }

    [JsonPropertyName("startedWith")]
    public required long StartedWith { get; set; }

    [JsonPropertyName("changedBy")]
    public required long ChangedBy { get; set; }

    [JsonPropertyName("total")]
    public required long Total { get; set; }
}
