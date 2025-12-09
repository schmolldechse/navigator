using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        services.AddDbContext<DataContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddHttpClient();

        services
            .AddTransient<IStationRepository, StationRepository>()
            .AddTransient<IStationRilRepository, StationRilRepository>()
            .AddTransient<IStationTransportRepository, StationTransportRepository>();
        return services;
    }
}
