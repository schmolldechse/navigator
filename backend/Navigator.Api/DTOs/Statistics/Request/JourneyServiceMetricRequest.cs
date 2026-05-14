using Navigator.Data.Enums;
using Navigator.Data.Enums.Metric;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Statistics.Request;

public class JourneyServiceMetricRequest : BaseMetricRequest
{
    private static readonly HashSet<MetricSeriesType> ValidSeriesTypes =
    [
        MetricSeriesType.JourneyServiceOperatorJourneys,
        MetricSeriesType.JourneyServiceOperatorCancellations,
        MetricSeriesType.JourneyServiceDailyOperatorJourneys,
        MetricSeriesType.JourneyServiceDailyOperatorCancellations,
        MetricSeriesType.JourneyServiceJourneyTypeJourneys,
        MetricSeriesType.JourneyServiceReplacementTransports
    ];

    [JsonPropertyName("seriesType")]
    [Description("The specific journey service metric to retrieve.")]
    public required MetricSeriesType SeriesType { get; set; }

    [JsonPropertyName("start")]
    public required DateTimeOffset Start { get; set; }

    [JsonPropertyName("end")]
    public required DateTimeOffset End { get; set; }

    [JsonPropertyName("transportTypes")]
    public TransportType[]? TransportTypes { get; set; }

    [JsonPropertyName("journeyTypes")]
    public JourneyType[]? JourneyTypes { get; set; }

    [JsonPropertyName("operatorCodes")]
    public string[]? OperatorCodes { get; set; }

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Start > End)
            yield return new ValidationResult("Start time must be earlier than or equal to end time.", [nameof(Start), nameof(End)]);

        if (!ValidSeriesTypes.Contains(SeriesType))
            yield return new ValidationResult($"SeriesType must be one of: {string.Join(", ", ValidSeriesTypes)}.", [nameof(SeriesType)]);
    }
}
