using Microsoft.Extensions.Logging;

namespace daemon.Manager;

public abstract class Daemon : IDisposable
{
    private CancellationTokenSource _cancellationTokenSource;
    private Task _executionTask;
    private bool _disposed = false;
    
    protected readonly ILogger<Daemon> Logger;
    
    public String Name { get; }
    public TimeSpan Interval { get; }
    public TimeSpan RetryErrorDelay { get; }
    
    public bool IsRunning => _executionTask != null && !_executionTask.IsCompleted;

    protected Daemon(string name, TimeSpan interval, TimeSpan retryErrorDelay, ILogger<Daemon> logger)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name), "Daemon name cannot be null");
        if (interval <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(interval), "Interval must be greater than zero");
        if (retryErrorDelay < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(retryErrorDelay), "Retry error delay cannot be negative");
        Interval = interval;
        RetryErrorDelay = retryErrorDelay;
        
        Logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null");
    }

    public void Start()
    {
        if (IsRunning)
        {
            Logger.LogWarning("Daemon '{Name}' tried to start but is already running", Name);
            return;
        }
        
        _cancellationTokenSource = new CancellationTokenSource();
        _executionTask = RunAsync(_cancellationTokenSource.Token);
        
        Logger.LogInformation("Daemon '{Name}' started", Name);
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
            catch (Exception exception)
            {
                Logger.LogError(exception, "Error in daemon '{Name}' occurred: {Error}", Name, exception.Message);
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

    public async Task StopAsync()
    {
        if (!IsRunning)
        {
            Logger.LogWarning("Daemon '{Name}' tried to stop but is not running", Name);
            return;
        }

        _cancellationTokenSource?.Cancel();

        try
        {
            await _executionTask;
        }
        catch (Exception exception)
        {
            Logger.LogError("Error while stopping daemon '{Name}': {Error}", Name, exception.Message);
        }
        Logger.LogInformation("Daemon '{Name}' stopped", Name);
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
            _executionTask?.Dispose();
        }

        _disposed = true;
    }
}