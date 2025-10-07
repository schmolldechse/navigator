using System.Data;
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

internal readonly record struct SingleJourneyResponse(IdentifiedRisId RisId, DateTime NewLastSeen, Journey? Journey);

public class GatheringJourneyDaemon(
    ILogger<GatheringJourneyDaemon> logger,
    IServiceProvider serviceProvider,
    ProxyRotator proxyRotator) : Daemon("Gathering Journey", TimeSpan.FromSeconds(15), logger)
{
    private readonly string _apiUrl = "https://apis.deutschebahn.com/db/apis/ris-journeys/v2/batch";
    private readonly string _journeyDescriptionPattern = @"\s\(.*?\)";

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

        List<SingleJourneyResponse> risIdsToProcess;
        await using (var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken))
        {
            var risIds = await (
                    from risId in dbContext.RisIds
                    where !risId.IsLocked && risId.Active && (risId.LastSeen == null || risId.LastSeen < DateTime.UtcNow.Date.AddDays(-2))
                    orderby risId.LastSeen ?? DateTime.MinValue
                    select risId
                ).Take(15000)
                .ToListAsync(cancellationToken);
            risIdsToProcess = (
                    from risId in risIds
                    orderby Random.Shared.Next()
                    select new SingleJourneyResponse(risId, EvaluateLastSeen(risId), null))
                .Take(384)
                .ToList();
            if (risIdsToProcess.Count == 0)
            {
                await transaction.RollbackAsync(cancellationToken);
                return;
            }

            risIdsToProcess.ForEach(journeyResponse => journeyResponse.RisId.IsLocked = true);
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        try
        {
            var journeys = await CallApi(risIdsToProcess, dbContext, cancellationToken);
            foreach (var journeyResponse in journeys)
            {
                journeyResponse.RisId.LastSeen = journeyResponse.NewLastSeen;
                if (journeyResponse.Journey != null)
                {
                    journeyResponse.RisId.LastSucceededAt = journeyResponse.NewLastSeen;
                    var exists = await dbContext.Journeys.AnyAsync(journey => journey.Id == journeyResponse.Journey!.Id, cancellationToken);
                    if (!exists) dbContext.Journeys.Add(journeyResponse.Journey);
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Successfully inserted {Count} journeys", journeys.Where(j => j.Journey != null).Count());
        }
        finally
        {
            using var unlockScope = serviceProvider.CreateScope();
            var unlockDbContext = unlockScope.ServiceProvider.GetRequiredService<NavigatorDbContext>();
            var idsToUnlock = risIdsToProcess.Select(risId => risId.RisId.Id).ToList();
            await unlockDbContext.RisIds
                .Where(risId => idsToUnlock.Contains(risId.Id))
                .ExecuteUpdateAsync(s => s.SetProperty(r => r.IsLocked, false), cancellationToken);
        }
    }

    private DateTime EvaluateLastSeen(IdentifiedRisId risId)
    {
        /*
         * For RIS IDs that have never been processed (LastSeen is null):
         * - Gets the last timetable change date (starting from the previous day at midnight).
         * - Going back 7 days from the last timetable change date. (=> this could be lowered, but 7 days is a good buffer)
         */
        if (risId.LastSeen == null)
        {
            var lastSeen = GetLastTimetableChange(DateTime.UtcNow.Date.AddDays(-1));
            return new DateTime(DateOnly.FromDateTime(lastSeen.AddDays(-7)), TimeOnly.FromTimeSpan(DateTime.UtcNow.TimeOfDay), DateTimeKind.Utc);
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
        return new DateTime(DateOnly.FromDateTime(risId.LastSeen!.Value.Date.AddDays(1)), TimeOnly.FromTimeSpan(DateTime.UtcNow.TimeOfDay), DateTimeKind.Utc);
    }

    private async Task<List<SingleJourneyResponse>> CallApi(List<SingleJourneyResponse> risIds, NavigatorDbContext dbContext, CancellationToken cancellationToken)
    {
        logger.LogInformation("Trying to solve journeys for {Count}x RIS IDs", risIds.Count);

        var httpClient = proxyRotator.GetRandomProxy();
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, string.Format(_apiUrl));
        httpRequest.Content = new StringContent(JsonSerializer.Serialize(new 
        {
            includeReferences = true,
            journeyIDs = risIds.Select(journeyResponse => journeyResponse.NewLastSeen.ToString("yyyyMMdd") + "-" + journeyResponse.RisId.Id).ToArray(),
            separateCancelled = false
        }), System.Text.Encoding.UTF8, "application/json");
        httpRequest.Headers.Add("Accept", "application/vnd.de.db.ris+json");
        httpRequest.Headers.Add("DB-Client-Id", Environment.GetEnvironmentVariable("JOURNEYS_CLIENT_ID") ?? throw new InvalidOperationException("JOURNEYS_CLIENT_ID environment variable is not set."));
        httpRequest.Headers.Add("DB-Api-Key", Environment.GetEnvironmentVariable("JOURNEYS_API_KEY") ?? throw new InvalidOperationException("JOURNEYS_API_KEY environment variable is not set."));

        var response = await httpClient.SendAsync(httpRequest);
        if (!response.IsSuccessStatusCode)
        {
            logger.LogDebug("Failed to retrieve journeys. Aborting. Received {StatusCode} ({Response})", response.StatusCode, await response.Content.ReadAsStringAsync(cancellationToken));
            return risIds;
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var content = (await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken)).RootElement;
        logger.LogInformation("Successfully retrieved journeys for {Count}x RIS IDs. Received {StatusCode}", risIds.Count, response.StatusCode);

        if (content.TryGetProperty("journeys", out var journeysArray) && journeysArray.ValueKind == JsonValueKind.Array && journeysArray.GetArrayLength() > 0)
        {
            logger.LogDebug("Received {Count} journeys", journeysArray.GetArrayLength());
            foreach (var journeyElement in journeysArray.EnumerateArray())
            {
                var journey = await BuildJourney(journeyElement, dbContext, cancellationToken);

                var index = risIds.FindIndex(journeyResponse => journeyResponse.NewLastSeen.ToString("yyyyMMdd") + "-" + journeyResponse.RisId.Id == journeyElement.GetProperty("journeyID").GetString()!);
                if (index == -1) continue;

                risIds[index] = risIds[index] with { Journey = journey };
            }
        }
        return risIds;
    }

    private async Task<Journey?> BuildJourney(JsonElement journeyElement, NavigatorDbContext dbContext, CancellationToken cancellationToken)
    {
        if (!journeyElement.TryGetProperty("journeyID", out var journeyIdElement) || journeyIdElement.ValueKind != JsonValueKind.String || !journeyElement.TryGetProperty("info", out var infoObject) || infoObject.ValueKind != JsonValueKind.Object)
            return null;

        var journeyId = journeyIdElement.GetString()!;
        var date = DateOnly.ParseExact(journeyId[..8], "yyyyMMdd");

        if (!infoObject.TryGetProperty("headerAdministration", out var headerAdministrationObject) || headerAdministrationObject.ValueKind != JsonValueKind.Object)
            return null;
        var existingAdministration = await dbContext.Administrations.FirstOrDefaultAsync(
            administration => administration.AdministrationId == headerAdministrationObject.GetProperty("administrationID").GetString()! &&
                              administration.OperatorCode == headerAdministrationObject.GetProperty("operatorCode").GetString()! &&
                              administration.OperatorName == headerAdministrationObject.GetProperty("operatorName").GetString()!,
            cancellationToken);
        if (existingAdministration == null)
        {
            existingAdministration = new Administration()
            {
                AdministrationId = headerAdministrationObject.GetProperty("administrationID").GetString()!,
                OperatorCode = headerAdministrationObject.GetProperty("operatorCode").GetString()!,
                OperatorName = headerAdministrationObject.GetProperty("operatorName").GetString()!
            };
            dbContext.Administrations.Add(existingAdministration);
        }
        
        var informationDict = journeyElement.TryGetProperty("messages", out var messagesObject) && messagesObject.ValueKind == JsonValueKind.Object ? BuildInformationDict(journeyElement.GetProperty("messages")) : new Dictionary<int, List<Information>>();
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

        void ProcessMessageArray(string propertyName, Func<JsonElement, Information> func)
        {
            if (messagesObject.TryGetProperty(propertyName, out var messageArray) && messageArray.ValueKind == JsonValueKind.Array)
            {
                foreach (var message in messageArray.EnumerateArray())
                {
                    var messageId = message.GetProperty("messageID").GetInt32();
                    if (!infos.ContainsKey(messageId)) infos[messageId] = new List<Information>();
                    infos[messageId].Add(func(message));
                }
            }
        }

        ProcessMessageArray("attributes", attributeArray => new Information()
        {
            Type = InformationType.JOURNEY_ATTRIBUTE,
            Key = attributeArray.GetProperty("code").GetString()!,
            Text = attributeArray.GetProperty("text").GetString()!,
        });


        if (messagesObject.TryGetProperty("disruptions", out var disruptionsArray) && disruptionsArray.ValueKind == JsonValueKind.Array)
        {
            foreach (var disruption in disruptionsArray.EnumerateArray())
            {
                var messageId = disruption.GetProperty("messageID").GetInt32();
                if (!infos.ContainsKey(messageId)) infos[messageId] = new List<Information>();

                void AddDisruptionInfo(string langProperty)
                {
                    if (disruption.TryGetProperty(langProperty, out var langObject) && langObject.ValueKind == JsonValueKind.Object)
                    {
                        infos[messageId].Add(new Information()
                        {
                            Type = InformationType.DISRUPTION,
                            Key = "general-warning",
                            DisruptionCommunicationId = disruption.TryGetProperty("disruptionCommunicationID", out var disruptionCommunicationIdObject) ? disruptionCommunicationIdObject.GetString() : null,
                            DisruptionId = disruption.TryGetProperty("disruptionID", out var disruptionIdObject) ? disruptionIdObject.GetString() : null,
                            Text = langObject.GetProperty("text").GetString()!,
                            TextShort = langObject.GetProperty("textShort").GetString() ?? null,
                        });
                    }
                }

                AddDisruptionInfo("langDe");
                AddDisruptionInfo("langEn");
            }
        }

        if (messagesObject.TryGetProperty("notes", out var notesArray) && notesArray.ValueKind == JsonValueKind.Array)
        {
            ProcessMessageArray("notes", noteObject => new Information()
            {
                Type = InformationType.MESSAGE,
                Key = noteObject.TryGetProperty("code", out var codeElement) && codeElement.ValueKind == JsonValueKind.String ? codeElement.GetString()! : "information",
                Text = noteObject.GetProperty("text").GetString()!,
                TextShort = noteObject.TryGetProperty("textShort", out var textShortElement) && textShortElement.ValueKind == JsonValueKind.String ? textShortElement.GetString()!: null
            });
        }

        if (messagesObject.TryGetProperty("risCauseCodes", out var risCauseCodesArray) && risCauseCodesArray.ValueKind == JsonValueKind.Array)
        {
            ProcessMessageArray("risCauseCodes", risCauseCode => new Information()
            {
                Type = InformationType.RIS_CAUSE_REASON,
                Key = risCauseCode.GetProperty("code").GetString()!, 
                Text = risCauseCode.GetProperty("text").GetString()!,
            });
        }

        if (messagesObject.TryGetProperty("risQualityDeviations", out var risQualityDeviationsArray) && risQualityDeviationsArray.ValueKind == JsonValueKind.Array)
        {
            ProcessMessageArray("risQualityDeviations", risQualityDeviation => new Information() 
            {
                Type = InformationType.RIS_QUALITY_DEVIATION,
                Key = risQualityDeviation.GetProperty("code").GetString()!,
                Text = risQualityDeviation.GetProperty("text").GetString()!,
            });
        }

        return infos;
    }

    private Transport BuildTransport(JsonElement infoObject)
    {
        var transportAtStartObject = infoObject.GetProperty("transportAtStart");
        if (transportAtStartObject.ValueKind != JsonValueKind.Object) throw new InvalidOperationException("Invalid transportAtStart object");

        var replacementType =
            transportAtStartObject.TryGetProperty("replacementTransport", out var replacementTransportElement) &&
            replacementTransportElement.ValueKind == JsonValueKind.Object &&
            replacementTransportElement.TryGetProperty("realType", out var realTypeElement) ? realTypeElement.GetString() : null;

        return new Transport()
        {
            Type = ParseTransportType(transportAtStartObject.GetProperty("type").GetString()!),
            ReplacementType = replacementType == string.Empty ? null : ParseTransportType(replacementType),
            Label = transportAtStartObject.GetProperty("label").GetString()!,
            Category = transportAtStartObject.GetProperty("category").GetString()!,
            CategoryInternal = transportAtStartObject.GetProperty("categoryInternal").GetString()!,
            JourneyDescription = SimplifyJourneyDescription(transportAtStartObject.GetProperty("journeyDescription").GetString()!),
            Number = transportAtStartObject.GetProperty("journeyNumber").GetInt32(),
            Line = transportAtStartObject.TryGetProperty("line", out var lineObject) ? lineObject.GetString() : null,
        };
    }

    private ScheduleAtStopPlace BuildSchedule(DateOnly date, JsonElement scheduleObject,
        Dictionary<int, List<Information>> infoDict)
    {
        var plannedTime = DateTime.Parse(scheduleObject.GetProperty("timeSchedule").GetString() ?? throw new InvalidOperationException("Tried to parse 'timeSchedule' but it was missing.")).ToUniversalTime();
        var actualTime = DateTime.TryParse(scheduleObject.GetProperty("time").GetString(), out var time) ? time.ToUniversalTime() : plannedTime;

        var plannedPlatform = scheduleObject.TryGetProperty("platformSchedule", out var plannedPlatformElement) ? plannedPlatformElement.GetString() : null;
        var actualPlatform = scheduleObject.TryGetProperty("platform", out var actualPlatformElement) ? actualPlatformElement.GetString() : plannedPlatform;

        var stopPlace = scheduleObject.GetProperty("stopPlace");
        if (stopPlace.ValueKind != JsonValueKind.Object)
            throw new InvalidOperationException("Tried to parse 'stopPlace' but it was missing.");

        return new ScheduleAtStopPlace()
        {
            Type = ParseScheduleType(scheduleObject.GetProperty("type").GetString()!),
            Date = date,
            Name = stopPlace.GetProperty("name").GetString()!,
            EvaNumber = int.TryParse(stopPlace.GetProperty("evaNumber").GetString(), out var evaNumber) ? evaNumber : throw new InvalidOperationException("Tried to parse 'evaNumber' but it failed."),
            PlannedTime = plannedTime,
            ActualTime = actualTime,
            Delay = (int)(actualTime - plannedTime).TotalSeconds,
            PlannedPlatform = plannedPlatform,
            ActualPlatform = actualPlatform,
            Additional = scheduleObject.TryGetProperty("additional", out var additionalElement) && additionalElement.ValueKind == JsonValueKind.True,
            Cancelled = scheduleObject.TryGetProperty("cancelled", out var cancelledElement) && cancelledElement.ValueKind == JsonValueKind.True,
            Demand = scheduleObject.TryGetProperty("onDemand", out var onDemandElement) && onDemandElement.ValueKind == JsonValueKind.True,
            NoPassengerChange = scheduleObject.TryGetProperty("noPassengerChange", out var noPassengerChangeElement) && noPassengerChangeElement.ValueKind == JsonValueKind.True,
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

    private TEnum ParseEnum<TEnum>(string? input, TEnum defaultValue) where TEnum : struct, Enum => Enum.TryParse<TEnum>(input, true, out var result) ? result : defaultValue;

    private TransportType ParseTransportType(string? input) => ParseEnum<TransportType>(input, TransportType.UNKNOWN);
    private JourneyType ParseJourneyType(string? input) => ParseEnum<JourneyType>(input, JourneyType.REGULAR);
    private ScheduleType ParseScheduleType(string? input) => ParseEnum<ScheduleType>(input, ScheduleType.DEPARTURE);

    private string SimplifyJourneyDescription(string input) => Regex.Replace(input, _journeyDescriptionPattern, "");
}