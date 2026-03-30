using Microsoft.EntityFrameworkCore;
using Navigator.Data.Entities.Journey;
using Navigator.Data.Entities.Journey.Message;
using Navigator.Data.Entities.RisId;
using Navigator.Data.Entities.Station;
using Navigator.Data.Entities.Statistics;
using Navigator.Data.Entities.Views;

namespace Navigator.Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    // station
    public DbSet<Station> Stations { get; set; }
    public DbSet<StationRil100> StationRil { get; set; }
    public DbSet<StationTransport> StationTransport { get; set; }
    // risid
    public DbSet<RisId> RisIds { get; set; }
    // statistics
    public DbSet<DatabaseSize> DatabaseSizes { get; set; }
    public DbSet<RisIdSnapshot> RisIdSnapshots { get; set; }
    public DbSet<JourneySnapshot> JourneySnapshots { get; set; }
    // journey
    public DbSet<Administration> Administrations { get; set; }
    public DbSet<Journey> Journeys { get; set; }
    public DbSet<JourneyTransport> JourneyTransports { get; set; }
    public DbSet<JourneyStopPlace> JourneyStopPlaces { get; set; }
    // journey messages
    public DbSet<JourneyMessage> JourneyMessages { get; set; }
    public DbSet<JourneyMessageReference> JourneyMessageReferences { get; set; }
    public DbSet<JourneyStopPlaceMessage> JourneyStopPlaceMessages { get; set; }

    // views
    public DbSet<HourlyStationSnapshot> HourlyStationSnapshots { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresExtension("cube")
            .HasPostgresExtension("earthdistance")
            .HasPostgresExtension("pg_cron");

        modelBuilder.Entity<HourlyStationSnapshot>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("hourly_station_snapshots", "statistics");
            entity.ToTable("hourly_station_snapshots", "statistics", table => table.ExcludeFromMigrations());
        });

        // tables
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

        modelBuilder.Entity<Journey>(entity =>
        {
            entity.HasOne(journey => journey.Transport)
                .WithOne(transport => transport.Journey)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(journey => journey.StopPlaces)
                .WithOne(stopPlace => stopPlace.Journey)
                .HasForeignKey(stopPlace => stopPlace.JourneyId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(journey => journey.Messages)
                .WithOne(message => message.Journey)
                .HasForeignKey(message => message.JourneyId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<JourneyStopPlace>(entity =>
        {
            entity.Property(stopPlace => stopPlace.Delay)
                .HasComputedColumnSql("EXTRACT(EPOCH FROM (actual_time - planned_time))::integer",
                stored: true
            );

            entity.HasIndex(stopPlace => new { stopPlace.StationEvaNumber, stopPlace.PlannedTime })
                .IncludeProperties(stopPlace => new
                {
                    stopPlace.JourneyId,
                    stopPlace.ScheduleType,
                    stopPlace.Delay,
                    stopPlace.Cancelled,
                    stopPlace.Additional,
                    stopPlace.Demand,
                    stopPlace.NoPassengerChange,
                    stopPlace.PlannedPlatform,
                    stopPlace.ActualPlatform
                })
                .HasDatabaseName("IX_journey_stop_places_station_analytics");

            entity.HasMany(stopPlace => stopPlace.Messages)
                .WithOne(join => join.StopPlace)
                .HasForeignKey(join => join.StopPlaceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<JourneyMessage>(entity =>
        {
            entity.HasMany(message => message.References)
                .WithOne(reference => reference.Message)
                .HasForeignKey(reference => reference.MessageId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(message => message.JourneyStopPlaceMessages)
                .WithOne(join => join.Message)
                .HasForeignKey(join => join.MessageId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        base.OnModelCreating(modelBuilder);
    }
}
