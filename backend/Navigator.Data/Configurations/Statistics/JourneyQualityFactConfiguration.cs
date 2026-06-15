using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Navigator.Data.Entities.Statistics;

namespace Navigator.Data.Configurations.Statistics;

internal class JourneyQualityFactConfiguration : IEntityTypeConfiguration<JourneyQualityFact>
{
    public void Configure(EntityTypeBuilder<JourneyQualityFact> builder)
    {
        builder.ToTable("journey_quality_facts", "statistics", table => table.ExcludeFromMigrations());
        builder.HasKey(fact => new { fact.JourneyId, fact.JourneyDate, fact.BucketHour });

        builder.Property(fact => fact.JourneyId).HasMaxLength(82);
        builder.Property(fact => fact.JourneyDescription).HasMaxLength(64);
    }
}
