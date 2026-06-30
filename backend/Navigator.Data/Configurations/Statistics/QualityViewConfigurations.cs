using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Navigator.Data.Entities.Views;

namespace Navigator.Data.Configurations.Statistics;

internal class NetworkEventQualityHourlyConfiguration : IEntityTypeConfiguration<NetworkEventQualityHourly>
{
    public void Configure(EntityTypeBuilder<NetworkEventQualityHourly> builder) =>
        builder.ConfigureKeylessView("network_event_quality_hourly");
}

internal class NetworkEventDelayDistributionHourlyConfiguration : IEntityTypeConfiguration<NetworkEventDelayDistributionHourly>
{
    public void Configure(EntityTypeBuilder<NetworkEventDelayDistributionHourly> builder) =>
        builder.ConfigureKeylessView("network_event_delay_distribution_hourly");
}

internal class NetworkJourneyQualityHourlyConfiguration : IEntityTypeConfiguration<NetworkJourneyQualityHourly>
{
    public void Configure(EntityTypeBuilder<NetworkJourneyQualityHourly> builder) =>
        builder.ConfigureKeylessView("network_journey_quality_hourly");
}

internal class StationEventQualityHourlyConfiguration : IEntityTypeConfiguration<StationEventQualityHourly>
{
    public void Configure(EntityTypeBuilder<StationEventQualityHourly> builder) =>
        builder.ConfigureKeylessView("station_event_quality_hourly");
}

internal class StationAdministrationQualityHourlyConfiguration : IEntityTypeConfiguration<StationAdministrationQualityHourly>
{
    public void Configure(EntityTypeBuilder<StationAdministrationQualityHourly> builder) =>
        builder.ConfigureKeylessView("station_administration_quality_hourly");
}

internal class LineEventQualityHourlyConfiguration : IEntityTypeConfiguration<LineEventQualityHourly>
{
    public void Configure(EntityTypeBuilder<LineEventQualityHourly> builder)
    {
        builder.ConfigureKeylessView("line_event_quality_hourly");
        builder.Property(row => row.JourneyDescription).HasMaxLength(64);
    }
}

internal class StationLineQualityHourlyConfiguration : IEntityTypeConfiguration<StationLineQualityHourly>
{
    public void Configure(EntityTypeBuilder<StationLineQualityHourly> builder)
    {
        builder.ConfigureKeylessView("station_line_quality_hourly");
        builder.Property(row => row.JourneyDescription).HasMaxLength(64);
    }
}

internal class JourneyAdministrationQualityHourlyConfiguration : IEntityTypeConfiguration<JourneyAdministrationQualityHourly>
{
    public void Configure(EntityTypeBuilder<JourneyAdministrationQualityHourly> builder) =>
        builder.ConfigureKeylessView("journey_administration_quality_hourly");
}

internal class LineJourneyQualityHourlyConfiguration : IEntityTypeConfiguration<LineJourneyQualityHourly>
{
    public void Configure(EntityTypeBuilder<LineJourneyQualityHourly> builder)
    {
        builder.ConfigureKeylessView("line_journey_quality_hourly");
        builder.Property(row => row.JourneyDescription).HasMaxLength(64);
    }
}

internal class JourneyNumberQualityHourlyConfiguration : IEntityTypeConfiguration<JourneyNumberQualityHourly>
{
    public void Configure(EntityTypeBuilder<JourneyNumberQualityHourly> builder)
    {
        builder.ConfigureKeylessView("journey_number_quality_hourly");
        builder.Property(row => row.JourneyDescription).HasMaxLength(64);
    }
}

internal class StationJourneyEventDetailConfiguration : IEntityTypeConfiguration<StationJourneyEventDetail>
{
    public void Configure(EntityTypeBuilder<StationJourneyEventDetail> builder)
    {
        builder.ConfigureKeylessView("station_journey_event_details");
        builder.Property(row => row.JourneyId).HasMaxLength(82);
        builder.Property(row => row.JourneyDescription).HasMaxLength(64);
    }
}

internal class JourneyQualityDetailConfiguration : IEntityTypeConfiguration<JourneyQualityDetail>
{
    public void Configure(EntityTypeBuilder<JourneyQualityDetail> builder)
    {
        builder.ConfigureKeylessView("journey_quality_details");
        builder.Property(row => row.JourneyId).HasMaxLength(82);
        builder.Property(row => row.JourneyDescription).HasMaxLength(64);
    }
}

internal static class QualityViewConfigurationExtensions
{
    public static void ConfigureKeylessView<TEntity>(
        this EntityTypeBuilder<TEntity> builder,
        string name
    ) where TEntity : class
    {
        builder.ToTable(name, "statistics", table => table.ExcludeFromMigrations());
        builder.HasNoKey();
        builder.ToView(name, "statistics");
    }
}
