using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Navigator.Data.Entities.Journey.Message;

namespace Navigator.Data.Configurations.Journeys;

public class JourneyStopPlaceMessageConfiguration : IEntityTypeConfiguration<JourneyStopPlaceMessage>
{
    public void Configure(EntityTypeBuilder<JourneyStopPlaceMessage> builder)
    {
        builder.ToTable("journey_stop_place_messages", "core", table => table.ExcludeFromMigrations());
        builder.HasKey(stopPlaceMessage => new { stopPlaceMessage.StopPlaceId, stopPlaceMessage.MessageId, stopPlaceMessage.Date });

        builder.Property(stopPlaceMessage => stopPlaceMessage.StopPlaceId).ValueGeneratedNever();
        builder.Property(stopPlaceMessage => stopPlaceMessage.MessageId).ValueGeneratedNever();

        builder.HasOne(stopPlaceMessage => stopPlaceMessage.Message)
             .WithMany(message => message.JourneyStopPlaceMessages)
             .HasForeignKey(stopPlaceMessage => new { stopPlaceMessage.MessageId, stopPlaceMessage.Date })
             .HasPrincipalKey(message => new { message.Id, message.Date })
             .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(stopPlaceMessage => stopPlaceMessage.StopPlace)
             .WithMany(stopPlace => stopPlace.Messages)
             .HasForeignKey(stopPlaceMessage => new { stopPlaceMessage.StopPlaceId, stopPlaceMessage.Date })
             .HasPrincipalKey(stopPlace => new { stopPlace.Id, stopPlace.Date })
             .OnDelete(DeleteBehavior.Cascade);
    }
}