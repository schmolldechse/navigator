using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Navigator.Data.Entities.Statistics;

namespace Navigator.Data.Configurations.Statistics;

public class DatabaseSizeSnapshotConfiguration : IEntityTypeConfiguration<DatabaseSizeSnapshot>
{
    public void Configure(EntityTypeBuilder<DatabaseSizeSnapshot> builder)
    {
        builder.ToTable("database_size_snapshots", "statistics");
        builder.HasKey(snapshot => snapshot.Id);

        builder.Property(snapshot => snapshot.Id).ValueGeneratedOnAdd();

        builder.HasIndex(snapshot => snapshot.MeasuredAt);
    }
}
