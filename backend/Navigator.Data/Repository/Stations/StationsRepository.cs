using Microsoft.Extensions.Logging;
using Navigator.Data.DTOs.Station;
using Navigator.Data.Entities;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Web;

namespace Navigator.Data.Repository.Stations;

public class StationsRepository(IHttpClientFactory httpClientFactory, ILogger<StationsRepository> logger) : IStationsRepository
{
    private const string _stationsUrl = "https://app.services-bahn.de/mob/location/search";

    public async Task<IEnumerable<Station>> GetStationsAsync(StationSearchRequestDTO dto)
    {
        using var httpClient = httpClientFactory.CreateClient();

        var request = new HttpRequestMessage(HttpMethod.Post, _stationsUrl);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x.db.vendo.mob.location.v3+json"));
        request.Headers.Add("X-Correlation-ID", Guid.NewGuid() + "_" + Guid.NewGuid());

        request.Content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/x.db.vendo.mob.location.v3+json");
        request.Content.Headers.ContentType!.CharSet = null; // results in receiving 405 Method Not Allowed response when not setting to null

        using var response = await httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode) 
        {
            logger.LogError("Failed to fetch stations. Status Code: {StatusCode}", response.StatusCode);
            return Enumerable.Empty<Station>();
        }

        var stations = JsonSerializer.Deserialize<StationSearchResponseDTO[]>(await response.Content.ReadAsStringAsync());
        logger.LogInformation("Got {Count} stations", stations.Count());
        return null;
    }
}
