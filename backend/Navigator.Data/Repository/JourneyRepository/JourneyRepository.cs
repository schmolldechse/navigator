using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Navigator.Data.Entities.Journey;
using Navigator.Data.Infrastructure;
using Navigator.Data.Models.Journey;
using Navigator.Data.Models.Ris;
using System.Text;
using System.Text.Json;

namespace Navigator.Data.Repository.JourneyRepository;

public class JourneyRepository(
    ILogger<JourneyRepository> logger,
    ProxyHttpClientFactory proxyHttpClientFactory,
    DataContext dataContext
) : IJourneyRepository
{
    private const string _risJourneysBatchUrl = "https://apis.deutschebahn.com/db/apis/ris-journeys/v2/batch";

    public async Task<RisJourneys.JourneyBatchResponse?> GetJourneysBatchAsync(IEnumerable<JourneyOnDateRequest> request)
    {
        using var httpClient = proxyHttpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Add("DB-Client-Id", Environment.GetEnvironmentVariable("JOURNEYS_CLIENT_ID"));
        httpClient.DefaultRequestHeaders.Add("DB-Api-Key", Environment.GetEnvironmentVariable("JOURNEYS_API_KEY"));

        using var message = new HttpRequestMessage(HttpMethod.Post, _risJourneysBatchUrl);
        message.Content = new StringContent(JsonSerializer.Serialize(new RisJourneys.JourneyBatchRequest()
        {
            IncludeReferences = true,
            SeparateCancelled = false,
            JourneyIDs = request.Select(entry => entry.FetchingDate.ToString("yyyyMMdd") + "-" + entry.Id).ToArray()
        }), Encoding.UTF8, "application/json");

        var response = await httpClient.SendAsync(message);
        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("Failed to fetch journeys. Status Code: {StatusCode}", response.StatusCode);
            return null;
        }

        var journeys = JsonSerializer.Deserialize<RisJourneys.JourneyBatchResponse>(await response.Content.ReadAsStringAsync());
        return journeys ?? null;
    }

    public async Task SaveJourneysBatchAsync(IEnumerable<Journey> journeys)
    {
        var journeyIds = journeys.Select(journey => journey.Id).ToList();
        var existingIds = await dataContext.Journeys
            .Where(journey => journeyIds.Contains(journey.Id))
            .Select(risId => risId.Id)
            .ToHashSetAsync();

        var toInsert = journeys.Where(journey => !existingIds.Contains(journey.Id));
        if (toInsert.Any()) dataContext.AddRange(toInsert);
        await dataContext.SaveChangesAsync();
    }
}
