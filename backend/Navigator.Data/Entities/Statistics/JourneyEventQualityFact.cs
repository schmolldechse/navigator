using Navigator.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Statistics;

/// <summary>
/// Derived event-level analytics row used as the source for TimescaleDB continuous aggregates.
/// Raw journey tables remain the source of truth.
/// </summary>
public class JourneyEventQualityFact
{
    [Column("bucket_hour")]
    public required DateTime BucketHour { get; set; }

    [Column("stop_place_id")]
    public Guid StopPlaceId { get; set; }

    [Column("journey_id")]
    public required string JourneyId { get; set; }

    [Column("journey_date")]
    public required DateOnly JourneyDate { get; set; }

    [Column("planned_time")]
    public required DateTime PlannedTime { get; set; }

    [Column("journey_start_time")]
    public required DateTime JourneyStartTime { get; set; }

    [Column("journey_end_time")]
    public required DateTime JourneyEndTime { get; set; }

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

    [Column("is_replacement")]
    public required bool IsReplacement { get; set; }

    [Column("origin_eva_number")]
    public required int OriginEvaNumber { get; set; }

    [Column("destination_eva_number")]
    public required int DestinationEvaNumber { get; set; }

    [Column("stop_cancelled")]
    public required bool StopCancelled { get; set; }

    [Column("event_delay_seconds")]
    public required int EventDelaySeconds { get; set; }

}
