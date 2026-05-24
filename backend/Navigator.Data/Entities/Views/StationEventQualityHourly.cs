using Navigator.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Views;

public class StationEventQualityHourly
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

    [Column("is_replacement_transport")]
    public required bool IsReplacementTransport { get; set; }

    [Column("event_count")]
    public required long EventCount { get; set; }

    [Column("cancelled_count")]
    public required long CancelledCount { get; set; }

    [Column("delay_sample_count")]
    public required long DelaySampleCount { get; set; }

    [Column("delay_sum_seconds")]
    public required long DelaySumSeconds { get; set; }

    [Column("punctual_5_count")]
    public required long Punctual5Count { get; set; }

    [Column("punctual_15_count")]
    public required long Punctual15Count { get; set; }
}
