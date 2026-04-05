using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Navigator.Data.Entities.Station;

namespace Navigator.Data.Configurations.Stations;

public class StationTransportConfiguration : IEntityTypeConfiguration<StationTransport>
{
    public void Configure(EntityTypeBuilder<StationTransport> builder)
    {
        builder.ToTable("station_transports", "core");
        builder.HasKey(transport => transport.Id);

        builder.Property(transport => transport.Id).ValueGeneratedOnAdd();

        builder.HasIndex(transport => new { transport.EvaNumber, transport.TransportType }).IsUnique();
    }
}