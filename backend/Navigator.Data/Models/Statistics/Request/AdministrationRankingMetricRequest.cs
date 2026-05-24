using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Navigator.Data.Enums;
using Navigator.Data.Enums.Metric;

namespace Navigator.Data.Models.Statistics.Request;

public class AdministrationRankingMetricRequest : BaseMetricRequest
{
    private static readonly HashSet<MetricSeriesType> ValidSeriesTypes =
    [
        MetricSeriesType.AdministrationRankingCount,
        MetricSeriesType.AdministrationRankingCancellationCount,
        MetricSeriesType.AdministrationRankingCancellationRate,
        MetricSeriesType.AdministrationRankingAverageDelay,
        MetricSeriesType.AdministrationRankingPunctuality5Rate,
        MetricSeriesType.AdministrationRankingPunctuality15Rate,
    ];

    [JsonPropertyName("seriesType")]
    [AllowedValues(
        MetricSeriesType.AdministrationRankingCount,
        MetricSeriesType.AdministrationRankingCancellationCount,
        MetricSeriesType.AdministrationRankingCancellationRate,
        MetricSeriesType.AdministrationRankingAverageDelay,
        MetricSeriesType.AdministrationRankingPunctuality5Rate,
        MetricSeriesType.AdministrationRankingPunctuality15Rate
    )]
    [Description("The administration ranking metric to retrieve.")]
    public override required MetricSeriesType SeriesType { get; set; }

    [JsonPropertyName("start")]
    public required DateTimeOffset Start { get; set; }

    [JsonPropertyName("end")]
    public required DateTimeOffset End { get; set; }

    [JsonPropertyName("evaNumbers")]
    [Description("Optional array of EVA numbers. If provided, station event quality is ranked instead of global journey route quality.")]
    public int[] EvaNumbers { get; set; } = [];

    [JsonPropertyName("transportTypes")]
    [Description("Optional list of transport types to filter the ranking by. If omitted or empty, all transport types will be included.")]
    public TransportType[]? TransportTypes { get; set; } = [];

    [JsonPropertyName("includeReplacementTransport")]
    [Description("Whether replacement transport should be included in the metric.")]
    public bool IncludeReplacementTransport { get; set; } = true;

    [JsonPropertyName("limit")]
    [Range(1, 500)]
    public int Limit { get; set; } = 500;

    [JsonPropertyName("offset")]
    [Range(0, int.MaxValue)]
    public int Offset { get; set; } = 0;

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
