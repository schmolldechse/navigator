using Microsoft.Extensions.Logging;

namespace daemon.Manager;

public class DaemonManager : IDisposable
{
    private readonly List<Daemon> _daemons = new();
    private readonly ILogger<DaemonManager> _logger;
    private bool _disposed = false;

    public DaemonManager(ILogger<DaemonManager> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null");
    }

    public void AddDaemon(Daemon daemon)
    {
        if (daemon == null) throw new ArgumentNullException(nameof(daemon));
        if (_daemons.Any(d => d.Name == daemon.Name))
        {
            _logger.LogWarning("Daemon with name '{Name}' already exists", daemon.Name);
            return;
        }

        _daemons.Add(daemon);
    }

    public void StartAll()
    {
        _logger.LogInformation("Starting all daemons...");
        foreach (var daemon in _daemons)
        {
            daemon.Start();
        }

        _logger.LogInformation("Daemons started: {Count}", _daemons.Count);
    }

    public async Task StopAll()
    {
        _logger.LogInformation("Stopping all daemons...");

        var tasks = _daemons.Select(daemon => daemon.StopAsync()).ToArray();
        await Task.WhenAll(tasks);
        _logger.LogInformation("All daemons stopped");
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    public virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            StopAll().GetAwaiter().GetResult();
            _daemons.ForEach(daemon => daemon.Dispose());
            _daemons.Clear();
        }

        _disposed = true;
    }
}
