using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Navigator.Data.Enums;
using Navigator.Data.Infrastructure;
using Navigator.Data.RisIds;
using Navigator.Data.Repository;
using Navigator.Data.Repository.JourneyRepository;
using Navigator.Data.Repository.RisIdRepository;
using Navigator.Data.Repository.StationRepository;
using Navigator.Data.Repository.StationRil100Repository;
using Navigator.Data.Repository.StationTransportRepository;
using Navigator.Data.Repository.StatisticsRepository;
using Navigator.Data.Repository.StatisticsRepository.MetricSeriesBuilders;
using Navigator.Data.Repository.TimetableRepository;
using Navigator.Data.StatisticsRefresh;

namespace Navigator.Data;

public static class Injection
{
    public static IServiceCollection AddServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbContext<DataContext>(options => options.UseNpgsql(
            configuration.GetConnectionString("DefaultConnection"),
            npgsqlOptions =>
            {
                npgsqlOptions.MapEnum<ScheduleType>("schedule_type", "core");
                npgsqlOptions.MapEnum<TransportType>("transport_type", "core");
                npgsqlOptions.MapEnum<TimeType>("time_type", "core");
                npgsqlOptions.MapEnum<JourneyType>("journey_type", "core");
                npgsqlOptions.MapEnum<StatisticsRefreshQueueStatus>("statistics_refresh_queue_status", "statistics");
                npgsqlOptions.MapEnum<StatisticsRefreshQueueSource>("statistics_refresh_queue_source", "statistics");
            }));

        services.AddHttpClient();
        services.AddSingleton<ProxyHttpClientFactory>();

        services.AddScoped<Estimator>();
        services.Configure<StatisticsRefreshOptions>(
            configuration.GetSection(StatisticsRefreshOptions.SectionName));
        services.Configure<RisIdLifecycleOptions>(
            configuration.GetSection(RisIdLifecycleOptions.SectionName));
        services.AddScoped<IStatisticsRollupRefreshService, StatisticsRollupRefreshService>();
        services.AddScoped<IStatisticsRefreshQueueService, StatisticsRefreshQueueService>();
        services.AddScoped<IStatisticsRefreshWindowPlanner, StatisticsRefreshWindowPlanner>();
        services.AddScoped<IRisIdReactivationHoldService, RisIdReactivationHoldService>();

        services.AddTransient<IStatisticsMetricBuilder, NetworkStatisticsMetricSeriesBuilder>()
            .AddTransient<IStatisticsMetricBuilder, StationStatisticsMetricSeriesBuilder>()
            .AddTransient<IStatisticsMetricBuilder, LineStatisticsMetricSeriesBuilder>()
            .AddTransient<IStatisticsMetricBuilder, JourneyStatisticsMetricSeriesBuilder>();

        services.AddTransient<IJourneyRepository, JourneyRepository>()
            .AddTransient<IRisIdRepository, RisIdRepository>()
            .AddTransient<IStationRepository, StationRepository>()
            .AddTransient<IStationRil100Repository, StationRil100Repository>()
            .AddTransient<IStationTransportRepository, StationTransportRepository>()
            .AddTransient<IStatisticsRepository, StatisticsRepository>()
            .AddTransient<ITimetableRepository, TimetableRepository>();
        return services;
    }
}
