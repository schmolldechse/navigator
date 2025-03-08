using Daemon.System;
using Daemon.System.Impl;

namespace Daemon;

class Program
{
    private static readonly ManualResetEventSlim _shutdownEvent = new ManualResetEventSlim(false);

    static async Task Main(string[] args)
    {
        Console.CancelKeyPress += (sender, e) =>
        {
            Console.WriteLine("Shutting down...");
            e.Cancel = true;
            _shutdownEvent.Set();
        };

        using (var manager = new DaemonManager())
        {
            manager.AddDaemon(new MonitorStations());
            manager.StartAll();

            _shutdownEvent.Wait();

            Console.WriteLine("Stopping daemons...");
            await manager.StopAllAsync();
        }
    }
}