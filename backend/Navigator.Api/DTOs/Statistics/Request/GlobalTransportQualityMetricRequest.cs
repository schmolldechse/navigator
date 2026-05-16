using Navigator.Data.Enums;
using Navigator.Data.Enums.Metric;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Statistics.Request;

public class GlobalTransportQualityMetricRequest : BaseMetricRequest
{
    private static readonly HashSet<int> ValidStepping = [1, 2, 3, 4, 6, 8, 12, 24];

    private static readonly HashSet<MetricSeriesType> ValidSeriesTypes =
    [
        MetricSeriesType.GlobalStationEventArrivalCount,
        MetricSeriesType.GlobalStationEventArrivalCancellationCount,
        MetricSeriesType.GlobalStationEventArrivalCancellationRate,
        MetricSeriesType.GlobalStationEventArrivalDelaySum,
        MetricSeriesType.GlobalStationEventArrivalPunctuality5Rate,
        MetricSeriesType.GlobalStationEventArrivalPunctuality15Rate,
        MetricSeriesType.GlobalStationEventDepartureCount,
        MetricSeriesType.GlobalStationEventDepartureCancellationCount,
        MetricSeriesType.GlobalStationEventDepartureCancellationRate,
        MetricSeriesType.GlobalStationEventDepartureDelaySum,
        MetricSeriesType.GlobalStationEventDeparturePunctuality5Rate,
        MetricSeriesType.GlobalStationEventDeparturePunctuality15Rate,
    ];

    [JsonPropertyName("seriesType")]
    [AllowedValues(
        MetricSeriesType.GlobalStationEventArrivalCount,
        MetricSeriesType.GlobalStationEventArrivalCancellationCount,
        MetricSeriesType.GlobalStationEventArrivalCancellationRate,
        MetricSeriesType.GlobalStationEventArrivalDelaySum,
        MetricSeriesType.GlobalStationEventArrivalPunctuality5Rate,
        MetricSeriesType.GlobalStationEventArrivalPunctuality15Rate,
        MetricSeriesType.GlobalStationEventDepartureCount,
        MetricSeriesType.GlobalStationEventDepartureCancellationCount,
        MetricSeriesType.GlobalStationEventDepartureCancellationRate,
        MetricSeriesType.GlobalStationEventDepartureDelaySum,
        MetricSeriesType.GlobalStationEventDeparturePunctuality5Rate,
        MetricSeriesType.GlobalStationEventDeparturePunctuality15Rate
    )]
    [Description("The specific global station-event transport metric series to retrieve.")]
    public required MetricSeriesType SeriesType { get; set; }

    [JsonPropertyName("start")]
    public required DateTimeOffset Start { get; set; }

    [JsonPropertyName("end")]
    public required DateTimeOffset End { get; set; }

    [JsonPropertyName("transportTypes")]
    [Description("Optional list of transport types to filter the metric by. If not provided, all transport types will be included.")]
    public TransportType[]? TransportTypes { get; set; }

    [JsonPropertyName("stepping")]
    [Description("Time step in hours for aggregation. Must divide 24 evenly. Allowed: 1,2,3,4,6,8,12,24. Default: 1")]
    [AllowedValues(1, 2, 3, 4, 6, 8, 12, 24)]
    public int Stepping { get; set; } = 1;

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Start > End)
        {
            yield return new ValidationResult(
                "Start time must be earlier than or equal to end time.",
                new[] { nameof(Start), nameof(End) });
        }

        if (!ValidStepping.Contains(Stepping))
        {
            yield return new ValidationResult(
                "Stepping must be one of: 1, 2, 3, 4, 6, 8, 12, 24.",
                new[] { nameof(Stepping) });
        }

        if (!ValidSeriesTypes.Contains(SeriesType))
        {
            yield return new ValidationResult(
                $"SeriesType must be one of: {string.Join(", ", ValidSeriesTypes)}.",
                new[] { nameof(SeriesType) });
        }
    }
}
