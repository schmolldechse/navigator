using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Navigator.Data.Entities.Views;

namespace Navigator.Data.Configurations.Statistics;

internal class StationEventQualityHourlyConfiguration : IEntityTypeConfiguration<StationEventQualityHourly>
{
    public void Configure(EntityTypeBuilder<StationEventQualityHourly> builder)
    {
        builder.ToTable("station_event_quality_hourly", "statistics", table => table.ExcludeFromMigrations());
        builder.HasNoKey();

        builder.ToView("station_event_quality_hourly", "statistics");
    }
}
