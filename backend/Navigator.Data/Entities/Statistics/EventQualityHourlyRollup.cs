using System.ComponentModel.DataAnnotations.Schema;
using Navigator.Data.Enums;

namespace Navigator.Data.Entities.Statistics;

public sealed class EventQualityHourlyRollup
{
    [Column("bucket_hour")]
    public required DateTime BucketHour { get; set; }

    [Column("station_eva_number")]
    public required int StationEvaNumber { get; set; }

    [Column("schedule_type")]
    public required ScheduleType ScheduleType { get; set; }

    [Column("administration_id")]
    public required Guid AdministrationId { get; set; }

    [Column("transport_type")]
    public required TransportType TransportType { get; set; }

    [Column("journey_description")]
    public required string JourneyDescription { get; set; }

    [Column("journey_number")]
    public required int JourneyNumber { get; set; }

    [Column("origin_eva_number")]
    public required int OriginEvaNumber { get; set; }

    [Column("destination_eva_number")]
    public required int DestinationEvaNumber { get; set; }

    [Column("is_replacement")]
    public required bool IsReplacement { get; set; }

    [Column("event_count")]
    public required long EventCount { get; set; }

    [Column("cancelled_count")]
    public required long CancelledCount { get; set; }

    [Column("delay_sum_seconds")]
    public required long DelaySumSeconds { get; set; }

    [Column("delayed_count")]
    public required long DelayedCount { get; set; }

    [Column("punctual_count")]
    public required long PunctualCount { get; set; }

    [Column("delay_less_than_5_minutes_count")]
    public required long DelayLessThan5MinutesCount { get; set; }

    [Column("delay_5_to_10_minutes_count")]
    public required long Delay5To10MinutesCount { get; set; }

    [Column("delay_10_to_15_minutes_count")]
    public required long Delay10To15MinutesCount { get; set; }

    [Column("delay_15_to_30_minutes_count")]
    public required long Delay15To30MinutesCount { get; set; }

    [Column("delay_30_to_60_minutes_count")]
    public required long Delay30To60MinutesCount { get; set; }

    [Column("delay_more_than_60_minutes_count")]
    public required long DelayMoreThan60MinutesCount { get; set; }

    [Column("refreshed_at")]
    public required DateTime RefreshedAt { get; set; }
}
