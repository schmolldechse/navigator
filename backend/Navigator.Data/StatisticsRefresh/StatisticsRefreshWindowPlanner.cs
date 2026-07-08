using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Navigator.Data.StatisticsRefresh;

public sealed class StatisticsRefreshWindowPlanner(
    DataContext dataContext,
    IOptions<StatisticsRefreshOptions> options,
    IStatisticsRefreshQueueService queueService) : IStatisticsRefreshWindowPlanner
{
    public async Task<StatisticsRefreshWindowPlan> PlanDaemonWindowsAsync(
        DateTime endExclusiveUtc,
        CancellationToken cancellationToken)
    {
        var refreshOptions = options.Value;
        var alignedEnd = AlignToHour(endExclusiveUtc);
        var hotStart = alignedEnd.AddHours(-refreshOptions.HotLookbackHours);
        var catchupStart = alignedEnd.AddHours(-refreshOptions.CatchupLookbackHours);

        var selected = EnumerateWindows(hotStart, alignedEnd, refreshOptions.WindowHours).ToList();
        var remaining = refreshOptions.MaxWindowsPerRun - selected.Count;
        var selectedSet = selected.Distinct().ToHashSet();
        var queuedWindows = Array.Empty<StatisticsRefreshWindow>();

        if (remaining > 0)
        {
            var queueLimit = Math.Min(remaining, refreshOptions.QueuedWindowMaxPerRun);
            queuedWindows = (await queueService.LoadQueuedWindowsAsync(
                queueLimit,
                alignedEnd,
                selectedSet,
                cancellationToken)).ToArray();

            selected.AddRange(queuedWindows);
            selectedSet = selected.Distinct().ToHashSet();
            remaining = refreshOptions.MaxWindowsPerRun - selectedSet.Count;
        }

        if (remaining > 0)
        {
            selected.AddRange(await LoadFailedWindowsAsync(catchupStart, hotStart, remaining, cancellationToken));
            selectedSet = selected.Distinct().ToHashSet();
            remaining = refreshOptions.MaxWindowsPerRun - selectedSet.Count;
        }

        if (remaining > 0)
        {
            selected.Add(SelectRotatingCatchupWindow(catchupStart, hotStart, refreshOptions.WindowHours, alignedEnd));
        }

        var windows = selected
            .Distinct()
            .Where(window => window.Start < window.End)
            .OrderBy(window => window.Start)
            .Take(refreshOptions.MaxWindowsPerRun)
            .ToArray();

        return new StatisticsRefreshWindowPlan(
            alignedEnd,
            windows,
            queuedWindows.ToHashSet());
    }

    private async Task<IReadOnlyList<StatisticsRefreshWindow>> LoadFailedWindowsAsync(
        DateTime catchupStart,
        DateTime hotStart,
        int limit,
        CancellationToken cancellationToken)
    {
        var rows = await dataContext.StatisticsRefreshProgress
            .Where(row => row.Operation == "daemon"
                       && row.Status == "failed"
                       && row.WindowStart >= catchupStart
                       && row.WindowEnd <= hotStart)
            .OrderBy(row => row.WindowStart)
            .Take(limit)
            .Select(row => new { row.WindowStart, row.WindowEnd })
            .ToListAsync(cancellationToken);

        return rows
            .Select(row => new StatisticsRefreshWindow(
                DateTime.SpecifyKind(row.WindowStart, DateTimeKind.Utc),
                DateTime.SpecifyKind(row.WindowEnd, DateTimeKind.Utc)))
            .ToArray();
    }

    private static StatisticsRefreshWindow SelectRotatingCatchupWindow(
        DateTime catchupStart,
        DateTime hotStart,
        int windowHours,
        DateTime alignedEnd)
    {
        var candidates = EnumerateWindows(catchupStart, hotStart, windowHours).ToArray();
        if (candidates.Length == 0)
        {
            return new StatisticsRefreshWindow(hotStart, hotStart);
        }

        var slot = Math.Abs(alignedEnd.Ticks / TimeSpan.TicksPerHour) % candidates.Length;
        return candidates[slot];
    }

    private static IEnumerable<StatisticsRefreshWindow> EnumerateWindows(
        DateTime start,
        DateTime end,
        int windowHours)
    {
        for (var windowStart = AlignToHour(start); windowStart < end; windowStart = windowStart.AddHours(windowHours))
        {
            yield return new StatisticsRefreshWindow(windowStart, windowStart.AddHours(windowHours));
        }
    }

    private static DateTime AlignToHour(DateTime value) =>
        new(value.Year, value.Month, value.Day, value.Hour, 0, 0, DateTimeKind.Utc);
}
