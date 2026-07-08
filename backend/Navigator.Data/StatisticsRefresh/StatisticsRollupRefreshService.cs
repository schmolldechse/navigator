using System.Data;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Navigator.Data.StatisticsRefresh;

internal sealed record StatisticsRollupRecomputeResult(
    long EventRowsAffected,
    long JourneyRowsAffected);

public sealed class StatisticsRollupRefreshService(
    DataContext dataContext,
    IOptions<StatisticsRefreshOptions> options,
    ILogger<StatisticsRollupRefreshService> logger) : IStatisticsRollupRefreshService
{
    public async Task<bool> HasSuccessfulWindowAsync(
        StatisticsRefreshWindow window,
        string operation,
        CancellationToken cancellationToken)
    {
        window.Validate();

        return await dataContext.StatisticsRefreshProgress.AnyAsync(
            row => row.Operation == operation
                && row.WindowStart == window.Start
                && row.WindowEnd == window.End
                && row.Status == "success",
            cancellationToken);
    }

    public async Task<StatisticsRefreshResult> RefreshWindowAsync(
        StatisticsRefreshWindow window,
        string operation,
        bool refreshContinuousAggregates,
        CancellationToken cancellationToken)
    {
        window.Validate();
        var stopwatch = Stopwatch.StartNew();

        await dataContext.Database.OpenConnectionAsync(cancellationToken);

        var lockAcquired = await TryAcquireRefreshLockAsync(cancellationToken);
        if (!lockAcquired)
        {
            await dataContext.Database.CloseConnectionAsync();
            throw new InvalidOperationException("Statistics refresh is already running in another process.");
        }

        try
        {
            await dataContext.Database.ExecuteSqlInterpolatedAsync($"""
                INSERT INTO statistics.statistics_refresh_progress (
                    operation,
                    window_start,
                    window_end,
                    status,
                    attempt,
                    event_rows_affected,
                    journey_rows_affected,
                    cagg_refresh_count,
                    started_at
                )
                VALUES ({operation}, {window.Start}, {window.End}, 'running', 1, 0, 0, 0, now())
                ON CONFLICT (operation, window_start, window_end)
                DO UPDATE SET status = 'running',
                              attempt = statistics.statistics_refresh_progress.attempt + 1,
                              started_at = now(),
                              finished_at = NULL,
                              error_kind = NULL;
                """, cancellationToken);

            var recomputeStopwatch = Stopwatch.StartNew();
            var recomputeResult = await ExecuteRollupRecomputeAsync(window, cancellationToken);
            recomputeStopwatch.Stop();

            var eventRows = recomputeResult.EventRowsAffected;
            var journeyRows = recomputeResult.JourneyRowsAffected;

            logger.LogInformation(
                "Statistics rollup recompute succeeded. WindowStart={WindowStart} WindowEnd={WindowEnd} EventRows={EventRows} JourneyRows={JourneyRows} DurationMs={DurationMs}",
                window.Start,
                window.End,
                eventRows,
                journeyRows,
                recomputeStopwatch.ElapsedMilliseconds);

            var caggRefreshResult = refreshContinuousAggregates
                ? await RefreshContinuousAggregatesCoreAsync(window, operation, cancellationToken)
                : new StatisticsCaggRefreshResult(window, 0, TimeSpan.Zero);
            var caggRefreshCount = caggRefreshResult.CaggRefreshCount;

            await dataContext.Database.ExecuteSqlInterpolatedAsync($"""
                UPDATE statistics.statistics_refresh_progress
                   SET status = 'success',
                       event_rows_affected = {eventRows},
                       journey_rows_affected = {journeyRows},
                       cagg_refresh_count = {caggRefreshCount},
                       finished_at = now(),
                       error_kind = NULL
                 WHERE operation = {operation}
                   AND window_start = {window.Start}
                   AND window_end = {window.End};
                """, cancellationToken);

            stopwatch.Stop();

            logger.LogInformation(
                "Statistics refresh window succeeded. Operation={Operation} WindowStart={WindowStart} WindowEnd={WindowEnd} EventRows={EventRows} JourneyRows={JourneyRows} CaggRefreshCount={CaggRefreshCount} CaggRefreshDeferred={CaggRefreshDeferred} DurationMs={DurationMs}",
                operation,
                window.Start,
                window.End,
                eventRows,
                journeyRows,
                caggRefreshCount,
                !refreshContinuousAggregates,
                stopwatch.ElapsedMilliseconds);

            return new StatisticsRefreshResult(window, eventRows, journeyRows, caggRefreshCount, stopwatch.Elapsed);
        }
        catch (Exception exception)
        {
            await dataContext.Database.ExecuteSqlInterpolatedAsync($"""
                UPDATE statistics.statistics_refresh_progress
                   SET status = 'failed',
                       finished_at = now(),
                       error_kind = {exception.GetType().Name}
                 WHERE operation = {operation}
                   AND window_start = {window.Start}
                   AND window_end = {window.End};
                """, cancellationToken);

            logger.LogError(
                exception,
                "Statistics refresh window failed. Operation={Operation} WindowStart={WindowStart} WindowEnd={WindowEnd} FailureKind={FailureKind}",
                operation,
                window.Start,
                window.End,
                exception.GetType().Name);

            throw;
        }
        finally
        {
            await ReleaseRefreshLockAsync(cancellationToken);
            await dataContext.Database.CloseConnectionAsync();
        }
    }

    private async Task<StatisticsRollupRecomputeResult> ExecuteRollupRecomputeAsync(
        StatisticsRefreshWindow window,
        CancellationToken cancellationToken)
    {
        await using var command = dataContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = """
            SELECT event_rows_affected, journey_rows_affected
            FROM statistics.recompute_quality_hourly_rollups(@window_start, @window_end);
            """;
        command.CommandTimeout = options.Value.CommandTimeoutSeconds;
        command.Transaction = dataContext.Database.CurrentTransaction?.GetDbTransaction();

        var start = command.CreateParameter();
        start.ParameterName = "window_start";
        start.Value = window.Start;
        command.Parameters.Add(start);

        var end = command.CreateParameter();
        end.ParameterName = "window_end";
        end.Value = window.End;
        command.Parameters.Add(end);

        if (command.Connection!.State != ConnectionState.Open)
        {
            await command.Connection.OpenAsync(cancellationToken);
        }

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            throw new InvalidOperationException("Unified statistics recompute returned no result row.");
        }

        return new StatisticsRollupRecomputeResult(
            reader.GetInt64(0),
            reader.GetInt64(1));
    }

    public async Task<StatisticsCaggRefreshResult> RefreshContinuousAggregatesAsync(
        StatisticsRefreshWindow window,
        string operation,
        CancellationToken cancellationToken)
    {
        window.Validate();

        await dataContext.Database.OpenConnectionAsync(cancellationToken);

        var lockAcquired = await TryAcquireRefreshLockAsync(cancellationToken);
        if (!lockAcquired)
        {
            await dataContext.Database.CloseConnectionAsync();
            throw new InvalidOperationException("Statistics refresh is already running in another process.");
        }

        try
        {
            return await RefreshContinuousAggregatesCoreAsync(window, operation, cancellationToken);
        }
        finally
        {
            await ReleaseRefreshLockAsync(cancellationToken);
            await dataContext.Database.CloseConnectionAsync();
        }
    }

    private async Task<StatisticsCaggRefreshResult> RefreshContinuousAggregatesCoreAsync(
        StatisticsRefreshWindow window,
        string operation,
        CancellationToken cancellationToken)
    {
        if (dataContext.Database.CurrentTransaction is not null)
        {
            throw new InvalidOperationException("Continuous aggregate refresh must not run inside an EF transaction.");
        }

        var stopwatch = Stopwatch.StartNew();
        var refreshedCount = 0;

        foreach (var continuousAggregate in StatisticsCaggCatalog.Names)
        {
            logger.LogDebug(
                "Refreshing continuous aggregate. Operation={Operation} CaggName={CaggName} WindowStart={WindowStart} WindowEnd={WindowEnd}",
                operation,
                continuousAggregate,
                window.Start,
                window.End);

            await using var command = dataContext.Database.GetDbConnection().CreateCommand();
            command.CommandText =
                $"CALL refresh_continuous_aggregate('statistics.{continuousAggregate}', @window_start, @window_end);";
            command.CommandTimeout = options.Value.CommandTimeoutSeconds;

            var start = command.CreateParameter();
            start.ParameterName = "window_start";
            start.Value = window.Start;
            command.Parameters.Add(start);

            var end = command.CreateParameter();
            end.ParameterName = "window_end";
            end.Value = window.End;
            command.Parameters.Add(end);

            if (command.Connection!.State != ConnectionState.Open)
            {
                await command.Connection.OpenAsync(cancellationToken);
            }

            await command.ExecuteNonQueryAsync(cancellationToken);
            refreshedCount++;
        }

        stopwatch.Stop();

        logger.LogInformation(
            "Statistics continuous aggregate refresh succeeded. Operation={Operation} WindowStart={WindowStart} WindowEnd={WindowEnd} CaggRefreshCount={CaggRefreshCount} DurationMs={DurationMs}",
            operation,
            window.Start,
            window.End,
            refreshedCount,
            stopwatch.ElapsedMilliseconds);

        return new StatisticsCaggRefreshResult(window, refreshedCount, stopwatch.Elapsed);
    }

    private async Task<bool> TryAcquireRefreshLockAsync(CancellationToken cancellationToken)
    {
        await using var command = dataContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = "SELECT pg_try_advisory_lock(hashtext(@lock_name));";
        command.CommandTimeout = options.Value.CommandTimeoutSeconds;

        var lockName = command.CreateParameter();
        lockName.ParameterName = "lock_name";
        lockName.Value = options.Value.AdvisoryLockName;
        command.Parameters.Add(lockName);

        var result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToBoolean(result);
    }

    private async Task ReleaseRefreshLockAsync(CancellationToken cancellationToken)
    {
        await using var command = dataContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = "SELECT pg_advisory_unlock(hashtext(@lock_name));";
        command.CommandTimeout = options.Value.CommandTimeoutSeconds;

        var lockName = command.CreateParameter();
        lockName.ParameterName = "lock_name";
        lockName.Value = options.Value.AdvisoryLockName;
        command.Parameters.Add(lockName);

        await command.ExecuteScalarAsync(cancellationToken);
    }
}
