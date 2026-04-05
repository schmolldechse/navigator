using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Navigator.Data.Entities.Journey.Message;

namespace Navigator.Data.Configurations.Journeys;

public class JourneyMessageConfiguration : IEntityTypeConfiguration<JourneyMessage>
{
    public void Configure(EntityTypeBuilder<JourneyMessage> builder)
    {
        builder.ToTable("journey_messages", "core", table => table.ExcludeFromMigrations());
        builder.HasKey(message => new { message.JourneyId, message.Date });

        builder.Property(message => message.Id).ValueGeneratedOnAdd();
        builder.Property(message => message.JourneyId)
            .HasMaxLength(82)
            .ValueGeneratedNever();

        builder.Property(message => message.Code).HasMaxLength(64);
        builder.Property(message => message.Text).HasMaxLength(2048);
        builder.Property(message => message.TextShort).HasMaxLength(2048);
        builder.Property(message => message.DisruptionCause).HasMaxLength(128);
        builder.Property(message => message.DisruptionEffect).HasMaxLength(128);
        builder.Property(message => message.NoteCategory).HasMaxLength(128);

        builder.HasOne(message => message.Journey)
            .WithMany(journey => journey.Messages)
            .HasForeignKey(message => new { message.JourneyId, message.Date })
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(message => message.JourneyStopPlaceMessages)
            .WithOne(stopPlaceMessage => stopPlaceMessage.Message)
            .HasForeignKey(stopPlaceMessage => new { stopPlaceMessage.MessageId, stopPlaceMessage.Date })
            .OnDelete(DeleteBehavior.Cascade);
    }
}