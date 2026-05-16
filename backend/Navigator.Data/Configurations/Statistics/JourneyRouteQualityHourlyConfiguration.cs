using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Navigator.Data.Entities.Views;

namespace Navigator.Data.Configurations.Statistics;

internal class JourneyRouteQualityHourlyConfiguration : IEntityTypeConfiguration<JourneyRouteQualityHourly>
{
    public void Configure(EntityTypeBuilder<JourneyRouteQualityHourly> builder)
    {
        builder.ToTable("journey_route_quality_hourly", "statistics", table => table.ExcludeFromMigrations());
        builder.HasNoKey();

        builder.ToView("journey_route_quality_hourly", "statistics");
    }
}
