using System.ComponentModel;
using System.Text.Json.Serialization;
using Navigator.Data.Enums;

namespace Navigator.Data.Models.Statistics.Api;

public sealed record JourneyPatternResult(
    [property: JsonPropertyName("journeyNumber")]
    [property: Description("Journey number represented by the pattern.")]
    int JourneyNumber,

    [property: JsonPropertyName("lineName")]
    [property: Description("Line or journey description if available.")]
    string? LineName,

    [property: JsonPropertyName("transportType")]
    [property: Description("Dominant transport type if available.")]
    TransportType? TransportType,

    [property: JsonPropertyName("scheduledStartTime")]
    [property: Description("Typical scheduled start time in Europe/Berlin local time.")]
    TimeSpan ScheduledStartTime,

    [property: JsonPropertyName("scheduledEndTime")]
    [property: Description("Typical scheduled end time in Europe/Berlin local time.")]
    TimeSpan ScheduledEndTime,

    [property: JsonPropertyName("mainOrigin")]
    [property: Description("Most frequently observed origin station.")]
    StationReference? MainOrigin,

    [property: JsonPropertyName("mainDestination")]
    [property: Description("Most frequently observed destination station.")]
    StationReference? MainDestination,

    [property: JsonPropertyName("observedServiceDays")]
    [property: Description("Number of service days observed in the selected range.")]
    int ObservedServiceDays,

    [property: JsonPropertyName("routeVariantCount")]
    [property: Description("Number of observed origin-destination variants.")]
    int RouteVariantCount
) : StatisticsMetricResult;

public sealed record JourneyDailyOutcomesResult(
    [property: JsonPropertyName("items")]
    [property: Description("Daily outcomes for the journey number.")]
    IReadOnlyList<JourneyDailyOutcomeItem> Items
) : StatisticsMetricResult;

public sealed record JourneyDailyOutcomeItem(
    [property: JsonPropertyName("serviceDate")]
    [property: Description("Service date represented by the row.")]
    DateOnly ServiceDate,

    [property: JsonPropertyName("outcome")]
    [property: Description("Derived journey outcome for the service date.")]
    string Outcome,

    [property: JsonPropertyName("destinationDelaySeconds")]
    [property: Description("Destination delay in seconds, if available.")]
    int? DestinationDelaySeconds,

    [property: JsonPropertyName("destinationPunctualityClass")]
    [property: Description("Punctuality class derived from the destination delay.")]
    string DestinationPunctualityClass,

    [property: JsonPropertyName("fullyCancelled")]
    [property: Description("Whether the journey was fully cancelled.")]
    bool FullyCancelled,

    [property: JsonPropertyName("partiallyCancelled")]
    [property: Description("Whether the journey was partially cancelled.")]
    bool PartiallyCancelled,

    [property: JsonPropertyName("destinationNotReached")]
    [property: Description("Whether the destination stop was not reached.")]
    bool DestinationNotReached
);

public sealed record JourneyStopProfileResult(
    [property: JsonPropertyName("items")]
    [property: Description("Stop profile rows for the journey number.")]
    IReadOnlyList<JourneyStopProfileItem> Items
) : StatisticsMetricResult;

public sealed record JourneyStopProfileItem(
    [property: JsonPropertyName("station")]
    [property: Description("Station represented by the stop profile row.")]
    StationReference Station,

    [property: JsonPropertyName("plannedTime")]
    [property: Description("Typical planned stop time in Europe/Berlin local time.")]
    TimeSpan PlannedTime,

    [property: JsonPropertyName("scheduleType")]
    [property: Description("Arrival or departure schedule type.")]
    ScheduleType ScheduleType,

    [property: JsonPropertyName("plannedEvents")]
    [property: Description("Number of planned stop events at this stop.")]
    long PlannedEvents,

    [property: JsonPropertyName("cancelledEvents")]
    [property: Description("Number of cancelled stop events at this stop.")]
    long CancelledEvents,

    [property: JsonPropertyName("customerReliability5Rate")]
    [property: Description("Share of planned stop events that were served with less than six minutes delay.")]
    decimal? CustomerReliability5Rate,

    [property: JsonPropertyName("medianDelaySeconds")]
    [property: Description("Median stop-event delay in seconds.")]
    decimal? MedianDelaySeconds,

    [property: JsonPropertyName("p95DelaySeconds")]
    [property: Description("95th percentile stop-event delay in seconds.")]
    decimal? P95DelaySeconds
);

public sealed record JourneyDelayBuildUpResult(
    [property: JsonPropertyName("items")]
    [property: Description("Delay distribution along the journey stops.")]
    IReadOnlyList<JourneyDelayBuildUpItem> Items
) : StatisticsMetricResult;

public sealed record JourneyDelayBuildUpItem(
    [property: JsonPropertyName("station")]
    [property: Description("Station represented by the delay build-up row.")]
    StationReference Station,

    [property: JsonPropertyName("medianDelaySeconds")]
    [property: Description("Median stop-event delay in seconds.")]
    decimal? MedianDelaySeconds,

    [property: JsonPropertyName("p95DelaySeconds")]
    [property: Description("95th percentile stop-event delay in seconds.")]
    decimal? P95DelaySeconds
);

public sealed record JourneyCalendarResult(
    [property: JsonPropertyName("items")]
    [property: Description("Calendar rows for the journey number.")]
    IReadOnlyList<JourneyCalendarItem> Items
) : StatisticsMetricResult;

public sealed record JourneyCalendarItem(
    [property: JsonPropertyName("serviceDate")]
    [property: Description("Service date represented by the row.")]
    DateOnly ServiceDate,

    [property: JsonPropertyName("status")]
    [property: Description("Destination punctuality status for the service date.")]
    string Status,

    [property: JsonPropertyName("destinationDelaySeconds")]
    [property: Description("Destination delay in seconds, if available.")]
    int? DestinationDelaySeconds,

    [property: JsonPropertyName("outcome")]
    [property: Description("Derived journey outcome for the service date.")]
    string Outcome
);
