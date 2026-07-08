using System.ComponentModel;
using System.Text.Json.Serialization;
using Navigator.Data.Enums;

namespace Navigator.Data.Models.Statistics.Api;

public sealed record EventMetrics(
    [property: JsonPropertyName("plannedEvents")]
    [property: Description("Number of planned stop events.")]
    long PlannedEvents,

    [property: JsonPropertyName("cancelledEvents")]
    [property: Description("Number of stop events that were cancelled.")]
    long CancelledEvents,

    [property: JsonPropertyName("servedEvents")]
    [property: Description("Number of stop events that were not cancelled.")]
    long ServedEvents,

    [property: JsonPropertyName("cancellationRate")]
    [property: Description("Share of planned stop events that were cancelled.")]
    decimal? CancellationRate,

    [property: JsonPropertyName("operativePunctuality5Rate")]
    [property: Description("Share of served stop events with less than six minutes delay.")]
    decimal? OperativePunctuality5Rate,

    [property: JsonPropertyName("customerReliability5Rate")]
    [property: Description("Share of planned stop events that were served with less than six minutes delay.")]
    decimal? CustomerReliability5Rate,

    [property: JsonPropertyName("operativePunctuality15Rate")]
    [property: Description("Share of served stop events with less than fifteen minutes delay.")]
    decimal? OperativePunctuality15Rate,

    [property: JsonPropertyName("customerReliability15Rate")]
    [property: Description("Share of planned stop events that were served with less than fifteen minutes delay.")]
    decimal? CustomerReliability15Rate,

    [property: JsonPropertyName("averageDelaySeconds")]
    [property: Description("Average delay in seconds across served stop events.")]
    decimal? AverageDelaySeconds,

    [property: JsonPropertyName("delayDebtMinutes")]
    [property: Description("Sum of positive stop-event delays in minutes.")]
    decimal DelayDebtMinutes,

    [property: JsonPropertyName("late30Rate")]
    [property: Description("Share of planned stop events delayed by at least thirty minutes.")]
    decimal? Late30Rate,

    [property: JsonPropertyName("late60Rate")]
    [property: Description("Share of planned stop events delayed by at least sixty minutes.")]
    decimal? Late60Rate
);

public sealed record JourneyMetrics(
    [property: JsonPropertyName("plannedJourneys")]
    [property: Description("Number of planned journeys.")]
    long PlannedJourneys,

    [property: JsonPropertyName("completedJourneys")]
    [property: Description("Number of journeys that were not fully cancelled and reached the destination.")]
    long CompletedJourneys,

    [property: JsonPropertyName("fullyCancelledJourneys")]
    [property: Description("Number of journeys where all stop events were cancelled.")]
    long FullyCancelledJourneys,

    [property: JsonPropertyName("partiallyCancelledJourneys")]
    [property: Description("Number of journeys with at least one cancelled stop event, but not fully cancelled.")]
    long PartiallyCancelledJourneys,

    [property: JsonPropertyName("destinationNotReachedJourneys")]
    [property: Description("Number of journeys whose destination stop was not reached.")]
    long DestinationNotReachedJourneys,

    [property: JsonPropertyName("fullCancellationRate")]
    [property: Description("Share of planned journeys that were fully cancelled.")]
    decimal? FullCancellationRate,

    [property: JsonPropertyName("partialCancellationRate")]
    [property: Description("Share of planned journeys that were partially cancelled.")]
    decimal? PartialCancellationRate,

    [property: JsonPropertyName("destinationNotReachedRate")]
    [property: Description("Share of planned journeys whose destination stop was not reached.")]
    decimal? DestinationNotReachedRate,

    [property: JsonPropertyName("journeyCompletionRate")]
    [property: Description("Share of planned journeys that reached the destination and were not fully cancelled.")]
    decimal? JourneyCompletionRate,

    [property: JsonPropertyName("destinationPunctuality5Rate")]
    [property: Description("Share of planned journeys whose destination delay was below six minutes.")]
    decimal? DestinationPunctuality5Rate,

    [property: JsonPropertyName("destinationPunctuality15Rate")]
    [property: Description("Share of planned journeys whose destination delay was below fifteen minutes.")]
    decimal? DestinationPunctuality15Rate,

    [property: JsonPropertyName("averageDestinationDelaySeconds")]
    [property: Description("Average destination delay in seconds for journeys with a destination delay sample.")]
    decimal? AverageDestinationDelaySeconds,

    [property: JsonPropertyName("destinationDelayDebtMinutes")]
    [property: Description("Sum of positive destination delays in minutes.")]
    decimal DestinationDelayDebtMinutes
);

public sealed record JourneyOutcomeMetrics(
    [property: JsonPropertyName("plannedJourneys")]
    [property: Description("Number of planned journeys.")]
    long PlannedJourneys,

    [property: JsonPropertyName("completedJourneys")]
    [property: Description("Number of journeys that were completed without partial cancellation or destination failure.")]
    long CompletedJourneys,

    [property: JsonPropertyName("partiallyCancelledDestinationReachedJourneys")]
    [property: Description("Number of partially cancelled journeys that still reached their destination.")]
    long PartiallyCancelledDestinationReachedJourneys,

    [property: JsonPropertyName("destinationNotReachedJourneys")]
    [property: Description("Number of not fully cancelled journeys whose destination stop was not reached.")]
    long DestinationNotReachedJourneys,

    [property: JsonPropertyName("fullyCancelledJourneys")]
    [property: Description("Number of journeys where all stop events were cancelled.")]
    long FullyCancelledJourneys
);

public sealed record StationReference(
    [property: JsonPropertyName("stationEvaNumber")]
    [property: Description("Station EVA number.")]
    int StationEvaNumber,

    [property: JsonPropertyName("stationName")]
    [property: Description("Station display name if available.")]
    string? StationName = null,

    [property: JsonPropertyName("latitude")]
    [property: Description("Station latitude if available.")]
    double? Latitude = null,

    [property: JsonPropertyName("longitude")]
    [property: Description("Station longitude if available.")]
    double? Longitude = null
);

public sealed record LineReference(
    [property: JsonPropertyName("lineName")]
    [property: Description("Line or journey description.")]
    string LineName,

    [property: JsonPropertyName("journeyNumber")]
    [property: Description("Journey number if the row represents a concrete journey number.")]
    int? JourneyNumber,

    [property: JsonPropertyName("transportType")]
    [property: Description("Transport type of the line.")]
    TransportType TransportType,

    [property: JsonPropertyName("origin")]
    [property: Description("Origin station of the line or route variant.")]
    StationReference? Origin,

    [property: JsonPropertyName("destination")]
    [property: Description("Destination station of the line or route variant.")]
    StationReference? Destination
);

public sealed record MetricPage(
    [property: JsonPropertyName("offset")]
    [property: Description("Number of skipped items.")]
    int Offset,

    [property: JsonPropertyName("limit")]
    [property: Description("Maximum number of returned items.")]
    int Limit,

    [property: JsonPropertyName("totalItems")]
    [property: Description("Total number of matching items before paging.")]
    int TotalItems
);

public sealed record EventTimeSeriesPoint(
    [property: JsonPropertyName("bucketStart")]
    [property: Description("Start of the returned time bucket using the request offset.")]
    DateTimeOffset BucketStart,

    [property: JsonPropertyName("eventMetrics")]
    [property: Description("Aggregated event metrics for the bucket.")]
    EventMetrics EventMetrics
);

public sealed record JourneyTimeSeriesPoint(
    [property: JsonPropertyName("bucketStart")]
    [property: Description("Start of the returned time bucket using the request offset.")]
    DateTimeOffset BucketStart,

    [property: JsonPropertyName("journeyMetrics")]
    [property: Description("Aggregated journey metrics for the bucket.")]
    JourneyMetrics JourneyMetrics
);

public sealed record JourneyOutcomeTimeSeriesPoint(
    [property: JsonPropertyName("bucketStart")]
    [property: Description("Start of the returned time bucket using the request offset.")]
    DateTimeOffset BucketStart,

    [property: JsonPropertyName("journeyOutcomeMetrics")]
    [property: Description("Disjoint journey outcome metrics for the bucket.")]
    JourneyOutcomeMetrics JourneyOutcomeMetrics
);

public sealed record LineTimeSeriesPoint(
    [property: JsonPropertyName("bucketStart")]
    [property: Description("Start of the returned time bucket using the request offset.")]
    DateTimeOffset BucketStart,

    [property: JsonPropertyName("journeyMetrics")]
    [property: Description("Aggregated journey metrics for the bucket.")]
    JourneyMetrics JourneyMetrics,

    [property: JsonPropertyName("eventMetrics")]
    [property: Description("Aggregated event metrics for the bucket.")]
    EventMetrics EventMetrics
);

public sealed record EventHeatmapCell(
    [property: JsonPropertyName("weekday")]
    [property: Description("ISO weekday number, Monday is 1 and Sunday is 7.")]
    int Weekday,

    [property: JsonPropertyName("hour")]
    [property: Description("Hour of day using the request offset.")]
    int Hour,

    [property: JsonPropertyName("eventMetrics")]
    [property: Description("Aggregated event metrics for the weekday-hour cell.")]
    EventMetrics EventMetrics
);

public sealed record JourneyHeatmapCell(
    [property: JsonPropertyName("weekday")]
    [property: Description("ISO weekday number, Monday is 1 and Sunday is 7.")]
    int Weekday,

    [property: JsonPropertyName("hour")]
    [property: Description("Hour of day using the request offset.")]
    int Hour,

    [property: JsonPropertyName("journeyMetrics")]
    [property: Description("Aggregated journey metrics for the weekday-hour cell.")]
    JourneyMetrics JourneyMetrics
);

public sealed record StationEventRankingItem(
    [property: JsonPropertyName("station")]
    [property: Description("Ranked station.")]
    StationReference Station,

    [property: JsonPropertyName("eventMetrics")]
    [property: Description("Aggregated event metrics for the station.")]
    EventMetrics EventMetrics
);

public sealed record LineJourneyRankingItem(
    [property: JsonPropertyName("line")]
    [property: Description("Ranked line or route.")]
    LineReference Line,

    [property: JsonPropertyName("journeyMetrics")]
    [property: Description("Aggregated journey metrics for the line.")]
    JourneyMetrics JourneyMetrics
);

public sealed record StationLineRankingItem(
    [property: JsonPropertyName("line")]
    [property: Description("Ranked line at the station.")]
    LineReference Line,

    [property: JsonPropertyName("eventMetrics")]
    [property: Description("Aggregated event metrics for the station-line combination.")]
    EventMetrics EventMetrics
);
