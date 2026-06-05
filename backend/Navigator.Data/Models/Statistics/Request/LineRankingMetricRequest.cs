using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Navigator.Data.Enums;
using Navigator.Data.Enums.Metric;
using Navigator.Data.Infrastructure;

namespace Navigator.Data.Models.Statistics.Request;

public class LineRankingMetricRequest : BaseMetricRequest, IEvaNumberMetricRequest
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

    [JsonPropertyName("scheduleType")]
    [Description("Whether to retrieve arrival or departure station-event ranking metrics. Only applies when evaNumbers are provided.")]
    public ScheduleType? ScheduleType { get; set; }

    [JsonPropertyName("evaNumbers")]
    [Description("Optional array of station EVA numbers. If provided, line quality is ranked for station visits instead of global journey route quality.")]
    public int[] EvaNumbers { get; set; } = [];

    [JsonPropertyName("includeRil100")]
    [Description("Whether EVA number filters should include all station EVA numbers that share a RIL100 code.")]
    public bool IncludeRil100 { get; set; } = false;

    [JsonPropertyName("transportTypes")]
    [Description("Optional list of transport types to filter the ranking by. If omitted or empty, all transport types will be included.")]
    public TransportType[]? TransportTypes { get; set; } = [];

    [JsonPropertyName("includeReplacementTransport")]
    [Description("Whether replacement transport should be included in the metric.")]
    public bool IncludeReplacementTransport { get; set; } = true;

    [JsonPropertyName("journeyDescription")]
    [Regex(
        @"^[\p{L}\p{N}\s_\-/\^\$\.\|\(\)\[\]\+\*\?\{\},]+$",
        Exception = "Journey description may only contain letters, digits, spaces and basic regex operators.",
        MaxLength = 64)]
    [Description("Optional regex filter for the journey description.")]
    public string? JourneyDescription { get; set; }

    [JsonPropertyName("number")]
    [Regex(
        @"^[0-9\s\^\$\.\|\(\)\[\]\+\*\?\{\},]+$",
        Exception = "Number may only contain digits and basic regex operators.",
        MaxLength = 64)]
    [Description("Optional regex filter for the journey number.")]
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
