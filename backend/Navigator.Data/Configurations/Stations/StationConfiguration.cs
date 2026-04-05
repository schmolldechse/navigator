using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Navigator.Data.Entities.Station;

namespace Navigator.Data.Configurations.Stations;

public class StationConfiguration : IEntityTypeConfiguration<Station>
{
    public void Configure(EntityTypeBuilder<Station> builder)
    {
        builder.ToTable("stations", "core");
        builder.HasKey(station => station.EvaNumber);

        builder.Property(station => station.EvaNumber).ValueGeneratedNever();

        builder.HasIndex(station => new { station.QueryingEnabled, station.LastQueried });

        builder.HasMany(station => station.Ril100)
            .WithOne(ril => ril.Station)
            .HasForeignKey(ril => ril.EvaNumber)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(station => station.Transports)
            .WithOne(transport => transport.Station)
            .HasForeignKey(transport => transport.EvaNumber)
            .OnDelete(DeleteBehavior.Cascade);
    }
}