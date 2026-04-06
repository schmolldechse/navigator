using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Navigator.Data.Entities;

namespace Navigator.Data.Configurations;

public class RisIdConfiguration : IEntityTypeConfiguration<RisId>
{
    public void Configure(EntityTypeBuilder<RisId> builder)
    {
        builder.ToTable("ris_ids", "core");

        builder.HasKey(risId => risId.Id);
        builder.Property(risId => risId.Id).ValueGeneratedNever();

        builder.HasIndex(risId => new { risId.Active, risId.LastSeen });
        builder.HasIndex(risId => new { risId.Active, risId.DiscoveredAt, risId.LastInserted });

        builder.Property(risId => risId.Id).HasMaxLength(73);
    }
}