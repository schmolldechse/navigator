using Navigator.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Views;

public class DailyStationMessageSnapshot
{
    [Column("bucket_day")]
    public required DateTime BucketDay { get; set; }

    [Column("eva_number")]
    public required int EvaNumber { get; set; }

    [Column("transport_type")]
    public required TransportType TransportType { get; set; }

    [Column("message_type")]
    public required MessageType MessageType { get; set; }

    [Column("message_code")]
    public required string MessageCode { get; set; }

    [Column("disruption_cause")]
    public required string DisruptionCause { get; set; }

    [Column("disruption_effect")]
    public required string DisruptionEffect { get; set; }

    [Column("note_category")]
    public required string NoteCategory { get; set; }

    [Column("message_count")]
    public required int MessageCount { get; set; }

    [Column("affected_stop_place_count")]
    public required int AffectedStopPlaceCount { get; set; }

    [Column("affected_journey_count")]
    public required int AffectedJourneyCount { get; set; }
}
