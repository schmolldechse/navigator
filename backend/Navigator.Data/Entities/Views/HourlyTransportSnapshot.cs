using Navigator.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Views;

public class HourlyTransportSnapshot
{
    [Column("bucket_hour")]
    public required DateTime BucketHour { get; set; }

    [Column("transport_type")]
    public required TransportType TransportType { get; set; }

    // --- ARRIVALS ---
    [Column("arrival_count")]
    public required int ArrivalCount { get; set; }

    [Column("arrival_cancellation_count")]
    public required int ArrivalCancellationCount { get; set; }

    [Column("arrival_delay_sum")]
    public required double ArrivalDelaySum { get; set; }

    // --- DEPARTURES ---
    [Column("departure_count")]
    public required int DepartureCount { get; set; }

    [Column("departure_cancellation_count")]
    public required int DepartureCancellationCount { get; set; }

    [Column("departure_delay_sum")]
    public required double DepartureDelaySum { get; set; }
}
