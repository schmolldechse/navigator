using Microsoft.Extensions.Logging;
using Navigator.Data.Enums;
using Navigator.Data.Models.Journey;
using Navigator.Data.Models.RisId;
using Navigator.Data.Repository.JourneyRepository;
using Navigator.Data.Repository.RisIdRepository;
using Navigator.Observability;
using Quartz;

namespace Navigator.Daemon.Infrastructure;

[DisallowConcurrentExecution]
public class StaleRisIdDeactivationJob(
    IRisIdRepository risIdRepository,
    IJourneyRepository journeyRepository,
    ILogger<StaleRisIdDeactivationJob> logger
) : IJob
{
    public async Task Execute(IJobExecutionContext context) => await logger.RunJobAsync(nameof(StaleRisIdDeactivationJob), context.FireInstanceId, ExecuteCoreAsync);

    private async Task ExecuteCoreAsync()
    {
        var cutoffDiscoverd = DateTime.UtcNow.Date.AddDays(-21);
        var cutoffInserted = DateTime.UtcNow.Date.AddDays(-45);

        // 1. fetch random active RisIds as a sample
        var risIds = (await risIdRepository.GetRisIdsBatchAsync(new RisIdBatchRequest()
        {
            OnlyActive = true,
            IncludeNullDates = false,
            CutoffDiscovered = cutoffDiscoverd,
            CutoffLastInserted = cutoffInserted,
            OrderBy = RisIdOrder.Random
        })).ToList();
        if (!risIds.Any()) return;
        logger.LogInformation("Fetched {StaleRisIdCount} stale RisIds for deactivation check.", risIds.Count);

        // 2. check Journey occurences for each RisId
        var occuredJourneyIds = new HashSet<string>();
        for (int dayOffset = -2; dayOffset < 8; dayOffset++)
        {
            var journeyRequest = risIds.Select(risId => new JourneyOnDateRequest()
            {
                Id = risId.Id,
                FetchingDate = risId.LastSeen!.Value.Date.AddDays(dayOffset)
            });
            var journeyBatch = await journeyRepository.GetJourneyBatchAsync(journeyRequest);
            if (journeyBatch is null) return;

            foreach (var journey in journeyBatch.Journeys)
            {
                // JourneyID format: yyyyMMdd-{UUID}
                var parsedJourneyId = journey.JourneyID.Substring(9);
                occuredJourneyIds.Add(parsedJourneyId);
            }

            logger.LogInformation("Checked journeys for batch {BatchIndex}: {JourneyCount} journeys found.", dayOffset + 3, journeyBatch.Journeys.Count);
            if (dayOffset < 7) await Task.Delay(3000);
        }

        // 3. deacivate RisIds
        logger.LogInformation("Found {UniqueJourneyCount} unique journeys. Beginning with deactivation of stale RIS IDs.", occuredJourneyIds.Count);

        var risIdsToDeactivate = risIds.Where(risId => !occuredJourneyIds.Contains(risId.Id)).ToList();
        foreach (var risId in risIdsToDeactivate)
        {
            risId.Active = false;
        }

        if (!risIdsToDeactivate.Any()) logger.LogDebug("No RIS IDs found to deactivate.");
        else
        {
            await risIdRepository.SaveRisIdsBatchAsync(risIdsToDeactivate);
            logger.LogInformation("Deactivated {DeactivatedRisIdCount} RisIds.", risIdsToDeactivate.Count);
        }
    }
}
