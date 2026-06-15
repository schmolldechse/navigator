using Navigator.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Views;

public class JourneyQualityDetail
{
    [Column("bucket_hour")]
    public required DateTime BucketHour { get; set; }

    [Column("journey_id")]
    public required string JourneyId { get; set; }

    [Column("journey_date")]
    public required DateOnly JourneyDate { get; set; }

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

    [Column("journey_start_time")]
    public required DateTime JourneyStartTime { get; set; }

    [Column("journey_end_time")]
    public required DateTime JourneyEndTime { get; set; }

    [Column("destination_delay_seconds")]
    public int? DestinationDelaySeconds { get; set; }

    [Column("fully_cancelled")]
    public required bool FullyCancelled { get; set; }

    [Column("partially_cancelled")]
    public required bool PartiallyCancelled { get; set; }

    [Column("destination_not_reached")]
    public required bool DestinationNotReached { get; set; }
}
