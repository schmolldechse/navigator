using System.Text.Json.Serialization;
using Navigator.Data.Models.Statistics.Subject;

namespace Navigator.Data.Models.Statistics.DataPoint;

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

    [JsonPropertyName("routeStartTime")]
    public DateTimeOffset? RouteStartTime { get; set; }

    [JsonPropertyName("routeEndTime")]
    public DateTimeOffset? RouteEndTime { get; set; }
}
