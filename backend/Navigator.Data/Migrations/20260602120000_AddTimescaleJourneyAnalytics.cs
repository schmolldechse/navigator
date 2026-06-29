using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Navigator.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTimescaleJourneyAnalytics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE EXTENSION IF NOT EXISTS timescaledb;");

            #region Journey Administration
            migrationBuilder.Sql(@"CREATE TABLE IF NOT EXISTS core.journey_administrations (
                            id uuid NOT NULL DEFAULT gen_random_uuid(),
                            administration_id character varying(32) NOT NULL,
                            operator_code character varying(32) NOT NULL,
                            operator_name character varying(128) NOT NULL,
                            CONSTRAINT PK_journey_administrations PRIMARY KEY (id)
                        );");

            migrationBuilder.Sql(@"CREATE UNIQUE INDEX IF NOT EXISTS IX_journey_administrations_unique
                        ON core.journey_administrations (administration_id, operator_code, operator_name);");
            #endregion

            #region Journey Raw Hypertables
            migrationBuilder.Sql(@"CREATE TABLE IF NOT EXISTS core.journeys (
                            id character varying(82) NOT NULL,
                            date date NOT NULL,
                            inserted_at timestamp with time zone NOT NULL DEFAULT now(),
                            administration_id uuid NOT NULL,
                            cancelled boolean NOT NULL,
                            journey_type core.journey_type NOT NULL,
                            CONSTRAINT PK_journeys PRIMARY KEY (id, date),
                            CONSTRAINT FK_journeys_administrations FOREIGN KEY (administration_id) REFERENCES core.journey_administrations (id) ON DELETE RESTRICT
                        );");

            migrationBuilder.Sql(@"SELECT create_hypertable('core.journeys', 'date', chunk_time_interval => INTERVAL '14 days', if_not_exists => TRUE);");

            migrationBuilder.Sql(@"CREATE TABLE IF NOT EXISTS core.journey_transports (
                            journey_id character varying(82) NOT NULL,
                            date date NOT NULL,
                            transport_type core.transport_type NOT NULL,
                            replacement_transport_type core.transport_type,
                            category character varying(64) NOT NULL,
                            category_internal character varying(64) NOT NULL,
                            journey_description character varying(64) NOT NULL,
                            label character varying(64) NOT NULL,
                            line text,
                            number integer NOT NULL,
                            CONSTRAINT PK_journey_transports PRIMARY KEY (journey_id, date)
                        );");

            migrationBuilder.Sql(@"SELECT create_hypertable('core.journey_transports', 'date', chunk_time_interval => INTERVAL '14 days', if_not_exists => TRUE);");

            migrationBuilder.Sql(@"CREATE TABLE IF NOT EXISTS core.journey_stop_places (
                            id uuid NOT NULL DEFAULT gen_random_uuid(),
                            journey_id character varying(82) NOT NULL,
                            date date NOT NULL,
                            schedule_type core.schedule_type NOT NULL,
                            station_eva_number integer NOT NULL,
                            cancelled boolean NOT NULL,
                            additional boolean NOT NULL,
                            demand boolean NOT NULL,
                            no_passenger_change boolean NOT NULL,
                            planned_time timestamp with time zone NOT NULL,
                            actual_time timestamp with time zone NOT NULL,
                            time_type core.time_type NOT NULL,
                            delay integer GENERATED ALWAYS AS (EXTRACT(EPOCH FROM (actual_time - planned_time))::integer) STORED,
                            planned_platform character varying(32),
                            actual_platform character varying(32),
                            CONSTRAINT PK_journey_stop_places PRIMARY KEY (id, date)
                        );");

            migrationBuilder.Sql(@"SELECT create_hypertable('core.journey_stop_places', 'date', chunk_time_interval => INTERVAL '14 days', if_not_exists => TRUE);");
            #endregion

            #region Journey Raw Indexes
            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS IX_journey_stop_places_station_date_planned_time
                        ON core.journey_stop_places (station_eva_number, date, planned_time)
                        INCLUDE (id, journey_id, schedule_type, cancelled, delay);");

            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS IX_journey_stop_places_journey_date_planned_time
                        ON core.journey_stop_places (journey_id, date, planned_time)
                        INCLUDE (station_eva_number, schedule_type, cancelled, delay);");

            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS IX_journey_transports_journey_description_number_date
                        ON core.journey_transports (journey_description, number, date)
                        INCLUDE (journey_id, transport_type, replacement_transport_type);");
            #endregion

            #region Statistics Fact Hypertables
            migrationBuilder.Sql(@"CREATE TABLE IF NOT EXISTS statistics.journey_event_quality_facts (
                            stop_place_id uuid NOT NULL,
                            journey_id character varying(82) NOT NULL,
                            date date NOT NULL,
                            planned_time timestamp with time zone NOT NULL,
                            journey_start_time timestamp with time zone NOT NULL,
                            journey_end_time timestamp with time zone NOT NULL,
                            station_eva_number integer NOT NULL,
                            schedule_type core.schedule_type NOT NULL,
                            administration_id uuid NOT NULL,
                            transport_type core.transport_type NOT NULL,
                            journey_description character varying(64) NOT NULL,
                            number integer NOT NULL,
                            is_replacement_transport boolean NOT NULL,
                            origin_eva_number integer NOT NULL,
                            destination_eva_number integer NOT NULL,
                            cancelled boolean NOT NULL,
                            delay integer NOT NULL,
                            CONSTRAINT PK_journey_event_quality_facts PRIMARY KEY (stop_place_id, planned_time)
                        );");

            migrationBuilder.Sql(@"SELECT create_hypertable('statistics.journey_event_quality_facts', 'planned_time', chunk_time_interval => INTERVAL '14 days', if_not_exists => TRUE);");

            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS IX_journey_event_quality_facts_station_time
                        ON statistics.journey_event_quality_facts (station_eva_number, planned_time, transport_type, is_replacement_transport);");

            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS IX_journey_event_quality_facts_time_administration
                        ON statistics.journey_event_quality_facts (planned_time, administration_id, transport_type, is_replacement_transport);");

            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS IX_journey_event_quality_facts_line_route
                        ON statistics.journey_event_quality_facts (
                            station_eva_number,
                            planned_time,
                            schedule_type,
                            journey_description,
                            number,
                            transport_type,
                            is_replacement_transport
                        );");

            migrationBuilder.Sql(@"CREATE TABLE IF NOT EXISTS statistics.journey_route_quality_facts (
                            journey_id character varying(82) NOT NULL,
                            date date NOT NULL,
                            journey_start_time timestamp with time zone NOT NULL,
                            journey_end_time timestamp with time zone NOT NULL,
                            administration_id uuid NOT NULL,
                            transport_type core.transport_type NOT NULL,
                            journey_description character varying(64) NOT NULL,
                            number integer NOT NULL,
                            is_replacement_transport boolean NOT NULL,
                            origin_eva_number integer NOT NULL,
                            destination_eva_number integer NOT NULL,
                            journey_cancelled boolean NOT NULL,
                            terminal_delay_seconds integer,
                            CONSTRAINT PK_journey_route_quality_facts PRIMARY KEY (journey_id, date, journey_start_time)
                        );");

            migrationBuilder.Sql(@"SELECT create_hypertable('statistics.journey_route_quality_facts', 'journey_start_time', chunk_time_interval => INTERVAL '14 days', if_not_exists => TRUE);");

            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS IX_journey_route_quality_facts_time_administration
                        ON statistics.journey_route_quality_facts (journey_start_time, administration_id, transport_type, is_replacement_transport);");

            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS IX_journey_route_quality_facts_line_route
                        ON statistics.journey_route_quality_facts (journey_description, number, journey_start_time, transport_type, is_replacement_transport);");
            #endregion

            #region Statistics Projection Backlog
            migrationBuilder.Sql(@"CREATE TABLE IF NOT EXISTS statistics.journey_fact_projection_backlog (
                            journey_id character varying(82) NOT NULL,
                            date date NOT NULL,
                            created_at timestamp with time zone NOT NULL DEFAULT now(),
                            available_at timestamp with time zone NOT NULL DEFAULT now(),
                            attempts integer NOT NULL DEFAULT 0,
                            last_attempt_at timestamp with time zone,
                            locked_until timestamp with time zone,
                            locked_by character varying(128),
                            last_error character varying(2048),
                            dead_lettered_at timestamp with time zone,
                            CONSTRAINT PK_journey_fact_projection_backlog PRIMARY KEY (journey_id, date)
                        );");

            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS IX_journey_fact_projection_backlog_available
                        ON statistics.journey_fact_projection_backlog (available_at, created_at)
                        WHERE dead_lettered_at IS NULL;");

            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS IX_journey_fact_projection_backlog_dead_lettered
                        ON statistics.journey_fact_projection_backlog (dead_lettered_at)
                        WHERE dead_lettered_at IS NOT NULL;");

            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION statistics.notify_journey_fact_projection_backlog()
                RETURNS trigger
                LANGUAGE plpgsql
                AS $$
                BEGIN
                    PERFORM pg_notify('journey_fact_projection_backlog', '');
                    RETURN NULL;
                END;
                $$;
            ");

            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS TRG_journey_fact_projection_backlog_notify ON statistics.journey_fact_projection_backlog;");

            migrationBuilder.Sql(@"
                CREATE TRIGGER TRG_journey_fact_projection_backlog_notify
                AFTER INSERT OR UPDATE OF available_at
                ON statistics.journey_fact_projection_backlog
                FOR EACH STATEMENT
                EXECUTE FUNCTION statistics.notify_journey_fact_projection_backlog();
            ");

            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION statistics.enqueue_missing_journey_fact_projections(
                    p_from_date date DEFAULT NULL,
                    p_to_date date DEFAULT NULL,
                    p_reset_failed boolean DEFAULT FALSE
                )
                RETURNS integer
                LANGUAGE plpgsql
                AS $$
                DECLARE
                    enqueued_count integer;
                BEGIN
                    WITH candidates AS (
                        SELECT journeys.id, journeys.date
                        FROM core.journeys AS journeys
                        WHERE (p_from_date IS NULL OR journeys.date >= p_from_date)
                          AND (p_to_date IS NULL OR journeys.date < p_to_date)
                          AND EXISTS (
                              SELECT 1
                              FROM core.journey_stop_places AS stop_places
                              WHERE stop_places.journey_id = journeys.id
                                AND stop_places.date = journeys.date
                          )
                          AND (
                              NOT EXISTS (
                                  SELECT 1
                                  FROM statistics.journey_route_quality_facts AS route_facts
                                  WHERE route_facts.journey_id = journeys.id
                                    AND route_facts.date = journeys.date
                              )
                              OR EXISTS (
                                  SELECT 1
                                  FROM core.journey_stop_places AS stop_places
                                  WHERE stop_places.journey_id = journeys.id
                                    AND stop_places.date = journeys.date
                                    AND NOT EXISTS (
                                        SELECT 1
                                        FROM statistics.journey_event_quality_facts AS event_facts
                                        WHERE event_facts.stop_place_id = stop_places.id
                                          AND event_facts.planned_time = stop_places.planned_time
                                    )
                              )
                          )
                    ),
                    upserted AS (
                        INSERT INTO statistics.journey_fact_projection_backlog (
                            journey_id,
                            date,
                            created_at,
                            available_at,
                            attempts,
                            last_attempt_at,
                            locked_until,
                            locked_by,
                            last_error,
                            dead_lettered_at
                        )
                        SELECT
                            candidates.id,
                            candidates.date,
                            now(),
                            now(),
                            0,
                            NULL,
                            NULL,
                            NULL,
                            NULL,
                            NULL
                        FROM candidates
                        ON CONFLICT (journey_id, date) DO UPDATE
                        SET
                            available_at = now(),
                            attempts = CASE
                                WHEN p_reset_failed THEN 0
                                ELSE statistics.journey_fact_projection_backlog.attempts
                            END,
                            last_attempt_at = CASE
                                WHEN p_reset_failed THEN NULL
                                ELSE statistics.journey_fact_projection_backlog.last_attempt_at
                            END,
                            locked_until = NULL,
                            locked_by = NULL,
                            last_error = CASE
                                WHEN p_reset_failed THEN NULL
                                ELSE statistics.journey_fact_projection_backlog.last_error
                            END,
                            dead_lettered_at = CASE
                                WHEN p_reset_failed THEN NULL
                                ELSE statistics.journey_fact_projection_backlog.dead_lettered_at
                            END
                        WHERE p_reset_failed
                           OR statistics.journey_fact_projection_backlog.dead_lettered_at IS NULL
                        RETURNING 1
                    )
                    SELECT count(*)::integer INTO enqueued_count
                    FROM upserted;

                    IF enqueued_count > 0 THEN
                        PERFORM pg_notify('journey_fact_projection_backlog', enqueued_count::text);
                    END IF;

                    RETURN enqueued_count;
                END;
                $$;
            ");
            #endregion

            #region Continuous Aggregates
            migrationBuilder.Sql(@"
                CREATE MATERIALIZED VIEW IF NOT EXISTS statistics.station_line_route_quality_hourly
                WITH (timescaledb.continuous) AS
                SELECT
                    time_bucket(INTERVAL '1 hour', planned_time) AS bucket_hour,
                    station_eva_number,
                    schedule_type,
                    administration_id,
                    transport_type,
                    journey_description,
                    number,
                    is_replacement_transport,
                    origin_eva_number,
                    destination_eva_number,
                    min(journey_start_time) AS first_planned_time,
                    max(journey_end_time) AS last_planned_time,
                    count(*)::bigint AS event_count,
                    count(*) FILTER (WHERE cancelled IS TRUE)::bigint AS cancelled_count,
                    count(*) FILTER (WHERE cancelled IS NOT TRUE)::bigint AS delay_sample_count,
                    coalesce(sum(delay) FILTER (WHERE cancelled IS NOT TRUE), 0)::bigint AS delay_sum_seconds,
                    count(*) FILTER (WHERE cancelled IS NOT TRUE AND delay < 360)::bigint AS punctual_5_count,
                    count(*) FILTER (WHERE cancelled IS NOT TRUE AND delay < 900)::bigint AS punctual_15_count
                FROM statistics.journey_event_quality_facts
                GROUP BY
                    bucket_hour,
                    station_eva_number,
                    schedule_type,
                    administration_id,
                    transport_type,
                    journey_description,
                    number,
                    is_replacement_transport,
                    origin_eva_number,
                    destination_eva_number
                WITH NO DATA;
            ");

            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS IX_station_line_route_quality_hourly_station
                        ON statistics.station_line_route_quality_hourly (
                            station_eva_number,
                            schedule_type,
                            transport_type,
                            is_replacement_transport,
                            bucket_hour
                        );");

            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS IX_station_line_route_quality_hourly_network
                        ON statistics.station_line_route_quality_hourly (
                            schedule_type,
                            transport_type,
                            is_replacement_transport,
                            bucket_hour
                        );");

            migrationBuilder.Sql(@"
                CREATE MATERIALIZED VIEW IF NOT EXISTS statistics.journey_route_quality_hourly
                WITH (timescaledb.continuous) AS
                SELECT
                    time_bucket(INTERVAL '1 hour', journey_start_time) AS bucket_hour,
                    administration_id,
                    transport_type,
                    journey_description,
                    number,
                    is_replacement_transport,
                    origin_eva_number,
                    destination_eva_number,
                    min(journey_start_time) AS first_planned_time,
                    max(journey_end_time) AS last_planned_time,
                    count(*)::bigint AS journey_count,
                    count(*) FILTER (WHERE journey_cancelled IS TRUE)::bigint AS journey_cancelled_count,
                    count(*) FILTER (WHERE journey_cancelled IS NOT TRUE AND terminal_delay_seconds IS NOT NULL)::bigint AS delay_sample_count,
                    coalesce(sum(terminal_delay_seconds) FILTER (
                        WHERE journey_cancelled IS NOT TRUE AND terminal_delay_seconds IS NOT NULL
                    ), 0)::bigint AS delay_sum_seconds,
                    count(*) FILTER (WHERE journey_cancelled IS NOT TRUE AND terminal_delay_seconds < 360)::bigint AS punctual_5_count,
                    count(*) FILTER (WHERE journey_cancelled IS NOT TRUE AND terminal_delay_seconds < 900)::bigint AS punctual_15_count
                FROM statistics.journey_route_quality_facts
                GROUP BY
                    bucket_hour,
                    administration_id,
                    transport_type,
                    journey_description,
                    number,
                    is_replacement_transport,
                    origin_eva_number,
                    destination_eva_number
                WITH NO DATA;
            ");

            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS IX_journey_route_quality_hourly_window
                        ON statistics.journey_route_quality_hourly (
                            transport_type,
                            is_replacement_transport,
                            bucket_hour
                        );");

            migrationBuilder.Sql(@"SELECT add_continuous_aggregate_policy('statistics.station_line_route_quality_hourly',
                        start_offset => INTERVAL '210 days',
                        end_offset => INTERVAL '1 hour',
                        schedule_interval => INTERVAL '1 hour',
                        if_not_exists => TRUE);");

            migrationBuilder.Sql(@"SELECT add_continuous_aggregate_policy('statistics.journey_route_quality_hourly',
                        start_offset => INTERVAL '210 days',
                        end_offset => INTERVAL '1 hour',
                        schedule_interval => INTERVAL '1 hour',
                        if_not_exists => TRUE);");
            #endregion

            #region Compression
            migrationBuilder.Sql(@"ALTER TABLE core.journey_stop_places SET (
                        timescaledb.compress,
                        timescaledb.compress_segmentby = 'station_eva_number',
                        timescaledb.compress_orderby = 'date, planned_time'
                    );");

            migrationBuilder.Sql(@"ALTER TABLE statistics.journey_event_quality_facts SET (
                        timescaledb.compress,
                        timescaledb.compress_segmentby = 'station_eva_number, transport_type',
                        timescaledb.compress_orderby = 'planned_time'
                    );");

            migrationBuilder.Sql(@"ALTER TABLE statistics.journey_route_quality_facts SET (
                        timescaledb.compress,
                        timescaledb.compress_segmentby = 'transport_type',
                        timescaledb.compress_orderby = 'journey_start_time'
                    );");

            migrationBuilder.Sql(@"SELECT add_compression_policy('core.journey_stop_places', INTERVAL '240 days', if_not_exists => TRUE);");
            migrationBuilder.Sql(@"SELECT add_compression_policy('statistics.journey_event_quality_facts', INTERVAL '240 days', if_not_exists => TRUE);");
            migrationBuilder.Sql(@"SELECT add_compression_policy('statistics.journey_route_quality_facts', INTERVAL '240 days', if_not_exists => TRUE);");
            #endregion
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"SELECT remove_continuous_aggregate_policy('statistics.station_line_route_quality_hourly', if_exists => TRUE);");
            migrationBuilder.Sql(@"SELECT remove_continuous_aggregate_policy('statistics.journey_route_quality_hourly', if_exists => TRUE);");

            migrationBuilder.Sql(@"DROP INDEX IF EXISTS _timescaledb_internal.IX_journey_route_quality_hourly_window;");
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS _timescaledb_internal.IX_station_line_route_quality_hourly_network;");
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS _timescaledb_internal.IX_station_line_route_quality_hourly_station;");

            migrationBuilder.Sql(@"DROP MATERIALIZED VIEW IF EXISTS statistics.journey_route_quality_hourly;");
            migrationBuilder.Sql(@"DROP MATERIALIZED VIEW IF EXISTS statistics.station_line_route_quality_hourly;");

            migrationBuilder.Sql(@"SELECT remove_compression_policy('statistics.journey_route_quality_facts', if_exists => TRUE);");
            migrationBuilder.Sql(@"SELECT remove_compression_policy('statistics.journey_event_quality_facts', if_exists => TRUE);");
            migrationBuilder.Sql(@"SELECT remove_compression_policy('core.journey_stop_places', if_exists => TRUE);");

            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS TRG_journey_fact_projection_backlog_notify ON statistics.journey_fact_projection_backlog;");
            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS statistics.enqueue_missing_journey_fact_projections(date, date, boolean);");
            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS statistics.notify_journey_fact_projection_backlog();");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS statistics.journey_fact_projection_backlog CASCADE;");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS statistics.journey_route_quality_facts CASCADE;");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS statistics.journey_event_quality_facts CASCADE;");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS core.journey_stop_places CASCADE;");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS core.journey_transports CASCADE;");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS core.journeys CASCADE;");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS core.journey_administrations CASCADE;");
        }
    }
}
