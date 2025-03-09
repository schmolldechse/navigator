using Microsoft.Extensions.Logging;

namespace Daemon.System;

public abstract class Daemon : IDisposable
{
    private CancellationTokenSource _cancellationTokenSource;
    private Task _executionTask;
    private bool _disposed = false;
    
    protected readonly ILogger _logger;

    public String Name { get; }
    public TimeSpan Interval { get; }
    public TimeSpan RetryErrorDelay { get; }
    public bool IsRunning => _executionTask != null && !_executionTask.IsCompleted;

    protected Daemon(string name, TimeSpan interval, TimeSpan retryErrorDelay, ILogger logger)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Interval = interval;
        RetryErrorDelay = retryErrorDelay;
        
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void Start()
    {
        if (IsRunning) return;

        _cancellationTokenSource = new CancellationTokenSource();
        _executionTask = RunAsync(_cancellationTokenSource.Token);

        _logger.LogInformation($"Daemon {Name} started");
    }

    public async Task StopAsync()
    {
        if (!IsRunning) return;

        _cancellationTokenSource?.Cancel();

        try
        {
            await _executionTask;
        }
        catch (Exception e)
        {
            _logger.LogError($"Exception during '{Name}' daemon shutdown: {e.Message}");
        }

        _logger.LogInformation($"Daemon '{Name}' stopped");
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
            catch (Exception e)
            {
                _logger.LogError($"Error in daemon '{Name}': {e.Message}");
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