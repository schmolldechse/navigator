using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Navigator.Data.Entities.Statistics;

namespace Navigator.Data.Configurations.Statistics;

public class JourneySnapshotConfiguration : IEntityTypeConfiguration<JourneySnapshot>
{
    public void Configure(EntityTypeBuilder<JourneySnapshot> builder)
    {
        builder.ToTable("journey_snapshots", "statistics");
        builder.HasKey(snapshot => snapshot.Id);

        builder.Property(snapshot => snapshot.Id).ValueGeneratedOnAdd();

        builder.HasIndex(snapshot => snapshot.MeasuredAt);
    }
}
