using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Navigator.Data.Entities.Journey;

namespace Navigator.Data.Configurations.Journeys;

public class JourneyAdministrationConfiguration : IEntityTypeConfiguration<Administration>
{
    public void Configure(EntityTypeBuilder<Administration> builder)
    {
        builder.ToTable("journey_administrations", "core", table => table.ExcludeFromMigrations());
        builder.HasKey(administration => administration.Id);

        builder.Property(administration => administration.Id).ValueGeneratedOnAdd();
        builder.Property(administration => administration.AdministrationId).HasMaxLength(32);
        builder.Property(administration => administration.OperatorCode).HasMaxLength(32);
        builder.Property(administration => administration.OperatorName).HasMaxLength(128);

        builder.HasIndex(administration => new { administration.AdministrationId, administration.OperatorCode, administration.OperatorName }).IsUnique();
    }
}
