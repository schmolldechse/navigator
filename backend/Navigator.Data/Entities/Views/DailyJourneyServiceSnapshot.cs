using Navigator.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Views;

public class DailyJourneyServiceSnapshot
{
    [Column("bucket_day")]
    public required DateTime BucketDay { get; set; }

    [Column("transport_type")]
    public required TransportType TransportType { get; set; }

    [Column("journey_type")]
    public required JourneyType JourneyType { get; set; }

    [Column("operator_code")]
    public required string OperatorCode { get; set; }

    [Column("operator_name")]
    public required string OperatorName { get; set; }

    [Column("journey_count")]
    public required int JourneyCount { get; set; }

    [Column("journey_cancellation_count")]
    public required int JourneyCancellationCount { get; set; }

    [Column("replacement_transport_count")]
    public required int ReplacementTransportCount { get; set; }

    [Column("stop_count")]
    public required int StopCount { get; set; }

    [Column("arrival_stop_count")]
    public required int ArrivalStopCount { get; set; }

    [Column("departure_stop_count")]
    public required int DepartureStopCount { get; set; }

    [Column("cancelled_stop_count")]
    public required int CancelledStopCount { get; set; }

    [Column("delay_sum")]
    public required double DelaySum { get; set; }

    [Column("delay_sample_count")]
    public required int DelaySampleCount { get; set; }

    [Column("message_count")]
    public required int MessageCount { get; set; }

    [Column("disruption_message_count")]
    public required int DisruptionMessageCount { get; set; }
}
