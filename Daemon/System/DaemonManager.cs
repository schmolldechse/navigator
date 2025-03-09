using Microsoft.Extensions.Logging;

namespace Daemon.System;

public class DaemonManager : IDisposable
{
    private readonly List<Daemon> _daemons = new();
    private readonly ILogger _logger;
    private bool _disposed = false;

    public DaemonManager(ILogger<DaemonManager> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public void AddDaemon(Daemon daemon)
    {
        if (daemon == null)
            throw new ArgumentNullException(nameof(daemon));
        _daemons.Add(daemon);
        _logger.LogInformation($"Added daemon {daemon.Name}");
    }
    
    public void StartAll()
    {
        _logger.LogInformation("Starting daemons...");
        foreach (var daemon in _daemons)
        {
            daemon.Start();
        }
        _logger.LogInformation("Daemons started");
    }

    public async Task StopAllAsync()
    {
        _logger.LogInformation("Stopping daemons...");
        var stopTasks = new List<Task>();

        foreach (var daemon in _daemons)
        {
            stopTasks.Add(daemon.StopAsync());
        }
        
        await Task.WhenAll(stopTasks);
        _logger.LogInformation("Daemons stopped");
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _logger.LogInformation("Disposing daemon manager");
            StopAllAsync().GetAwaiter().GetResult();

            foreach (var daemon in _daemons)
            {
                daemon.Dispose();
            }
            
            _daemons.Clear();
            _logger.LogInformation("Daemon manager disposed");
        }
        
        _disposed = true;
    }
}