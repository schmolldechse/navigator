using System.ComponentModel.DataAnnotations.Schema;
using Navigator.Data.Enums;

namespace Navigator.Data.Entities.Statistics;

public sealed class JourneyQualityHourlyRollup
{
    [Column("bucket_hour")]
    public required DateTime BucketHour { get; set; }

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

    [Column("journey_count")]
    public required long JourneyCount { get; set; }

    [Column("fully_cancelled_count")]
    public required long FullyCancelledCount { get; set; }

    [Column("partially_cancelled_count")]
    public required long PartiallyCancelledCount { get; set; }

    [Column("destination_reached_count")]
    public required long DestinationReachedCount { get; set; }

    [Column("destination_not_reached_count")]
    public required long DestinationNotReachedCount { get; set; }

    [Column("completed_count")]
    public required long CompletedCount { get; set; }

    [Column("partially_cancelled_destination_reached_count")]
    public required long PartiallyCancelledDestinationReachedCount { get; set; }

    [Column("destination_not_reached_without_full_cancel_count")]
    public required long DestinationNotReachedWithoutFullCancelCount { get; set; }

    [Column("destination_delay_sum_seconds")]
    public required long DestinationDelaySumSeconds { get; set; }

    [Column("destination_delayed_count")]
    public required long DestinationDelayedCount { get; set; }

    [Column("destination_punctual_count")]
    public required long DestinationPunctualCount { get; set; }

    [Column("refreshed_at")]
    public required DateTime RefreshedAt { get; set; }
}
