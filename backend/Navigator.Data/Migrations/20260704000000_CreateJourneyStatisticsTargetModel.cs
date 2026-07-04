using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Navigator.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateJourneyStatisticsTargetModel : Migration
    {
        private static readonly string[] ContinuousAggregates =
        [
            "network_event_quality_hourly",
            "network_event_delay_distribution_hourly",
            "station_event_quality_hourly",
            "station_administration_quality_hourly",
            "line_event_quality_hourly",
            "station_line_quality_hourly",
            "network_journey_quality_hourly",
            "journey_administration_quality_hourly",
            "line_journey_quality_hourly",
            "journey_number_quality_hourly",
            "network_journey_outcome_hourly"
        ];

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE SCHEMA IF NOT EXISTS statistics;");

            #region Rollup Tables
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS statistics.event_quality_hourly_rollups (
                    bucket_hour timestamp with time zone NOT NULL,
                    station_eva_number integer NOT NULL,
                    schedule_type core.schedule_type NOT NULL,
                    administration_id uuid NOT NULL,
                    transport_type core.transport_type NOT NULL,
                    journey_description text NOT NULL DEFAULT '',
                    journey_number integer NOT NULL,
                    origin_eva_number integer NOT NULL,
                    destination_eva_number integer NOT NULL,
                    is_replacement boolean NOT NULL DEFAULT false,
                    event_count bigint NOT NULL,
                    cancelled_count bigint NOT NULL,
                    delay_sum_seconds bigint NOT NULL,
                    positive_delay_sum_seconds bigint NOT NULL,
                    delayed_count bigint NOT NULL,
                    punctual_5_count bigint NOT NULL,
                    punctual_15_count bigint NOT NULL,
                    late_30_count bigint NOT NULL,
                    late_60_count bigint NOT NULL,
                    delay_lt_minus_5_count bigint NOT NULL,
                    delay_gte_minus_5_lt_0_count bigint NOT NULL,
                    delay_gte_0_lt_5_count bigint NOT NULL,
                    delay_gte_5_lt_10_count bigint NOT NULL,
                    delay_gte_10_lt_15_count bigint NOT NULL,
                    delay_gte_15_lt_30_count bigint NOT NULL,
                    delay_gte_30_lt_60_count bigint NOT NULL,
                    delay_gte_60_lt_120_count bigint NOT NULL,
                    delay_gte_120_count bigint NOT NULL,
                    refreshed_at timestamp with time zone NOT NULL,
                    CONSTRAINT pk_event_quality_hourly_rollups PRIMARY KEY (
                        bucket_hour,
                        station_eva_number,
                        schedule_type,
                        administration_id,
                        transport_type,
                        journey_description,
                        journey_number,
                        origin_eva_number,
                        destination_eva_number,
                        is_replacement
                    )
                );
            ");

            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ix_event_quality_hourly_rollups_station_hour ON statistics.event_quality_hourly_rollups (station_eva_number, bucket_hour DESC);");
            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ix_event_quality_hourly_rollups_admin_hour ON statistics.event_quality_hourly_rollups (administration_id, bucket_hour DESC);");
            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ix_event_quality_hourly_rollups_line_hour ON statistics.event_quality_hourly_rollups (journey_description, journey_number, origin_eva_number, destination_eva_number, bucket_hour DESC);");

            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS statistics.journey_quality_hourly_rollups (
                    bucket_hour timestamp with time zone NOT NULL,
                    administration_id uuid NOT NULL,
                    transport_type core.transport_type NOT NULL,
                    journey_description text NOT NULL DEFAULT '',
                    journey_number integer NOT NULL,
                    origin_eva_number integer NOT NULL,
                    destination_eva_number integer NOT NULL,
                    is_replacement boolean NOT NULL DEFAULT false,
                    journey_count bigint NOT NULL,
                    fully_cancelled_count bigint NOT NULL,
                    partially_cancelled_count bigint NOT NULL,
                    destination_reached_count bigint NOT NULL,
                    destination_not_reached_count bigint NOT NULL,
                    completed_count bigint NOT NULL,
                    partially_cancelled_destination_reached_count bigint NOT NULL,
                    destination_not_reached_without_full_cancel_count bigint NOT NULL,
                    destination_delay_sum_seconds bigint NOT NULL,
                    destination_positive_delay_sum_seconds bigint NOT NULL,
                    destination_delayed_count bigint NOT NULL,
                    destination_punctual_5_count bigint NOT NULL,
                    destination_punctual_15_count bigint NOT NULL,
                    destination_late_30_count bigint NOT NULL,
                    destination_late_60_count bigint NOT NULL,
                    refreshed_at timestamp with time zone NOT NULL,
                    CONSTRAINT pk_journey_quality_hourly_rollups PRIMARY KEY (
                        bucket_hour,
                        administration_id,
                        transport_type,
                        journey_description,
                        journey_number,
                        origin_eva_number,
                        destination_eva_number,
                        is_replacement
                    )
                );
            ");

            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ix_journey_quality_hourly_rollups_admin_hour ON statistics.journey_quality_hourly_rollups (administration_id, bucket_hour DESC);");
            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ix_journey_quality_hourly_rollups_line_hour ON statistics.journey_quality_hourly_rollups (journey_description, journey_number, origin_eva_number, destination_eva_number, bucket_hour DESC);");

            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS statistics.statistics_refresh_progress (
                    operation text NOT NULL,
                    window_start timestamp with time zone NOT NULL,
                    window_end timestamp with time zone NOT NULL,
                    status text NOT NULL,
                    attempt integer NOT NULL,
                    event_rows_affected bigint NOT NULL,
                    journey_rows_affected bigint NOT NULL,
                    cagg_refresh_count integer NOT NULL,
                    started_at timestamp with time zone NOT NULL,
                    finished_at timestamp with time zone NULL,
                    error_kind text NULL,
                    CONSTRAINT pk_statistics_refresh_progress PRIMARY KEY (operation, window_start, window_end)
                );
            ");

            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ix_statistics_refresh_progress_status_window ON statistics.statistics_refresh_progress (operation, status, window_start, window_end);");

            migrationBuilder.Sql(@"SELECT create_hypertable('statistics.event_quality_hourly_rollups', 'bucket_hour', chunk_time_interval => INTERVAL '7 days', if_not_exists => TRUE);");
            migrationBuilder.Sql(@"SELECT create_hypertable('statistics.journey_quality_hourly_rollups', 'bucket_hour', chunk_time_interval => INTERVAL '7 days', if_not_exists => TRUE);");
            #endregion

            #region Recompute Functions
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION statistics.recompute_quality_hourly_rollups(
                    window_start timestamp with time zone,
                    window_end timestamp with time zone
                ) RETURNS TABLE (
                    event_rows_affected bigint,
                    journey_rows_affected bigint
                )
                LANGUAGE plpgsql
                AS $$
                DECLARE
                    event_rows bigint;
                    journey_rows bigint;
                BEGIN
                    IF window_end <= window_start THEN
                        RAISE EXCEPTION 'window_end must be after window_start';
                    END IF;

                    PERFORM set_config('work_mem', '256MB', true);

                    DELETE FROM statistics.event_quality_hourly_rollups
                    WHERE bucket_hour >= window_start
                      AND bucket_hour < window_end;

                    DELETE FROM statistics.journey_quality_hourly_rollups
                    WHERE bucket_hour >= window_start
                      AND bucket_hour < window_end;

                    DROP TABLE IF EXISTS pg_temp.statistics_recovery_window_stop_places;
                    CREATE TEMP TABLE statistics_recovery_window_stop_places
                    ON COMMIT DROP
                    AS
                    SELECT DISTINCT ON (
                        stop_place.journey_id,
                        stop_place.date,
                        stop_place.station_eva_number,
                        stop_place.schedule_type,
                        stop_place.planned_time
                    )
                        stop_place.journey_id,
                        stop_place.date,
                        stop_place.id AS stop_place_id,
                        time_bucket(INTERVAL '1 hour', stop_place.planned_time) AS bucket_hour,
                        stop_place.planned_time,
                        stop_place.station_eva_number,
                        stop_place.schedule_type,
                        stop_place.cancelled AS stop_cancelled,
                        stop_place.delay
                    FROM core.journey_stop_places AS stop_place
                    WHERE stop_place.date >= (window_start AT TIME ZONE 'UTC')::date - 1
                      AND stop_place.date <= (window_end AT TIME ZONE 'UTC')::date + 1
                      AND stop_place.planned_time >= window_start
                      AND stop_place.planned_time < window_end
                    ORDER BY
                        stop_place.journey_id,
                        stop_place.date,
                        stop_place.station_eva_number,
                        stop_place.schedule_type,
                        stop_place.planned_time,
                        CASE stop_place.time_type
                            WHEN 'REAL'::core.time_type THEN 0
                            WHEN 'PREVIEW'::core.time_type THEN 1
                            ELSE 2
                        END,
                        stop_place.actual_time DESC,
                        stop_place.id;

                    CREATE INDEX statistics_recovery_window_stop_places_journey_idx
                        ON statistics_recovery_window_stop_places (journey_id, date);
                    CREATE INDEX statistics_recovery_window_stop_places_bucket_idx
                        ON statistics_recovery_window_stop_places (bucket_hour);
                    ANALYZE statistics_recovery_window_stop_places;

                    DROP TABLE IF EXISTS pg_temp.statistics_recovery_touched_journeys;
                    CREATE TEMP TABLE statistics_recovery_touched_journeys
                    ON COMMIT DROP
                    AS
                    SELECT DISTINCT journey_id, date
                    FROM statistics_recovery_window_stop_places;

                    CREATE UNIQUE INDEX statistics_recovery_touched_journeys_pk
                        ON statistics_recovery_touched_journeys (journey_id, date);
                    ANALYZE statistics_recovery_touched_journeys;

                    DROP TABLE IF EXISTS pg_temp.statistics_recovery_journey_stop_places;
                    CREATE TEMP TABLE statistics_recovery_journey_stop_places
                    ON COMMIT DROP
                    AS
                    SELECT DISTINCT ON (
                        stop_place.journey_id,
                        stop_place.date,
                        stop_place.station_eva_number,
                        stop_place.schedule_type,
                        stop_place.planned_time
                    )
                        stop_place.journey_id,
                        stop_place.date,
                        stop_place.planned_time,
                        stop_place.station_eva_number,
                        stop_place.schedule_type,
                        stop_place.cancelled AS stop_cancelled,
                        stop_place.delay
                    FROM core.journey_stop_places AS stop_place
                    INNER JOIN statistics_recovery_touched_journeys AS touched_journeys
                        ON touched_journeys.journey_id = stop_place.journey_id
                       AND touched_journeys.date = stop_place.date
                    ORDER BY
                        stop_place.journey_id,
                        stop_place.date,
                        stop_place.station_eva_number,
                        stop_place.schedule_type,
                        stop_place.planned_time,
                        CASE stop_place.time_type
                            WHEN 'REAL'::core.time_type THEN 0
                            WHEN 'PREVIEW'::core.time_type THEN 1
                            ELSE 2
                        END,
                        stop_place.actual_time DESC,
                        stop_place.id;

                    CREATE INDEX statistics_recovery_journey_stop_places_journey_idx
                        ON statistics_recovery_journey_stop_places (journey_id, date);
                    CREATE INDEX statistics_recovery_journey_stop_places_order_idx
                        ON statistics_recovery_journey_stop_places (journey_id, date, planned_time);
                    ANALYZE statistics_recovery_journey_stop_places;

                    DROP TABLE IF EXISTS pg_temp.statistics_recovery_journey_dimensions;
                    CREATE TEMP TABLE statistics_recovery_journey_dimensions
                    ON COMMIT DROP
                    AS
                    SELECT
                        journeys.id AS journey_id,
                        journeys.date,
                        journeys.administration_id,
                        journeys.cancelled AS journey_cancelled,
                        journeys.journey_type,
                        transports.transport_type,
                        transports.replacement_transport_type,
                        COALESCE(
                            NULLIF(transports.journey_description, ''),
                            NULLIF(transports.line, ''),
                            transports.number::text
                        ) AS journey_description,
                        transports.number AS journey_number
                    FROM statistics_recovery_touched_journeys AS touched_journeys
                    INNER JOIN core.journeys AS journeys
                        ON journeys.id = touched_journeys.journey_id
                       AND journeys.date = touched_journeys.date
                    INNER JOIN core.journey_transports AS transports
                        ON transports.journey_id = journeys.id
                       AND transports.date = journeys.date;

                    CREATE UNIQUE INDEX statistics_recovery_journey_dimensions_pk
                        ON statistics_recovery_journey_dimensions (journey_id, date);
                    ANALYZE statistics_recovery_journey_dimensions;

                    DROP TABLE IF EXISTS pg_temp.statistics_recovery_journey_bounds;
                    CREATE TEMP TABLE statistics_recovery_journey_bounds
                    ON COMMIT DROP
                    AS
                    SELECT
                        journey_stop_places.journey_id,
                        journey_stop_places.date,
                        COALESCE(
                            (array_agg(journey_stop_places.station_eva_number ORDER BY journey_stop_places.planned_time, journey_stop_places.station_eva_number)
                                FILTER (WHERE journey_stop_places.schedule_type = 'DEPARTURE'::core.schedule_type))[1],
                            (array_agg(journey_stop_places.station_eva_number ORDER BY journey_stop_places.planned_time, journey_stop_places.station_eva_number))[1]
                        ) AS origin_eva_number,
                        COALESCE(
                            (array_agg(journey_stop_places.station_eva_number ORDER BY journey_stop_places.planned_time DESC, journey_stop_places.station_eva_number DESC)
                                FILTER (WHERE journey_stop_places.schedule_type = 'ARRIVAL'::core.schedule_type))[1],
                            (array_agg(journey_stop_places.station_eva_number ORDER BY journey_stop_places.planned_time DESC, journey_stop_places.station_eva_number DESC))[1]
                        ) AS destination_eva_number,
                        COALESCE(
                            MIN(journey_stop_places.planned_time) FILTER (WHERE journey_stop_places.schedule_type = 'DEPARTURE'::core.schedule_type),
                            MIN(journey_stop_places.planned_time)
                        ) AS journey_start_time,
                        COALESCE(
                            MAX(journey_stop_places.planned_time) FILTER (WHERE journey_stop_places.schedule_type = 'ARRIVAL'::core.schedule_type),
                            MAX(journey_stop_places.planned_time)
                        ) AS journey_end_time,
                        (array_agg(journey_stop_places.delay ORDER BY CASE WHEN journey_stop_places.schedule_type = 'ARRIVAL'::core.schedule_type THEN 0 ELSE 1 END, journey_stop_places.planned_time DESC, journey_stop_places.station_eva_number DESC))[1] AS destination_delay_raw,
                        (array_agg(journey_stop_places.stop_cancelled ORDER BY CASE WHEN journey_stop_places.schedule_type = 'ARRIVAL'::core.schedule_type THEN 0 ELSE 1 END, journey_stop_places.planned_time DESC, journey_stop_places.station_eva_number DESC))[1] AS destination_stop_cancelled,
                        bool_and(journey_stop_places.stop_cancelled) AS all_stops_cancelled,
                        bool_or(journey_stop_places.stop_cancelled) AS has_cancelled_stop
                    FROM statistics_recovery_journey_stop_places AS journey_stop_places
                    GROUP BY journey_stop_places.journey_id, journey_stop_places.date;

                    CREATE UNIQUE INDEX statistics_recovery_journey_bounds_pk
                        ON statistics_recovery_journey_bounds (journey_id, date);
                    CREATE INDEX statistics_recovery_journey_bounds_start_idx
                        ON statistics_recovery_journey_bounds (journey_start_time);
                    ANALYZE statistics_recovery_journey_bounds;

                    INSERT INTO statistics.event_quality_hourly_rollups (
                        bucket_hour,
                        station_eva_number,
                        schedule_type,
                        administration_id,
                        transport_type,
                        journey_description,
                        journey_number,
                        origin_eva_number,
                        destination_eva_number,
                        is_replacement,
                        event_count,
                        cancelled_count,
                        delay_sum_seconds,
                        positive_delay_sum_seconds,
                        delayed_count,
                        punctual_5_count,
                        punctual_15_count,
                        late_30_count,
                        late_60_count,
                        delay_lt_minus_5_count,
                        delay_gte_minus_5_lt_0_count,
                        delay_gte_0_lt_5_count,
                        delay_gte_5_lt_10_count,
                        delay_gte_10_lt_15_count,
                        delay_gte_15_lt_30_count,
                        delay_gte_30_lt_60_count,
                        delay_gte_60_lt_120_count,
                        delay_gte_120_count,
                        refreshed_at
                    )
                    SELECT
                        window_stop_places.bucket_hour,
                        window_stop_places.station_eva_number,
                        window_stop_places.schedule_type,
                        journey_dimensions.administration_id,
                        journey_dimensions.transport_type,
                        journey_dimensions.journey_description,
                        journey_dimensions.journey_number,
                        journey_bounds.origin_eva_number,
                        journey_bounds.destination_eva_number,
                        journey_dimensions.journey_type = 'REPLACEMENT'::core.journey_type
                            OR journey_dimensions.replacement_transport_type IS NOT NULL,
                        COUNT(*)::bigint,
                        COUNT(*) FILTER (WHERE window_stop_places.stop_cancelled)::bigint,
                        COALESCE(SUM(window_stop_places.delay) FILTER (WHERE NOT window_stop_places.stop_cancelled), 0)::bigint,
                        COALESCE(SUM(GREATEST(window_stop_places.delay, 0)) FILTER (WHERE NOT window_stop_places.stop_cancelled), 0)::bigint,
                        COUNT(*) FILTER (WHERE NOT window_stop_places.stop_cancelled AND window_stop_places.delay > 0)::bigint,
                        COUNT(*) FILTER (WHERE NOT window_stop_places.stop_cancelled AND window_stop_places.delay < 360)::bigint,
                        COUNT(*) FILTER (WHERE NOT window_stop_places.stop_cancelled AND window_stop_places.delay < 900)::bigint,
                        COUNT(*) FILTER (WHERE NOT window_stop_places.stop_cancelled AND window_stop_places.delay >= 1800)::bigint,
                        COUNT(*) FILTER (WHERE NOT window_stop_places.stop_cancelled AND window_stop_places.delay >= 3600)::bigint,
                        COUNT(*) FILTER (WHERE NOT window_stop_places.stop_cancelled AND window_stop_places.delay < -300)::bigint,
                        COUNT(*) FILTER (WHERE NOT window_stop_places.stop_cancelled AND window_stop_places.delay >= -300 AND window_stop_places.delay < 0)::bigint,
                        COUNT(*) FILTER (WHERE NOT window_stop_places.stop_cancelled AND window_stop_places.delay >= 0 AND window_stop_places.delay < 300)::bigint,
                        COUNT(*) FILTER (WHERE NOT window_stop_places.stop_cancelled AND window_stop_places.delay >= 300 AND window_stop_places.delay < 600)::bigint,
                        COUNT(*) FILTER (WHERE NOT window_stop_places.stop_cancelled AND window_stop_places.delay >= 600 AND window_stop_places.delay < 900)::bigint,
                        COUNT(*) FILTER (WHERE NOT window_stop_places.stop_cancelled AND window_stop_places.delay >= 900 AND window_stop_places.delay < 1800)::bigint,
                        COUNT(*) FILTER (WHERE NOT window_stop_places.stop_cancelled AND window_stop_places.delay >= 1800 AND window_stop_places.delay < 3600)::bigint,
                        COUNT(*) FILTER (WHERE NOT window_stop_places.stop_cancelled AND window_stop_places.delay >= 3600 AND window_stop_places.delay < 7200)::bigint,
                        COUNT(*) FILTER (WHERE NOT window_stop_places.stop_cancelled AND window_stop_places.delay >= 7200)::bigint,
                        now()
                    FROM statistics_recovery_window_stop_places AS window_stop_places
                    INNER JOIN statistics_recovery_journey_dimensions AS journey_dimensions
                        ON journey_dimensions.journey_id = window_stop_places.journey_id
                       AND journey_dimensions.date = window_stop_places.date
                    INNER JOIN statistics_recovery_journey_bounds AS journey_bounds
                        ON journey_bounds.journey_id = window_stop_places.journey_id
                       AND journey_bounds.date = window_stop_places.date
                    GROUP BY
                        window_stop_places.bucket_hour,
                        window_stop_places.station_eva_number,
                        window_stop_places.schedule_type,
                        journey_dimensions.administration_id,
                        journey_dimensions.transport_type,
                        journey_dimensions.journey_description,
                        journey_dimensions.journey_number,
                        journey_bounds.origin_eva_number,
                        journey_bounds.destination_eva_number,
                        journey_dimensions.journey_type = 'REPLACEMENT'::core.journey_type
                            OR journey_dimensions.replacement_transport_type IS NOT NULL;

                    GET DIAGNOSTICS event_rows = ROW_COUNT;

                    WITH journey_facts AS (
                        SELECT
                            time_bucket(INTERVAL '1 hour', journey_bounds.journey_start_time) AS bucket_hour,
                            journey_dimensions.administration_id,
                            journey_dimensions.transport_type,
                            journey_dimensions.journey_description,
                            journey_dimensions.journey_number,
                            journey_bounds.origin_eva_number,
                            journey_bounds.destination_eva_number,
                            journey_dimensions.journey_type = 'REPLACEMENT'::core.journey_type
                                OR journey_dimensions.replacement_transport_type IS NOT NULL AS is_replacement,
                            journey_dimensions.journey_cancelled OR journey_bounds.all_stops_cancelled AS fully_cancelled,
                            journey_bounds.has_cancelled_stop
                                AND NOT (journey_dimensions.journey_cancelled OR journey_bounds.all_stops_cancelled) AS partially_cancelled,
                            journey_dimensions.journey_cancelled
                                OR journey_bounds.all_stops_cancelled
                                OR journey_bounds.destination_stop_cancelled AS destination_not_reached,
                            CASE
                                WHEN journey_dimensions.journey_cancelled
                                  OR journey_bounds.all_stops_cancelled
                                  OR journey_bounds.destination_stop_cancelled THEN NULL
                                ELSE journey_bounds.destination_delay_raw
                            END AS destination_delay_seconds
                        FROM statistics_recovery_journey_bounds AS journey_bounds
                        INNER JOIN statistics_recovery_journey_dimensions AS journey_dimensions
                            ON journey_dimensions.journey_id = journey_bounds.journey_id
                           AND journey_dimensions.date = journey_bounds.date
                        WHERE journey_bounds.journey_start_time >= window_start
                          AND journey_bounds.journey_start_time < window_end
                    )
                    INSERT INTO statistics.journey_quality_hourly_rollups (
                        bucket_hour,
                        administration_id,
                        transport_type,
                        journey_description,
                        journey_number,
                        origin_eva_number,
                        destination_eva_number,
                        is_replacement,
                        journey_count,
                        fully_cancelled_count,
                        partially_cancelled_count,
                        destination_reached_count,
                        destination_not_reached_count,
                        completed_count,
                        partially_cancelled_destination_reached_count,
                        destination_not_reached_without_full_cancel_count,
                        destination_delay_sum_seconds,
                        destination_positive_delay_sum_seconds,
                        destination_delayed_count,
                        destination_punctual_5_count,
                        destination_punctual_15_count,
                        destination_late_30_count,
                        destination_late_60_count,
                        refreshed_at
                    )
                    SELECT
                        journey_facts.bucket_hour,
                        journey_facts.administration_id,
                        journey_facts.transport_type,
                        journey_facts.journey_description,
                        journey_facts.journey_number,
                        journey_facts.origin_eva_number,
                        journey_facts.destination_eva_number,
                        journey_facts.is_replacement,
                        COUNT(*)::bigint,
                        COUNT(*) FILTER (WHERE journey_facts.fully_cancelled)::bigint,
                        COUNT(*) FILTER (WHERE journey_facts.partially_cancelled)::bigint,
                        COUNT(*) FILTER (WHERE NOT journey_facts.destination_not_reached)::bigint,
                        COUNT(*) FILTER (WHERE journey_facts.destination_not_reached)::bigint,
                        COUNT(*) FILTER (WHERE NOT journey_facts.fully_cancelled AND NOT journey_facts.partially_cancelled AND NOT journey_facts.destination_not_reached)::bigint,
                        COUNT(*) FILTER (WHERE journey_facts.partially_cancelled AND NOT journey_facts.destination_not_reached)::bigint,
                        COUNT(*) FILTER (WHERE journey_facts.destination_not_reached AND NOT journey_facts.fully_cancelled)::bigint,
                        COALESCE(SUM(journey_facts.destination_delay_seconds), 0)::bigint,
                        COALESCE(SUM(GREATEST(journey_facts.destination_delay_seconds, 0)) FILTER (WHERE journey_facts.destination_delay_seconds IS NOT NULL), 0)::bigint,
                        COUNT(*) FILTER (WHERE journey_facts.destination_delay_seconds > 0)::bigint,
                        COUNT(*) FILTER (WHERE journey_facts.destination_delay_seconds IS NOT NULL AND journey_facts.destination_delay_seconds < 360)::bigint,
                        COUNT(*) FILTER (WHERE journey_facts.destination_delay_seconds IS NOT NULL AND journey_facts.destination_delay_seconds < 900)::bigint,
                        COUNT(*) FILTER (WHERE journey_facts.destination_delay_seconds IS NOT NULL AND journey_facts.destination_delay_seconds >= 1800)::bigint,
                        COUNT(*) FILTER (WHERE journey_facts.destination_delay_seconds IS NOT NULL AND journey_facts.destination_delay_seconds >= 3600)::bigint,
                        now()
                    FROM journey_facts
                    GROUP BY
                        journey_facts.bucket_hour,
                        journey_facts.administration_id,
                        journey_facts.transport_type,
                        journey_facts.journey_description,
                        journey_facts.journey_number,
                        journey_facts.origin_eva_number,
                        journey_facts.destination_eva_number,
                        journey_facts.is_replacement;

                    GET DIAGNOSTICS journey_rows = ROW_COUNT;

                    event_rows_affected := event_rows;
                    journey_rows_affected := journey_rows;
                    RETURN NEXT;
                END;
                $$;");
            #endregion

            #region Detail Views
            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW statistics.station_journey_event_details AS
                WITH stop_places AS (
                    SELECT DISTINCT ON (
                        stop_place.journey_id,
                        stop_place.date,
                        stop_place.station_eva_number,
                        stop_place.schedule_type,
                        stop_place.planned_time
                    )
                        stop_place.journey_id,
                        stop_place.date,
                        stop_place.id AS stop_place_id,
                        time_bucket(INTERVAL '1 hour', stop_place.planned_time) AS bucket_hour,
                        stop_place.planned_time,
                        stop_place.station_eva_number,
                        stop_place.schedule_type,
                        stop_place.cancelled AS stop_cancelled,
                        stop_place.delay AS event_delay_seconds
                    FROM core.journey_stop_places AS stop_place
                    ORDER BY
                        stop_place.journey_id,
                        stop_place.date,
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
                journeys_with_transport AS (
                    SELECT
                        journeys.id AS journey_id,
                        journeys.date,
                        journeys.administration_id,
                        journeys.journey_type,
                        transports.transport_type,
                        transports.replacement_transport_type,
                        COALESCE(NULLIF(transports.journey_description, ''), NULLIF(transports.line, ''), transports.number::text) AS journey_description,
                        transports.number AS journey_number
                    FROM core.journeys AS journeys
                    INNER JOIN core.journey_transports AS transports
                        ON transports.journey_id = journeys.id
                       AND transports.date = journeys.date
                ),
                journey_bounds AS (
                    SELECT
                        stop_places.journey_id,
                        stop_places.date,
                        COALESCE((array_agg(stop_places.station_eva_number ORDER BY stop_places.planned_time, stop_places.station_eva_number) FILTER (WHERE stop_places.schedule_type = 'DEPARTURE'::core.schedule_type))[1], (array_agg(stop_places.station_eva_number ORDER BY stop_places.planned_time, stop_places.station_eva_number))[1]) AS origin_eva_number,
                        COALESCE((array_agg(stop_places.station_eva_number ORDER BY stop_places.planned_time DESC, stop_places.station_eva_number DESC) FILTER (WHERE stop_places.schedule_type = 'ARRIVAL'::core.schedule_type))[1], (array_agg(stop_places.station_eva_number ORDER BY stop_places.planned_time DESC, stop_places.station_eva_number DESC))[1]) AS destination_eva_number,
                        COALESCE(MIN(stop_places.planned_time) FILTER (WHERE stop_places.schedule_type = 'DEPARTURE'::core.schedule_type), MIN(stop_places.planned_time)) AS journey_start_time,
                        COALESCE(MAX(stop_places.planned_time) FILTER (WHERE stop_places.schedule_type = 'ARRIVAL'::core.schedule_type), MAX(stop_places.planned_time)) AS journey_end_time
                    FROM stop_places
                    GROUP BY stop_places.journey_id, stop_places.date
                )
                SELECT
                    stop_places.bucket_hour,
                    stop_places.stop_place_id,
                    stop_places.journey_id,
                    stop_places.date AS journey_date,
                    stop_places.station_eva_number,
                    stop_places.planned_time,
                    stop_places.schedule_type,
                    journeys_with_transport.administration_id,
                    journeys_with_transport.journey_number,
                    journeys_with_transport.journey_description,
                    journey_bounds.origin_eva_number,
                    journey_bounds.journey_start_time,
                    journey_bounds.destination_eva_number,
                    journey_bounds.journey_end_time,
                    stop_places.stop_cancelled,
                    journeys_with_transport.transport_type,
                    stop_places.event_delay_seconds,
                    journeys_with_transport.journey_type = 'REPLACEMENT'::core.journey_type
                        OR journeys_with_transport.replacement_transport_type IS NOT NULL AS is_replacement
                FROM stop_places
                INNER JOIN journeys_with_transport
                    ON journeys_with_transport.journey_id = stop_places.journey_id
                   AND journeys_with_transport.date = stop_places.date
                INNER JOIN journey_bounds
                    ON journey_bounds.journey_id = stop_places.journey_id
                   AND journey_bounds.date = stop_places.date;
            ");

            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW statistics.journey_quality_details AS
                WITH stop_places AS (
                    SELECT DISTINCT ON (
                        stop_place.journey_id,
                        stop_place.date,
                        stop_place.station_eva_number,
                        stop_place.schedule_type,
                        stop_place.planned_time
                    )
                        stop_place.journey_id,
                        stop_place.date,
                        stop_place.planned_time,
                        stop_place.station_eva_number,
                        stop_place.schedule_type,
                        stop_place.cancelled AS stop_cancelled,
                        stop_place.delay
                    FROM core.journey_stop_places AS stop_place
                    ORDER BY
                        stop_place.journey_id,
                        stop_place.date,
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
                journeys_with_transport AS (
                    SELECT
                        journeys.id AS journey_id,
                        journeys.date,
                        journeys.administration_id,
                        journeys.cancelled AS journey_cancelled,
                        journeys.journey_type,
                        transports.transport_type,
                        transports.replacement_transport_type,
                        COALESCE(NULLIF(transports.journey_description, ''), NULLIF(transports.line, ''), transports.number::text) AS journey_description,
                        transports.number AS journey_number
                    FROM core.journeys AS journeys
                    INNER JOIN core.journey_transports AS transports
                        ON transports.journey_id = journeys.id
                       AND transports.date = journeys.date
                ),
                journey_bounds AS (
                    SELECT
                        stop_places.journey_id,
                        stop_places.date,
                        COALESCE((array_agg(stop_places.station_eva_number ORDER BY stop_places.planned_time, stop_places.station_eva_number) FILTER (WHERE stop_places.schedule_type = 'DEPARTURE'::core.schedule_type))[1], (array_agg(stop_places.station_eva_number ORDER BY stop_places.planned_time, stop_places.station_eva_number))[1]) AS origin_eva_number,
                        COALESCE((array_agg(stop_places.station_eva_number ORDER BY stop_places.planned_time DESC, stop_places.station_eva_number DESC) FILTER (WHERE stop_places.schedule_type = 'ARRIVAL'::core.schedule_type))[1], (array_agg(stop_places.station_eva_number ORDER BY stop_places.planned_time DESC, stop_places.station_eva_number DESC))[1]) AS destination_eva_number,
                        COALESCE(MIN(stop_places.planned_time) FILTER (WHERE stop_places.schedule_type = 'DEPARTURE'::core.schedule_type), MIN(stop_places.planned_time)) AS journey_start_time,
                        COALESCE(MAX(stop_places.planned_time) FILTER (WHERE stop_places.schedule_type = 'ARRIVAL'::core.schedule_type), MAX(stop_places.planned_time)) AS journey_end_time,
                        (array_agg(stop_places.delay ORDER BY CASE WHEN stop_places.schedule_type = 'ARRIVAL'::core.schedule_type THEN 0 ELSE 1 END, stop_places.planned_time DESC, stop_places.station_eva_number DESC))[1] AS destination_delay_raw,
                        (array_agg(stop_places.stop_cancelled ORDER BY CASE WHEN stop_places.schedule_type = 'ARRIVAL'::core.schedule_type THEN 0 ELSE 1 END, stop_places.planned_time DESC, stop_places.station_eva_number DESC))[1] AS destination_stop_cancelled,
                        bool_and(stop_places.stop_cancelled) AS all_stops_cancelled,
                        bool_or(stop_places.stop_cancelled) AS has_cancelled_stop
                    FROM stop_places
                    GROUP BY stop_places.journey_id, stop_places.date
                )
                SELECT
                    time_bucket(INTERVAL '1 hour', journey_bounds.journey_start_time) AS bucket_hour,
                    journeys_with_transport.journey_id,
                    journeys_with_transport.date AS journey_date,
                    journeys_with_transport.administration_id,
                    journeys_with_transport.transport_type,
                    journeys_with_transport.journey_description,
                    journeys_with_transport.journey_number,
                    journeys_with_transport.journey_type = 'REPLACEMENT'::core.journey_type
                        OR journeys_with_transport.replacement_transport_type IS NOT NULL AS is_replacement,
                    journey_bounds.origin_eva_number,
                    journey_bounds.destination_eva_number,
                    journey_bounds.journey_start_time,
                    journey_bounds.journey_end_time,
                    CASE
                        WHEN journeys_with_transport.journey_cancelled
                          OR journey_bounds.all_stops_cancelled
                          OR journey_bounds.destination_stop_cancelled THEN NULL
                        ELSE journey_bounds.destination_delay_raw
                    END AS destination_delay_seconds,
                    journeys_with_transport.journey_cancelled OR journey_bounds.all_stops_cancelled AS fully_cancelled,
                    journey_bounds.has_cancelled_stop
                        AND NOT (journeys_with_transport.journey_cancelled OR journey_bounds.all_stops_cancelled) AS partially_cancelled,
                    journeys_with_transport.journey_cancelled
                        OR journey_bounds.all_stops_cancelled
                        OR journey_bounds.destination_stop_cancelled AS destination_not_reached
                FROM journey_bounds
                INNER JOIN journeys_with_transport
                    ON journeys_with_transport.journey_id = journey_bounds.journey_id
                   AND journeys_with_transport.date = journey_bounds.date;
            ");
            #endregion

            CreateContinuousAggregates(migrationBuilder);
            CreateContinuousAggregatePolicies(migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            foreach (var continuousAggregate in ContinuousAggregates)
            {
                migrationBuilder.Sql($@"SELECT remove_continuous_aggregate_policy('statistics.{continuousAggregate}', if_exists => TRUE);");
            }

            foreach (var continuousAggregate in ContinuousAggregates.Reverse())
            {
                migrationBuilder.Sql($@"DROP MATERIALIZED VIEW IF EXISTS statistics.{continuousAggregate};");
            }

            migrationBuilder.Sql(@"DROP VIEW IF EXISTS statistics.journey_quality_details;");
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS statistics.station_journey_event_details;");
            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS statistics.refresh_statistics_caggs(timestamptz, timestamptz);");
            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS statistics.recompute_quality_hourly_rollups(timestamptz, timestamptz);");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS statistics.statistics_refresh_progress;");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS statistics.journey_quality_hourly_rollups;");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS statistics.event_quality_hourly_rollups;");
        }

        private static void CreateContinuousAggregates(MigrationBuilder migrationBuilder)
        {
            CreateEventAggregate(migrationBuilder, "network_event_quality_hourly", "schedule_type, transport_type, is_replacement");
            CreateEventDelayDistributionAggregate(migrationBuilder);
            CreateEventAggregate(migrationBuilder, "station_event_quality_hourly", "station_eva_number, schedule_type, transport_type, is_replacement");
            CreateEventAggregate(migrationBuilder, "station_administration_quality_hourly", "station_eva_number, schedule_type, administration_id, transport_type, is_replacement");
            CreateEventAggregate(migrationBuilder, "line_event_quality_hourly", "journey_description, schedule_type, transport_type, is_replacement, origin_eva_number, destination_eva_number, administration_id");
            CreateEventAggregate(migrationBuilder, "station_line_quality_hourly", "station_eva_number, schedule_type, journey_description, transport_type, is_replacement, origin_eva_number, destination_eva_number, administration_id");

            CreateJourneyAggregate(migrationBuilder, "network_journey_quality_hourly", "transport_type, is_replacement");
            CreateJourneyAggregate(migrationBuilder, "journey_administration_quality_hourly", "administration_id, transport_type, is_replacement");
            CreateJourneyAggregate(migrationBuilder, "line_journey_quality_hourly", "journey_description, transport_type, is_replacement, origin_eva_number, destination_eva_number, administration_id");
            CreateJourneyAggregate(migrationBuilder, "journey_number_quality_hourly", "journey_number, journey_description, transport_type, is_replacement, origin_eva_number, destination_eva_number, administration_id");
            CreateJourneyOutcomeAggregate(migrationBuilder);
        }

        private static void CreateEventAggregate(MigrationBuilder migrationBuilder, string name, string dimensions)
        {
            migrationBuilder.Sql($@"
                CREATE MATERIALIZED VIEW statistics.{name}
                WITH (timescaledb.continuous) AS
                SELECT
                    time_bucket(INTERVAL '1 hour', bucket_hour) AS bucket_hour,
                    {dimensions},
                    SUM(event_count)::bigint AS event_count,
                    SUM(cancelled_count)::bigint AS stop_cancelled_count,
                    SUM(delay_sum_seconds)::bigint AS event_delay_sum_seconds,
                    SUM(positive_delay_sum_seconds)::bigint AS event_positive_delay_sum_seconds,
                    SUM(punctual_5_count)::bigint AS event_punctual_5_count,
                    SUM(punctual_15_count)::bigint AS event_punctual_15_count,
                    SUM(late_30_count)::bigint AS event_late_30_count,
                    SUM(late_60_count)::bigint AS event_late_60_count
                FROM statistics.event_quality_hourly_rollups
                GROUP BY time_bucket(INTERVAL '1 hour', bucket_hour), {dimensions}
                WITH NO DATA;
            ");

            migrationBuilder.Sql($@"CREATE INDEX IX_{name}_window ON statistics.{name} ({dimensions}, bucket_hour);");
        }

        private static void CreateEventDelayDistributionAggregate(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE MATERIALIZED VIEW statistics.network_event_delay_distribution_hourly
                WITH (timescaledb.continuous) AS
                SELECT
                    time_bucket(INTERVAL '1 hour', bucket_hour) AS bucket_hour,
                    station_eva_number,
                    schedule_type,
                    transport_type,
                    is_replacement,
                    SUM(delay_lt_minus_5_count)::bigint AS delay_lt_minus_5_count,
                    SUM(delay_gte_minus_5_lt_0_count)::bigint AS delay_gte_minus_5_lt_0_count,
                    SUM(delay_gte_0_lt_5_count)::bigint AS delay_gte_0_lt_5_count,
                    SUM(delay_gte_5_lt_10_count)::bigint AS delay_gte_5_lt_10_count,
                    SUM(delay_gte_10_lt_15_count)::bigint AS delay_gte_10_lt_15_count,
                    SUM(delay_gte_15_lt_30_count)::bigint AS delay_gte_15_lt_30_count,
                    SUM(delay_gte_30_lt_60_count)::bigint AS delay_gte_30_lt_60_count,
                    SUM(delay_gte_60_lt_120_count)::bigint AS delay_gte_60_lt_120_count,
                    SUM(delay_gte_120_count)::bigint AS delay_gte_120_count
                FROM statistics.event_quality_hourly_rollups
                GROUP BY
                    time_bucket(INTERVAL '1 hour', bucket_hour),
                    station_eva_number,
                    schedule_type,
                    transport_type,
                    is_replacement
                WITH NO DATA;
            ");
        }

        private static void CreateJourneyAggregate(MigrationBuilder migrationBuilder, string name, string dimensions)
        {
            migrationBuilder.Sql($@"
                CREATE MATERIALIZED VIEW statistics.{name}
                WITH (timescaledb.continuous) AS
                SELECT
                    time_bucket(INTERVAL '1 hour', bucket_hour) AS bucket_hour,
                    {dimensions},
                    SUM(journey_count)::bigint AS journey_count,
                    SUM(fully_cancelled_count)::bigint AS fully_cancelled_count,
                    SUM(partially_cancelled_count)::bigint AS partially_cancelled_count,
                    SUM(destination_not_reached_count)::bigint AS destination_not_reached_count,
                    SUM(destination_delay_sum_seconds)::bigint AS destination_delay_sum_seconds,
                    SUM(destination_positive_delay_sum_seconds)::bigint AS destination_positive_delay_sum_seconds,
                    SUM(destination_punctual_5_count)::bigint AS destination_punctual_5_count,
                    SUM(destination_punctual_15_count)::bigint AS destination_punctual_15_count,
                    SUM(destination_late_30_count)::bigint AS destination_late_30_count,
                    SUM(destination_late_60_count)::bigint AS destination_late_60_count
                FROM statistics.journey_quality_hourly_rollups
                GROUP BY time_bucket(INTERVAL '1 hour', bucket_hour), {dimensions}
                WITH NO DATA;
            ");

            migrationBuilder.Sql($@"CREATE INDEX IX_{name}_window ON statistics.{name} ({dimensions}, bucket_hour);");
        }

        private static void CreateJourneyOutcomeAggregate(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE MATERIALIZED VIEW statistics.network_journey_outcome_hourly
                WITH (timescaledb.continuous) AS
                SELECT
                    time_bucket(INTERVAL '1 hour', bucket_hour) AS bucket_hour,
                    administration_id,
                    transport_type,
                    is_replacement,
                    SUM(journey_count)::bigint AS journey_count,
                    SUM(completed_count)::bigint AS completed_count,
                    SUM(partially_cancelled_destination_reached_count)::bigint AS partially_cancelled_destination_reached_count,
                    SUM(destination_not_reached_without_full_cancel_count)::bigint AS destination_not_reached_without_full_cancel_count,
                    SUM(fully_cancelled_count)::bigint AS fully_cancelled_count
                FROM statistics.journey_quality_hourly_rollups
                GROUP BY
                    time_bucket(INTERVAL '1 hour', bucket_hour),
                    administration_id,
                    transport_type,
                    is_replacement
                WITH NO DATA;
            ");

            migrationBuilder.Sql(@"CREATE INDEX IX_network_journey_outcome_hourly_window ON statistics.network_journey_outcome_hourly (administration_id, transport_type, is_replacement, bucket_hour);");
        }

        private static void CreateContinuousAggregatePolicies(MigrationBuilder migrationBuilder)
        {
            foreach (var continuousAggregate in ContinuousAggregates)
            {
                migrationBuilder.Sql($@"
                    SELECT add_continuous_aggregate_policy(
                        'statistics.{continuousAggregate}',
                        start_offset => INTERVAL '7 days',
                        end_offset => INTERVAL '1 hour',
                        schedule_interval => INTERVAL '1 hour',
                        if_not_exists => TRUE
                    );
                ");
            }
        }
    }
}
