using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Navigator.Data.Repository.Stations;

namespace Navigator.Data;

public static class Injection
{
    public static IServiceCollection AddData(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbContext<DataContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddHttpClient();

        services.AddTransient<IStationsRepository, StationsRepository>();
        return services;
    }
}
