using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Navigator.Data;
using Navigator.Data.Entities.Journey;
using Navigator.Data.Entities.Statistics;
using Navigator.Data.Enums;
using Navigator.Data.Infrastructure;
using NpgsqlTypes;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace Navigator.Daemon.Infrastructure;

public sealed class JourneyFactProjectionService(
    ILogger<JourneyFactProjectionService> logger,
    DataContext dataContext
)
{
    private const int BatchSize = 500;
    private const int EventFactInsertChunkSize = 250;
    private const int RouteFactInsertChunkSize = 500;
    private const int BacklogDeleteChunkSize = 500;
    private const int MaxAttempts = 10;
    private static readonly TimeSpan _lockDuration = TimeSpan.FromMinutes(5);

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
        var journeyKeys = entries.Select(entry => (entry.JourneyId, entry.Date)).ToHashSet();
        var journeyIds = journeyKeys.Select(key => key.JourneyId).ToList();
        var journeyDates = journeyKeys.Select(key => key.Date).ToList();

        var journeys = (await dataContext.Journeys
                .AsSplitQuery()
                .Include(journey => journey.Transport)
                .Include(journey => journey.StopPlaces)
                .Where(journey => journeyIds.Contains(journey.Id))
                .Where(journey => journeyDates.Contains(journey.Date))
                .ToListAsync(cancellationToken))
            .Where(journey => journeyKeys.Contains((journey.Id, journey.Date)))
            .ToList();
        var foundKeys = journeys.Select(journey => (journey.Id, journey.Date)).ToHashSet();
        var skippedCount = entries.Count(entry => !foundKeys.Contains((entry.JourneyId, entry.Date)));

        var (eventFacts, routeFacts) = JourneyQualityFactBuilder.Build(journeys);

        await using var transaction = await dataContext.Database.BeginTransactionAsync(cancellationToken);
        await InsertEventFactsAsync(eventFacts, cancellationToken);
        await InsertRouteFactsAsync(routeFacts, cancellationToken);
        await DeleteBacklogEntriesAsync(entries, cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new JourneyFactProjectionRunResult(
            ClaimedCount: entries.Count,
            ProjectedCount: journeys.Count,
            SkippedCount: skippedCount,
            FailedCount: 0,
            EventFactCount: eventFacts.Count,
            RouteFactCount: routeFacts.Count);
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

    private async Task InsertEventFactsAsync(
        IReadOnlyCollection<JourneyEventQualityFact> facts,
        CancellationToken cancellationToken
    )
    {
        foreach (var chunk in facts.Chunk(EventFactInsertChunkSize))
        {
            await using var command = CreateCommand();
            var rows = new List<string>(chunk.Length);

            foreach (var fact in chunk)
            {
                var stopPlaceId = AddParameter(command, fact.StopPlaceId);
                var journeyId = AddParameter(command, fact.JourneyId);
                var date = AddParameter(command, fact.Date);
                var plannedTime = AddParameter(command, fact.PlannedTime);
                var journeyStartTime = AddParameter(command, fact.JourneyStartTime);
                var journeyEndTime = AddParameter(command, fact.JourneyEndTime);
                var stationEvaNumber = AddParameter(command, fact.StationEvaNumber);
                var scheduleType = AddParameter(command, GetPgName(fact.ScheduleType));
                var administrationId = AddParameter(command, fact.AdministrationId);
                var transportType = AddParameter(command, GetPgName(fact.TransportType));
                var journeyDescription = AddParameter(command, fact.JourneyDescription);
                var number = AddParameter(command, fact.Number);
                var isReplacementTransport = AddParameter(command, fact.IsReplacementTransport);
                var originEvaNumber = AddParameter(command, fact.OriginEvaNumber);
                var destinationEvaNumber = AddParameter(command, fact.DestinationEvaNumber);
                var cancelled = AddParameter(command, fact.Cancelled);
                var delay = AddParameter(command, fact.Delay);

                rows.Add($"""
                    ({stopPlaceId}, {journeyId}, {date}, {plannedTime}, {journeyStartTime}, {journeyEndTime}, {stationEvaNumber},
                     {scheduleType}::core.schedule_type, {administrationId}, {transportType}::core.transport_type, {journeyDescription},
                     {number}, {isReplacementTransport}, {originEvaNumber}, {destinationEvaNumber}, {cancelled}, {delay})
                    """);
            }

            command.CommandText = $"""
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
                VALUES {string.Join(", ", rows)}
                ON CONFLICT DO NOTHING;
                """;

            await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }

    private async Task InsertRouteFactsAsync(
        IReadOnlyCollection<JourneyRouteQualityFact> facts,
        CancellationToken cancellationToken
    )
    {
        foreach (var chunk in facts.Chunk(RouteFactInsertChunkSize))
        {
            await using var command = CreateCommand();
            var rows = new List<string>(chunk.Length);

            foreach (var fact in chunk)
            {
                var journeyId = AddParameter(command, fact.JourneyId);
                var date = AddParameter(command, fact.Date);
                var journeyStartTime = AddParameter(command, fact.JourneyStartTime);
                var journeyEndTime = AddParameter(command, fact.JourneyEndTime);
                var administrationId = AddParameter(command, fact.AdministrationId);
                var transportType = AddParameter(command, GetPgName(fact.TransportType));
                var journeyDescription = AddParameter(command, fact.JourneyDescription);
                var number = AddParameter(command, fact.Number);
                var isReplacementTransport = AddParameter(command, fact.IsReplacementTransport);
                var originEvaNumber = AddParameter(command, fact.OriginEvaNumber);
                var destinationEvaNumber = AddParameter(command, fact.DestinationEvaNumber);
                var journeyCancelled = AddParameter(command, fact.JourneyCancelled);
                var terminalDelaySeconds = AddParameter(command, fact.TerminalDelaySeconds);

                rows.Add($"""
                    ({journeyId}, {date}, {journeyStartTime}, {journeyEndTime}, {administrationId}, {transportType}::core.transport_type,
                     {journeyDescription}, {number}, {isReplacementTransport}, {originEvaNumber}, {destinationEvaNumber},
                     {journeyCancelled}, {terminalDelaySeconds})
                    """);
            }

            command.CommandText = $"""
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
                VALUES {string.Join(", ", rows)}
                ON CONFLICT DO NOTHING;
                """;

            await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }

    private async Task DeleteBacklogEntriesAsync(
        IReadOnlyList<ClaimedJourneyFactProjection> entries,
        CancellationToken cancellationToken
    )
    {
        foreach (var chunk in entries.Chunk(BacklogDeleteChunkSize))
        {
            await using var command = CreateCommand();
            var rows = new List<string>(chunk.Length);
            foreach (var entry in chunk)
            {
                var journeyId = AddParameter(command, entry.JourneyId);
                var date = AddParameter(command, entry.Date);
                rows.Add($"({journeyId}, {date})");
            }

            command.CommandText = $"""
                DELETE FROM statistics.journey_fact_projection_backlog AS backlog
                USING (VALUES {string.Join(", ", rows)}) AS processed(journey_id, date)
                WHERE backlog.journey_id = processed.journey_id
                  AND backlog.date = processed.date;
                """;

            await command.ExecuteNonQueryAsync(cancellationToken);
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

    private static string GetPgName<TEnum>(TEnum value) where TEnum : struct, Enum
    {
        var member = typeof(TEnum).GetMember(value.ToString()).Single();
        return member.GetCustomAttribute<PgNameAttribute>()?.PgName ?? value.ToString();
    }

    private sealed class ClaimedJourneyFactProjection
    {
        public required string JourneyId { get; set; }

        public required DateOnly Date { get; set; }

        public required int Attempts { get; set; }
    }
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
