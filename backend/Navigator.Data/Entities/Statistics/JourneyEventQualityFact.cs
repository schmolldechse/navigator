using Navigator.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Statistics;

/// <summary>
/// Derived event-level analytics row used as the source for TimescaleDB continuous aggregates.
/// Raw journey tables remain the source of truth.
/// </summary>
public class JourneyEventQualityFact
{
    [Column("stop_place_id")]
    public Guid StopPlaceId { get; set; }

    [Column("journey_id")]
    public required string JourneyId { get; set; }

    [Column("date")]
    public required DateOnly Date { get; set; }

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

    [Column("number")]
    public required int Number { get; set; }

    [Column("is_replacement_transport")]
    public required bool IsReplacementTransport { get; set; }

    [Column("origin_eva_number")]
    public required int OriginEvaNumber { get; set; }

    [Column("destination_eva_number")]
    public required int DestinationEvaNumber { get; set; }

    [Column("cancelled")]
    public required bool Cancelled { get; set; }

    [Column("delay")]
    public required int Delay { get; set; }
}
