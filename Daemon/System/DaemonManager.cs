namespace Daemon.System;

public class DaemonManager : IDisposable
{
    private readonly List<Daemon> _daemons = new();
    private bool _disposed = false;

    public void AddDaemon(Daemon daemon)
    {
        if (daemon == null)
            throw new ArgumentNullException(nameof(daemon));
        _daemons.Add(daemon);
    }
    
    public void StartAll()
    {
        foreach (var daemon in _daemons)
        {
            daemon.Start();
        }
    }

    public async Task StopAllAsync()
    {
        var stopTasks = new List<Task>();

        foreach (var daemon in _daemons)
        {
            stopTasks.Add(daemon.StopAsync());
        }
        
        await Task.WhenAll(stopTasks);
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
            StopAllAsync().GetAwaiter().GetResult();

            foreach (var daemon in _daemons)
            {
                daemon.Dispose();
            }
            
            _daemons.Clear();
        }
        
        _disposed = true;
    }
}