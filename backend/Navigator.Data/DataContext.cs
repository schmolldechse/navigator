using Microsoft.EntityFrameworkCore;
using Navigator.Data.Entities;
using Navigator.Data.Entities.Journey;
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
    public DbSet<EventQualityHourlyRollup> EventQualityHourlyRollups { get; set; }
    public DbSet<JourneyQualityHourlyRollup> JourneyQualityHourlyRollups { get; set; }
    public DbSet<StatisticsRefreshProgress> StatisticsRefreshProgress { get; set; }
    public DbSet<StationJourneyEventDetail> StationJourneyEventDetails { get; set; }
    public DbSet<JourneyQualityDetail> JourneyQualityDetails { get; set; }
    public DbSet<NetworkEventQualityHourly> NetworkEventQualities { get; set; }
    public DbSet<NetworkEventDelayDistributionHourly> NetworkEventDelayDistributions { get; set; }
    public DbSet<NetworkJourneyQualityHourly> NetworkJourneyQualities { get; set; }
    public DbSet<NetworkJourneyOutcomeHourly> NetworkJourneyOutcomes { get; set; }
    public DbSet<StationEventQualityHourly> StationEventQualities { get; set; }
    public DbSet<StationAdministrationQualityHourly> StationAdministrationQualities { get; set; }
    public DbSet<LineEventQualityHourly> LineEventQualities { get; set; }
    public DbSet<StationLineQualityHourly> StationLineQualities { get; set; }
    public DbSet<JourneyAdministrationQualityHourly> JourneyAdministrationQualities { get; set; }
    public DbSet<LineJourneyQualityHourly> LineJourneyQualities { get; set; }
    public DbSet<JourneyNumberQualityHourly> JourneyNumberQualities { get; set; }
    // journey
    public DbSet<Administration> Administrations { get; set; }
    public DbSet<Journey> Journeys { get; set; }
    public DbSet<JourneyTransport> JourneyTransports { get; set; }
    public DbSet<JourneyStopPlace> JourneyStopPlaces { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("cube")
            .HasPostgresExtension("earthdistance")
            .HasPostgresExtension("timescaledb");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
