using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Navigator.Data.Entities.Station;
using Navigator.Data.Models.Station;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Navigator.Data.Repository.StationRepository;

public class StationRepository(
    IHttpClientFactory httpClientFactory, 
    DataContext dataContext, 
    ILogger<StationRepository> logger
) : IStationRepository
{
    private const string _stationsUrl = "https://app.services-bahn.de/mob/location/search";

    public async Task<IEnumerable<VendoStation>> GetVendoStationsAsync(VendoStationsBySearchRequest request)
    {
        using var httpClient = httpClientFactory.CreateClient();

        var message = new HttpRequestMessage(HttpMethod.Post, _stationsUrl);
        message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x.db.vendo.mob.location.v3+json"));
        message.Headers.Add("X-Correlation-ID", Guid.NewGuid() + "_" + Guid.NewGuid());

        message.Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/x.db.vendo.mob.location.v3+json");
        message.Content.Headers.ContentType!.CharSet = null; // results in receiving 405 Method Not Allowed response when not setting to null

        using var response = await httpClient.SendAsync(message);
        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("Failed to fetch stations. Status Code: {StatusCode}", response.StatusCode);
            return Enumerable.Empty<VendoStation>();
        }

        var stations = JsonSerializer.Deserialize<VendoStation[]>(await response.Content.ReadAsStringAsync());
        return stations ?? Enumerable.Empty<VendoStation>();
    }

    public async Task<IEnumerable<Station>> GetStationsByCoordinatesAsync(StationsByCoordinateRequest request) => await dataContext.Stations
        .FromSql($@"
            SELECT *
            FROM (
                SELECT DISTINCT ON (s.name)
                    s.*,
                    public.earth_distance(
                        public.ll_to_earth({request.Latitude}, {request.Longitude}),
                        public.ll_to_earth(s.latitude, s.longitude)
                    ) as distance_in_meters
                FROM ""core"".""stations"" AS s
                WHERE public.earth_distance(
                    public.ll_to_earth({request.Latitude}, {request.Longitude}),
                    public.ll_to_earth(s.latitude, s.longitude)
                ) <= {request.MaxDistance}
                ORDER BY s.name, distance_in_meters ASC
            ) AS unique_stations
            ORDER BY distance_in_meters ASC
            LIMIT {request.Limit}")
        .ToListAsync();

    public async Task<Station?> GetByEvaNumberAsync(int evaNumber) => await dataContext.Stations
        .Include(station => station.Ril100)
        .Include(station => station.Transports)
        .Where(station => station.EvaNumber == evaNumber)
        .FirstOrDefaultAsync();

    public async Task SaveStationsAsync(IEnumerable<Station> stations)
    {
        var evaNumbers = stations.Select(station => station.EvaNumber).ToList();
        var existingStations = await dataContext.Stations
            .Include(station => station.Ril100)
            .Include(station => station.Transports)
            .Where(station => evaNumbers.Contains(station.EvaNumber))
            .ToDictionaryAsync(station => station.EvaNumber);

        foreach (var incomingStation in stations)
        {
            if (!existingStations.TryGetValue(incomingStation.EvaNumber, out var station))
            {
                await dataContext.Stations.AddAsync(incomingStation);
                continue;
            }

            station.LastQueried = incomingStation.LastQueried;

            var existingRil = station.Ril100
                .Select(ril => ril.Ril100Code)
                .ToHashSet();
            foreach (var incomingRil in incomingStation.Ril100)
            {
                if (existingRil.Contains(incomingRil.Ril100Code)) continue;
                station.Ril100.Add(new StationRil100()
                {
                    EvaNumber = station.EvaNumber,
                    Ril100Code = incomingRil.Ril100Code
                });
            }

            var existingTransports = station.Transports
                .Select(transport => transport.TransportType)
                .ToHashSet();
            foreach (var incomingTransport in incomingStation.Transports)
            {
                if (existingTransports.Contains(incomingTransport.TransportType)) continue;
                station.Transports.Add(new StationTransport()
                {
                    EvaNumber = station.EvaNumber,
                    TransportType = incomingTransport.TransportType,
                    Enabled = false
                });
            }
        }

        await dataContext.SaveChangesAsync();
    }
}
