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
    public DbSet<JourneyFactProjectionBacklog> JourneyFactProjectionBacklog { get; set; }
    public DbSet<JourneyEventQualityFact> JourneyEventQualityFacts { get; set; }
    public DbSet<JourneyRouteQualityFact> JourneyRouteQualityFacts { get; set; }
    public DbSet<JourneyRouteQualityHourly> JourneyRouteQualities { get; set; }
    public DbSet<StationLineRouteQualityHourly> StationLineRouteQualities { get; set; }
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
            .HasPostgresExtension("timescaledb");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    public override int SaveChanges()
    {
        AddJourneyFactProjectionBacklogEntries();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AddJourneyFactProjectionBacklogEntries();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void AddJourneyFactProjectionBacklogEntries()
    {
        var currentTime = DateTime.UtcNow;
        var existingBacklogKeys = ChangeTracker.Entries<JourneyFactProjectionBacklog>()
            .Select(entry => (entry.Entity.JourneyId, entry.Entity.Date))
            .ToHashSet();

        var newJourneyKeys = ChangeTracker.Entries<Journey>()
            .Where(entry => entry.State == EntityState.Added)
            .Select(entry => (entry.Entity.Id, entry.Entity.Date))
            .Distinct()
            .Where(key => !existingBacklogKeys.Contains(key))
            .ToList();

        foreach (var (journeyId, date) in newJourneyKeys)
        {
            JourneyFactProjectionBacklog.Add(new JourneyFactProjectionBacklog
            {
                JourneyId = journeyId,
                Date = date,
                CreatedAt = currentTime,
                AvailableAt = currentTime,
                Attempts = 0
            });
        }
    }
}
