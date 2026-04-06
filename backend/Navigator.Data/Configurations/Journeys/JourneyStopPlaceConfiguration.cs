using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Navigator.Data.Entities.Journey;

namespace Navigator.Data.Configurations.Journeys;

public class JourneyStopPlaceConfiguration : IEntityTypeConfiguration<JourneyStopPlace>
{
    public void Configure(EntityTypeBuilder<JourneyStopPlace> builder)
    {
        builder.ToTable("journey_stop_places", "core", table => table.ExcludeFromMigrations());
        builder.HasKey(stopPlace => new { stopPlace.Id, stopPlace.Date });

        builder.Property(stopPlace => stopPlace.Id).ValueGeneratedOnAdd();
        builder.Property(stopPlace => stopPlace.JourneyId)
            .HasMaxLength(82)
            .ValueGeneratedNever();
        builder.Property(stopPlace => stopPlace.Delay).HasComputedColumnSql(
            "EXTRACT(EPOCH FROM (actual_time - planned_time))::integer",
            stored: true
        );
        builder.Property(stopPlace => stopPlace.PlannedPlatform).HasMaxLength(32);
        builder.Property(stopPlace => stopPlace.ActualPlatform).HasMaxLength(32);

        builder.HasOne(stopPlace => stopPlace.Journey)
            .WithMany(journey => journey.StopPlaces)
            .HasForeignKey(stopPlace => new { stopPlace.JourneyId, stopPlace.Date })
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(stopPlace => stopPlace.Messages)
            .WithOne(message => message.StopPlace)
            .HasForeignKey(message => new { message.StopPlaceId, message.Date })
            .OnDelete(DeleteBehavior.Cascade);
    }
}
