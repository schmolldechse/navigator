using System.ComponentModel;
using System.Text.Json.Serialization;
using Navigator.Data.Enums;

namespace Navigator.Data.Models.Statistics.Api;

public sealed record NetworkEventTimeSeriesResult(
    [property: JsonPropertyName("items")]
    [property: Description("Event metric points grouped by the requested bucket.")]
    IReadOnlyList<EventTimeSeriesPoint> Items
) : StatisticsMetricResult;

public sealed record NetworkJourneyTimeSeriesResult(
    [property: JsonPropertyName("items")]
    [property: Description("Journey metric points grouped by the requested bucket.")]
    IReadOnlyList<JourneyTimeSeriesPoint> Items
) : StatisticsMetricResult;

public sealed record EventWeekdayHourHeatmapResult(
    [property: JsonPropertyName("items")]
    [property: Description("Event metric cells grouped by weekday and hour.")]
    IReadOnlyList<EventHeatmapCell> Items
) : StatisticsMetricResult;

public sealed record JourneyWeekdayHourHeatmapResult(
    [property: JsonPropertyName("items")]
    [property: Description("Journey metric cells grouped by weekday and hour.")]
    IReadOnlyList<JourneyHeatmapCell> Items
) : StatisticsMetricResult;

public sealed record TransportTypeComparisonResult(
    [property: JsonPropertyName("items")]
    [property: Description("Event and journey metrics grouped by transport type.")]
    IReadOnlyList<TransportTypeComparisonItem> Items
) : StatisticsMetricResult;

public sealed record TransportTypeComparisonItem(
    [property: JsonPropertyName("transportType")]
    [property: Description("Transport type represented by the item.")]
    TransportType TransportType,

    [property: JsonPropertyName("eventMetrics")]
    [property: Description("Aggregated event metrics for the transport type.")]
    EventMetrics EventMetrics,

    [property: JsonPropertyName("journeyMetrics")]
    [property: Description("Aggregated journey metrics for the transport type.")]
    JourneyMetrics JourneyMetrics
);

public sealed record NetworkStationRankingResult(
    [property: JsonPropertyName("items")]
    [property: Description("Ranked stations.")]
    IReadOnlyList<StationEventRankingItem> Items,

    [property: JsonPropertyName("page")]
    [property: Description("Paging metadata for the ranking.")]
    MetricPage Page
) : StatisticsMetricResult;

public sealed record NetworkLineRankingResult(
    [property: JsonPropertyName("items")]
    [property: Description("Ranked lines.")]
    IReadOnlyList<LineJourneyRankingItem> Items,

    [property: JsonPropertyName("page")]
    [property: Description("Paging metadata for the ranking.")]
    MetricPage Page
) : StatisticsMetricResult;

public sealed record NetworkMapHotspotsResult(
    [property: JsonPropertyName("featureCollection")]
    [property: Description("GeoJSON feature collection for station hotspots.")]
    GeoJsonFeatureCollection FeatureCollection
) : StatisticsMetricResult;

public sealed record EventDelayDistributionResult(
    [property: JsonPropertyName("summary")]
    [property: Description("Summary quantiles for served stop-event delay samples.")]
    EventDelayDistributionSummary Summary,

    [property: JsonPropertyName("bins")]
    [property: Description("Histogram bins for served stop-event delay samples.")]
    IReadOnlyList<EventDelayDistributionBin> Bins
) : StatisticsMetricResult;

public sealed record EventDelayDistributionSummary(
    [property: JsonPropertyName("sampleCount")]
    [property: Description("Number of served stop-event delay samples.")]
    long SampleCount,

    [property: JsonPropertyName("medianDelaySeconds")]
    [property: Description("Median served stop-event delay in seconds.")]
    decimal? MedianDelaySeconds,

    [property: JsonPropertyName("p95DelaySeconds")]
    [property: Description("95th percentile served stop-event delay in seconds.")]
    decimal? P95DelaySeconds
);

public sealed record EventDelayDistributionBin(
    [property: JsonPropertyName("lowerBoundSeconds")]
    [property: Description("Inclusive lower delay bound in seconds.")]
    int LowerBoundSeconds,

    [property: JsonPropertyName("upperBoundSeconds")]
    [property: Description("Exclusive upper delay bound in seconds.")]
    int UpperBoundSeconds,

    [property: JsonPropertyName("count")]
    [property: Description("Number of delay samples inside the bin.")]
    long Count,

    [property: JsonPropertyName("cumulativeShare")]
    [property: Description("Cumulative share up to and including this bin.")]
    decimal CumulativeShare
);
