using System.Text.Json.Serialization;
using Navigator.Api.DTOs.Statistics.Subject;

namespace Navigator.Api.DTOs.Statistics.DataPoint;

public class LineRankingDataPoint : BaseMetricDataPoint
{
    [JsonPropertyName("line")]
    public required LineMetricSubject Line { get; set; }

    [JsonPropertyName("administration")]
    public required AdministrationMetricSubject Administration { get; set; }

    [JsonPropertyName("startStation")]
    public required StationMetricSubject StartStation { get; set; }

    [JsonPropertyName("endStation")]
    public required StationMetricSubject EndStation { get; set; }
}
