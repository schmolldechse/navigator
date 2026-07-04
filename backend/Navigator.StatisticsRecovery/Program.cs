using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Navigator.Data;
using Navigator.Data.StatisticsRefresh;
using Navigator.Observability;

var recoveryArguments = RecoveryArguments.Parse(args);

using var cancellationTokenSource = new CancellationTokenSource();
Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    cancellationTokenSource.Cancel();
};

var builder = Host.CreateApplicationBuilder(args);
builder.AddNavigatorObservability("Navigator.StatisticsRecovery");
builder.Services.AddServices(builder.Configuration);

using var host = builder.Build();
await host.StartAsync(cancellationTokenSource.Token);
using var scope = host.Services.CreateScope();

var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
    .CreateLogger("Navigator.StatisticsRecovery");
var refreshService = scope.ServiceProvider.GetRequiredService<IStatisticsRollupRefreshService>();

using var recoveryActivity = NavigatorLogging.StartJobActivity("StatisticsRecovery");
recoveryActivity?.SetTag("statistics.recovery.operation", recoveryArguments.Operation);
recoveryActivity?.SetTag("statistics.recovery.start", recoveryArguments.Start.ToString("O"));
recoveryActivity?.SetTag("statistics.recovery.end_exclusive", recoveryArguments.EndExclusive.ToString("O"));
recoveryActivity?.SetTag("statistics.recovery.chunk_hours", recoveryArguments.ChunkHours);
recoveryActivity?.SetTag("statistics.recovery.resume", recoveryArguments.Resume);
recoveryActivity?.SetTag("statistics.recovery.cagg_refresh_mode", recoveryArguments.CaggRefreshMode.ToString());
recoveryActivity?.SetTag("statistics.recovery.cagg_refresh_hours", recoveryArguments.CaggRefreshHours);

using var recoveryScope = logger.BeginScope(new Dictionary<string, object?>
{
    ["JobName"] = "StatisticsRecovery",
    ["EventType"] = "statistics_recovery",
    ["Operation"] = recoveryArguments.Operation
});

logger.LogInformation(
    "Starting statistics recovery. Start={Start} EndExclusive={EndExclusive} ChunkHours={ChunkHours} Resume={Resume} Operation={Operation} CaggRefreshMode={CaggRefreshMode} CaggRefreshHours={CaggRefreshHours}",
    recoveryArguments.Start,
    recoveryArguments.EndExclusive,
    recoveryArguments.ChunkHours,
    recoveryArguments.Resume,
    recoveryArguments.Operation,
    recoveryArguments.CaggRefreshMode,
    recoveryArguments.CaggRefreshHours);

StatisticsRefreshWindow? pendingCaggRefreshWindow = null;

try
{
    for (var windowStart = recoveryArguments.Start;
         windowStart < recoveryArguments.EndExclusive;
         windowStart = windowStart.AddHours(recoveryArguments.ChunkHours))
    {
        var windowEnd = Min(windowStart.AddHours(recoveryArguments.ChunkHours), recoveryArguments.EndExclusive);
        var window = new StatisticsRefreshWindow(windowStart, windowEnd);

        using var windowActivity = NavigatorLogging.StartJobActivity("StatisticsRecoveryWindow");
        windowActivity?.SetTag("statistics.recovery.operation", recoveryArguments.Operation);
        windowActivity?.SetTag("statistics.recovery.window_start", window.Start.ToString("O"));
        windowActivity?.SetTag("statistics.recovery.window_end", window.End.ToString("O"));

        using var windowScope = logger.BeginScope(new Dictionary<string, object?>
        {
            ["JobName"] = "StatisticsRecovery",
            ["EventType"] = "statistics_recovery_window",
            ["Operation"] = recoveryArguments.Operation,
            ["WindowStart"] = window.Start,
            ["WindowEnd"] = window.End
        });

        if (recoveryArguments.Resume &&
            await refreshService.HasSuccessfulWindowAsync(window, recoveryArguments.Operation, cancellationTokenSource.Token))
        {
            logger.LogInformation(
                "Skipping successful statistics recovery window. WindowStart={WindowStart} WindowEnd={WindowEnd}",
                window.Start,
                window.End);

            if (recoveryArguments.CaggRefreshMode == CaggRefreshMode.Batched)
            {
                pendingCaggRefreshWindow = ExtendRange(pendingCaggRefreshWindow, window);
                if (ShouldFlushCaggRefresh(pendingCaggRefreshWindow, recoveryArguments.CaggRefreshHours))
                {
                    await FlushPendingCaggRefreshAsync();
                }
            }

            continue;
        }

        var result = await refreshService.RefreshWindowAsync(
            window,
            recoveryArguments.Operation,
            recoveryArguments.CaggRefreshMode == CaggRefreshMode.PerWindow,
            cancellationTokenSource.Token);

        logger.LogInformation(
            "Completed statistics recovery window. WindowStart={WindowStart} WindowEnd={WindowEnd} EventRowsAffected={EventRowsAffected} JourneyRowsAffected={JourneyRowsAffected} CaggRefreshCount={CaggRefreshCount} DurationMs={DurationMs}",
            result.Window.Start,
            result.Window.End,
            result.EventRowsAffected,
            result.JourneyRowsAffected,
            result.CaggRefreshCount,
            result.Duration.TotalMilliseconds);

        if (recoveryArguments.CaggRefreshMode == CaggRefreshMode.Batched)
        {
            pendingCaggRefreshWindow = ExtendRange(pendingCaggRefreshWindow, window);
            if (ShouldFlushCaggRefresh(pendingCaggRefreshWindow, recoveryArguments.CaggRefreshHours))
            {
                await FlushPendingCaggRefreshAsync();
            }
        }
    }

    if (recoveryArguments.CaggRefreshMode == CaggRefreshMode.Batched)
    {
        await FlushPendingCaggRefreshAsync();
    }

    logger.LogInformation("Statistics recovery completed.");
    return 0;
}
catch (OperationCanceledException)
{
    logger.LogWarning("Statistics recovery was cancelled.");
    return 2;
}
catch (Exception exception)
{
    logger.LogError(exception, "Statistics recovery failed.");
    return 1;
}
finally
{
    await host.StopAsync(CancellationToken.None);
}

async Task FlushPendingCaggRefreshAsync()
{
    if (pendingCaggRefreshWindow is null)
    {
        return;
    }

    var window = pendingCaggRefreshWindow.Value;
    pendingCaggRefreshWindow = null;

    var result = await refreshService.RefreshContinuousAggregatesAsync(
        window,
        recoveryArguments.Operation,
        cancellationTokenSource.Token);

    logger.LogInformation(
        "Completed statistics recovery CAGG batch. WindowStart={WindowStart} WindowEnd={WindowEnd} CaggRefreshCount={CaggRefreshCount} DurationMs={DurationMs}",
        result.Window.Start,
        result.Window.End,
        result.CaggRefreshCount,
        result.Duration.TotalMilliseconds);
}

static DateTime Min(DateTime left, DateTime right) => left <= right ? left : right;

static StatisticsRefreshWindow ExtendRange(StatisticsRefreshWindow? current, StatisticsRefreshWindow next) =>
    current is null
        ? next
        : new StatisticsRefreshWindow(
            current.Value.Start <= next.Start ? current.Value.Start : next.Start,
            current.Value.End >= next.End ? current.Value.End : next.End);

static bool ShouldFlushCaggRefresh(StatisticsRefreshWindow? window, int caggRefreshHours) =>
    window is not null && window.Value.End - window.Value.Start >= TimeSpan.FromHours(caggRefreshHours);

internal enum CaggRefreshMode
{
    PerWindow,
    Batched,
    Disabled
}

internal sealed record RecoveryArguments(
    DateTime Start,
    DateTime EndExclusive,
    int ChunkHours,
    bool Resume,
    string Operation,
    CaggRefreshMode CaggRefreshMode,
    int CaggRefreshHours)
{
    public static RecoveryArguments Parse(string[] args)
    {
        var values = ParseKeyValues(args);

        var start = ParseUtc(values.GetValueOrDefault("start", "2024-10-01"));
        var endExclusive = ParseUtc(values.GetValueOrDefault("end-exclusive", "2026-07-01"));
        var chunkHours = ParsePositiveInt(values.GetValueOrDefault("chunk-hours", "1"), "chunk-hours");
        var resume = ParseBool(values.GetValueOrDefault("resume", "true"), "resume");
        var operation = values.GetValueOrDefault("operation", "recovery");
        var caggRefreshMode = ParseCaggRefreshMode(values.GetValueOrDefault("cagg-refresh-mode", "batched"));
        var caggRefreshHours = ParsePositiveInt(values.GetValueOrDefault("cagg-refresh-hours", "24"), "cagg-refresh-hours");

        if (endExclusive <= start)
        {
            throw new ArgumentException("--end-exclusive must be after --start.");
        }

        return new RecoveryArguments(start, endExclusive, chunkHours, resume, operation, caggRefreshMode, caggRefreshHours);
    }

    private static Dictionary<string, string> ParseKeyValues(string[] args)
    {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        for (var index = 0; index < args.Length; index++)
        {
            var token = args[index];
            if (!token.StartsWith("--", StringComparison.Ordinal)) continue;

            var keyValue = token[2..].Split('=', 2);
            if (keyValue.Length == 2)
            {
                values[keyValue[0]] = keyValue[1];
                continue;
            }

            if (index + 1 >= args.Length || args[index + 1].StartsWith("--", StringComparison.Ordinal))
            {
                throw new ArgumentException($"Missing value for argument --{keyValue[0]}.");
            }

            values[keyValue[0]] = args[++index];
        }

        return values;
    }

    private static DateTime ParseUtc(string value)
    {
        if (DateOnly.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            return date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        }

        return DateTimeOffset.Parse(
                value,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal)
            .UtcDateTime;
    }

    private static int ParsePositiveInt(string value, string name)
    {
        if (!int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var parsed) || parsed <= 0)
        {
            throw new ArgumentException($"--{name} must be a positive integer.");
        }

        return parsed;
    }

    private static bool ParseBool(string value, string name)
    {
        if (!bool.TryParse(value, out var parsed))
        {
            throw new ArgumentException($"--{name} must be true or false.");
        }

        return parsed;
    }

    private static CaggRefreshMode ParseCaggRefreshMode(string value) =>
        value.ToLowerInvariant() switch
        {
            "per-window" => CaggRefreshMode.PerWindow,
            "batched" => CaggRefreshMode.Batched,
            "disabled" => CaggRefreshMode.Disabled,
            _ => throw new ArgumentException("--cagg-refresh-mode must be per-window, batched, or disabled.")
        };
}
