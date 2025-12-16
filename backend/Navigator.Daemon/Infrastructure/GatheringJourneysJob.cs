using AutoMapper;
using Microsoft.Extensions.Logging;
using Navigator.Data.Entities.Journey;
using Navigator.Data.Entities.RisId;
using Navigator.Data.Models.Journey;
using Navigator.Data.Models.RisId;
using Navigator.Data.Repository.AdministrationRepository;
using Navigator.Data.Repository.JourneyRepository;
using Navigator.Data.Repository.RisIdRepository;
using Quartz;

namespace Navigator.Daemon.Infrastructure;

[DisallowConcurrentExecution]
public class GatheringJourneysJob(
    ILogger<GatheringJourneysJob> logger,
    IRisIdRepository risIdRepository,
    IJourneyRepository journeyRepository,
    IAdministrationRepository administrationRepository,
    IMapper mapper
) : IJob
{
    private readonly DateTime[] _timetableChanges =
    {
        new(2024, 12, 17, 0, 0, 0, DateTimeKind.Utc),
        new(2025, 6, 15, 0, 0, 0, DateTimeKind.Utc),
        new(2025, 12, 14, 0, 0, 0, DateTimeKind.Utc)
    };

    public async Task Execute(IJobExecutionContext context)
    {
        var risIds = await risIdRepository.GetRisIdsBatchAsync(new ShuffledRisIdRequest()
        {
            OnlyIncludeActive = true,
            LastSeen = DateTime.UtcNow.Date.AddDays(-2),
            Limit = 384
        });
        if (!risIds.Any()) return;

        var journeyRequest = risIds.Select(risId => new JourneyOnDateRequest()
        {
            Id = risId.Id,
            FetchingDate = EstimateLastQueried(risId)
        });
        var journeys = await journeyRepository.GetJourneysBatchAsync(journeyRequest);
        if (journeys is null) return;

        if (journeys.ErroneousJourneys.Any())
        {
            var journeyIds = journeys.ErroneousJourneys
                .Select(erroneousJourney =>
                {
                    var datePart = erroneousJourney.JourneyID.Substring(0, 8);
                    if (!DateTime.TryParseExact(datePart, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out _))
                        return null;

                    return erroneousJourney.JourneyID.Substring(9);
                })
                .Where(journeyId => journeyId != null)
                .Select(journeyId => journeyId!)
                .ToHashSet();
            var failedRisIds = risIds
                .Where(risId => journeyIds.Contains(risId.Id))
                .ToList();

            foreach (var risId in failedRisIds)
            {
                risId.LastSeen = journeyRequest
                    .Where(request => request.Id == risId.Id)
                    .Select(request => request)
                    .FirstOrDefault()?
                    .FetchingDate ?? null;
            }
            await risIdRepository.SaveRisIdsBatchAsync(failedRisIds);
        }

        if (journeys.Journeys.Any())
        {
            var journeyIds = journeys.Journeys
                .Select(journey =>
                {
                    var datePart = journey.JourneyID.Substring(0, 8);
                    if (!DateTime.TryParseExact(datePart, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out _))
                        return null;

                    return journey.JourneyID.Substring(9);
                })
                .Where(journeyId => journeyId != null)
                .Select(journeyId => journeyId!)
                .ToHashSet();
            var successfulRisIds = risIds
                .Where(risId => journeyIds.Contains(risId.Id))
                .ToList();

            foreach (var risId in successfulRisIds)
            {
                var fetchingDate = journeyRequest
                    .Where(request => request.Id == risId.Id)
                    .Select(request => request)
                    .FirstOrDefault()?
                    .FetchingDate ?? null;

                risId.LastSeen = fetchingDate;
                risId.LastInserted = fetchingDate;
            }

            await risIdRepository.SaveRisIdsBatchAsync(successfulRisIds);

            DateTime now = DateTime.UtcNow;
            var mappedJourneys = mapper.Map<IEnumerable<Journey>>(journeys.Journeys);
            foreach (var mappedJourney in mappedJourneys)
            {
                mappedJourney.InsertedAt = now;

                var journeyAdministration = journeys.Journeys
                    .Where(journey => journey.JourneyID == mappedJourney.Id)
                    .Select(journey => journey.Info.HeaderAdministration)
                    .Select(async administration => await administrationRepository.GetOrCreateAdministrationAsync(
                        administration.AdministrationID,
                        administration.OperatorCode,
                        administration.OperatorName
                    ))
                    .FirstOrDefault();
                mappedJourney.Administration = journeyAdministration!.Result;
            }
            await journeyRepository.SaveJourneysBatchAsync(mappedJourneys);

            logger.LogInformation("Gathered {Count} journeys for {RisIdCount} RisIds.", mappedJourneys.Count(), successfulRisIds.Count);
        }
    }

    private DateTime EstimateLastQueried(RisId risId)
    {
        if (risId.LastSeen == null)
        {
            var lastSeen = GetLastTimetableChange(DateTime.UtcNow.Date.AddDays(-1));
            return new DateTime(
                DateOnly.FromDateTime(lastSeen.AddDays(-7)),
                TimeOnly.FromTimeSpan(DateTime.UtcNow.TimeOfDay),
                DateTimeKind.Utc
            );
        }
           
        return new DateTime(
            DateOnly.FromDateTime(risId.LastSeen.Value.Date.AddDays(1)),
            TimeOnly.FromTimeSpan(DateTime.UtcNow.TimeOfDay),
            DateTimeKind.Utc
        );
    }

    private DateTime GetLastTimetableChange(DateTime? comparing = null)
    {
        comparing ??= DateTime.UtcNow;
        return _timetableChanges
            .Where(timetableChange => timetableChange <= comparing)
            .DefaultIfEmpty(_timetableChanges.Min())
            .Max();
    }
}
