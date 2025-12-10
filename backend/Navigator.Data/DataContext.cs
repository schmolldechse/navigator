using Microsoft.EntityFrameworkCore;
using Navigator.Data.Entities.Journey;
using Navigator.Data.Entities.RisId;
using Navigator.Data.Entities.Station;
using Navigator.Data.Entities.Statistics;

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
    // journey
    public DbSet<Administration> Administrations { get; set; }
    public DbSet<Journey> Journeys { get; set; }
    public DbSet<JourneyTransport> JourneyTransports { get; set; }
    public DbSet<JourneyScheduledStopPlace> JourneyScheduledStopPlaces { get; set; }
    public DbSet<JourneyStopPlaceInformation> JourneyStopPlaceInformation { get; set; }

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

        modelBuilder.Entity<Journey>(entity =>
        {
            entity.HasOne(journey => journey.Transport)
                .WithOne(transport => transport.Journey)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(journey => journey.ScheduledStopPlaces)
                .WithOne(stopPlace => stopPlace.Journey)
                .HasForeignKey(stopPlace => stopPlace.JourneyId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<JourneyScheduledStopPlace>(entity =>
        {
            entity.Property(stopPlace => stopPlace.Delay)
                .HasComputedColumnSql("EXTRACT(EPOCH FROM (actual_time - planned_time))::integer", stored: true);

            entity.HasMany(stopPlace => stopPlace.Informations)
                .WithOne(info => info.ScheduledStopPlace)
                .HasForeignKey(info => info.ScheduledStopPlaceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        base.OnModelCreating(modelBuilder);
    }
}
