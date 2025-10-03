using System.Data;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using daemon.Database;
using daemon.Models.Database;
using daemon.Models.Database.Journey;
using daemon.Models.Database.RISIdentifier;
using daemon.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace daemon.Manager;

public class GatheringJourneyDaemon(
    ILogger<GatheringJourneyDaemon> logger,
    IServiceProvider serviceProvider,
    ProxyRotator proxyRotator) : Daemon("Gathering Journey", TimeSpan.FromSeconds(15), logger)
{
    private readonly string _apiUrl = "https://apis.deutschebahn.com/db/apis/ris-journeys/v2/batch";
    private readonly string _journeyDescriptionPattern = @"\s\(.*?\)";

    // timetable changes
    private readonly DateTime[] _timetableChanges =
    {
        new(2024, 12, 17, 0, 0, 0, DateTimeKind.Utc),
        new(2025, 6, 15, 0, 0, 0, DateTimeKind.Utc),
        new(2025, 12, 14, 0, 0, 0, DateTimeKind.Utc)
    };

    private DateTime GetLastTimetableChange(DateTime? compareTo = null)
    {
        compareTo ??= DateTime.UtcNow;
        return _timetableChanges.Where(change => change <= compareTo).DefaultIfEmpty(_timetableChanges.Min()).Max();
    }

    protected override async Task ExecuteCoreAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<NavigatorDbContext>();
        
        List<IdentifiedRisId> risIds;
        await using (var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken))
        {
            risIds = await dbContext.RisIds
                .Where(risId => !risId.IsLocked && risId.Active)
                .Where(risId => risId.LastSeen == null || risId.LastSeen < DateTime.UtcNow.Date.AddDays(-1))
                .OrderBy(risId => risId.LastSeen ?? DateTime.MinValue)
                .Take(5000)
                .ToListAsync(cancellationToken);
            risIds = risIds.OrderBy(_ => Random.Shared.Next()).Take(320).ToList();
            if (risIds.Count == 0)
            {
                await transaction.RollbackAsync(cancellationToken);
                return;
            }
            
            risIds.ForEach(risId => risId.IsLocked = true);
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        var journeys = await CallApi(risIds, dbContext, cancellationToken);
        try
        {
            await using var unlockTransaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken); 
            foreach (var journeyResponse in journeys)
            {
                journeyResponse.RisId.LastSeen = journeyResponse.LastSeen;
                if (journeyResponse.Journey != null)
                {
                    journeyResponse.RisId.LastSucceededAt = journeyResponse.LastSeen;
                    var exists = await dbContext.Journeys.AnyAsync(journey => journey.Id == journeyResponse.Journey!.Id, cancellationToken);
                    if (!exists) dbContext.Journeys.Add(journeyResponse.Journey);
                }
                journeyResponse.RisId.IsLocked = false;
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            await unlockTransaction.CommitAsync(cancellationToken);
            
            logger.LogInformation("Successfully inserted {Count} journeys", journeys.Where(j => j.Journey != null).Count());
        }
        finally
        {
            await using var unlockTransaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
            risIds.ForEach(risId => risId.IsLocked = false);
            await dbContext.SaveChangesAsync(cancellationToken);
            await unlockTransaction.CommitAsync(cancellationToken);
        }
    }
    
    private DateTime? EvaluateLastSeen(IdentifiedRisId risId)
    {
        /*
         * For RIS IDs that have never been processed (LastSeen is null):
         * - Gets the last timetable change date (starting from the previous day at midnight).
         * - Going back 7 days from the last timetable change date. (=> this could be lowered, but 7 days is a good buffer)
         */
        if (risId.LastSeen == null)
        {
            var lastSeen = GetLastTimetableChange(DateTime.UtcNow.Date.AddDays(-1));
            lastSeen = new DateTime(
                DateOnly.FromDateTime(lastSeen.AddDays(-7)),
                TimeOnly.FromTimeSpan(DateTime.UtcNow.TimeOfDay),
                DateTimeKind.Utc
            );
            return lastSeen;
        }
        /*
         * For previously processed RIS IDs, we are comparing:
         * - Midnight of LastSeen (+1 day)              |       exp: 2025-06-13 20:17:00     ->    2025-06-14 00:00:00
         * - Midnight of current date (-1 day)          |       exp: 2025-06-15 13:00:00     ->    2025-06-14 00:00:00
         *
         * We add one day to LastSeen (as this is the day which we want to gather) and then compare it to the current date (-1 day).
         * Result: LastSeen of the RIS ID is going to be set to : 2025-06-14 00:00:00
         * 
         * If LastSeen is older than the current date, the RIS ID is processed. Would it be newer, the journey could be not complete yet.
         */
        if (risId.LastSeen.Value.Date.AddDays(1) < DateTime.UtcNow.Date.AddDays(-1))
        {
            var lastSeen = new DateTime(
                DateOnly.FromDateTime(risId.LastSeen!.Value.Date.AddDays(1)),
                TimeOnly.FromTimeSpan(DateTime.UtcNow.TimeOfDay),
                DateTimeKind.Utc
            );
            return lastSeen;
        }
        return null;
    }

    private async Task<List<JourneyResponse>> CallApi(List<IdentifiedRisId> risIds, NavigatorDbContext dbContext, CancellationToken cancellationToken)
    {
        List<JourneyResponse> journeys = risIds
            .Select(risId => new { risId, lastSeen = EvaluateLastSeen(risId) })
            .Where(x => x.lastSeen != null)
            .Select(x => new JourneyResponse
            {
                RisId = x.risId,
                Journey = null,
                LastSeen = x.lastSeen!.Value
            })
            .ToList();
        logger.LogInformation("Trying to solve journeys for {Count}x RIS IDs", risIds.Count);

        var httpClient = proxyRotator.GetRandomProxy();
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, string.Format(_apiUrl));
        httpRequest.Content = new StringContent(JsonSerializer.Serialize(new
        {
            includeReferences = true,
            journeyIDs = journeys.Select(journey => journey.LastSeen.ToString("yyyyMMdd") + "-" + journey.RisId.Id).ToArray(),
            separateCancelled = false
        }), System.Text.Encoding.UTF8, "application/json");
        httpRequest.Headers.Add("Accept", "application/vnd.de.db.ris+json");
        httpRequest.Headers.Add("DB-Client-Id",
            Environment.GetEnvironmentVariable("JOURNEYS_CLIENT_ID") ??
            throw new InvalidOperationException("JOURNEYS_CLIENT_ID environment variable is not set."));
        httpRequest.Headers.Add("DB-Api-Key",
            Environment.GetEnvironmentVariable("JOURNEYS_API_KEY") ??
            throw new InvalidOperationException("JOURNEYS_API_KEY environment variable is not set."));
        
        var response = await httpClient.SendAsync(httpRequest);
        if (!response.IsSuccessStatusCode)
        { 
            logger.LogDebug("Failed to retrieve journeys. Aborting. Received {StatusCode} ({Response})", response.StatusCode, await response.Content.ReadAsStringAsync(cancellationToken));
            return new List<JourneyResponse>();
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var content = (await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken)).RootElement;
        logger.LogInformation("Successfully retrieved journeys for {Count}x RIS IDs. Received {StatusCode}", risIds.Count, response.StatusCode);
        
        if (content.TryGetProperty("erroneousJourneys", out var erroneousJourneysArray) &&
            erroneousJourneysArray.ValueKind == JsonValueKind.Array && erroneousJourneysArray.GetArrayLength() > 0)
        {
            logger.LogDebug("Received {Count} erroneous journeys", erroneousJourneysArray.GetArrayLength());
            // No need to set the JourneyResponse.Journey as it is null
        }

        if (content.TryGetProperty("journeys", out var journeysArray) &&
            journeysArray.ValueKind == JsonValueKind.Array && journeysArray.GetArrayLength() > 0)
        {
            logger.LogDebug("Received {Count} journeys", journeysArray.GetArrayLength()); 
            foreach (var journeyElement in journeysArray.EnumerateArray()) 
            { 
                var journey = await BuildJourney(journeyElement, dbContext, cancellationToken); 
                
                var journeyResponse = journeys.FirstOrDefault(jp => jp.LastSeen.ToString("yyyyMMdd") + "-" + jp.RisId.Id == journeyElement.GetProperty("journeyID").GetString()!);
                if (journeyResponse == null) continue;
                
                journeyResponse.Journey = journey;
            }
        }
        
        await dbContext.SaveChangesAsync(cancellationToken);
        return journeys;
    }
    
    private async Task<Journey?> BuildJourney(JsonElement journeyElement, NavigatorDbContext dbContext, CancellationToken cancellationToken)
    {
        if (journeyElement.GetProperty("journeyID").ValueKind != JsonValueKind.String ||
            journeyElement.GetProperty("info").ValueKind != JsonValueKind.Object)
            return null;
        
        var journeyId = journeyElement.GetProperty("journeyID").GetString()!;
        var date = DateOnly.ParseExact(journeyId[..8], "yyyyMMdd");
        var infoObject = journeyElement.GetProperty("info");
        
        if (infoObject.GetProperty("headerAdministration").ValueKind != JsonValueKind.Object) 
            return null;
        var headerAdministrationObject = infoObject.GetProperty("headerAdministration");
        
        var existingAdministration = await dbContext.Administrations.FirstOrDefaultAsync(
            administration => administration.AdministrationId == headerAdministrationObject.GetProperty("administrationID").GetString()! &&
                              administration.OperatorCode == headerAdministrationObject.GetProperty("operatorCode").GetString()! &&
                              administration.OperatorName == headerAdministrationObject.GetProperty("operatorName").GetString()!,
            cancellationToken);
        if (existingAdministration == null)
            existingAdministration = new Administration()
            {
                AdministrationId = headerAdministrationObject.GetProperty("administrationID").GetString()!,
                OperatorCode = headerAdministrationObject.GetProperty("operatorCode").GetString()!,
                OperatorName = headerAdministrationObject.GetProperty("operatorName").GetString()!
            };
        
        var informationDict = journeyElement.TryGetProperty("messages", out var messagesObject) && messagesObject.ValueKind == JsonValueKind.Object 
            ? BuildInformationDict(journeyElement.GetProperty("messages"))
            : new Dictionary<int, List<Information>>();
        return new Journey()
        {
            Id = journeyId,
            Date = date,
            InsertedAt = DateTime.UtcNow,
            Administration = existingAdministration,
            Cancelled = infoObject.TryGetProperty("journeyCancelled", out var journeyCancelledElement) && journeyCancelledElement.ValueKind == JsonValueKind.True,
            Transport = BuildTransport(infoObject),
            Type = ParseJourneyType(infoObject.GetProperty("type").GetString()!),
            ViaStops = journeyElement.GetProperty("events").EnumerateArray().Select(scheduleObject => BuildSchedule(date, scheduleObject, informationDict)).ToList()
        };
    }

    private Dictionary<int, List<Information>> BuildInformationDict(JsonElement messagesObject)
    {
        Dictionary<int, List<Information>> infos = new Dictionary<int, List<Information>>();
        
        if (messagesObject.TryGetProperty("attributes", out var attributesArray) &&
            attributesArray.ValueKind == JsonValueKind.Array)
        {
            attributesArray.EnumerateArray().ToList().ForEach(attributeObject =>
            {
                int messageId = attributeObject.GetProperty("messageID").GetInt32();
                if (!infos.ContainsKey(messageId)) infos[messageId] = new List<Information>();
                
                var information = new Information()
                {
                    Type = InformationType.JOURNEY_ATTRIBUTE,
                    Key = attributeObject.GetProperty("code").GetString()!,
                    Text = attributeObject.GetProperty("text").GetString()!,
                };
                infos[messageId].Add(information);
            });
        }

        if (messagesObject.TryGetProperty("disruptions", out var disruptionsArray) &&
            disruptionsArray.ValueKind == JsonValueKind.Array)
        {
            disruptionsArray.EnumerateArray().ToList().ForEach(disruptionObject =>
            {
                int messageId = disruptionObject.GetProperty("messageID").GetInt32();
                if (!infos.ContainsKey(messageId)) infos[messageId] = new List<Information>();

                if (disruptionObject.TryGetProperty("langDe", out var langDeObject) &&
                    langDeObject.ValueKind == JsonValueKind.Object)
                {
                    infos[messageId].Add(new Information()
                    {
                        Type = InformationType.DISRUPTION,
                        Key = "general-warning",
                        DisruptionCommunicationId = disruptionObject.TryGetProperty("disruptionCommunicationID", out var disruptionCommunicationIdObject) ? disruptionCommunicationIdObject.GetString() : null,
                        DisruptionId = disruptionObject.TryGetProperty("disruptionID", out var disruptionIdObject) ? disruptionIdObject.GetString() : null,
                        Text = langDeObject.GetProperty("text").GetString()!,
                        TextShort = langDeObject.GetProperty("textShort").GetString() ?? null,
                    });
                }

                if (disruptionObject.TryGetProperty("langEn", out var langEnObject) &&
                    langEnObject.ValueKind == JsonValueKind.Object)
                {
                    infos[messageId].Add(new Information()
                    {
                        Type = InformationType.DISRUPTION,
                        Key = "general-warning",
                        DisruptionCommunicationId = disruptionObject.TryGetProperty("disruptionCommunicationID", out var disruptionCommunicationIdObject) ? disruptionCommunicationIdObject.GetString() : null,
                        DisruptionId = disruptionObject.TryGetProperty("disruptionID", out var disruptionIdObject) ? disruptionIdObject.GetString() : null,
                        Text = langEnObject.GetProperty("text").GetString()!,
                        TextShort = langEnObject.GetProperty("textShort").GetString() ?? null,
                    });
                }
            });
        }

        if (messagesObject.TryGetProperty("notes", out var notesArray) && notesArray.ValueKind == JsonValueKind.Array)
        {
            notesArray.EnumerateArray().ToList().ForEach(noteObject =>
            {
                int messageId = noteObject.GetProperty("messageID").GetInt32();
                if (!infos.ContainsKey(messageId)) infos[messageId] = new List<Information>();
                
                var information = new Information()
                {
                    Type = InformationType.MESSAGE,
                    Key = noteObject.TryGetProperty("code", out var codeElement) && codeElement.ValueKind == JsonValueKind.String 
                        ? codeElement.GetString()!
                        : "information",
                    Text = noteObject.GetProperty("text").GetString()!,
                    TextShort = noteObject.TryGetProperty("textShort", out var textShortElement) && textShortElement.ValueKind == JsonValueKind.String 
                        ? textShortElement.GetString()!
                        : null
                };
                infos[messageId].Add(information);
            });
        }

        if (messagesObject.TryGetProperty("risCauseCodes", out var risCauseCodesArray) &&
            risCauseCodesArray.ValueKind == JsonValueKind.Array)
        {
            risCauseCodesArray.EnumerateArray().ToList().ForEach(risCauseCodeObject =>
            {
                int messageId = risCauseCodeObject.GetProperty("messageID").GetInt32();
                if (!infos.ContainsKey(messageId)) infos[messageId] = new List<Information>();
                
                var information = new Information()
                {
                    Type = InformationType.RIS_CAUSE_REASON,
                    Key = risCauseCodeObject.GetProperty("code").GetString()!,
                    Text = risCauseCodeObject.GetProperty("text").GetString()!,
                };
                infos[messageId].Add(information);
            });
        }
        
        if (messagesObject.TryGetProperty("risQualityDeviations", out var risQualityDeviationsArray) &&
            risQualityDeviationsArray.ValueKind == JsonValueKind.Array)
        {
            risQualityDeviationsArray.EnumerateArray().ToList().ForEach(risQualityDeviationObject =>
            {
                int messageId = risQualityDeviationObject.GetProperty("messageID").GetInt32();
                if (!infos.ContainsKey(messageId)) infos[messageId] = new List<Information>();
                
                var information = new Information()
                {
                    Type = InformationType.RIS_QUALITY_DEVIATION,
                    Key = risQualityDeviationObject.GetProperty("code").GetString()!,
                    Text = risQualityDeviationObject.GetProperty("text").GetString()!,
                };
                infos[messageId].Add(information);
            });
        }
        
        return infos;
    }
    
    private Transport BuildTransport(JsonElement infoObject)
    {
        if (infoObject.GetProperty("transportAtStart").ValueKind != JsonValueKind.Object)
            throw new InvalidOperationException("Invalid transportAtStart object");
        var transportAtStartObject = infoObject.GetProperty("transportAtStart");
        
        var replacementType = string.Empty;
        if (transportAtStartObject.TryGetProperty("replacementTransport", out var replacementTransportObject) && replacementTransportObject.ValueKind != JsonValueKind.Null)
        {
            if (replacementTransportObject.TryGetProperty("realType", out var realTypeElement) && realTypeElement.ValueKind == JsonValueKind.String)
            {
                replacementType = realTypeElement.GetString()!;
            }
        }
        
        return new Transport()
        {
            Type = ParseTransportType(transportAtStartObject.GetProperty("type").GetString()!),
            ReplacementType = replacementType == string.Empty ? null : ParseTransportType(replacementType),
            Label = transportAtStartObject.GetProperty("label").GetString()!,
            Category = transportAtStartObject.GetProperty("category").GetString()!,
            CategoryInternal = transportAtStartObject.GetProperty("categoryInternal").GetString()!,
            JourneyDescription = SimplifyJourneyDescription(transportAtStartObject.GetProperty("journeyDescription").GetString()!),
            Number = transportAtStartObject.GetProperty("journeyNumber").GetInt32(), 
            Line = transportAtStartObject.TryGetProperty("line", out var lineObject) && lineObject.ValueKind == JsonValueKind.String 
                ? lineObject.GetString()
                : null,
        };
    }

    private ScheduleAtStopPlace BuildSchedule(DateOnly date, JsonElement scheduleObject, Dictionary<int, List<Information>> infoDict)
    {
        var scheduleType = ParseScheduleType(scheduleObject.GetProperty("type").GetString()!);
        
        var plannedTime = DateTime.Parse(scheduleObject.GetProperty("timeSchedule").GetString() ?? throw new InvalidOperationException("Tried to parse 'timeSchedule' but it was missing."), null, DateTimeStyles.AdjustToUniversal);
        if (!DateTime.TryParse(scheduleObject.GetProperty("time").GetString() ?? throw new InvalidOperationException("Tried to parse 'time' but it was missing."), null, DateTimeStyles.AdjustToUniversal, out var actualTime))
            actualTime = plannedTime;
        
        var plannedPlatform = string.Empty;
        if (scheduleObject.TryGetProperty("platformSchedule", out var plannedPlatformElement) && plannedPlatformElement.ValueKind != JsonValueKind.Null)
            plannedPlatform = plannedPlatformElement.GetString() ?? string.Empty;

        string actualPlatform = string.Empty;
        if (scheduleObject.TryGetProperty("platform", out var actualPlatformElement) && actualPlatformElement.ValueKind != JsonValueKind.Null)
            actualPlatform = actualPlatformElement.GetString() ?? string.Empty;
        else actualPlatform = plannedPlatform;
        
        var stopPlace = scheduleObject.GetProperty("stopPlace");
        if (stopPlace.ValueKind != JsonValueKind.Object)
            throw new InvalidOperationException("Tried to parse 'stopPlace' but it was missing.");

        return new ScheduleAtStopPlace()
        {
            Type = scheduleType,
            Date = date,
            Name = stopPlace.GetProperty("name").GetString()!,
            EvaNumber = int.TryParse(stopPlace.GetProperty("evaNumber").GetString(), out var evaNumber)
                ? evaNumber
                : throw new InvalidOperationException("Tried to parse 'evaNumber' but it failed."),
            PlannedTime = plannedTime,
            ActualTime = actualTime,
            Delay = (int)(actualTime - plannedTime).TotalSeconds,
            PlannedPlatform = plannedPlatform,
            ActualPlatform = actualPlatform,
            Additional = scheduleObject.TryGetProperty("additional", out var additionalElement) &&
                         additionalElement.ValueKind == JsonValueKind.True,
            Cancelled = scheduleObject.TryGetProperty("cancelled", out var cancelledElement) &&
                        cancelledElement.ValueKind == JsonValueKind.True,
            Demand = scheduleObject.TryGetProperty("onDemand", out var onDemandElement) &&
                     onDemandElement.ValueKind == JsonValueKind.True,
            NoPassengerChange = scheduleObject.TryGetProperty("noPassengerChange", out var noPassengerChangeElement) &&
                                noPassengerChangeElement.ValueKind == JsonValueKind.True,
            Information = scheduleObject.TryGetProperty("messages", out var messagesElement) && messagesElement.ValueKind == JsonValueKind.Array
                ? messagesElement.EnumerateArray()
                    .Select(messageId => messageId.GetInt32())
                    .Where(infoDict.ContainsKey)
                    .SelectMany(messageId => infoDict[messageId])
                    .Select(info => info.Clone())
                    .ToList()
                : new List<Information>()
        };
    }
    
    private TransportType ParseTransportType(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) 
            return TransportType.UNKNOWN;
        if (Enum.TryParse<TransportType>(input, true, out var transportType))
            return transportType;
        return TransportType.UNKNOWN;
    }

    private JourneyType ParseJourneyType(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return JourneyType.REGULAR;
        if (Enum.TryParse<JourneyType>(input, true, out var journeyType))
            return journeyType;
        return JourneyType.REGULAR;
    }

    private ScheduleType ParseScheduleType(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return ScheduleType.DEPARTURE;
        if (Enum.TryParse<ScheduleType>(input, true, out var scheduleType))
            return scheduleType;
        return ScheduleType.DEPARTURE;
    }
    
    private string SimplifyJourneyDescription(string input)
    {
        return Regex.Replace(input, _journeyDescriptionPattern, "");
    }
}

class JourneyResponse
{
    public required IdentifiedRisId RisId { get; init; }
    public Journey? Journey { get; set; }
    public DateTime LastSeen { get; init; }
}