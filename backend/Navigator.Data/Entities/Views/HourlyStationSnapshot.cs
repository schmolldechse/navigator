using Navigator.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Navigator.Data.Entities.Views;

public class HourlyStationSnapshot
{
    [Column("bucket_hour")]
    public required DateTime BucketHour { get; set; }

    [Column("eva_number")]
    public required int EvaNumber { get; set; }

    [Column("transport_type")]
    public required TransportType TransportType { get; set; }

    // --- ARRIVALS ---
    [Column("arrival_count")]
    public required int ArrivalCount { get; set; }

    [Column("arrival_cancellation_count")]
    public required int ArrivalCancellationCount { get; set; }

    [Column("arrival_delay_sum")]
    public required double ArrivalDelaySum { get; set; }

    [Column("arrival_delay_sample_count")]
    public required int ArrivalDelaySampleCount { get; set; }

    [Column("arrival_punctual_count")]
    public required int ArrivalPunctualCount { get; set; }

    [Column("arrival_delay_minor_count")]
    public required int ArrivalDelayMinorCount { get; set; }

    [Column("arrival_delay_major_count")]
    public required int ArrivalDelayMajorCount { get; set; }

    [Column("arrival_delay_severe_count")]
    public required int ArrivalDelaySevereCount { get; set; }

    [Column("arrival_platform_change_count")]
    public required int ArrivalPlatformChangeCount { get; set; }

    [Column("arrival_additional_count")]
    public required int ArrivalAdditionalCount { get; set; }

    [Column("arrival_demand_count")]
    public required int ArrivalDemandCount { get; set; }

    [Column("arrival_no_passenger_change_count")]
    public required int ArrivalNoPassengerChangeCount { get; set; }

    // --- DEPARTURES ---
    [Column("departure_count")]
    public required int DepartureCount { get; set; }

    [Column("departure_cancellation_count")]
    public required int DepartureCancellationCount { get; set; }

    [Column("departure_delay_sum")]
    public required double DepartureDelaySum { get; set; }

    [Column("departure_delay_sample_count")]
    public required int DepartureDelaySampleCount { get; set; }

    [Column("departure_punctual_count")]
    public required int DeparturePunctualCount { get; set; }

    [Column("departure_delay_minor_count")]
    public required int DepartureDelayMinorCount { get; set; }

    [Column("departure_delay_major_count")]
    public required int DepartureDelayMajorCount { get; set; }

    [Column("departure_delay_severe_count")]
    public required int DepartureDelaySevereCount { get; set; }

    [Column("departure_platform_change_count")]
    public required int DeparturePlatformChangeCount { get; set; }

    [Column("departure_additional_count")]
    public required int DepartureAdditionalCount { get; set; }

    [Column("departure_demand_count")]
    public required int DepartureDemandCount { get; set; }

    [Column("departure_no_passenger_change_count")]
    public required int DepartureNoPassengerChangeCount { get; set; }
}
