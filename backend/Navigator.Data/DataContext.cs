using Microsoft.EntityFrameworkCore;
using Navigator.Data.Entities;
using Navigator.Data.Entities.Journey;
using Navigator.Data.Entities.Journey.Message;
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
    public DbSet<DatabaseSizeSnapshot> DatabaseSizeSnapshots { get; set; }
    public DbSet<RisIdSnapshot> RisIdSnapshots { get; set; }
    public DbSet<JourneySnapshot> JourneySnapshots { get; set; }
    public DbSet<HourlyStationSnapshot> HourlyStationSnapshots { get; set; }
    // journey
    public DbSet<Administration> Administrations { get; set; }
    public DbSet<Journey> Journeys { get; set; }
    public DbSet<JourneyTransport> JourneyTransports { get; set; }
    public DbSet<JourneyStopPlace> JourneyStopPlaces { get; set; }
    public DbSet<JourneyMessage> JourneyMessages { get; set; }
    public DbSet<JourneyStopPlaceMessage> JourneyStopPlaceMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("cube")
            .HasPostgresExtension("earthdistance")
            .HasPostgresExtension("partman", "pg_partman")
            .HasPostgresExtension("pg_cron");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
