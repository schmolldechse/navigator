using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Navigator.Data.Enums;
using Navigator.Data.Infrastructure;
using Navigator.Data.Repository.AdministrationRepository;
using Navigator.Data.Repository.RisIdRepository;
using Navigator.Data.Repository.StationRepository;
using Navigator.Data.Repository.StationRilRepository;
using Navigator.Data.Repository.StationTransportRepository;

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
                npgsqlOptions.MapEnum<InformationType>("information_type", "core");
                npgsqlOptions.MapEnum<ScheduleType>("schedule_type", "core");
                npgsqlOptions.MapEnum<TransportType>("transport_type", "core");
            }));

        services.AddHttpClient();
        services.AddSingleton<ProxyHttpClientFactory>();

        services
            .AddTransient<IStationRepository, StationRepository>()
            .AddTransient<IStationRilRepository, StationRilRepository>()
            .AddTransient<IStationTransportRepository, StationTransportRepository>()
            .AddTransient<IRisIdRepository, RisIdRepository>()
            .AddTransient<IAdministrationRepository, AdministrationRepository>();
        return services;
    }
}
