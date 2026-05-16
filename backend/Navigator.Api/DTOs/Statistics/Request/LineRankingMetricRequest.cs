using Navigator.Data.Enums.Metric;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Statistics.Request;

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
    public required MetricSeriesType SeriesType { get; set; }

    [JsonPropertyName("start")]
    public required DateTimeOffset Start { get; set; }

    [JsonPropertyName("end")]
    public required DateTimeOffset End { get; set; }

    [JsonPropertyName("evaNumber")]
    [Description("Optional array of station EVA numbers. If provided, station event quality is ranked instead of global journey route quality.")]
    public int[] EvaNumber { get; set; } = [];

    [JsonPropertyName("lineRegex")]
    [Description("Optional regex filter for the line key.")]
    public string? LineRegex { get; set; }

    [JsonPropertyName("numberRegex")]
    [Description("Optional regex filter for the journey or line number.")]
    public string? NumberRegex { get; set; }

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

        if (!IsValidRegex(LineRegex, out var lineRegexError))
        {
            yield return new ValidationResult(
                $"LineRegex is not a valid regular expression: {lineRegexError}",
                new[] { nameof(LineRegex) });
        }

        if (!IsValidRegex(NumberRegex, out var numberRegexError))
        {
            yield return new ValidationResult(
                $"NumberRegex is not a valid regular expression: {numberRegexError}",
                new[] { nameof(NumberRegex) });
        }
    }

    private static bool IsValidRegex(string? pattern, out string? error)
    {
        error = null;

        if (string.IsNullOrWhiteSpace(pattern))
            return true;

        try
        {
            _ = new Regex(pattern);
            return true;
        }
        catch (ArgumentException exception)
        {
            error = exception.Message;
            return false;
        }
    }
}
