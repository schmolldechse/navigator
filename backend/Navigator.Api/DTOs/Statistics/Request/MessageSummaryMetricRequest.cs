using Navigator.Data.Enums;
using Navigator.Data.Enums.Metric;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Navigator.Api.DTOs.Statistics.Request;

public class MessageSummaryMetricRequest : BaseMetricRequest
{
    private static readonly HashSet<MetricSeriesType> ValidSeriesTypes =
    [
        MetricSeriesType.MessageDisruptionCauses,
        MetricSeriesType.MessageDailyTypes,
        MetricSeriesType.StationMessageDisruptions,
        MetricSeriesType.StationMessageAffectedStops
    ];

    [JsonPropertyName("seriesType")]
    [Description("The specific message or disruption metric to retrieve.")]
    public required MetricSeriesType SeriesType { get; set; }

    [JsonPropertyName("start")]
    public required DateTimeOffset Start { get; set; }

    [JsonPropertyName("end")]
    public required DateTimeOffset End { get; set; }

    [JsonPropertyName("transportTypes")]
    public TransportType[]? TransportTypes { get; set; }

    [JsonPropertyName("messageTypes")]
    public MessageType[]? MessageTypes { get; set; }

    [JsonPropertyName("evaNumbers")]
    public int[]? EvaNumbers { get; set; }

    [JsonPropertyName("limit")]
    [Description("Maximum number of ranked categories to return. Default: 15.")]
    public int Limit { get; set; } = 15;

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Start > End)
            yield return new ValidationResult("Start time must be earlier than or equal to end time.", [nameof(Start), nameof(End)]);

        if (!ValidSeriesTypes.Contains(SeriesType))
            yield return new ValidationResult($"SeriesType must be one of: {string.Join(", ", ValidSeriesTypes)}.", [nameof(SeriesType)]);

        if (Limit is < 1 or > 100)
            yield return new ValidationResult("Limit must be between 1 and 100.", [nameof(Limit)]);
    }
}
