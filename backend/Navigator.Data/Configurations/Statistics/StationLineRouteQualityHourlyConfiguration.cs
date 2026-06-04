using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Navigator.Data.Entities.Views;

namespace Navigator.Data.Configurations.Statistics;

internal class StationLineRouteQualityHourlyConfiguration : IEntityTypeConfiguration<StationLineRouteQualityHourly>
{
    public void Configure(EntityTypeBuilder<StationLineRouteQualityHourly> builder)
    {
        builder.ToTable("station_line_route_quality_hourly", "statistics", table => table.ExcludeFromMigrations());
        builder.HasNoKey();

        builder.ToView("station_line_route_quality_hourly", "statistics");
    }
}
