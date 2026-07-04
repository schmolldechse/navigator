using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Navigator.Data.Entities.Statistics;

namespace Navigator.Data.Configurations.Statistics;

internal sealed class EventQualityHourlyRollupConfiguration : IEntityTypeConfiguration<EventQualityHourlyRollup>
{
    public void Configure(EntityTypeBuilder<EventQualityHourlyRollup> builder)
    {
        builder.ToTable("event_quality_hourly_rollups", "statistics", table => table.ExcludeFromMigrations());

        builder.HasKey(row => new
        {
            row.BucketHour,
            row.StationEvaNumber,
            row.ScheduleType,
            row.AdministrationId,
            row.TransportType,
            row.JourneyDescription,
            row.JourneyNumber,
            row.OriginEvaNumber,
            row.DestinationEvaNumber,
            row.IsReplacement
        });

        builder.Property(row => row.ScheduleType).HasColumnType("core.schedule_type");
        builder.Property(row => row.TransportType).HasColumnType("core.transport_type");
        builder.Property(row => row.JourneyDescription).HasMaxLength(64);
    }
}

internal sealed class JourneyQualityHourlyRollupConfiguration : IEntityTypeConfiguration<JourneyQualityHourlyRollup>
{
    public void Configure(EntityTypeBuilder<JourneyQualityHourlyRollup> builder)
    {
        builder.ToTable("journey_quality_hourly_rollups", "statistics", table => table.ExcludeFromMigrations());

        builder.HasKey(row => new
        {
            row.BucketHour,
            row.AdministrationId,
            row.TransportType,
            row.JourneyDescription,
            row.JourneyNumber,
            row.OriginEvaNumber,
            row.DestinationEvaNumber,
            row.IsReplacement
        });

        builder.Property(row => row.TransportType).HasColumnType("core.transport_type");
        builder.Property(row => row.JourneyDescription).HasMaxLength(64);
    }
}

internal sealed class StatisticsRefreshProgressConfiguration : IEntityTypeConfiguration<StatisticsRefreshProgress>
{
    public void Configure(EntityTypeBuilder<StatisticsRefreshProgress> builder)
    {
        builder.ToTable("statistics_refresh_progress", "statistics", table => table.ExcludeFromMigrations());
        builder.HasKey(row => new { row.Operation, row.WindowStart, row.WindowEnd });
        builder.Property(row => row.Operation).HasMaxLength(64);
        builder.Property(row => row.Status).HasMaxLength(32);
        builder.Property(row => row.ErrorKind).HasMaxLength(128);
    }
}
