using System.Text.Json.Serialization;
using Navigator.Api.DTOs.Statistics.Subject;

namespace Navigator.Api.DTOs.Statistics.DataPoint;

public class AdministrationRankingDataPoint : BaseMetricDataPoint
{
    [JsonPropertyName("administration")]
    public required AdministrationMetricSubject Administration { get; set; }
}
