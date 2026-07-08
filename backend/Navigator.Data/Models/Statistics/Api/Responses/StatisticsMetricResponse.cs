using System.ComponentModel;
using System.Text.Json.Serialization;
using Navigator.Data.Enums.Metric;

namespace Navigator.Data.Models.Statistics.Api;

public sealed record StatisticsMetricResponse(
    [property: JsonPropertyName("meta")]
    [property: Description("Metadata describing the evaluated metric, time range and filters.")]
    StatisticsResponseMeta Meta,

    [property: JsonPropertyName("result")]
    [property: Description("Typed metric result. The concrete shape is selected by result.type.")]
    StatisticsMetricResult Result
);

public sealed record StatisticsResponseMeta(
    [property: JsonPropertyName("scope")]
    [property: Description("Statistics topic that produced the metric.")]
    StatisticsScope Scope,

    [property: JsonPropertyName("metric")]
    [property: Description("Metric name that was evaluated.")]
    StatisticsMetricType Metric,

    [property: JsonPropertyName("from")]
    [property: Description("First included instant.")]
    DateTimeOffset From,

    [property: JsonPropertyName("to")]
    [property: Description("First excluded instant.")]
    DateTimeOffset To,

    [property: JsonPropertyName("bucket")]
    [property: Description("Aggregation bucket used for grouped results.")]
    StatisticsBucket? Bucket,

    [property: JsonPropertyName("filters")]
    [property: Description("Effective filters applied to the metric.")]
    IReadOnlyDictionary<string, object?> Filters
);
