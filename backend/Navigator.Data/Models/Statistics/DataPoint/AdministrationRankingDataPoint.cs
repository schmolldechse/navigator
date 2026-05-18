using System.Text.Json.Serialization;
using Navigator.Data.Models.Statistics.Subject;

namespace Navigator.Data.Models.Statistics.DataPoint;

public class AdministrationRankingDataPoint : BaseMetricDataPoint
{
    [JsonPropertyName("administration")]
    public required AdministrationMetricSubject Administration { get; set; }
}
