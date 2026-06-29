using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Navigator.Data.Entities.Statistics;

namespace Navigator.Data.Configurations.Statistics;

internal class JourneyEventQualityFactConfiguration : IEntityTypeConfiguration<JourneyEventQualityFact>
{
    public void Configure(EntityTypeBuilder<JourneyEventQualityFact> builder)
    {
        builder.ToTable("journey_event_quality_facts", "statistics", table => table.ExcludeFromMigrations());
        builder.HasKey(fact => new { fact.StopPlaceId, fact.PlannedTime });

        builder.Property(fact => fact.JourneyId).HasMaxLength(82);
        builder.Property(fact => fact.JourneyDescription).HasMaxLength(64);
    }
}
