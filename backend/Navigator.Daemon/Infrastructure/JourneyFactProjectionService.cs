using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Navigator.Data;
using System.Data;
using System.Data.Common;

namespace Navigator.Daemon.Infrastructure;

public sealed class JourneyFactProjectionService(
    ILogger<JourneyFactProjectionService> logger,
    DataContext dataContext
)
{
    private const int BatchSize = 10_000;
    private const int MaxAttempts = 10;
    private static readonly TimeSpan _lockDuration = TimeSpan.FromMinutes(30);

    public async Task<JourneyFactProjectionRunResult> ProjectAvailableAsync(CancellationToken cancellationToken = default)
    {
        var workerId = Environment.MachineName + ":" + Guid.NewGuid().ToString("N");
        var claimed = await ClaimBacklogEntriesAsync(workerId, cancellationToken);
        if (!claimed.Any()) return JourneyFactProjectionRunResult.Empty;

        var result = await ProjectClaimedAsync(claimed, cancellationToken);

        logger.LogInformation(
            "Projected journey quality facts. Claimed: {ClaimedCount}, Projected: {ProjectedCount}, Skipped: {SkippedCount}, Failed: {FailedCount}, EventFacts: {EventFactCount}, RouteFacts: {RouteFactCount}",
            result.ClaimedCount,
            result.ProjectedCount,
            result.SkippedCount,
            result.FailedCount,
            result.EventFactCount,
            result.RouteFactCount);

        return result;
    }

    private async Task<JourneyFactProjectionRunResult> ProjectClaimedAsync(
        IReadOnlyList<ClaimedJourneyFactProjection> entries,
        CancellationToken cancellationToken
    )
    {
        try
        {
            return await ProjectBatchAsync(entries, cancellationToken);
        }
        catch (Exception) when (entries.Count > 1)
        {
            dataContext.ChangeTracker.Clear();
            var midpoint = entries.Count / 2;
            var firstHalf = await ProjectClaimedAsync(entries.Take(midpoint).ToList(), cancellationToken);
            var secondHalf = await ProjectClaimedAsync(entries.Skip(midpoint).ToList(), cancellationToken);

            return firstHalf.Add(secondHalf);
        }
        catch (Exception exception)
        {
            var entry = entries.Single();
            dataContext.ChangeTracker.Clear();
            await ReleaseBacklogEntryAsync(entry, exception, cancellationToken);

            logger.LogWarning(
                exception,
                "Failed to project journey quality facts. JourneyId: {JourneyId}, Date: {Date}, Attempts: {Attempts}",
                entry.JourneyId,
                entry.Date,
                entry.Attempts);

            return new JourneyFactProjectionRunResult(
                ClaimedCount: 1,
                ProjectedCount: 0,
                SkippedCount: 0,
                FailedCount: 1,
                EventFactCount: 0,
                RouteFactCount: 0);
        }
        finally
        {
            dataContext.ChangeTracker.Clear();
        }
    }

    private async Task<JourneyFactProjectionRunResult> ProjectBatchAsync(
        IReadOnlyList<ClaimedJourneyFactProjection> entries,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await dataContext.Database.BeginTransactionAsync(cancellationToken);

        var result = await ProjectFactsInDatabaseAsync(entries, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return new JourneyFactProjectionRunResult(
            ClaimedCount: entries.Count,
            ProjectedCount: result.ProjectedCount,
            SkippedCount: entries.Count - result.ProjectedCount,
            FailedCount: 0,
            EventFactCount: result.EventFactCount,
            RouteFactCount: result.RouteFactCount);
    }

    private async Task<ProjectFactsResult> ProjectFactsInDatabaseAsync(
        IReadOnlyList<ClaimedJourneyFactProjection> entries,
        CancellationToken cancellationToken
    )
    {
        await using var command = CreateCommand();
        command.CommandTimeout = 900;

        var rows = new List<string>(entries.Count);
        foreach (var entry in entries)
        {
            var journeyId = AddParameter(command, entry.JourneyId);
            var date = AddParameter(command, entry.Date);
            rows.Add($"({journeyId}, {date})");
        }

        command.CommandText = $$"""
            WITH claimed(journey_id, date) AS (
                VALUES {{string.Join(", ", rows)}}
            ),
            journeys_with_transport AS (
                SELECT
                    journeys.id AS journey_id,
                    journeys.date,
                    journeys.administration_id,
                    journeys.cancelled AS journey_cancelled,
                    journeys.journey_type,
                    transports.transport_type,
                    transports.replacement_transport_type,
                    transports.journey_description,
                    transports.number
                FROM claimed
                INNER JOIN core.journeys AS journeys
                    ON journeys.id = claimed.journey_id
                   AND journeys.date = claimed.date
                INNER JOIN core.journey_transports AS transports
                    ON transports.journey_id = journeys.id
                   AND transports.date = journeys.date
            ),
            stop_places AS (
                SELECT
                    journey.journey_id,
                    journey.date,
                    journey.administration_id,
                    journey.journey_cancelled,
                    journey.journey_type,
                    journey.transport_type,
                    journey.replacement_transport_type,
                    journey.journey_description,
                    journey.number,
                    stop_place.id AS stop_place_id,
                    stop_place.planned_time,
                    stop_place.station_eva_number,
                    stop_place.schedule_type,
                    stop_place.cancelled,
                    stop_place.delay
                FROM journeys_with_transport AS journey
                INNER JOIN core.journey_stop_places AS stop_place
                    ON stop_place.journey_id = journey.journey_id
                   AND stop_place.date = journey.date
            ),
            journey_bounds AS (
                SELECT
                    journey_id,
                    date,
                    (array_agg(administration_id))[1] AS administration_id,
                    bool_or(journey_cancelled) AS journey_cancelled,
                    bool_or(journey_type = 'REPLACEMENT'::core.journey_type OR replacement_transport_type IS NOT NULL) AS is_replacement_transport,
                    (array_agg(transport_type))[1] AS transport_type,
                    (array_agg(journey_description))[1] AS journey_description,
                    (array_agg(number))[1] AS number,
                    coalesce(
                        (array_agg(station_eva_number ORDER BY planned_time, station_eva_number)
                            FILTER (WHERE schedule_type = 'DEPARTURE'::core.schedule_type))[1],
                        (array_agg(station_eva_number ORDER BY planned_time, station_eva_number))[1]
                    ) AS origin_eva_number,
                    coalesce(
                        (array_agg(station_eva_number ORDER BY planned_time DESC, station_eva_number DESC)
                            FILTER (WHERE schedule_type = 'ARRIVAL'::core.schedule_type))[1],
                        (array_agg(station_eva_number ORDER BY planned_time DESC, station_eva_number DESC))[1]
                    ) AS destination_eva_number,
                    coalesce(
                        min(planned_time) FILTER (WHERE schedule_type = 'DEPARTURE'::core.schedule_type),
                        min(planned_time)
                    ) AS journey_start_time,
                    coalesce(
                        max(planned_time) FILTER (WHERE schedule_type = 'ARRIVAL'::core.schedule_type),
                        max(planned_time)
                    ) AS journey_end_time,
                    (array_agg(delay ORDER BY CASE WHEN schedule_type = 'ARRIVAL'::core.schedule_type THEN 0 ELSE 1 END, planned_time DESC)
                        FILTER (WHERE cancelled IS NOT TRUE))[1] AS terminal_delay_seconds
                FROM stop_places
                GROUP BY journey_id, date
            ),
            inserted_event_facts AS (
                INSERT INTO statistics.journey_event_quality_facts (
                    stop_place_id,
                    journey_id,
                    date,
                    planned_time,
                    journey_start_time,
                    journey_end_time,
                    station_eva_number,
                    schedule_type,
                    administration_id,
                    transport_type,
                    journey_description,
                    number,
                    is_replacement_transport,
                    origin_eva_number,
                    destination_eva_number,
                    cancelled,
                    delay
                )
                SELECT
                    stop_place.stop_place_id,
                    stop_place.journey_id,
                    stop_place.date,
                    stop_place.planned_time,
                    bounds.journey_start_time,
                    bounds.journey_end_time,
                    stop_place.station_eva_number,
                    stop_place.schedule_type,
                    bounds.administration_id,
                    bounds.transport_type,
                    bounds.journey_description,
                    bounds.number,
                    bounds.is_replacement_transport,
                    bounds.origin_eva_number,
                    bounds.destination_eva_number,
                    stop_place.cancelled,
                    stop_place.delay
                FROM stop_places AS stop_place
                INNER JOIN journey_bounds AS bounds
                    ON bounds.journey_id = stop_place.journey_id
                   AND bounds.date = stop_place.date
                ON CONFLICT DO NOTHING
                RETURNING 1
            ),
            inserted_route_facts AS (
                INSERT INTO statistics.journey_route_quality_facts (
                    journey_id,
                    date,
                    journey_start_time,
                    journey_end_time,
                    administration_id,
                    transport_type,
                    journey_description,
                    number,
                    is_replacement_transport,
                    origin_eva_number,
                    destination_eva_number,
                    journey_cancelled,
                    terminal_delay_seconds
                )
                SELECT
                    journey_id,
                    date,
                    journey_start_time,
                    journey_end_time,
                    administration_id,
                    transport_type,
                    journey_description,
                    number,
                    is_replacement_transport,
                    origin_eva_number,
                    destination_eva_number,
                    journey_cancelled,
                    terminal_delay_seconds
                FROM journey_bounds
                ON CONFLICT DO NOTHING
                RETURNING 1
            ),
            deleted_backlog_entries AS (
                DELETE FROM statistics.journey_fact_projection_backlog AS backlog
                USING claimed
                WHERE backlog.journey_id = claimed.journey_id
                  AND backlog.date = claimed.date
                RETURNING 1
            )
            SELECT
                (SELECT count(*)::integer FROM journey_bounds) AS projected_count,
                (SELECT count(*)::integer FROM stop_places) AS event_fact_count,
                (SELECT count(*)::integer FROM journey_bounds) AS route_fact_count,
                (SELECT count(*)::integer FROM inserted_event_facts) AS inserted_event_fact_count,
                (SELECT count(*)::integer FROM inserted_route_facts) AS inserted_route_fact_count,
                (SELECT count(*)::integer FROM deleted_backlog_entries) AS deleted_backlog_entry_count;
            """;

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return new ProjectFactsResult(0, 0, 0);
        }

        return new ProjectFactsResult(
            ProjectedCount: reader.GetInt32(0),
            EventFactCount: reader.GetInt32(1),
            RouteFactCount: reader.GetInt32(2));
    }

    private async Task<List<ClaimedJourneyFactProjection>> ClaimBacklogEntriesAsync(
        string workerId,
        CancellationToken cancellationToken
    )
    {
        var now = DateTime.UtcNow;
        var lockUntil = now.Add(_lockDuration);
        var connection = dataContext.Database.GetDbConnection();
        var shouldCloseConnection = connection.State != ConnectionState.Open;
        if (shouldCloseConnection) await connection.OpenAsync(cancellationToken);

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = """
                WITH claimed AS (
                    SELECT ctid
                    FROM statistics.journey_fact_projection_backlog
                    WHERE dead_lettered_at IS NULL
                      AND available_at <= @now
                      AND (locked_until IS NULL OR locked_until <= @now)
                    ORDER BY available_at, created_at
                    LIMIT @batch_size
                    FOR UPDATE SKIP LOCKED
                )
                UPDATE statistics.journey_fact_projection_backlog AS backlog
                SET
                    attempts = backlog.attempts + 1,
                    last_attempt_at = @now,
                    locked_until = @lock_until,
                    locked_by = @worker_id,
                    last_error = NULL
                FROM claimed
                WHERE backlog.ctid = claimed.ctid
                RETURNING
                    backlog.journey_id,
                    backlog.date,
                    backlog.attempts;
                """;
            AddNamedParameter(command, "now", now);
            AddNamedParameter(command, "lock_until", lockUntil);
            AddNamedParameter(command, "worker_id", workerId);
            AddNamedParameter(command, "batch_size", BatchSize);

            var entries = new List<ClaimedJourneyFactProjection>();
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                entries.Add(new ClaimedJourneyFactProjection
                {
                    JourneyId = reader.GetString(0),
                    Date = reader.GetFieldValue<DateOnly>(1),
                    Attempts = reader.GetInt32(2)
                });
            }

            return entries;
        }
        finally
        {
            if (shouldCloseConnection) await connection.CloseAsync();
        }
    }

    private async Task ReleaseBacklogEntryAsync(
        ClaimedJourneyFactProjection entry,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        var now = DateTime.UtcNow;
        var shouldDeadLetter = entry.Attempts >= MaxAttempts;
        var availableAt = shouldDeadLetter
            ? now
            : now.AddSeconds(Math.Min(Math.Pow(2, entry.Attempts), 3600));
        var error = exception.ToString();
        if (error.Length > 2048) error = error[..2048];

        await dataContext.Database.ExecuteSqlInterpolatedAsync($"""
            UPDATE statistics.journey_fact_projection_backlog
            SET
                available_at = {availableAt},
                locked_until = NULL,
                locked_by = NULL,
                last_error = {error},
                dead_lettered_at = CASE WHEN {shouldDeadLetter} THEN {now} ELSE dead_lettered_at END
            WHERE journey_id = {entry.JourneyId}
              AND date = {entry.Date};
            """, cancellationToken);
    }

    private DbCommand CreateCommand()
    {
        var command = dataContext.Database.GetDbConnection().CreateCommand();
        command.Transaction = dataContext.Database.CurrentTransaction?.GetDbTransaction();
        return command;
    }

    private static string AddParameter(DbCommand command, object? value)
    {
        var parameterName = "@p" + command.Parameters.Count;
        var parameter = command.CreateParameter();
        parameter.ParameterName = parameterName;
        parameter.Value = value ?? DBNull.Value;
        command.Parameters.Add(parameter);
        return parameterName;
    }

    private static void AddNamedParameter(DbCommand command, string name, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }

    private sealed class ClaimedJourneyFactProjection
    {
        public required string JourneyId { get; set; }

        public required DateOnly Date { get; set; }

        public required int Attempts { get; set; }
    }

    private sealed record ProjectFactsResult(
        int ProjectedCount,
        int EventFactCount,
        int RouteFactCount
    );
}

public sealed record JourneyFactProjectionRunResult(
    int ClaimedCount,
    int ProjectedCount,
    int SkippedCount,
    int FailedCount,
    int EventFactCount,
    int RouteFactCount
)
{
    public static JourneyFactProjectionRunResult Empty { get; } = new(0, 0, 0, 0, 0, 0);

    public JourneyFactProjectionRunResult Add(JourneyFactProjectionRunResult other) => new(
        ClaimedCount + other.ClaimedCount,
        ProjectedCount + other.ProjectedCount,
        SkippedCount + other.SkippedCount,
        FailedCount + other.FailedCount,
        EventFactCount + other.EventFactCount,
        RouteFactCount + other.RouteFactCount);
}
