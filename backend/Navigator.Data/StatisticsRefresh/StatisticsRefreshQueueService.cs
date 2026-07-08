using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Navigator.Data.Entities.Journey;
using Navigator.Data.Enums;

namespace Navigator.Data.StatisticsRefresh;

public sealed class StatisticsRefreshQueueService(
    DataContext dataContext,
    IOptions<StatisticsRefreshOptions> options,
    ILogger<StatisticsRefreshQueueService> logger) : IStatisticsRefreshQueueService
{
    public async Task MarkWindowsForJourneysAsync(
        IReadOnlyCollection<Journey> journeys,
        StatisticsRefreshQueueSource source,
        CancellationToken cancellationToken)
    {
        var windows = journeys
            .SelectMany(GetAffectedWindows)
            .Distinct()
            .OrderBy(window => window.Start)
            .ToArray();

        foreach (var window in windows)
        {
            window.Validate();

            await dataContext.Database.ExecuteSqlInterpolatedAsync($"""
                INSERT INTO statistics.statistics_refresh_queue (
                    window_start,
                    window_end,
                    status,
                    source,
                    mark_count,
                    attempt,
                    first_marked_at,
                    last_marked_at
                )
                VALUES (
                    {window.Start},
                    {window.End},
                    {StatisticsRefreshQueueStatus.Pending},
                    {source},
                    1,
                    0,
                    now(),
                    now()
                )
                ON CONFLICT (window_start, window_end)
                DO UPDATE SET status = {StatisticsRefreshQueueStatus.Pending},
                              source = EXCLUDED.source,
                              mark_count = statistics.statistics_refresh_queue.mark_count + 1,
                              last_marked_at = now(),
                              error_kind = NULL;
                """, cancellationToken);
        }

        if (windows.Length > 0)
        {
            logger.LogInformation(
                "Marked statistics refresh windows. Source={Source} JourneyCount={JourneyCount} WindowCount={WindowCount} FirstWindowStart={FirstWindowStart} LastWindowEnd={LastWindowEnd}",
                source,
                journeys.Count,
                windows.Length,
                windows.First().Start,
                windows.Last().End);
        }
    }

    public async Task<IReadOnlyList<StatisticsRefreshWindow>> LoadQueuedWindowsAsync(
        int limit,
        DateTime endExclusiveUtc,
        IReadOnlySet<StatisticsRefreshWindow> excludedWindows,
        CancellationToken cancellationToken)
    {
        if (limit <= 0)
        {
            return [];
        }

        var alignedEndExclusiveUtc = AsUtc(endExclusiveUtc);
        var retryBefore = DateTime.UtcNow.AddMinutes(-options.Value.QueueRetryDelayMinutes);

        var rows = await dataContext.StatisticsRefreshQueue
            .Where(row => row.WindowEnd <= alignedEndExclusiveUtc)
            .Where(row => row.Status == StatisticsRefreshQueueStatus.Pending
                          || (row.Status == StatisticsRefreshQueueStatus.Failed
                              && row.FinishedAt != null
                              && row.FinishedAt <= retryBefore))
            .OrderBy(row => row.WindowStart)
            .Take(limit * 3)
            .Select(row => new { row.WindowStart, row.WindowEnd })
            .ToListAsync(cancellationToken);

        return rows
            .Select(row => new StatisticsRefreshWindow(
                AsUtc(row.WindowStart),
                AsUtc(row.WindowEnd)))
            .Where(window => !excludedWindows.Contains(window))
            .Take(limit)
            .ToArray();
    }

    public async Task MarkWindowRunningAsync(
        StatisticsRefreshWindow window,
        CancellationToken cancellationToken)
    {
        window.Validate();

        await dataContext.Database.ExecuteSqlInterpolatedAsync($"""
            UPDATE statistics.statistics_refresh_queue
               SET status = {StatisticsRefreshQueueStatus.Running},
                   attempt = attempt + 1,
                   started_at = now(),
                   finished_at = NULL,
                   error_kind = NULL
             WHERE window_start = {window.Start}
               AND window_end = {window.End};
            """, cancellationToken);
    }

    public async Task MarkWindowSucceededAsync(
        StatisticsRefreshWindow window,
        CancellationToken cancellationToken)
    {
        window.Validate();

        await dataContext.Database.ExecuteSqlInterpolatedAsync($"""
            UPDATE statistics.statistics_refresh_queue
               SET status = {StatisticsRefreshQueueStatus.Success},
                   finished_at = now(),
                   error_kind = NULL
             WHERE window_start = {window.Start}
               AND window_end = {window.End};
            """, cancellationToken);
    }

    public async Task MarkWindowFailedAsync(
        StatisticsRefreshWindow window,
        Exception exception,
        CancellationToken cancellationToken)
    {
        window.Validate();

        await dataContext.Database.ExecuteSqlInterpolatedAsync($"""
            UPDATE statistics.statistics_refresh_queue
               SET status = {StatisticsRefreshQueueStatus.Failed},
                   finished_at = now(),
                   error_kind = {exception.GetType().Name}
             WHERE window_start = {window.Start}
               AND window_end = {window.End};
            """, cancellationToken);
    }

    private static IEnumerable<StatisticsRefreshWindow> GetAffectedWindows(Journey journey)
    {
        var stopPlaces = journey.StopPlaces.ToArray();
        if (stopPlaces.Length == 0)
        {
            yield break;
        }

        foreach (var bucketStart in stopPlaces.Select(stopPlace => AlignToHour(AsUtc(stopPlace.PlannedTime))).Distinct())
        {
            yield return new StatisticsRefreshWindow(bucketStart, bucketStart.AddHours(1));
        }

        var journeyStart = stopPlaces
            .Where(stopPlace => stopPlace.ScheduleType == ScheduleType.Departure)
            .OrderBy(stopPlace => stopPlace.PlannedTime)
            .Select(stopPlace => (DateTime?)AsUtc(stopPlace.PlannedTime))
            .FirstOrDefault()
            ?? stopPlaces.Min(stopPlace => AsUtc(stopPlace.PlannedTime));

        var journeyBucketStart = AlignToHour(journeyStart);
        yield return new StatisticsRefreshWindow(journeyBucketStart, journeyBucketStart.AddHours(1));
    }

    private static DateTime AsUtc(DateTime value) =>
        value.Kind == DateTimeKind.Utc
            ? value
            : DateTime.SpecifyKind(value, DateTimeKind.Utc);

    private static DateTime AlignToHour(DateTime value) =>
        new(value.Year, value.Month, value.Day, value.Hour, 0, 0, DateTimeKind.Utc);
}
