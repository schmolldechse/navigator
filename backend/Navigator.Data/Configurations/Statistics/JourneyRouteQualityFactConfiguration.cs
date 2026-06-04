using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Navigator.Data.Entities.Statistics;

namespace Navigator.Data.Configurations.Statistics;

internal class JourneyRouteQualityFactConfiguration : IEntityTypeConfiguration<JourneyRouteQualityFact>
{
    public void Configure(EntityTypeBuilder<JourneyRouteQualityFact> builder)
    {
        builder.ToTable("journey_route_quality_facts", "statistics", table => table.ExcludeFromMigrations());
        builder.HasKey(fact => new { fact.JourneyId, fact.Date, fact.JourneyStartTime });

        builder.Property(fact => fact.JourneyId).HasMaxLength(82);
        builder.Property(fact => fact.JourneyDescription).HasMaxLength(64);
    }
}
