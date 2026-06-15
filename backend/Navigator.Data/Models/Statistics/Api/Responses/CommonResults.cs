using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Navigator.Data.Models.Statistics.Api;

public sealed record EventKpisResult(
    [property: JsonPropertyName("metrics")]
    [property: Description("Event quality KPIs.")]
    EventMetrics Metrics
) : StatisticsMetricResult;

public sealed record JourneyKpisResult(
    [property: JsonPropertyName("metrics")]
    [property: Description("Journey quality KPIs.")]
    JourneyMetrics Metrics
) : StatisticsMetricResult;
