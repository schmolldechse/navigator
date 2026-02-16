using Navigator.Data.Enums;
using Navigator.Data.Enums.Metric;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Statistics.Request;

public class HourlyStopSummaryMetricRequest : BaseMetricRequest
{
    [JsonIgnore]
    public override MetricQueryType MetricQueryType => MetricQueryType.HourlyStopSummary;

    [JsonPropertyName("start")]
    public required DateTimeOffset Start { get; set; }

    [JsonPropertyName("end")]
    public required DateTimeOffset End { get; set; }

    [JsonPropertyName("transportTypes")]
    [Description("Optional list of transport types to filter the metric by. If not provided, all transport types will be included.")]
    public TransportType[]? TransportTypes { get; set; }

    public override Navigator.Data.Models.Statistics.BaseMetricRequest BuildRequest() => new Navigator.Data.Models.Statistics.Request.HourlyStopSummaryMetricRequest()
    {
        Start = Start,
        End = End,
        TransportTypes = TransportTypes ?? Array.Empty<TransportType>()
    };

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Start > End)
        {
            yield return new ValidationResult(
                "Start time must be earlier than or equal to end time.",
                new[] { nameof(Start), nameof(End) });
        }
    }
}
