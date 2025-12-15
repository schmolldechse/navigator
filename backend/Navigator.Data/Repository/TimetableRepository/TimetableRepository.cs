using Microsoft.Extensions.Logging;
using Navigator.Data.Infrastructure;
using Navigator.Data.Models.Ris;
using Navigator.Data.Models.Timetable;
using System.Text.Json;

namespace Navigator.Data.Repository.TimetableRepository;

public class TimetableRepository(
    ILogger<TimetableRepository> logger,
    ProxyHttpClientFactory proxyHttpClientFactory
) : ITimetableRepository
{
    private const string _risBoardsUrl = "https://apis.deutschebahn.com/db/apis/ris-boards/v1/public/{0}/{1}?timeStart={2}&timeEnd={3}";

    public async Task<RisBoards.BoardPublicArrival> GetArrivalsAsync(RisBoardRequest request)
    {
        using var httpClient = proxyHttpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Add("DB-Client-Id", Environment.GetEnvironmentVariable("BOARDS_CLIENT_ID"));
        httpClient.DefaultRequestHeaders.Add("DB-Api-Key", Environment.GetEnvironmentVariable("BOARDS_API_KEY"));

        var url = string.Format(
            _risBoardsUrl,
            "arrivals",
            request.EvaNumber,
            Uri.EscapeDataString(request.TimeStart.ToString("yyyy-MM-ddTHH:mm:sszzz")),
            Uri.EscapeDataString(request.TimeStart.AddMinutes(request.Duration).ToString("yyyy-MM-ddTHH:mm:sszzz"))
        );
        logger.LogInformation("Fetching arrivals from URL: {Url}", url);
        var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url));
        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("Failed to fetch stations. Status Code: {StatusCode}", response.StatusCode);
            return new RisBoards.BoardPublicArrival
            {
                Arrivals = Array.Empty<RisBoards.StopArrival>(),
                Disruptions = Array.Empty<RisBoards.DisruptionCommunicationEmbeddedLegacy>()
            };
        }

        var board = JsonSerializer.Deserialize<RisBoards.BoardPublicArrival>(await response.Content.ReadAsStringAsync());
        return board ?? new RisBoards.BoardPublicArrival
        {
            Arrivals = Array.Empty<RisBoards.StopArrival>(),
            Disruptions = Array.Empty<RisBoards.DisruptionCommunicationEmbeddedLegacy>()
        };
    }

    public async Task<RisBoards.BoardPublicDeparture> GetDeparturesAsync(RisBoardRequest request)
    {
        using var httpClient = proxyHttpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Add("DB-Client-Id", Environment.GetEnvironmentVariable("BOARDS_CLIENT_ID"));
        httpClient.DefaultRequestHeaders.Add("DB-Api-Key", Environment.GetEnvironmentVariable("BOARDS_API_KEY"));

        var url = string.Format(
            _risBoardsUrl,
            "departures",
            request.EvaNumber,
            Uri.EscapeDataString(request.TimeStart.ToString("yyyy-MM-ddTHH:mm:sszzz")),
            Uri.EscapeDataString(request.TimeStart.AddMinutes(request.Duration).ToString("yyyy-MM-ddTHH:mm:sszzz"))
        );
        var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url));
        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("Failed to fetch stations. Status Code: {StatusCode}", response.StatusCode);
            return new RisBoards.BoardPublicDeparture
            {
                Departures = Array.Empty<RisBoards.StopDeparture>(),
                Disruptions = Array.Empty<RisBoards.DisruptionCommunicationEmbeddedLegacy>()
            };
        }

        var board = JsonSerializer.Deserialize<RisBoards.BoardPublicDeparture>(await response.Content.ReadAsStringAsync());
        return board ?? new RisBoards.BoardPublicDeparture
        {
            Departures = Array.Empty<RisBoards.StopDeparture>(),
            Disruptions = Array.Empty<RisBoards.DisruptionCommunicationEmbeddedLegacy>()
        };
    }
}
