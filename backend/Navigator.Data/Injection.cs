using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Navigator.Data.Enums;
using Navigator.Data.Infrastructure;
using Navigator.Data.Repository.AdministrationRepository;
using Navigator.Data.Repository.JourneyRepository;
using Navigator.Data.Repository.RisIdRepository;
using Navigator.Data.Repository.StationRepository;
using Navigator.Data.Repository.StationRilRepository;
using Navigator.Data.Repository.StationTransportRepository;
using Navigator.Data.Repository.StatisticsRepository;
using Navigator.Data.Repository.TimetableRepository;

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
                npgsqlOptions.MapEnum<MessageType>("message_type", "core");
                npgsqlOptions.MapEnum<MessageReferenceType>("message_reference_type", "core");
                npgsqlOptions.MapEnum<JourneyType>("journey_type", "core");
            }));

        services.AddHttpClient();
        services.AddSingleton<ProxyHttpClientFactory>();

        services
            .AddTransient<IAdministrationRepository, AdministrationRepository>()
            .AddTransient<IJourneyRepository, JourneyRepository>()
            .AddTransient<IRisIdRepository, RisIdRepository>()
            .AddTransient<IStationRepository, StationRepository>()
            .AddTransient<IStationRilRepository, StationRilRepository>()
            .AddTransient<IStationTransportRepository, StationTransportRepository>()
            .AddTransient<IStatisticsRepository, StatisticsRepository>()
            .AddTransient<ITimetableRepository, TimetableRepository>();
        return services;
    }
}
