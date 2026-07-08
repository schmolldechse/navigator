using Navigator.Data.Entities.Journey;
using Navigator.Data.Enums;

namespace Navigator.Data.StatisticsRefresh;

public interface IStatisticsRefreshQueueService
{
    Task MarkWindowsForJourneysAsync(
        IReadOnlyCollection<Journey> journeys,
        StatisticsRefreshQueueSource source,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<StatisticsRefreshWindow>> LoadQueuedWindowsAsync(
        int limit,
        DateTime endExclusiveUtc,
        IReadOnlySet<StatisticsRefreshWindow> excludedWindows,
        CancellationToken cancellationToken);

    Task MarkWindowRunningAsync(
        StatisticsRefreshWindow window,
        CancellationToken cancellationToken);

    Task MarkWindowSucceededAsync(
        StatisticsRefreshWindow window,
        CancellationToken cancellationToken);

    Task MarkWindowFailedAsync(
        StatisticsRefreshWindow window,
        Exception exception,
        CancellationToken cancellationToken);
}
