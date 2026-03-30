using Navigator.Data.Enums;
using Navigator.Data.Enums.Metric;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Statistics.Request;

public class StationSummaryMetricRequest : BaseMetricRequest
{
    private static readonly HashSet<MetricSeriesType> ValidSeriesTypes =
    [
        MetricSeriesType.StationArrivals,
        MetricSeriesType.StationArrivalCancellations,
        MetricSeriesType.StationArrivalDelayAvg,
        MetricSeriesType.StationDepartures,
        MetricSeriesType.StationDepartureCancellations,
        MetricSeriesType.StationDepartureDelayAvg
    ];

    [JsonPropertyName("seriesType")]
    [AllowedValues(
        MetricSeriesType.StationArrivals,
        MetricSeriesType.StationArrivalCancellations,
        MetricSeriesType.StationArrivalDelayAvg,
        MetricSeriesType.StationDepartures,
        MetricSeriesType.StationDepartureCancellations,
        MetricSeriesType.StationDepartureDelayAvg)]
    [Description("The specific station metric series to retrieve.")]
    public required MetricSeriesType SeriesType { get; set; }

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

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Start.HasValue && Start.Value > End)
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
