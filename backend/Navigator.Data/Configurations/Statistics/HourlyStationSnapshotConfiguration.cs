using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Navigator.Data.Entities.Views;

namespace Navigator.Data.Configurations.Statistics;

public class HourlyStationSnapshotConfiguration : IEntityTypeConfiguration<HourlyStationSnapshot>
{
    public void Configure(EntityTypeBuilder<HourlyStationSnapshot> builder)
    {
        builder.ToTable("hourly_station_snapshots", "statistics", table => table.ExcludeFromMigrations());
        builder.HasNoKey();

        builder.ToView("hourly_station_snapshots", "statistics");
    }
}