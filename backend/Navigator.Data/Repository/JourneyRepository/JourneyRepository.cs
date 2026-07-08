using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Navigator.Data.Entities.Journey;
using Navigator.Data.Enums;
using Navigator.Data.Infrastructure;
using Navigator.Data.Models.Journey;
using Navigator.Data.Models.Ris;
using Navigator.Data.StatisticsRefresh;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Navigator.Data.Repository.JourneyRepository;

public class JourneyRepository(
    ILogger<JourneyRepository> logger,
    ProxyHttpClientFactory proxyHttpClientFactory,
    DataContext dataContext,
    IStatisticsRefreshQueueService statisticsRefreshQueueService
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

            var exception = new HttpRequestException($"Failed to fetch journey with ID {journeyId}", null, response.StatusCode);
            logger.LogError(
                exception,
                "Failed to fetch journey from {UpstreamService} at {UpstreamEndpoint}. StatusCode: {StatusCode}. JourneyId: {JourneyId}",
                "RIS::Journeys",
                "GetJourney",
                (int)response.StatusCode,
                journeyId);
            throw exception;
        }

        var journey = JsonSerializer.Deserialize<RisJourneys.JourneyEventBased>(await response.Content.ReadAsStringAsync());
        if (journey is null)
        {
            var exception = new JsonException($"Failed to deserialize journey with ID {journeyId}");
            logger.LogError(
                exception,
                "Failed to deserialize journey from {UpstreamService} at {UpstreamEndpoint}. JourneyId: {JourneyId}",
                "RIS::Journeys",
                "GetJourney",
                journeyId);
            throw exception;
        }

        return journey;
    }

    public async Task<RisJourneys.JourneyBatchResponse> GetJourneyBatchAsync(IEnumerable<JourneyOnDateRequest> request)
    {
        var requestList = request.ToList();
        using var httpClient = proxyHttpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Add("DB-Client-Id", Environment.GetEnvironmentVariable("JOURNEYS_CLIENT_ID"));
        httpClient.DefaultRequestHeaders.Add("DB-Api-Key", Environment.GetEnvironmentVariable("JOURNEYS_API_KEY"));

        using var message = new HttpRequestMessage(HttpMethod.Post, _risJourneysBatchMatchUrl);
        message.Content = new StringContent(JsonSerializer.Serialize(new RisJourneys.JourneyBatchRequest()
        {
            IncludeReferences = true,
            SeparateCancelled = false,
            JourneyIDs = requestList.Select(entry => entry.FetchingDate.ToString("yyyyMMdd") + "-" + entry.Id).ToArray()
        }), Encoding.UTF8, "application/json");

        var response = await httpClient.SendAsync(message);
        if (!response.IsSuccessStatusCode)
        {
            var exception = new HttpRequestException($"Failed to fetch journeys batch for {requestList.Count} entries", null, response.StatusCode);
            logger.LogError(
                exception,
                "Failed to fetch journey batch from {UpstreamService} at {UpstreamEndpoint}. StatusCode: {StatusCode}. RequestCount: {RequestCount}",
                "RIS::Journeys",
                "GetJourneyBatch",
                (int)response.StatusCode,
                requestList.Count);
            throw exception;
        }

        var journeyBatch = JsonSerializer.Deserialize<RisJourneys.JourneyBatchResponse>(await response.Content.ReadAsStringAsync());
        if (journeyBatch is null)
        {
            var exception = new JsonException($"Failed to deserialize journeys batch for {requestList.Count} entries");
            logger.LogError(
                exception,
                "Failed to deserialize journey batch from {UpstreamService} at {UpstreamEndpoint}. RequestCount: {RequestCount}",
                "RIS::Journeys",
                "GetJourneyBatch",
                requestList.Count);
            throw exception;
        }

        return journeyBatch;
    }

    public async Task SaveJourneyBatchAsync(IEnumerable<Journey> journeys)
    {
        var incomingJourneys = journeys
            .GroupBy(journey => new { journey.Id, journey.Date })
            .Select(group => group.First())
            .ToList();
        if (!incomingJourneys.Any()) return;

        var incomingKeys = incomingJourneys
            .Select(journey => (journey.Id, journey.Date))
            .ToHashSet();
        var journeyIds = incomingKeys.Select(key => key.Id).ToList();
        var journeyDates = incomingKeys.Select(key => key.Date).ToList();

        var existingIds = await dataContext.Journeys
            .Where(journey => journeyIds.Contains(journey.Id))
            .Where(journey => journeyDates.Contains(journey.Date))
            .Select(journey => new { journey.Id, journey.Date })
            .ToListAsync();
        var existingKeys = existingIds
            .Select(journey => (journey.Id, journey.Date))
            .Where(incomingKeys.Contains)
            .ToHashSet();

        var toInsert = incomingJourneys
            .Where(journey => !existingKeys.Contains((journey.Id, journey.Date)))
            .Where(journey => journey.Administration != null)
            .ToList();

        await using var transaction = await dataContext.Database.BeginTransactionAsync();

        if (toInsert.Any())
        {
            var currentTime = DateTime.UtcNow;
            toInsert.ForEach(journey => journey.InsertedAt = currentTime);

            // extract unique incoming Administrations
            var administrations = toInsert
                .Select(journey => journey.Administration!)
                .GroupBy(administration => new { administration.AdministrationId, administration.OperatorCode, administration.OperatorName })
                .Select(group => group.First())
                .ToList();

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

            await statisticsRefreshQueueService.MarkWindowsForJourneysAsync(
                toInsert,
                StatisticsRefreshQueueSource.JourneyImport,
                CancellationToken.None);
        }

        await transaction.CommitAsync();
    }
}
