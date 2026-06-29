using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Navigator.Data.Entities.Statistics;

namespace Navigator.Data.Configurations.Statistics;

internal class JourneyFactProjectionBacklogConfiguration : IEntityTypeConfiguration<JourneyFactProjectionBacklog>
{
    public void Configure(EntityTypeBuilder<JourneyFactProjectionBacklog> builder)
    {
        builder.ToTable("journey_fact_projection_backlog", "statistics", table => table.ExcludeFromMigrations());
        builder.HasKey(entry => new { entry.JourneyId, entry.Date });

        builder.Property(entry => entry.JourneyId).HasMaxLength(82);
        builder.Property(entry => entry.LockedBy).HasMaxLength(128);
        builder.Property(entry => entry.LastError).HasMaxLength(2048);
    }
}
