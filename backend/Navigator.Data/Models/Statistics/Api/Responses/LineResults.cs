using System.ComponentModel;
using System.Text.Json.Serialization;
using Navigator.Data.Enums;

namespace Navigator.Data.Models.Statistics.Api;

public sealed record LineProfileResult(
    [property: JsonPropertyName("lineName")]
    [property: Description("Line or journey description.")]
    string LineName,

    [property: JsonPropertyName("transportType")]
    [property: Description("Dominant transport type for the line, if available.")]
    TransportType? TransportType,

    [property: JsonPropertyName("representativeJourneyNumbers")]
    [property: Description("Most frequently observed journey numbers for the line.")]
    IReadOnlyList<int> RepresentativeJourneyNumbers,

    [property: JsonPropertyName("routeVariantCount")]
    [property: Description("Number of observed origin-destination variants.")]
    int RouteVariantCount,

    [property: JsonPropertyName("mainOrigin")]
    [property: Description("Most frequently observed origin station.")]
    StationReference? MainOrigin,

    [property: JsonPropertyName("mainDestination")]
    [property: Description("Most frequently observed destination station.")]
    StationReference? MainDestination
) : StatisticsMetricResult;

public sealed record LineTimeSeriesResult(
    [property: JsonPropertyName("items")]
    [property: Description("Line metric points grouped by the requested bucket.")]
    IReadOnlyList<LineTimeSeriesPoint> Items
) : StatisticsMetricResult;

public sealed record LineRouteVariantsResult(
    [property: JsonPropertyName("items")]
    [property: Description("Observed origin-destination variants for the line.")]
    IReadOnlyList<RouteVariantItem> Items
) : StatisticsMetricResult;

public sealed record RouteVariantItem(
    [property: JsonPropertyName("variantKey")]
    [property: Description("Stable key for the origin-destination variant.")]
    string VariantKey,

    [property: JsonPropertyName("origin")]
    [property: Description("Origin station of the variant.")]
    StationReference Origin,

    [property: JsonPropertyName("destination")]
    [property: Description("Destination station of the variant.")]
    StationReference Destination,

    [property: JsonPropertyName("transportType")]
    [property: Description("Transport type of the variant.")]
    TransportType TransportType,

    [property: JsonPropertyName("journeyMetrics")]
    [property: Description("Aggregated journey metrics for the variant.")]
    JourneyMetrics JourneyMetrics
);

public sealed record LineStationPerformanceResult(
    [property: JsonPropertyName("items")]
    [property: Description("Station performance rows for the line.")]
    IReadOnlyList<LineStationPerformanceItem> Items,

    [property: JsonPropertyName("page")]
    [property: Description("Paging metadata for the station rows.")]
    MetricPage Page
) : StatisticsMetricResult;

public sealed record LineProblemStationsResult(
    [property: JsonPropertyName("items")]
    [property: Description("Stations with the highest delay impact for the line.")]
    IReadOnlyList<LineStationPerformanceItem> Items,

    [property: JsonPropertyName("page")]
    [property: Description("Paging metadata for the station rows.")]
    MetricPage Page
) : StatisticsMetricResult;

public sealed record LineStationPerformanceItem(
    [property: JsonPropertyName("station")]
    [property: Description("Station on the line.")]
    StationReference Station,

    [property: JsonPropertyName("eventMetrics")]
    [property: Description("Aggregated event metrics for the line at this station.")]
    EventMetrics EventMetrics
);

public sealed record LineJourneyNumberRankingResult(
    [property: JsonPropertyName("items")]
    [property: Description("Journey numbers ranked within the line.")]
    IReadOnlyList<JourneyNumberRankingItem> Items,

    [property: JsonPropertyName("page")]
    [property: Description("Paging metadata for the ranking.")]
    MetricPage Page
) : StatisticsMetricResult;

public sealed record JourneyNumberRankingItem(
    [property: JsonPropertyName("journeyNumber")]
    [property: Description("Journey number represented by the row.")]
    int JourneyNumber,

    [property: JsonPropertyName("line")]
    [property: Description("Line and route reference for the journey number.")]
    LineReference Line,

    [property: JsonPropertyName("journeyMetrics")]
    [property: Description("Aggregated journey metrics for the journey number.")]
    JourneyMetrics JourneyMetrics
);
