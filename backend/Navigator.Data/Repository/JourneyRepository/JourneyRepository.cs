using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Navigator.Data.Entities.Journey;
using Navigator.Data.Entities.Statistics;
using Navigator.Data.Infrastructure;
using Navigator.Data.Enums;
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
        }

        await EnsureQualityFactsAsync(incomingKeys);

        await transaction.CommitAsync();
    }

    private async Task EnsureQualityFactsAsync(IReadOnlySet<(string Id, DateOnly Date)> journeyKeys)
    {
        if (!journeyKeys.Any()) return;

        var journeyIds = journeyKeys.Select(key => key.Id).ToList();
        var journeyDates = journeyKeys.Select(key => key.Date).ToList();
        var persistedJourneys = (await dataContext.Journeys
                .Include(journey => journey.Transport)
                .Include(journey => journey.StopPlaces)
                .Where(journey => journeyIds.Contains(journey.Id))
                .Where(journey => journeyDates.Contains(journey.Date))
                .ToListAsync())
            .Where(journey => journeyKeys.Contains((journey.Id, journey.Date)))
            .ToList();
        if (!persistedJourneys.Any()) return;

        var (eventFacts, routeFacts) = BuildQualityFacts(persistedJourneys);
        if (!eventFacts.Any() && !routeFacts.Any()) return;

        var stopPlaceIds = eventFacts.Select(fact => fact.StopPlaceId).ToList();
        var existingEventFacts = await dataContext.JourneyEventQualityFacts
            .Where(fact => stopPlaceIds.Contains(fact.StopPlaceId))
            .Select(fact => new { fact.StopPlaceId, fact.PlannedTime })
            .ToListAsync();
        var existingEventFactKeys = existingEventFacts
            .Select(fact => (fact.StopPlaceId, fact.PlannedTime))
            .ToHashSet();
        var eventFactsToInsert = eventFacts
            .Where(fact => !existingEventFactKeys.Contains((fact.StopPlaceId, fact.PlannedTime)))
            .ToList();

        var routeJourneyIds = routeFacts.Select(fact => fact.JourneyId).ToList();
        var routeDates = routeFacts.Select(fact => fact.Date).ToList();
        var existingRouteFacts = await dataContext.JourneyRouteQualityFacts
            .Where(fact => routeJourneyIds.Contains(fact.JourneyId))
            .Where(fact => routeDates.Contains(fact.Date))
            .Select(fact => new { fact.JourneyId, fact.Date, fact.JourneyStartTime })
            .ToListAsync();
        var existingRouteFactKeys = existingRouteFacts
            .Select(fact => (fact.JourneyId, fact.Date, fact.JourneyStartTime))
            .ToHashSet();
        var routeFactsToInsert = routeFacts
            .Where(fact => !existingRouteFactKeys.Contains((fact.JourneyId, fact.Date, fact.JourneyStartTime)))
            .ToList();

        dataContext.JourneyEventQualityFacts.AddRange(eventFactsToInsert);
        dataContext.JourneyRouteQualityFacts.AddRange(routeFactsToInsert);
        await dataContext.SaveChangesAsync();
    }

    private static (List<JourneyEventQualityFact> EventFacts, List<JourneyRouteQualityFact> RouteFacts) BuildQualityFacts(
        IEnumerable<Journey> journeys
    )
    {
        var eventFacts = new List<JourneyEventQualityFact>();
        var routeFacts = new List<JourneyRouteQualityFact>();

        foreach (var journey in journeys)
        {
            var orderedStopPlaces = journey.StopPlaces
                .OrderBy(stopPlace => stopPlace.PlannedTime)
                .ThenBy(stopPlace => stopPlace.StationEvaNumber)
                .ToList();
            if (!orderedStopPlaces.Any()) continue;

            var transport = journey.Transport;
            var isReplacementTransport = journey.JourneyType == JourneyType.Replacement
                || transport.ReplacementTransportType is not null;
            var originEvaNumber = ResolveOriginEvaNumber(orderedStopPlaces);
            var destinationEvaNumber = ResolveDestinationEvaNumber(orderedStopPlaces);
            var journeyStartTime = ResolveJourneyStartTime(orderedStopPlaces);
            var terminalDelaySeconds = orderedStopPlaces
                .Where(stopPlace => stopPlace.Cancelled is not true)
                .OrderBy(stopPlace => stopPlace.ScheduleType == ScheduleType.Arrival ? 0 : 1)
                .ThenByDescending(stopPlace => stopPlace.PlannedTime)
                .Select(stopPlace => (int?)stopPlace.Delay)
                .FirstOrDefault();

            routeFacts.Add(new JourneyRouteQualityFact
            {
                JourneyId = journey.Id,
                Date = journey.Date,
                JourneyStartTime = journeyStartTime,
                AdministrationId = journey.AdministrationId,
                TransportType = transport.TransportType,
                JourneyDescription = transport.JourneyDescription,
                Number = transport.Number,
                IsReplacementTransport = isReplacementTransport,
                OriginEvaNumber = originEvaNumber,
                DestinationEvaNumber = destinationEvaNumber,
                JourneyCancelled = journey.Cancelled,
                TerminalDelaySeconds = terminalDelaySeconds
            });

            var stationLineStopPlaceIds = orderedStopPlaces
                .GroupBy(stopPlace => new { stopPlace.JourneyId, stopPlace.Date, stopPlace.StationEvaNumber })
                .Select(group => group
                    .OrderBy(stopPlace => stopPlace.ScheduleType == ScheduleType.Departure ? 0 : 1)
                    .ThenByDescending(stopPlace => stopPlace.PlannedTime)
                    .ThenBy(stopPlace => stopPlace.Id)
                    .First()
                    .Id)
                .ToHashSet();

            foreach (var stopPlace in journey.StopPlaces)
            {
                eventFacts.Add(new JourneyEventQualityFact
                {
                    StopPlaceId = stopPlace.Id,
                    JourneyId = journey.Id,
                    Date = journey.Date,
                    PlannedTime = stopPlace.PlannedTime,
                    StationEvaNumber = stopPlace.StationEvaNumber,
                    ScheduleType = stopPlace.ScheduleType,
                    AdministrationId = journey.AdministrationId,
                    TransportType = transport.TransportType,
                    JourneyDescription = transport.JourneyDescription,
                    Number = transport.Number,
                    IsReplacementTransport = isReplacementTransport,
                    OriginEvaNumber = originEvaNumber,
                    DestinationEvaNumber = destinationEvaNumber,
                    IsStationLineEvent = stationLineStopPlaceIds.Contains(stopPlace.Id),
                    Cancelled = stopPlace.Cancelled,
                    Delay = stopPlace.Delay
                });
            }
        }

        return (eventFacts, routeFacts);
    }

    private static int ResolveOriginEvaNumber(IEnumerable<JourneyStopPlace> stopPlaces)
    {
        var ordered = stopPlaces.ToList();
        return ordered
            .Where(stopPlace => stopPlace.ScheduleType == ScheduleType.Departure)
            .OrderBy(stopPlace => stopPlace.PlannedTime)
            .ThenBy(stopPlace => stopPlace.StationEvaNumber)
            .Select(stopPlace => (int?)stopPlace.StationEvaNumber)
            .FirstOrDefault()
            ?? ordered
                .OrderBy(stopPlace => stopPlace.PlannedTime)
                .ThenBy(stopPlace => stopPlace.StationEvaNumber)
                .Select(stopPlace => stopPlace.StationEvaNumber)
                .FirstOrDefault();
    }

    private static int ResolveDestinationEvaNumber(IEnumerable<JourneyStopPlace> stopPlaces)
    {
        var ordered = stopPlaces.ToList();
        return ordered
            .Where(stopPlace => stopPlace.ScheduleType == ScheduleType.Arrival)
            .OrderByDescending(stopPlace => stopPlace.PlannedTime)
            .ThenByDescending(stopPlace => stopPlace.StationEvaNumber)
            .Select(stopPlace => (int?)stopPlace.StationEvaNumber)
            .FirstOrDefault()
            ?? ordered
                .OrderByDescending(stopPlace => stopPlace.PlannedTime)
                .ThenByDescending(stopPlace => stopPlace.StationEvaNumber)
                .Select(stopPlace => stopPlace.StationEvaNumber)
                .FirstOrDefault();
    }

    private static DateTime ResolveJourneyStartTime(IEnumerable<JourneyStopPlace> stopPlaces)
    {
        var ordered = stopPlaces.ToList();
        return ordered
            .Where(stopPlace => stopPlace.ScheduleType == ScheduleType.Departure)
            .OrderBy(stopPlace => stopPlace.PlannedTime)
            .Select(stopPlace => (DateTime?)stopPlace.PlannedTime)
            .FirstOrDefault()
            ?? ordered
                .OrderBy(stopPlace => stopPlace.PlannedTime)
                .Select(stopPlace => stopPlace.PlannedTime)
                .First();
    }
}
