namespace Navigator.Daemon.Infrastructure;

public sealed class JourneyFactProjectionCoordinator
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public async Task<T> RunExclusiveAsync<T>(
        Func<CancellationToken, Task<T>> action,
        CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            return await action(cancellationToken);
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
