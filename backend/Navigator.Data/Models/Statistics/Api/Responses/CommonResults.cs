using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Navigator.Data.Models.Statistics.Api;

public sealed record EventSummaryResult(
    [property: JsonPropertyName("metrics")]
    [property: Description("Event quality summary metrics.")]
    EventMetrics Metrics
) : StatisticsMetricResult;

public sealed record JourneySummaryResult(
    [property: JsonPropertyName("metrics")]
    [property: Description("Journey quality summary metrics.")]
    JourneyMetrics Metrics
) : StatisticsMetricResult;
