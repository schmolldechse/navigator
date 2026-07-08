using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Navigator.Data.Enums;

namespace Navigator.Data;

public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
{
    // dummy implementation for dotnet ef tooling
    public DataContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=password",
            npgsqlOptions =>
        {
            npgsqlOptions.MapEnum<ScheduleType>("schedule_type", "core");
            npgsqlOptions.MapEnum<TransportType>("transport_type", "core");
            npgsqlOptions.MapEnum<TimeType>("time_type", "core");
            npgsqlOptions.MapEnum<JourneyType>("journey_type", "core");
            npgsqlOptions.MapEnum<StatisticsRefreshQueueStatus>("statistics_refresh_queue_status", "statistics");
            npgsqlOptions.MapEnum<StatisticsRefreshQueueSource>("statistics_refresh_queue_source", "statistics");
        });
        return new DataContext(optionsBuilder.Options);
    }
}
