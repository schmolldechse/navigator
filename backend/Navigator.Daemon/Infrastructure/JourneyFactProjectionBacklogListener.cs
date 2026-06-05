using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Navigator.Daemon.Infrastructure;

public sealed class JourneyFactProjectionBacklogListener(
    ILogger<JourneyFactProjectionBacklogListener> logger,
    IConfiguration configuration,
    IServiceScopeFactory scopeFactory
) : BackgroundService
{
    private const string ChannelName = "journey_fact_projection_backlog";
    private static readonly TimeSpan _reconnectDelay = TimeSpan.FromSeconds(10);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            logger.LogWarning("Journey fact projection backlog listener is disabled because no default connection string is configured.");
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync(stoppingToken);

                await using (var command = new NpgsqlCommand($"LISTEN {ChannelName};", connection))
                {
                    await command.ExecuteNonQueryAsync(stoppingToken);
                }

                logger.LogInformation("Listening for journey fact projection backlog notifications on PostgreSQL channel {ChannelName}.", ChannelName);
                await DrainBacklogAsync(stoppingToken);

                while (!stoppingToken.IsCancellationRequested)
                {
                    await connection.WaitAsync(stoppingToken);
                    await DrainBacklogAsync(stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception exception)
            {
                logger.LogWarning(exception, "Journey fact projection backlog listener failed. Reconnecting in {ReconnectDelay}.", _reconnectDelay);
                await Task.Delay(_reconnectDelay, stoppingToken);
            }
        }
    }

    private async Task DrainBacklogAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            using var scope = scopeFactory.CreateScope();
            var projectionService = scope.ServiceProvider.GetRequiredService<JourneyFactProjectionService>();
            var result = await projectionService.ProjectAvailableAsync(cancellationToken);
            if (result.ClaimedCount == 0) return;
        }
    }
}
