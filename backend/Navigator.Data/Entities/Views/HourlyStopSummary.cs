using Navigator.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Views;

public class HourlyStopSummary
{
    [Column("bucket_hour")]
    public required DateTime BucketHour { get; set; }

    [Column("transport_type")]
    public required TransportType TransportType { get; set; }

    // --- ARRIVALS ---
    [Column("arrivals_count")]
    public required int ArrivalsCount { get; set; }

    [Column("arrival_cancellations_count")]
    public required int ArrivalCancellationCount { get; set; }

    [Column("arrival_delay_sum")]
    public required double ArrivalDelaySum { get; set; }

    [Column("arrival_delay_avg")]
    public required double ArrivalDelayAvg { get; set; }

    // --- DEPARTURES ---
    [Column("departures_count")]
    public required int DeparturesCount { get; set; }

    [Column("departure_cancellations_count")]
    public required int DepartureCancellationCount { get; set; }

    [Column("departure_delay_sum")]
    public required double DepartureDelaySum { get; set; }

    [Column("departure_delay_avg")]
    public required double DepartureDelayAvg { get; set; }
}
