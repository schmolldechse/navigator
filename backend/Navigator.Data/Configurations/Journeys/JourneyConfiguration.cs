using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Navigator.Data.Entities.Journey;

namespace Navigator.Data.Configurations.Journeys;

public class JourneyConfiguration : IEntityTypeConfiguration<Journey>
{
    public void Configure(EntityTypeBuilder<Journey> builder)
    {
        builder.ToTable("journeys", "core", table => table.ExcludeFromMigrations());
        builder.HasKey(journey => new { journey.Id, journey.Date });

        builder.Property(journey => journey.Id).HasMaxLength(82);

        builder.HasOne(journey => journey.Administration)
            .WithMany()
            .HasForeignKey(journey => journey.AdministrationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
