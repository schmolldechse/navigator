using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Navigator.Data.Enums;
using Navigator.Data.Enums.Metric;

namespace Navigator.Data.Models.Statistics.Request;

public class NetworkStationEventQualityTimeSeriesMetricRequest : BaseMetricRequest
{
    private static readonly HashSet<MetricSeriesType> ValidSeriesTypes =
    [
        MetricSeriesType.StationEventCount,
        MetricSeriesType.StationEventCancellationCount,
        MetricSeriesType.StationEventCancellationRate,
        MetricSeriesType.StationEventDelayAverage,
        MetricSeriesType.StationEventPunctuality5Rate,
        MetricSeriesType.StationEventPunctuality15Rate,
    ];

    [JsonPropertyName("seriesType")]
    [AllowedValues(
        MetricSeriesType.StationEventCount,
        MetricSeriesType.StationEventCancellationCount,
        MetricSeriesType.StationEventCancellationRate,
        MetricSeriesType.StationEventDelayAverage,
        MetricSeriesType.StationEventPunctuality5Rate,
        MetricSeriesType.StationEventPunctuality15Rate
    )]
    [Description("The station-event quality metric to retrieve as a network-wide time series.")]
    public override required MetricSeriesType SeriesType { get; set; }

    [JsonPropertyName("scheduleType")]
    [Description("Whether to retrieve arrival or departure metrics.")]
    public required ScheduleType ScheduleType { get; set; }

    [JsonPropertyName("start")]
    public required DateTimeOffset Start { get; set; }

    [JsonPropertyName("end")]
    public required DateTimeOffset End { get; set; }

    [JsonPropertyName("transportTypes")]
    [Description("Optional list of transport types to filter the metric by. If omitted or empty, all transport types will be included.")]
    public TransportType[]? TransportTypes { get; set; } = [];

    [JsonPropertyName("includeReplacementTransport")]
    [Description("Whether replacement transport should be included in the metric.")]
    public bool IncludeReplacementTransport { get; set; } = true;

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
