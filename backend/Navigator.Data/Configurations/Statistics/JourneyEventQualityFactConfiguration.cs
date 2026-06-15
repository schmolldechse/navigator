using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Navigator.Data.Entities.Statistics;

namespace Navigator.Data.Configurations.Statistics;

internal class JourneyEventQualityFactConfiguration : IEntityTypeConfiguration<JourneyEventQualityFact>
{
    public void Configure(EntityTypeBuilder<JourneyEventQualityFact> builder)
    {
        builder.ToTable("journey_event_quality_facts", "statistics", table => table.ExcludeFromMigrations());
        builder.HasKey(fact => new { fact.StopPlaceId, fact.BucketHour });

        builder.HasIndex(fact => new
        {
            fact.JourneyId,
            fact.JourneyDate,
            fact.PlannedTime,
            fact.ScheduleType,
            fact.StationEvaNumber,
            fact.BucketHour
        }).IsUnique();

        builder.Property(fact => fact.JourneyId).HasMaxLength(82);
        builder.Property(fact => fact.JourneyDescription).HasMaxLength(64);
    }
}
