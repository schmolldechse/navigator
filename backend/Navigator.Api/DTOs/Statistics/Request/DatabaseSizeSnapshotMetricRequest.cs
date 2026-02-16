using Navigator.Data.Enums.Metric;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Statistics.Request;

public class DatabaseSizeSnapshotMetricRequest : BaseMetricRequest
{
    [JsonIgnore]
    public override MetricQueryType MetricQueryType => MetricQueryType.DatabaseSizeSnapshot;

    [JsonPropertyName("start")]
    public required DateTimeOffset Start { get; set; }

    [JsonPropertyName("end")]
    public required DateTimeOffset End { get; set; }

    public override Navigator.Data.Models.Statistics.BaseMetricRequest BuildRequest() => new Navigator.Data.Models.Statistics.Request.DatabaseSizeSnapshotMetricRequest()
    {
        Start = Start,
        End = End
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