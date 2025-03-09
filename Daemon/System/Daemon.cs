namespace Daemon.System;

public abstract class Daemon : IDisposable
{
    private CancellationTokenSource _cancellationTokenSource;
    private Task _executionTask;
    private bool _disposed = false;

    public String Name { get; }
    public TimeSpan Interval { get; }
    public TimeSpan RetryErrorDelay { get; }
    public bool IsRunning => _executionTask != null && !_executionTask.IsCompleted;

    protected Daemon(string name, TimeSpan interval, TimeSpan retryErrorDelay)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Interval = interval;
        RetryErrorDelay = retryErrorDelay;
    }

    public void Start()
    {
        if (IsRunning) return;

        _cancellationTokenSource = new CancellationTokenSource();
        _executionTask = RunAsync(_cancellationTokenSource.Token);

        Console.WriteLine($"Daemon {Name} started");
    }

    public async Task StopAsync()
    {
        if (!IsRunning) return;

        _cancellationTokenSource?.Cancel();

        try
        {
            await _executionTask;
        }
        catch (OperationCanceledException e)
        {
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception during '{Name}' daemon shutdown: {e.Message}");
        }

        Console.WriteLine($"Daemon '{Name}' stopped");
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await ExecuteCoreAsync(cancellationToken);
                if (!cancellationToken.IsCancellationRequested) await Task.Delay(Interval, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in daemon '{Name}': {e.Message}");
                if (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        await Task.Delay(RetryErrorDelay, cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }
            }
        }
    }

    protected abstract Task ExecuteCoreAsync(CancellationToken cancellationToken);

    public virtual void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            StopAsync().GetAwaiter().GetResult();
            _cancellationTokenSource?.Dispose();
        }

        _disposed = true;
    }
}