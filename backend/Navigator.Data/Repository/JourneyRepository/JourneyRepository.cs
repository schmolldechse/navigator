using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Navigator.Data.Entities.Journey;
using Navigator.Data.Infrastructure;
using Navigator.Data.Models.Journey;
using Navigator.Data.Models.Ris;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Navigator.Data.Repository.JourneyRepository;

public class JourneyRepository(
    ILogger<JourneyRepository> logger,
    ProxyHttpClientFactory proxyHttpClientFactory,
    DataContext dataContext
) : IJourneyRepository
{
    private const string _risJourneysBatchMatchUrl = "https://apis.deutschebahn.com/db/apis/ris-journeys/v2/batch";
    private const string _risJourneysMatchUrl = "https://apis.deutschebahn.com/db/apis/ris-journeys/v2/{0}";

    public async Task<RisJourneys.JourneyEventBased?> GetJourneyAsync(string journeyId)
    {
        using var httpClient = proxyHttpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Add("DB-Client-Id", Environment.GetEnvironmentVariable("JOURNEYS_CLIENT_ID"));
        httpClient.DefaultRequestHeaders.Add("DB-Api-Key", Environment.GetEnvironmentVariable("JOURNEYS_API_KEY"));

        using var message = new HttpRequestMessage(HttpMethod.Get, string.Format(_risJourneysMatchUrl, journeyId));

        var response = await httpClient.SendAsync(message);
        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.NotFound) return null;

            logger.LogError("Failed to fetch journey with ID {JourneyId}", journeyId);
            throw new HttpRequestException($"Failed to fetch journey with ID {journeyId}", null, response.StatusCode);
        }

        var journey = JsonSerializer.Deserialize<RisJourneys.JourneyEventBased>(await response.Content.ReadAsStringAsync());
        if (journey is null)
        {
            logger.LogError("Failed to deserialize journey with ID {JourneyId}", journeyId);
            throw new JsonException($"Failed to deserialize journey with ID {journeyId}");
        }

        return journey;
    }

    public async Task<RisJourneys.JourneyBatchResponse> GetJourneyBatchAsync(IEnumerable<JourneyOnDateRequest> request)
    {
        using var httpClient = proxyHttpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Add("DB-Client-Id", Environment.GetEnvironmentVariable("JOURNEYS_CLIENT_ID"));
        httpClient.DefaultRequestHeaders.Add("DB-Api-Key", Environment.GetEnvironmentVariable("JOURNEYS_API_KEY"));

        using var message = new HttpRequestMessage(HttpMethod.Post, _risJourneysBatchMatchUrl);
        message.Content = new StringContent(JsonSerializer.Serialize(new RisJourneys.JourneyBatchRequest()
        {
            IncludeReferences = true,
            SeparateCancelled = false,
            JourneyIDs = request.Select(entry => entry.FetchingDate.ToString("yyyyMMdd") + "-" + entry.Id).ToArray()
        }), Encoding.UTF8, "application/json");

        var response = await httpClient.SendAsync(message);
        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("Failed to fetch journeys batch for {Count} entries", request.Count());
            throw new HttpRequestException($"Failed to fetch journeys batch for {request.Count()} entries", null, response.StatusCode);
        }

        var journeyBatch = JsonSerializer.Deserialize<RisJourneys.JourneyBatchResponse>(await response.Content.ReadAsStringAsync());
        if (journeyBatch is null)
        {
            logger.LogError("Failed to deserialize journeys batch for {Count} entries", request.Count());
            throw new JsonException($"Failed to deserialize journeys batch for {request.Count()} entries");
        }

        return journeyBatch;
    }

    public async Task SaveJourneyBatchAsync(IEnumerable<Journey> journeys)
    {
        var journeyIds = journeys.Select(journey => journey.Id).ToList();
        var existingIds = await dataContext.Journeys
            .Where(journey => journeyIds.Contains(journey.Id))
            .Select(journey => journey.Id)
            .ToHashSetAsync();

        var toInsert = journeys
            .Where(journey => !existingIds.Contains(journey.Id))
            .Where(journey => journey.Administration != null)
            .ToList();
        if (!toInsert.Any()) return;

        var currentTime = DateTime.UtcNow;
        toInsert.ForEach(journey => journey.InsertedAt = currentTime);

        // extract unique incoming Administrations
        var administrations = toInsert
            .Select(journey => journey.Administration!)
            .GroupBy(administration => new { administration.AdministrationId, administration.OperatorCode, administration.OperatorName })
            .Select(group => group.First())
            .ToList();
        if (!administrations.Any()) return;

        var administrationIds = administrations.Select(administration => administration.AdministrationId).ToList();
        var operatorCodes = administrations.Select(administration => administration.OperatorCode).ToList();
        var operatorNames = administrations.Select(administration => administration.OperatorName).ToList();

        // query database
        var existingAdministrations = await dataContext.Administrations
            .Where(administration => administrationIds.Contains(administration.AdministrationId)
                && operatorCodes.Contains(administration.OperatorCode)
                && operatorNames.Contains(administration.OperatorName))
            .ToListAsync();
        var existingAdministrationsDictionary = existingAdministrations.ToDictionary(
            administration => (administration.AdministrationId, administration.OperatorCode, administration.OperatorName)
        );

        // re-link Journey Administrations
        var newAdministrationsDictionary = new Dictionary<(string AdministrationId, string OperatorCode, string OperatorName), Administration>();
        foreach (var journey in toInsert)
        {
            var administration = journey.Administration;
            if (administration is null) continue;

            var key = (administration.AdministrationId, administration.OperatorCode, administration.OperatorName);
            if (existingAdministrationsDictionary.TryGetValue(key, out var existingAdministration))
            {
                journey.Administration = existingAdministration;
                journey.AdministrationId = existingAdministration.Id;
            }
            else
            {
                if (!newAdministrationsDictionary.TryGetValue(key, out var newAdministration))
                {
                    newAdministration = new Administration()
                    {
                        AdministrationId = administration.AdministrationId,
                        OperatorCode = administration.OperatorCode,
                        OperatorName = administration.OperatorName
                    };
                    newAdministrationsDictionary[key] = newAdministration;
                }

                journey.Administration = newAdministration;
                journey.AdministrationId = newAdministration.Id;
            }
        }

        dataContext.AddRange(toInsert);
        await dataContext.SaveChangesAsync();
    }
}
