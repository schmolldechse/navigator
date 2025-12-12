using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Navigator.Data;
using Navigator.Data.Models.Ris;
using Navigator.Data.Repository.StationRepository;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddServices(builder.Configuration);

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
    await dataContext.Database.MigrateAsync();

    var stationsRepository = scope.ServiceProvider.GetRequiredService<IStationRepository>();
    await stationsRepository.GetRisStationsByCoordinatesAsync(new RisStationsByCoordinatesRequest()
    {
        Latitude = 48.539725,
        Longitude = 9.290156,
        GroupBy = RisStations.StopPlaceSearchGroupByKey.NONE,
        Radius = 1000,
        Limit = 100
    });

    await stationsRepository.GetStaDaAsync();
}

host.Run();