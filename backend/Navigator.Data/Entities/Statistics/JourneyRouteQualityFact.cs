using Navigator.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Statistics;

/// <summary>
/// Derived journey-level analytics row used as the source for TimescaleDB continuous aggregates.
/// Raw journey tables remain the source of truth.
/// </summary>
public class JourneyRouteQualityFact
{
    [Column("journey_id")]
    public required string JourneyId { get; set; }

    [Column("date")]
    public required DateOnly Date { get; set; }

    [Column("journey_start_time")]
    public required DateTime JourneyStartTime { get; set; }

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

    [Column("journey_cancelled")]
    public required bool JourneyCancelled { get; set; }

    [Column("terminal_delay_seconds")]
    public int? TerminalDelaySeconds { get; set; }
}
