using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Navigator.Data.Entities.Journey;

namespace Navigator.Data.Configurations.Journeys;

public class JourneyTransportConfiguration : IEntityTypeConfiguration<JourneyTransport>
{
    public void Configure(EntityTypeBuilder<JourneyTransport> builder)
    {
        builder.ToTable("journey_transports", "core", table => table.ExcludeFromMigrations());
        builder.HasKey(journeyTransport => new { journeyTransport.JourneyId, journeyTransport.Date });

        builder.Property(journeyTransport => journeyTransport.JourneyId)
            .HasMaxLength(82)
            .ValueGeneratedNever();
        builder.Property(journeyTransport => journeyTransport.Category).HasMaxLength(64);
        builder.Property(journeyTransport => journeyTransport.CategoryInternal).HasMaxLength(64);
        builder.Property(journeyTransport => journeyTransport.JourneyDescription).HasMaxLength(64);
        builder.Property(journeyTransport => journeyTransport.Label).HasMaxLength(64);

        builder.HasOne(journeyTransport => journeyTransport.Journey)
            .WithOne(journey => journey.Transport)
            .HasForeignKey<JourneyTransport>(journeyTransport => new { journeyTransport.JourneyId, journeyTransport.Date })
            .OnDelete(DeleteBehavior.Cascade);
    }
}
