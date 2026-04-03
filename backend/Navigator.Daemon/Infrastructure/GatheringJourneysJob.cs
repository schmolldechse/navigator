using Navigator.Daemon.Mapping;
using Microsoft.Extensions.Logging;
using Navigator.Data.Entities.RisId;
using Navigator.Data.Enums;
using Navigator.Data.Models.Journey;
using Navigator.Data.Models.RisId;
using Navigator.Data.Repository.JourneyRepository;
using Navigator.Data.Repository.RisIdRepository;
using Quartz;

namespace Navigator.Daemon.Infrastructure;

[DisallowConcurrentExecution]
public class GatheringJourneysJob(
    ILogger<GatheringJourneysJob> logger,
    IRisIdRepository risIdRepository,
    IJourneyRepository journeyRepository,
    JourneyMapper mapper
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
        var currentDate = DateTime.UtcNow;

        var risIdBatch = await risIdRepository.GetRisIdsBatchAsync(new RisIdBatchRequest()
        {
            OnlyActive = true,
            IncludeNullDates = true,
            CutoffLastSeen = currentDate.Date.AddDays(-2),
            OrderBy = RisIdOrder.LastSeen
        });
        if (!risIdBatch.Any()) return;

        var journeyBatchRequest = risIdBatch.Select(risId => new JourneyOnDateRequest()
        {
            Id = risId.Id,
            FetchingDate = EstimateLastQueried(risId)
        });

        var journeyBatch = await journeyRepository.GetJourneyBatchAsync(journeyBatchRequest);
        if (journeyBatch is null) return;

        if (journeyBatch.ErroneousJourneys.Any())
        {
            var journeyIds = journeyBatch.ErroneousJourneys
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
            var failedRisIds = risIdBatch
                .Where(risId => journeyIds.Contains(risId.Id))
                .ToList();

            foreach (var risId in failedRisIds)
            {
                risId.LastSeen = journeyBatchRequest
                    .Where(request => request.Id == risId.Id)
                    .Select(request => request)
                    .FirstOrDefault()?
                    .FetchingDate ?? null;
            }

            await risIdRepository.SaveRisIdsBatchAsync(failedRisIds);
        }
        if (journeyBatch.Journeys.Any())
        {
            var journeyIds = journeyBatch.Journeys
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
            var successfulRisIds = risIdBatch
                .Where(risId => journeyIds.Contains(risId.Id))
                .ToList();

            foreach (var risId in successfulRisIds)
            {
                var fetchingDate = journeyBatchRequest
                    .Where(request => request.Id == risId.Id)
                    .Select(request => request)
                    .FirstOrDefault()?
                    .FetchingDate ?? null;

                risId.LastSeen = fetchingDate;
                risId.LastInserted = fetchingDate;
            }

            var mappedJourneys = journeyBatch.Journeys.Select(journey =>
            {
                var mappedJourney = mapper.MapJourney(journey);
                mappedJourney.InsertedAt = currentDate;
                return mappedJourney;
            }).ToList();

            await risIdRepository.SaveRisIdsBatchAsync(successfulRisIds);
            await journeyRepository.SaveJourneyBatchAsync(mappedJourneys);
        }

        logger.LogInformation("Finished gathering journeys. {RisIdsCount} RisIds of which {JourneysCount} had journeys", risIdBatch.Count(), journeyBatch.Journeys.Count());
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
