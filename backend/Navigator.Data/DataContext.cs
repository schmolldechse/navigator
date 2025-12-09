using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Navigator.Data.Entities.RisId;
using Navigator.Data.Entities.Station;
using Navigator.Data.Entities.Statistics;
using Navigator.Data.Enums;

namespace Navigator.Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<Station> Stations { get; set; }
    public DbSet<StationRil100> StationRil { get; set; }
    public DbSet<StationTransport> StationTransport { get; set; }
    public DbSet<RisId> RisIds { get; set; }
    public DbSet<DatabaseSize> DatabaseSizes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("cube")
            .HasPostgresExtension("earthdistance");

        modelBuilder.Entity<Station>(entity =>
        {
            entity.HasMany(station => station.Ril100)
                .WithOne(ril => ril.Station)
                .HasForeignKey(ril => ril.EvaNumber)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(station => station.Transports)
                .WithOne(transport => transport.Station)
                .HasForeignKey(transport => transport.EvaNumber)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<StationTransport>(entity =>
        {
            entity.Property(transport => transport.TransportType)
                .HasConversion(new ValueConverter<TransportType, string>(
                    v => TransportTypeToString(v),
                    v => StringToTransportType(v)
                ));
        });

        base.OnModelCreating(modelBuilder);
    }

    private string TransportTypeToString(TransportType transportType) => transportType switch { 
        TransportType.HighSpeedTrain => "HIGH_SPEED_TRAIN",
        TransportType.IntercityTrain => "INTERCITY_TRAIN",
        TransportType.InterRegionalTrain => "INTER_REGIONAL_TRAIN",
        TransportType.RegionalTrain => "REGIONAL_TRAIN",
        TransportType.CityTrain => "CITY_TRAIN",
        TransportType.Subway => "SUBWAY",
        TransportType.Tram => "TRAM",
        TransportType.Bus => "BUS",
        TransportType.Ferry => "FERRY",
        TransportType.Flight => "FLIGHT",
        TransportType.Car => "CAR",
        TransportType.Taxi => "TAXI",
        TransportType.Shuttle => "SHUTTLE",
        TransportType.Bike => "BIKE",
        TransportType.Scooter => "SCOOTER",
        TransportType.Walk => "WALK",
        _ => "UNKNOWN",
    };

    private TransportType StringToTransportType(string transport)
    {
        if (string.IsNullOrWhiteSpace(transport)) return TransportType.Unknown;
        return transport switch
        {
            "HIGH_SPEED_TRAIN" => TransportType.HighSpeedTrain,
            "INTERCITY_TRAIN" => TransportType.IntercityTrain,
            "INTER_REGIONAL_TRAIN" => TransportType.InterRegionalTrain,
            "REGIONAL_TRAIN" => TransportType.RegionalTrain,
            "CITY_TRAIN" => TransportType.CityTrain,
            "SUBWAY" => TransportType.Subway,
            "TRAM" => TransportType.Tram,
            "BUS" => TransportType.Bus,
            "FERRY" => TransportType.Ferry,
            "FLIGHT" => TransportType.Flight,
            "CAR" => TransportType.Car,
            "TAXI" => TransportType.Taxi,
            "SHUTTLE" => TransportType.Shuttle,
            "BIKE" => TransportType.Bike,
            "SCOOTER" => TransportType.Scooter,
            "WALK" => TransportType.Walk,
            _ => TransportType.Unknown,
        };
    }
}
