using Navigator.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Views;

public interface IEventQualityHourly
{
    DateTime BucketHour { get; }
    TransportType TransportType { get; }
    bool IsReplacement { get; }
    long EventCount { get; }
    long StopCancelledCount { get; }
    long EventDelaySumSeconds { get; }
    long EventPositiveDelaySumSeconds { get; }
    long EventPunctual5Count { get; }
    long EventPunctual15Count { get; }
    long EventLate30Count { get; }
    long EventLate60Count { get; }
}

public interface IJourneyQualityHourly
{
    DateTime BucketHour { get; }
    TransportType TransportType { get; }
    bool IsReplacement { get; }
    long JourneyCount { get; }
    long FullyCancelledCount { get; }
    long PartiallyCancelledCount { get; }
    long DestinationNotReachedCount { get; }
    long DestinationDelaySumSeconds { get; }
    long DestinationPositiveDelaySumSeconds { get; }
    long DestinationPunctual5Count { get; }
    long DestinationPunctual15Count { get; }
    long DestinationLate30Count { get; }
    long DestinationLate60Count { get; }
}

public class NetworkEventQualityHourly : EventQualityHourlyBase
{
    [Column("schedule_type")]
    public required ScheduleType ScheduleType { get; set; }
}

public class NetworkEventDelayDistributionHourly
{
    [Column("bucket_hour")]
    public required DateTime BucketHour { get; set; }

    [Column("station_eva_number")]
    public required int StationEvaNumber { get; set; }

    [Column("schedule_type")]
    public required ScheduleType ScheduleType { get; set; }

    [Column("transport_type")]
    public required TransportType TransportType { get; set; }

    [Column("is_replacement")]
    public required bool IsReplacement { get; set; }

    [Column("delay_lt_minus_5_count")]
    public required long DelayLtMinus5Count { get; set; }

    [Column("delay_gte_minus_5_lt_0_count")]
    public required long DelayGteMinus5Lt0Count { get; set; }

    [Column("delay_gte_0_lt_5_count")]
    public required long DelayGte0Lt5Count { get; set; }

    [Column("delay_gte_5_lt_10_count")]
    public required long DelayGte5Lt10Count { get; set; }

    [Column("delay_gte_10_lt_15_count")]
    public required long DelayGte10Lt15Count { get; set; }

    [Column("delay_gte_15_lt_30_count")]
    public required long DelayGte15Lt30Count { get; set; }

    [Column("delay_gte_30_lt_60_count")]
    public required long DelayGte30Lt60Count { get; set; }

    [Column("delay_gte_60_lt_120_count")]
    public required long DelayGte60Lt120Count { get; set; }

    [Column("delay_gte_120_count")]
    public required long DelayGte120Count { get; set; }
}

public class StationEventQualityHourly : EventQualityHourlyBase
{
    [Column("station_eva_number")]
    public required int StationEvaNumber { get; set; }

    [Column("schedule_type")]
    public required ScheduleType ScheduleType { get; set; }
}

public class StationAdministrationQualityHourly : EventQualityHourlyBase
{
    [Column("station_eva_number")]
    public required int StationEvaNumber { get; set; }

    [Column("schedule_type")]
    public required ScheduleType ScheduleType { get; set; }

    [Column("administration_id")]
    public required Guid AdministrationId { get; set; }
}

public class LineEventQualityHourly : EventQualityHourlyBase
{
    [Column("journey_description")]
    public required string JourneyDescription { get; set; }

    [Column("schedule_type")]
    public required ScheduleType ScheduleType { get; set; }

    [Column("origin_eva_number")]
    public required int OriginEvaNumber { get; set; }

    [Column("destination_eva_number")]
    public required int DestinationEvaNumber { get; set; }

    [Column("administration_id")]
    public required Guid AdministrationId { get; set; }
}

public class StationLineQualityHourly : EventQualityHourlyBase
{
    [Column("station_eva_number")]
    public required int StationEvaNumber { get; set; }

    [Column("schedule_type")]
    public required ScheduleType ScheduleType { get; set; }

    [Column("journey_description")]
    public required string JourneyDescription { get; set; }

    [Column("origin_eva_number")]
    public required int OriginEvaNumber { get; set; }

    [Column("destination_eva_number")]
    public required int DestinationEvaNumber { get; set; }

    [Column("administration_id")]
    public required Guid AdministrationId { get; set; }
}

public class NetworkJourneyQualityHourly : JourneyQualityHourlyBase
{
}

public class JourneyAdministrationQualityHourly : JourneyQualityHourlyBase
{
    [Column("administration_id")]
    public required Guid AdministrationId { get; set; }
}

public class LineJourneyQualityHourly : JourneyQualityHourlyBase
{
    [Column("journey_description")]
    public required string JourneyDescription { get; set; }

    [Column("origin_eva_number")]
    public required int OriginEvaNumber { get; set; }

    [Column("destination_eva_number")]
    public required int DestinationEvaNumber { get; set; }

    [Column("administration_id")]
    public required Guid AdministrationId { get; set; }
}

public class JourneyNumberQualityHourly : JourneyQualityHourlyBase
{
    [Column("journey_number")]
    public required int JourneyNumber { get; set; }

    [Column("journey_description")]
    public required string JourneyDescription { get; set; }

    [Column("origin_eva_number")]
    public required int OriginEvaNumber { get; set; }

    [Column("destination_eva_number")]
    public required int DestinationEvaNumber { get; set; }

    [Column("administration_id")]
    public required Guid AdministrationId { get; set; }
}

[NotMapped]
public abstract class EventQualityHourlyBase : IEventQualityHourly
{
    [Column("bucket_hour")]
    public required DateTime BucketHour { get; set; }

    [Column("transport_type")]
    public required TransportType TransportType { get; set; }

    [Column("is_replacement")]
    public required bool IsReplacement { get; set; }

    [Column("event_count")]
    public required long EventCount { get; set; }

    [Column("stop_cancelled_count")]
    public required long StopCancelledCount { get; set; }

    [Column("event_delay_sum_seconds")]
    public required long EventDelaySumSeconds { get; set; }

    [Column("event_positive_delay_sum_seconds")]
    public required long EventPositiveDelaySumSeconds { get; set; }

    [Column("event_punctual_5_count")]
    public required long EventPunctual5Count { get; set; }

    [Column("event_punctual_15_count")]
    public required long EventPunctual15Count { get; set; }

    [Column("event_late_30_count")]
    public required long EventLate30Count { get; set; }

    [Column("event_late_60_count")]
    public required long EventLate60Count { get; set; }
}

[NotMapped]
public abstract class JourneyQualityHourlyBase : IJourneyQualityHourly
{
    [Column("bucket_hour")]
    public required DateTime BucketHour { get; set; }

    [Column("transport_type")]
    public required TransportType TransportType { get; set; }

    [Column("is_replacement")]
    public required bool IsReplacement { get; set; }

    [Column("journey_count")]
    public required long JourneyCount { get; set; }

    [Column("fully_cancelled_count")]
    public required long FullyCancelledCount { get; set; }

    [Column("partially_cancelled_count")]
    public required long PartiallyCancelledCount { get; set; }

    [Column("destination_not_reached_count")]
    public required long DestinationNotReachedCount { get; set; }

    [Column("destination_delay_sum_seconds")]
    public required long DestinationDelaySumSeconds { get; set; }

    [Column("destination_positive_delay_sum_seconds")]
    public required long DestinationPositiveDelaySumSeconds { get; set; }

    [Column("destination_punctual_5_count")]
    public required long DestinationPunctual5Count { get; set; }

    [Column("destination_punctual_15_count")]
    public required long DestinationPunctual15Count { get; set; }

    [Column("destination_late_30_count")]
    public required long DestinationLate30Count { get; set; }

    [Column("destination_late_60_count")]
    public required long DestinationLate60Count { get; set; }
}
