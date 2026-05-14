using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Navigator.Data.Entities.Views;

namespace Navigator.Data.Configurations.Statistics;

public class DailyJourneyServiceSnapshotConfiguration : IEntityTypeConfiguration<DailyJourneyServiceSnapshot>
{
    public void Configure(EntityTypeBuilder<DailyJourneyServiceSnapshot> builder)
    {
        builder.ToTable("daily_journey_service_snapshots", "statistics", table => table.ExcludeFromMigrations());
        builder.HasNoKey();

        builder.ToView("daily_journey_service_snapshots", "statistics");
    }
}
