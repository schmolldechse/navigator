using Navigator.Data.Enums;
using Navigator.Data.Enums.Metric;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Statistics.Request;

public class StationSummaryMetricRequest : BaseMetricRequest
{

    [JsonIgnore]
    public override MetricQueryType MetricQueryType => MetricQueryType.TotalStationSnapshot;

    [JsonPropertyName("snapshot")]
    public required StationSnapshotType SnapshotType { get; set; }

    [JsonPropertyName("start")]
    public DateTimeOffset? Start { get; set; }

    [JsonPropertyName("end")]
    public required DateTimeOffset End { get; set; }

    [JsonPropertyName("transportTypes")]
    [Description("Optional list of transport types to filter the metric by. If not provided, all transport types will be included.")]
    public TransportType[]? TransportTypes { get; set; }

    [JsonPropertyName("evaNumbers")]
    [Description("Optional list of EVA numbers to filter the metric by stations. If not provided, all stations will be included.")]
    public int[]? EvaNumbers { get; set; }

    public override Navigator.Data.Models.Statistics.BaseMetricRequest BuildRequest() => new Navigator.Data.Models.Statistics.Request.StationSummaryMetricRequest()
    {
        SnapshotType = SnapshotType,
        Start = Start,
        End = End,
        TransportTypes = TransportTypes,
        EvaNumbers = EvaNumbers
    };

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Start.HasValue && Start.Value > End)
        {
            yield return new ValidationResult(
                "Start time must be earlier than or equal to end time.",
                new[] { nameof(Start), nameof(End) });
        }
    }
}
