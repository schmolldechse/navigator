using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Navigator.Data.Enums.Metric;
using Navigator.Data.Infrastructure;

namespace Navigator.Data.Models.Statistics.Request;

public class LineRankingMetricRequest : BaseMetricRequest
{
    private static readonly HashSet<MetricSeriesType> ValidSeriesTypes =
    [
        MetricSeriesType.LineRankingCount,
        MetricSeriesType.LineRankingCancellationCount,
        MetricSeriesType.LineRankingCancellationRate,
        MetricSeriesType.LineRankingAverageDelay,
        MetricSeriesType.LineRankingPunctuality5Rate,
        MetricSeriesType.LineRankingPunctuality15Rate,
    ];

    [JsonPropertyName("seriesType")]
    [AllowedValues(
        MetricSeriesType.LineRankingCount,
        MetricSeriesType.LineRankingCancellationCount,
        MetricSeriesType.LineRankingCancellationRate,
        MetricSeriesType.LineRankingAverageDelay,
        MetricSeriesType.LineRankingPunctuality5Rate,
        MetricSeriesType.LineRankingPunctuality15Rate
    )]
    [Description("The line ranking metric to retrieve.")]
    public override required MetricSeriesType SeriesType { get; set; }

    [JsonPropertyName("start")]
    public required DateTimeOffset Start { get; set; }

    [JsonPropertyName("end")]
    public required DateTimeOffset End { get; set; }

    [JsonPropertyName("evaNumber")]
    [Description("Optional array of station EVA numbers. If provided, station event quality is ranked instead of global journey route quality.")]
    public int[] EvaNumber { get; set; } = [];

    [JsonPropertyName("line")]
    [Regex(
        @"^[\p{L}\p{N}\s_\-/\^\$\.\|\(\)\[\]\+\*\?\{\},]+$",
        Exception = "Line may only contain letters, digits, spaces and basic regex operators.",
        MaxLength = 64)]
    [Description("Optional regex filter for the line key.")]
    public string? Line { get; set; }

    [JsonPropertyName("number")]
    [Regex(
        @"^[0-9\s\^\$\.\|\(\)\[\]\+\*\?\{\},]+$",
        Exception = "Number may only contain digits and basic regex operators.",
        MaxLength = 64)]
    [Description("Optional regex filter for the journey or line number.")]
    public string? Number { get; set; }

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
