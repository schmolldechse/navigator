using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Navigator.Data.StatisticsRefresh;
using Navigator.Observability;
using Quartz;

namespace Navigator.Daemon.Infrastructure;

[DisallowConcurrentExecution]
public sealed class StatisticsAggregateRefreshJob(
    ILogger<StatisticsAggregateRefreshJob> logger,
    IOptions<StatisticsRefreshOptions> options,
    IStatisticsRefreshWindowPlanner windowPlanner,
    IStatisticsRollupRefreshService refreshService,
    IStatisticsRefreshQueueService queueService) : IJob
{
    public async Task Execute(IJobExecutionContext context) =>
        await logger.RunJobAsync(
            nameof(StatisticsAggregateRefreshJob),
            context.FireInstanceId,
            () => ExecuteCoreAsync(context.CancellationToken));

    private async Task ExecuteCoreAsync(CancellationToken cancellationToken)
    {
        var refreshOptions = options.Value;
        var end = AlignToHour(DateTime.UtcNow.AddHours(-refreshOptions.EndOffsetHours));
        var plan = await windowPlanner.PlanDaemonWindowsAsync(end, cancellationToken);

        logger.LogInformation(
            "Statistics aggregate refresh planned. EndExclusiveUtc={EndExclusiveUtc} WindowCount={WindowCount} QueuedWindowCount={QueuedWindowCount} HotLookbackHours={HotLookbackHours} CatchupLookbackHours={CatchupLookbackHours} MaxWindowsPerRun={MaxWindowsPerRun}",
            plan.EndExclusiveUtc,
            plan.Windows.Count,
            plan.QueuedWindows.Count,
            refreshOptions.HotLookbackHours,
            refreshOptions.CatchupLookbackHours,
            refreshOptions.MaxWindowsPerRun);

        foreach (var window in plan.Windows)
        {
            var isQueuedWindow = plan.QueuedWindows.Contains(window);

            if (isQueuedWindow)
            {
                logger.LogInformation(
                    "Processing queued statistics refresh window. WindowStart={WindowStart} WindowEnd={WindowEnd}",
                    window.Start,
                    window.End);

                await queueService.MarkWindowRunningAsync(window, cancellationToken);
            }

            try
            {
                await refreshService.RefreshWindowAsync(
                    window,
                    "daemon",
                    false,
                    cancellationToken);

                if (isQueuedWindow)
                {
                    await queueService.MarkWindowSucceededAsync(window, cancellationToken);
                }
            }
            catch (Exception exception) when (isQueuedWindow)
            {
                await queueService.MarkWindowFailedAsync(window, exception, CancellationToken.None);
                throw;
            }
        }

        var caggRefreshWindows = CoalesceContiguousWindows(plan.Windows);

        logger.LogInformation(
            "Statistics aggregate CAGG refresh planned. RangeCount={RangeCount}",
            caggRefreshWindows.Count);

        foreach (var window in caggRefreshWindows)
        {
            await refreshService.RefreshContinuousAggregatesAsync(
                window,
                "daemon",
                cancellationToken);
        }
    }

    private static DateTime AlignToHour(DateTime value) =>
        new(value.Year, value.Month, value.Day, value.Hour, 0, 0, DateTimeKind.Utc);

    private static IReadOnlyList<StatisticsRefreshWindow> CoalesceContiguousWindows(
        IReadOnlyCollection<StatisticsRefreshWindow> windows)
    {
        var ordered = windows.OrderBy(window => window.Start).ToArray();
        if (ordered.Length == 0)
        {
            return [];
        }

        var ranges = new List<StatisticsRefreshWindow>();
        var rangeStart = ordered[0].Start;
        var rangeEnd = ordered[0].End;

        foreach (var window in ordered.Skip(1))
        {
            if (window.Start <= rangeEnd)
            {
                if (window.End > rangeEnd)
                {
                    rangeEnd = window.End;
                }

                continue;
            }

            ranges.Add(new StatisticsRefreshWindow(rangeStart, rangeEnd));
            rangeStart = window.Start;
            rangeEnd = window.End;
        }

        ranges.Add(new StatisticsRefreshWindow(rangeStart, rangeEnd));
        return ranges;
    }
}
