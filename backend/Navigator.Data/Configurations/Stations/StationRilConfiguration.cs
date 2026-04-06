using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Navigator.Data.Entities.Station;

namespace Navigator.Data.Configurations.Stations;

public class StationRilConfiguration : IEntityTypeConfiguration<StationRil100>
{
    public void Configure(EntityTypeBuilder<StationRil100> builder)
    {
        builder.ToTable("station_ril100", "core");
        builder.HasKey(ril => ril.Id);

        builder.Property(ril => ril.Id).ValueGeneratedOnAdd();

        builder.HasIndex(ril => new { ril.EvaNumber, ril.Ril100Code }).IsUnique();
    }
}