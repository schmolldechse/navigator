using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Navigator.Data.Entities.Views;

namespace Navigator.Data.Configurations.Statistics;

public class DailyStationMessageSnapshotConfiguration : IEntityTypeConfiguration<DailyStationMessageSnapshot>
{
    public void Configure(EntityTypeBuilder<DailyStationMessageSnapshot> builder)
    {
        builder.ToTable("daily_station_message_snapshots", "statistics", table => table.ExcludeFromMigrations());
        builder.HasNoKey();

        builder.ToView("daily_station_message_snapshots", "statistics");
    }
}
