using Navigator.Daemon.Mapping;
using Microsoft.Extensions.Logging;
using Navigator.Data.Enums;
using Navigator.Data.Models.Journey;
using Navigator.Data.Models.RisId;
using Navigator.Data.Repository.JourneyRepository;
using Navigator.Data.Repository.RisIdRepository;
using Quartz;
using Navigator.Data.Entities;
using Navigator.Observability;

namespace Navigator.Daemon.Infrastructure;

[DisallowConcurrentExecution]
public class GatheringJourneysJob(
    ILogger<GatheringJourneysJob> logger,
    IRisIdRepository risIdRepository,
    IJourneyRepository journeyRepository,
    JourneyMapper mapper
) : IJob
{
    private const int TimetableChangesPerDirection = 6;
    private const int CompletedJourneyDelayDays = 2;

    public async Task Execute(IJobExecutionContext context) => await logger.RunJobAsync(nameof(GatheringJourneysJob), context.FireInstanceId, ExecuteCoreAsync);

    private async Task ExecuteCoreAsync()
    {
        var currentDate = DateTime.UtcNow;
        var latestCompletedDate = currentDate.Date.AddDays(-CompletedJourneyDelayDays);

        var risIdBatch = await risIdRepository.GetRisIdsBatchAsync(new RisIdBatchRequest()
        {
            OnlyActive = true,
            IncludeNullDates = true,
            CutoffLastSeen = latestCompletedDate,
            OrderBy = RisIdOrder.LastSeen
        });
        if (!risIdBatch.Any()) return;

        var journeyBatchRequest = risIdBatch.Select(risId => new JourneyOnDateRequest()
        {
            Id = risId.Id,
            FetchingDate = EstimateLastQueried(risId, latestCompletedDate)
        }).ToList();

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

            var mappedJourneys = journeyBatch.Journeys.Select(journey => mapper.MapJourney(journey)).ToList();

            await risIdRepository.SaveRisIdsBatchAsync(successfulRisIds);
            await journeyRepository.SaveJourneyBatchAsync(mappedJourneys);
        }

        logger.LogInformation("Finished gathering journeys. {RisIdsCount} RisIds of which {JourneysCount} had journeys", risIdBatch.Count(), journeyBatch.Journeys.Count());
    }

    private DateTime EstimateLastQueried(RisId risId, DateTime latestCompletedDate)
    {
        if (risId.LastSeen == null)
        {
            var lastSeen = GetLastTimetableChange(latestCompletedDate);
            return new DateTime(
                DateOnly.FromDateTime(lastSeen.AddDays(-7)),
                TimeOnly.FromTimeSpan(DateTime.UtcNow.TimeOfDay),
                DateTimeKind.Utc
            );
        }

        var nextFetchDate = risId.LastSeen.Value.Date.AddDays(1);
        if (nextFetchDate > latestCompletedDate) nextFetchDate = latestCompletedDate;

        return new DateTime(
            DateOnly.FromDateTime(nextFetchDate),
            TimeOnly.FromTimeSpan(DateTime.UtcNow.TimeOfDay),
            DateTimeKind.Utc
        );
    }

    private DateTime GetLastTimetableChange(DateTime? comparing = null)
    {
        comparing ??= DateTime.UtcNow;
        return GetTimetableChanges(comparing.Value)
            .Where(timetableChange => timetableChange <= comparing)
            .DefaultIfEmpty(GetTimetableChange(comparing.Value.Year, 6))
            .Max();
    }

    private static IEnumerable<DateTime> GetTimetableChanges(DateTime comparing)
    {
        var candidates = Enumerable
            .Range(comparing.Year - 4, 9)
            .SelectMany(candidateYear => new[]
            {
                GetTimetableChange(candidateYear, 6),
                GetTimetableChange(candidateYear, 12)
            })
            .Order()
            .ToList();

        return candidates
            .Where(timetableChange => timetableChange <= comparing)
            .TakeLast(TimetableChangesPerDirection)
            .Concat(candidates
                .Where(timetableChange => timetableChange > comparing)
                .Take(TimetableChangesPerDirection));
    }

    private static DateTime GetTimetableChange(int year, int month)
    {
        // European rail timetable changes start after the second Saturday in June and December.
        var firstDay = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var daysUntilSaturday = ((int)DayOfWeek.Saturday - (int)firstDay.DayOfWeek + 7) % 7;
        var secondSaturday = firstDay.AddDays(daysUntilSaturday + 7);
        return secondSaturday.AddDays(1);
    }
}
