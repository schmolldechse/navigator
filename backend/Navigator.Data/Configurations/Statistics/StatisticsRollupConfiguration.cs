using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Navigator.Data.Entities.Statistics;
using Navigator.Data.Enums;

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

internal sealed class StatisticsRefreshQueueItemConfiguration : IEntityTypeConfiguration<StatisticsRefreshQueueItem>
{
    public void Configure(EntityTypeBuilder<StatisticsRefreshQueueItem> builder)
    {
        builder.ToTable("statistics_refresh_queue", "statistics", table => table.ExcludeFromMigrations());
        builder.HasKey(row => new { row.WindowStart, row.WindowEnd });
        builder.Property(row => row.Status).HasColumnType("statistics.statistics_refresh_queue_status");
        builder.Property(row => row.Source).HasColumnType("statistics.statistics_refresh_queue_source");
        builder.Property(row => row.ErrorKind).HasMaxLength(128);
        builder.HasIndex(row => new { row.Status, row.WindowStart, row.WindowEnd });
        builder.HasIndex(row => row.LastMarkedAt);
    }
}

internal sealed class RisIdReactivationHoldConfiguration : IEntityTypeConfiguration<RisIdReactivationHold>
{
    public void Configure(EntityTypeBuilder<RisIdReactivationHold> builder)
    {
        builder.ToTable("ris_id_reactivation_holds", "statistics", table => table.ExcludeFromMigrations());
        builder.HasKey(row => row.RisId);
        builder.Property(row => row.RisId).HasMaxLength(73);
        builder.HasIndex(row => row.ProtectUntil);
    }
}
