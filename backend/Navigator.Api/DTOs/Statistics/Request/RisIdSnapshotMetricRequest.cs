using Navigator.Data.Enums.Metric;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Statistics.Request;

public class RisIdSnapshotMetricRequest : BaseMetricRequest
{
    private static readonly HashSet<MetricSeriesType> ValidSeriesTypes =
    [
        MetricSeriesType.RisIdsActive,
        MetricSeriesType.RisIdsInactive
    ];

    [JsonPropertyName("seriesType")]
    [AllowedValues(MetricSeriesType.RisIdsActive, MetricSeriesType.RisIdsInactive)]
    [Description("The specific RIS ID metric series to retrieve. Must be one of: RisIdsActive, RisIdsInactive.")]
    public required MetricSeriesType SeriesType { get; set; }

    [JsonPropertyName("start")]
    public required DateTimeOffset Start { get; set; }

    [JsonPropertyName("end")]
    public required DateTimeOffset End { get; set; }

    public override Navigator.Data.Models.Statistics.BaseMetricRequest BuildRequest() => new Navigator.Data.Models.Statistics.Request.RisIdSnapshotMetricRequest(SeriesType)
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

        if (!ValidSeriesTypes.Contains(SeriesType))
        {
            yield return new ValidationResult(
                $"SeriesType must be one of: {string.Join(", ", ValidSeriesTypes)}.",
                new[] { nameof(SeriesType) });
        }
    }
}
