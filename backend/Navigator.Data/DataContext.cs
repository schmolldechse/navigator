using Microsoft.EntityFrameworkCore;
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

        base.OnModelCreating(modelBuilder);
    }
}
