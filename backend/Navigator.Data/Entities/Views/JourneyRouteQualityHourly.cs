using Navigator.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Views;

public class JourneyRouteQualityHourly
{
    [Column("bucket_hour")]
    public required DateTime BucketHour { get; set; }

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

    [Column("first_planned_time")]
    public required DateTime FirstPlannedTime { get; set; }

    [Column("last_planned_time")]
    public required DateTime LastPlannedTime { get; set; }

    [Column("journey_count")]
    public required long JourneyCount { get; set; }

    [Column("journey_cancelled_count")]
    public required long JourneyCancelledCount { get; set; }

    [Column("delay_sample_count")]
    public required long DelaySampleCount { get; set; }

    [Column("delay_sum_seconds")]
    public required long DelaySumSeconds { get; set; }

    [Column("punctual_5_count")]
    public required long Punctual5Count { get; set; }

    [Column("punctual_15_count")]
    public required long Punctual15Count { get; set; }
}
