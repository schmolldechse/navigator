using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Navigator.Data;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using System.Data.Common;

namespace Navigator.Daemon.Infrastructure;

public sealed class JourneyFactProjectionService(
    ILogger<JourneyFactProjectionService> logger,
    IConfiguration configuration,
    DataContext dataContext
)
{
    private const int DefaultBatchSize = 10_000;
    private const int DefaultCommandTimeoutSeconds = 900;
    private const int MaxAttempts = 10;

    private static readonly TimeSpan _lockDuration = TimeSpan.FromMinutes(30);

    private readonly int _batchSize = ReadPositiveInt(configuration, "JobConfigs:JourneyFactProjectionJob:BatchSize", DefaultBatchSize);
    private readonly int _commandTimeoutSeconds = ReadPositiveInt(configuration, "JobConfigs:JourneyFactProjectionJob:CommandTimeoutSeconds", DefaultCommandTimeoutSeconds);

    public async Task<JourneyFactProjectionRunResult> ProjectAvailableAsync(CancellationToken cancellationToken = default)
    {
        var workerId = Environment.MachineName + ":" + Guid.NewGuid().ToString("N");

        var claimed = await ClaimBacklogEntriesAsync(workerId, cancellationToken);
        if (!claimed.Any()) return JourneyFactProjectionRunResult.Empty;

        var result = await ProjectClaimedAsync(claimed, cancellationToken);

        logger.LogInformation(
            "Projected journey quality facts. Claimed: {ClaimedCount}, Projected: {ProjectedCount}, Skipped: {SkippedCount}, Failed: {FailedCount}, EventFacts: {EventFactCount}, JourneyFacts: {JourneyFactCount}",
            result.ClaimedCount,
            result.ProjectedCount,
            result.SkippedCount,
            result.FailedCount,
            result.EventFactCount,
            result.JourneyFactCount);
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
        catch (Exception exception) when (entries.Count > 1 && ShouldSplitBatch(exception))
        {
            dataContext.ChangeTracker.Clear();
            var midpoint = entries.Count / 2;
            var firstHalf = await ProjectClaimedAsync(entries.Take(midpoint).ToList(), cancellationToken);
            var secondHalf = await ProjectClaimedAsync(entries.Skip(midpoint).ToList(), cancellationToken);

            return firstHalf.Add(secondHalf);
        }
        catch (Exception exception) when (entries.Count > 1)
        {
            dataContext.ChangeTracker.Clear();

            foreach (var entry in entries)
            {
                await ReleaseBacklogEntryAsync(entry, exception, cancellationToken);
            }

            logger.LogWarning(
                exception,
                "Failed to project journey quality fact batch. Entries: {EntryCount}, AttemptsMin: {AttemptsMin}, AttemptsMax: {AttemptsMax}",
                entries.Count,
                entries.Min(entry => entry.Attempts),
                entries.Max(entry => entry.Attempts));

            return new JourneyFactProjectionRunResult(
                ClaimedCount: entries.Count,
                ProjectedCount: 0,
                SkippedCount: 0,
                FailedCount: entries.Count,
                EventFactCount: 0,
                JourneyFactCount: 0);
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
                JourneyFactCount: 0);
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

        logger.LogInformation(
            "Projected journey fact batch. Entries: {EntryCount}, Projected: {ProjectedCount}, EventFacts: {EventFactCount}, JourneyFacts: {JourneyFactCount}, DeletedBacklogEntries: {DeletedBacklogEntryCount}",
            entries.Count,
            result.ProjectedCount,
            result.EventFactCount,
            result.JourneyFactCount,
            result.DeletedBacklogEntryCount);

        if (result.DeletedBacklogEntryCount != entries.Count)
        {
            logger.LogWarning(
                "Projected journey fact batch deleted an unexpected number of backlog entries. Entries: {EntryCount}, DeletedBacklogEntries: {DeletedBacklogEntryCount}",
                entries.Count,
                result.DeletedBacklogEntryCount);
        }

        return new JourneyFactProjectionRunResult(
            ClaimedCount: entries.Count,
            ProjectedCount: result.ProjectedCount,
            SkippedCount: entries.Count - result.ProjectedCount,
            FailedCount: 0,
            EventFactCount: result.EventFactCount,
            JourneyFactCount: result.JourneyFactCount);
    }

    private async Task<ProjectFactsResult> ProjectFactsInDatabaseAsync(
        IReadOnlyList<ClaimedJourneyFactProjection> entries,
        CancellationToken cancellationToken
    )
    {
        await using var command = CreateCommand();
        command.CommandTimeout = _commandTimeoutSeconds;

        AddNamedParameter(
            command,
            "journey_ids",
            entries.Select(entry => entry.JourneyId).ToArray(),
            NpgsqlDbType.Array | NpgsqlDbType.Varchar);
        AddNamedParameter(
            command,
            "dates",
            entries.Select(entry => entry.Date).ToArray(),
            NpgsqlDbType.Array | NpgsqlDbType.Date);

        command.CommandText = """
            WITH claimed(journey_id, date) AS (
                SELECT journey_id, date
                FROM unnest(
                    CAST(@journey_ids AS character varying(82)[]),
                    CAST(@dates AS date[])
                ) AS entries(journey_id, date)
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
                    coalesce(
                        nullif(btrim(transports.journey_description), ''),
                        nullif(btrim(concat_ws(' ', nullif(btrim(transports.category), ''), nullif(btrim(transports.line), ''))), ''),
                        transports.number::text
                    ) AS journey_description,
                    transports.number AS journey_number
                FROM claimed
                INNER JOIN core.journeys AS journeys
                    ON journeys.id = claimed.journey_id
                   AND journeys.date = claimed.date
                INNER JOIN core.journey_transports AS transports
                    ON transports.journey_id = journeys.id
                   AND transports.date = journeys.date
            ),
            stop_places AS (
                SELECT DISTINCT ON (
                    journey.journey_id,
                    journey.date,
                    stop_place.station_eva_number,
                    stop_place.schedule_type,
                    stop_place.planned_time
                )
                    journey.journey_id,
                    journey.date,
                    journey.administration_id,
                    journey.journey_cancelled,
                    journey.journey_type,
                    journey.transport_type,
                    journey.replacement_transport_type,
                    journey.journey_description,
                    journey.journey_number,
                    stop_place.id AS stop_place_id,
                    time_bucket(INTERVAL '1 hour', stop_place.planned_time) AS bucket_hour,
                    stop_place.planned_time,
                    stop_place.station_eva_number,
                    stop_place.schedule_type,
                    stop_place.cancelled AS stop_cancelled,
                    stop_place.delay
                FROM journeys_with_transport AS journey
                INNER JOIN core.journey_stop_places AS stop_place
                    ON stop_place.journey_id = journey.journey_id
                   AND stop_place.date = journey.date
                ORDER BY
                    journey.journey_id,
                    journey.date,
                    stop_place.station_eva_number,
                    stop_place.schedule_type,
                    stop_place.planned_time,
                    CASE stop_place.time_type
                        WHEN 'REAL'::core.time_type THEN 0
                        WHEN 'PREVIEW'::core.time_type THEN 1
                        ELSE 2
                    END,
                    stop_place.actual_time DESC,
                    stop_place.id
            ),
            journey_bounds AS (
                SELECT
                    journey_id,
                    date,
                    (array_agg(administration_id))[1] AS administration_id,
                    bool_or(journey_type = 'REPLACEMENT'::core.journey_type OR replacement_transport_type IS NOT NULL) AS is_replacement,
                    (array_agg(transport_type))[1] AS transport_type,
                    (array_agg(journey_description))[1] AS journey_description,
                    (array_agg(journey_number))[1] AS journey_number,
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
                    (array_agg(delay ORDER BY CASE WHEN schedule_type = 'ARRIVAL'::core.schedule_type THEN 0 ELSE 1 END, planned_time DESC, station_eva_number DESC))[1] AS destination_delay_raw,
                    (array_agg(stop_cancelled ORDER BY CASE WHEN schedule_type = 'ARRIVAL'::core.schedule_type THEN 0 ELSE 1 END, planned_time DESC, station_eva_number DESC))[1] AS destination_stop_cancelled,
                    bool_or(journey_cancelled) OR bool_and(stop_cancelled) AS fully_cancelled,
                    bool_or(stop_cancelled) AS has_cancelled_stop
                FROM stop_places
                GROUP BY journey_id, date
            ),
            journey_facts AS (
                SELECT
                    *,
                    time_bucket(INTERVAL '1 hour', journey_start_time) AS bucket_hour,
                    has_cancelled_stop AND NOT fully_cancelled AS partially_cancelled,
                    fully_cancelled OR destination_stop_cancelled AS destination_not_reached,
                    CASE
                        WHEN fully_cancelled OR destination_stop_cancelled THEN NULL
                        ELSE destination_delay_raw
                    END AS destination_delay_seconds
                FROM journey_bounds
            ),
            deleted_event_facts AS (
                DELETE FROM statistics.journey_event_quality_facts AS facts
                USING claimed
                WHERE facts.journey_id = claimed.journey_id
                  AND facts.journey_date = claimed.date
                RETURNING 1
            ),
            deleted_journey_facts AS (
                DELETE FROM statistics.journey_quality_facts AS facts
                USING claimed
                WHERE facts.journey_id = claimed.journey_id
                  AND facts.journey_date = claimed.date
                RETURNING 1
            ),
            delete_barrier AS (
                SELECT
                    (SELECT count(*) FROM deleted_event_facts)
                    + (SELECT count(*) FROM deleted_journey_facts) AS deleted_count
            ),
            inserted_event_facts AS (
                INSERT INTO statistics.journey_event_quality_facts (
                    bucket_hour,
                    stop_place_id,
                    journey_id,
                    journey_date,
                    planned_time,
                    journey_start_time,
                    journey_end_time,
                    station_eva_number,
                    schedule_type,
                    administration_id,
                    transport_type,
                    journey_description,
                    journey_number,
                    is_replacement,
                    origin_eva_number,
                    destination_eva_number,
                    stop_cancelled,
                    event_delay_seconds
                )
                SELECT
                    stop_place.bucket_hour,
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
                    bounds.journey_number,
                    bounds.is_replacement,
                    bounds.origin_eva_number,
                    bounds.destination_eva_number,
                    stop_place.stop_cancelled,
                    stop_place.delay
                FROM stop_places AS stop_place
                INNER JOIN journey_bounds AS bounds
                    ON bounds.journey_id = stop_place.journey_id
                   AND bounds.date = stop_place.date
                CROSS JOIN delete_barrier
                RETURNING 1
            ),
            inserted_journey_facts AS (
                INSERT INTO statistics.journey_quality_facts (
                    bucket_hour,
                    journey_id,
                    journey_date,
                    administration_id,
                    transport_type,
                    journey_description,
                    journey_number,
                    is_replacement,
                    origin_eva_number,
                    destination_eva_number,
                    journey_start_time,
                    journey_end_time,
                    destination_delay_seconds,
                    fully_cancelled,
                    partially_cancelled,
                    destination_not_reached
                )
                SELECT
                    bucket_hour,
                    journey_id,
                    date,
                    administration_id,
                    transport_type,
                    journey_description,
                    journey_number,
                    is_replacement,
                    origin_eva_number,
                    destination_eva_number,
                    journey_start_time,
                    journey_end_time,
                    destination_delay_seconds,
                    fully_cancelled,
                    partially_cancelled,
                    destination_not_reached
                FROM journey_facts
                CROSS JOIN delete_barrier
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
                (SELECT count(*)::integer FROM journey_facts) AS projected_count,
                (SELECT count(*)::integer FROM stop_places) AS event_fact_count,
                (SELECT count(*)::integer FROM journey_facts) AS journey_fact_count,
                (SELECT count(*)::integer FROM inserted_event_facts) AS inserted_event_fact_count,
                (SELECT count(*)::integer FROM inserted_journey_facts) AS inserted_journey_fact_count,
                (SELECT count(*)::integer FROM deleted_backlog_entries) AS deleted_backlog_entry_count;
            """;

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return new ProjectFactsResult(0, 0, 0, 0);
        }

        return new ProjectFactsResult(
            ProjectedCount: reader.GetInt32(0),
            EventFactCount: reader.GetInt32(1),
            JourneyFactCount: reader.GetInt32(2),
            DeletedBacklogEntryCount: reader.GetInt32(5));
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
            AddNamedParameter(command, "batch_size", _batchSize);

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

    private static bool ShouldSplitBatch(Exception exception)
    {
        return exception is not NpgsqlException { IsTransient: true };
    }

    private DbCommand CreateCommand()
    {
        var command = dataContext.Database.GetDbConnection().CreateCommand();
        command.Transaction = dataContext.Database.CurrentTransaction?.GetDbTransaction();
        return command;
    }

    private static void AddNamedParameter(DbCommand command, string name, object value, NpgsqlDbType? npgsqlDbType = null)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        if (npgsqlDbType is not null && parameter is NpgsqlParameter npgsqlParameter)
        {
            npgsqlParameter.NpgsqlDbType = npgsqlDbType.Value;
        }
        command.Parameters.Add(parameter);
    }

    private static int ReadPositiveInt(IConfiguration configuration, string key, int defaultValue)
    {
        var configuredValue = configuration.GetValue<int?>(key);
        return configuredValue is > 0 ? configuredValue.Value : defaultValue;
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
        int JourneyFactCount,
        int DeletedBacklogEntryCount
    );
}

public sealed record JourneyFactProjectionRunResult(
    int ClaimedCount,
    int ProjectedCount,
    int SkippedCount,
    int FailedCount,
    int EventFactCount,
    int JourneyFactCount
)
{
    public static JourneyFactProjectionRunResult Empty { get; } = new(0, 0, 0, 0, 0, 0);

    public JourneyFactProjectionRunResult Add(JourneyFactProjectionRunResult other) => new(
        ClaimedCount + other.ClaimedCount,
        ProjectedCount + other.ProjectedCount,
        SkippedCount + other.SkippedCount,
        FailedCount + other.FailedCount,
        EventFactCount + other.EventFactCount,
        JourneyFactCount + other.JourneyFactCount);
}
