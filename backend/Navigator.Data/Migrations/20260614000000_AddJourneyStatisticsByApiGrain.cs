using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Navigator.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddJourneyStatisticsByApiGrain : Migration
    {
        private static readonly string[] ContinuousAggregates =
        [
            "network_event_quality_hourly",
            "network_event_delay_distribution_hourly",
            "network_journey_quality_hourly",
            "station_event_quality_hourly",
            "station_administration_quality_hourly",
            "line_event_quality_hourly",
            "station_line_quality_hourly",
            "journey_administration_quality_hourly",
            "line_journey_quality_hourly",
            "journey_number_quality_hourly"
        ];

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            #region Fact tables
            migrationBuilder.Sql(@"
                CREATE TABLE statistics.journey_event_quality_facts (
                    bucket_hour timestamp with time zone NOT NULL,
                    stop_place_id uuid NOT NULL,
                    journey_id character varying(82) NOT NULL,
                    journey_date date NOT NULL,
                    station_eva_number integer NOT NULL,
                    planned_time timestamp with time zone NOT NULL,
                    schedule_type core.schedule_type NOT NULL,
                    administration_id uuid NOT NULL,
                    journey_number integer NOT NULL,
                    journey_description character varying(64) NOT NULL,
                    origin_eva_number integer NOT NULL,
                    journey_start_time timestamp with time zone NOT NULL,
                    destination_eva_number integer NOT NULL,
                    journey_end_time timestamp with time zone NOT NULL,
                    stop_cancelled boolean NOT NULL,
                    transport_type core.transport_type NOT NULL,
                    event_delay_seconds integer NOT NULL,
                    is_replacement boolean NOT NULL,
                    CONSTRAINT PK_journey_event_quality_facts PRIMARY KEY (stop_place_id, bucket_hour)
                );
            ");

            migrationBuilder.Sql(@"SELECT create_hypertable('statistics.journey_event_quality_facts', 'bucket_hour', chunk_time_interval => INTERVAL '14 days', if_not_exists => TRUE);");

            migrationBuilder.Sql(@"
                CREATE INDEX IX_journey_event_quality_facts_station_detail
                    ON statistics.journey_event_quality_facts (
                        station_eva_number,
                        planned_time,
                        schedule_type,
                        transport_type,
                        is_replacement,
                        journey_number,
                        origin_eva_number,
                        destination_eva_number
                    );
            ");

            migrationBuilder.Sql(@"
                CREATE INDEX IX_journey_event_quality_facts_journey_detail
                    ON statistics.journey_event_quality_facts (
                        journey_number,
                        planned_time,
                        schedule_type,
                        transport_type,
                        is_replacement,
                        origin_eva_number,
                        destination_eva_number,
                        journey_description
                    );
            ");

            migrationBuilder.Sql(@"
                CREATE TABLE statistics.journey_quality_facts (
                    bucket_hour timestamp with time zone NOT NULL,
                    journey_id character varying(82) NOT NULL,
                    journey_date date NOT NULL,
                    administration_id uuid NOT NULL,
                    journey_number integer NOT NULL,
                    journey_description character varying(64) NOT NULL,
                    origin_eva_number integer NOT NULL,
                    journey_start_time timestamp with time zone NOT NULL,
                    destination_eva_number integer NOT NULL,
                    journey_end_time timestamp with time zone NOT NULL,
                    transport_type core.transport_type NOT NULL,
                    destination_delay_seconds integer,
                    is_replacement boolean NOT NULL,
                    fully_cancelled boolean NOT NULL,
                    partially_cancelled boolean NOT NULL,
                    destination_not_reached boolean NOT NULL,
                    CONSTRAINT PK_journey_quality_facts PRIMARY KEY (journey_id, journey_date, bucket_hour)
                );
            ");

            migrationBuilder.Sql(@"SELECT create_hypertable('statistics.journey_quality_facts', 'bucket_hour', chunk_time_interval => INTERVAL '14 days', if_not_exists => TRUE);");

            migrationBuilder.Sql(@"
                CREATE INDEX IX_journey_quality_facts_journey_detail
                    ON statistics.journey_quality_facts (
                        journey_number,
                        journey_start_time,
                        transport_type,
                        is_replacement,
                        origin_eva_number,
                        destination_eva_number,
                        administration_id,
                        journey_description
                    );
            ");
            #endregion

            #region Detail views
            migrationBuilder.Sql(@"
                CREATE VIEW statistics.station_journey_event_details AS
                SELECT
                    bucket_hour,
                    stop_place_id,
                    journey_id,
                    journey_date,
                    station_eva_number,
                    planned_time,
                    schedule_type,
                    administration_id,
                    journey_number,
                    journey_description,
                    origin_eva_number,
                    journey_start_time,
                    destination_eva_number,
                    journey_end_time,
                    stop_cancelled,
                    transport_type,
                    event_delay_seconds,
                    is_replacement
                FROM statistics.journey_event_quality_facts;
            ");

            migrationBuilder.Sql(@"
                CREATE VIEW statistics.journey_quality_details AS
                SELECT
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
                FROM statistics.journey_quality_facts;
            ");
            #endregion

            #region Continuous Aggregates
            CreateEventAggregate(
                migrationBuilder,
                "network_event_quality_hourly",
                "schedule_type, transport_type, is_replacement");

            CreateEventDelayDistributionAggregate(
                migrationBuilder,
                "network_event_delay_distribution_hourly");

            CreateEventAggregate(
                migrationBuilder,
                "station_event_quality_hourly",
                "station_eva_number, schedule_type, transport_type, is_replacement");

            CreateEventAggregate(
                migrationBuilder,
                "station_administration_quality_hourly",
                "station_eva_number, schedule_type, administration_id, transport_type, is_replacement");

            CreateEventAggregate(
                migrationBuilder,
                "line_event_quality_hourly",
                "journey_description, schedule_type, transport_type, is_replacement, origin_eva_number, destination_eva_number, administration_id");

            CreateEventAggregate(
                migrationBuilder,
                "station_line_quality_hourly",
                "station_eva_number, schedule_type, journey_description, transport_type, is_replacement, origin_eva_number, destination_eva_number, administration_id");

            CreateJourneyAggregate(
                migrationBuilder,
                "network_journey_quality_hourly",
                "transport_type, is_replacement");

            CreateJourneyAggregate(
                migrationBuilder,
                "journey_administration_quality_hourly",
                "administration_id, transport_type, is_replacement");

            CreateJourneyAggregate(
                migrationBuilder,
                "line_journey_quality_hourly",
                "journey_description, transport_type, is_replacement, origin_eva_number, destination_eva_number, administration_id");

            CreateJourneyAggregate(
                migrationBuilder,
                "journey_number_quality_hourly",
                "journey_number, journey_description, transport_type, is_replacement, origin_eva_number, destination_eva_number, administration_id");

            foreach (var continuousAggregate in ContinuousAggregates)
            {
                migrationBuilder.Sql($@"
                    SELECT add_continuous_aggregate_policy('statistics.{continuousAggregate}',
                        start_offset => INTERVAL '210 days',
                        end_offset => INTERVAL '1 hour',
                        schedule_interval => INTERVAL '1 hour',
                        if_not_exists => TRUE);
                ");
            }
            #endregion

            #region Projection backlog
            migrationBuilder.Sql(@"
                CREATE TABLE statistics.journey_fact_projection_backlog (
                    journey_id character varying(82) NOT NULL,
                    date date NOT NULL,
                    created_at timestamp with time zone NOT NULL,
                    available_at timestamp with time zone NOT NULL,
                    attempts integer NOT NULL,
                    last_attempt_at timestamp with time zone,
                    locked_until timestamp with time zone,
                    locked_by character varying(128),
                    last_error character varying(2048),
                    dead_lettered_at timestamp with time zone,
                    CONSTRAINT PK_journey_fact_projection_backlog PRIMARY KEY (journey_id, date)
                );
            ");

            migrationBuilder.Sql(@"
                CREATE INDEX IX_journey_fact_projection_backlog_claimable
                    ON statistics.journey_fact_projection_backlog (available_at, created_at)
                    INCLUDE (locked_until, attempts)
                    WHERE dead_lettered_at IS NULL;
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
                                  FROM statistics.journey_quality_facts AS journey_facts
                                  WHERE journey_facts.journey_id = journeys.id
                                    AND journey_facts.journey_date = journeys.date
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

            migrationBuilder.Sql(@"
                CREATE TRIGGER TRG_journey_fact_projection_backlog_notify
                AFTER INSERT OR UPDATE OF available_at
                ON statistics.journey_fact_projection_backlog
                FOR EACH STATEMENT
                EXECUTE FUNCTION statistics.notify_journey_fact_projection_backlog();
            ");
            #endregion

            #region Compression
            migrationBuilder.Sql(@"
                ALTER TABLE statistics.journey_event_quality_facts SET (
                    timescaledb.compress,
                    timescaledb.compress_segmentby = 'station_eva_number, transport_type',
                    timescaledb.compress_orderby = 'bucket_hour, planned_time'
                );
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE statistics.journey_quality_facts SET (
                    timescaledb.compress,
                    timescaledb.compress_segmentby = 'transport_type',
                    timescaledb.compress_orderby = 'bucket_hour, journey_start_time'
                );
            ");

            migrationBuilder.Sql(@"SELECT add_compression_policy('statistics.journey_event_quality_facts', INTERVAL '240 days', if_not_exists => TRUE);");
            migrationBuilder.Sql(@"SELECT add_compression_policy('statistics.journey_quality_facts', INTERVAL '240 days', if_not_exists => TRUE);");
            #endregion
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            foreach (var continuousAggregate in ContinuousAggregates)
            {
                migrationBuilder.Sql($@"SELECT remove_continuous_aggregate_policy('statistics.{continuousAggregate}', if_exists => TRUE);");
            }

            migrationBuilder.Sql(@"SELECT remove_compression_policy('statistics.journey_quality_facts', if_exists => TRUE);");
            migrationBuilder.Sql(@"SELECT remove_compression_policy('statistics.journey_event_quality_facts', if_exists => TRUE);");

            foreach (var continuousAggregate in ContinuousAggregates)
            {
                migrationBuilder.Sql($@"DROP MATERIALIZED VIEW IF EXISTS statistics.{continuousAggregate};");
            }

            migrationBuilder.Sql(@"DROP VIEW IF EXISTS statistics.journey_quality_details;");
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS statistics.station_journey_event_details;");

            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS TRG_journey_fact_projection_backlog_notify ON statistics.journey_fact_projection_backlog;");

            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS statistics.enqueue_missing_journey_fact_projections(date, date, boolean);");
            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS statistics.notify_journey_fact_projection_backlog();");

            migrationBuilder.Sql(@"DROP TABLE IF EXISTS statistics.journey_fact_projection_backlog;");

            migrationBuilder.Sql(@"DROP TABLE IF EXISTS statistics.journey_quality_facts CASCADE;");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS statistics.journey_event_quality_facts CASCADE;");
        }

        private static void CreateEventAggregate(MigrationBuilder migrationBuilder, string name, string dimensions)
        {
            migrationBuilder.Sql($@"
                CREATE MATERIALIZED VIEW statistics.{name}
                WITH (timescaledb.continuous) AS
                SELECT
                    time_bucket(INTERVAL '1 hour', bucket_hour) AS bucket_hour,
                    {dimensions},
                    count(*)::bigint AS event_count,
                    count(*) FILTER (WHERE stop_cancelled IS TRUE)::bigint AS stop_cancelled_count,
                    coalesce(sum(event_delay_seconds) FILTER (WHERE stop_cancelled IS NOT TRUE), 0)::bigint AS event_delay_sum_seconds,
                    coalesce(sum(GREATEST(event_delay_seconds, 0)) FILTER (WHERE stop_cancelled IS NOT TRUE), 0)::bigint AS event_positive_delay_sum_seconds,
                    count(*) FILTER (WHERE stop_cancelled IS NOT TRUE AND event_delay_seconds < 360)::bigint AS event_punctual_5_count,
                    count(*) FILTER (WHERE stop_cancelled IS NOT TRUE AND event_delay_seconds < 900)::bigint AS event_punctual_15_count,
                    count(*) FILTER (WHERE stop_cancelled IS NOT TRUE AND event_delay_seconds >= 1800)::bigint AS event_late_30_count,
                    count(*) FILTER (WHERE stop_cancelled IS NOT TRUE AND event_delay_seconds >= 3600)::bigint AS event_late_60_count
                FROM statistics.journey_event_quality_facts
                GROUP BY time_bucket(INTERVAL '1 hour', bucket_hour), {dimensions}
                WITH NO DATA;
            ");

            migrationBuilder.Sql($@"CREATE INDEX IX_{name}_window ON statistics.{name} ({dimensions}, bucket_hour);");
        }

        private static void CreateEventDelayDistributionAggregate(MigrationBuilder migrationBuilder, string name)
        {
            migrationBuilder.Sql($@"
                CREATE MATERIALIZED VIEW statistics.{name}
                WITH (timescaledb.continuous) AS
                SELECT
                    time_bucket(INTERVAL '1 hour', bucket_hour) AS bucket_hour,
                    station_eva_number,
                    schedule_type,
                    transport_type,
                    is_replacement,
                    count(*) FILTER (WHERE stop_cancelled IS NOT TRUE AND event_delay_seconds < -300)::bigint AS delay_lt_minus_5_count,
                    count(*) FILTER (WHERE stop_cancelled IS NOT TRUE AND event_delay_seconds >= -300 AND event_delay_seconds < 0)::bigint AS delay_gte_minus_5_lt_0_count,
                    count(*) FILTER (WHERE stop_cancelled IS NOT TRUE AND event_delay_seconds >= 0 AND event_delay_seconds < 300)::bigint AS delay_gte_0_lt_5_count,
                    count(*) FILTER (WHERE stop_cancelled IS NOT TRUE AND event_delay_seconds >= 300 AND event_delay_seconds < 600)::bigint AS delay_gte_5_lt_10_count,
                    count(*) FILTER (WHERE stop_cancelled IS NOT TRUE AND event_delay_seconds >= 600 AND event_delay_seconds < 900)::bigint AS delay_gte_10_lt_15_count,
                    count(*) FILTER (WHERE stop_cancelled IS NOT TRUE AND event_delay_seconds >= 900 AND event_delay_seconds < 1800)::bigint AS delay_gte_15_lt_30_count,
                    count(*) FILTER (WHERE stop_cancelled IS NOT TRUE AND event_delay_seconds >= 1800 AND event_delay_seconds < 3600)::bigint AS delay_gte_30_lt_60_count,
                    count(*) FILTER (WHERE stop_cancelled IS NOT TRUE AND event_delay_seconds >= 3600 AND event_delay_seconds < 7200)::bigint AS delay_gte_60_lt_120_count,
                    count(*) FILTER (WHERE stop_cancelled IS NOT TRUE AND event_delay_seconds >= 7200)::bigint AS delay_gte_120_count
                FROM statistics.journey_event_quality_facts
                GROUP BY
                    time_bucket(INTERVAL '1 hour', bucket_hour),
                    station_eva_number,
                    schedule_type,
                    transport_type,
                    is_replacement
                WITH NO DATA;
            ");

            migrationBuilder.Sql($@"
                CREATE INDEX IX_{name}_window
                    ON statistics.{name} (
                        schedule_type,
                        transport_type,
                        is_replacement,
                        bucket_hour
                    );
            ");

            migrationBuilder.Sql($@"
                CREATE INDEX IX_{name}_station_window
                    ON statistics.{name} (
                        station_eva_number,
                        schedule_type,
                        transport_type,
                        is_replacement,
                        bucket_hour
                    );
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
                    count(*)::bigint AS journey_count,
                    count(*) FILTER (WHERE fully_cancelled IS TRUE)::bigint AS fully_cancelled_count,
                    count(*) FILTER (WHERE partially_cancelled IS TRUE)::bigint AS partially_cancelled_count,
                    count(*) FILTER (WHERE destination_not_reached IS TRUE)::bigint AS destination_not_reached_count,
                    coalesce(sum(destination_delay_seconds) FILTER (WHERE destination_delay_seconds IS NOT NULL), 0)::bigint AS destination_delay_sum_seconds,
                    coalesce(sum(GREATEST(destination_delay_seconds, 0)) FILTER (WHERE destination_delay_seconds IS NOT NULL), 0)::bigint AS destination_positive_delay_sum_seconds,
                    count(*) FILTER (WHERE destination_delay_seconds IS NOT NULL AND destination_delay_seconds < 360)::bigint AS destination_punctual_5_count,
                    count(*) FILTER (WHERE destination_delay_seconds IS NOT NULL AND destination_delay_seconds < 900)::bigint AS destination_punctual_15_count,
                    count(*) FILTER (WHERE destination_delay_seconds IS NOT NULL AND destination_delay_seconds >= 1800)::bigint AS destination_late_30_count,
                    count(*) FILTER (WHERE destination_delay_seconds IS NOT NULL AND destination_delay_seconds >= 3600)::bigint AS destination_late_60_count
                FROM statistics.journey_quality_facts
                GROUP BY time_bucket(INTERVAL '1 hour', bucket_hour), {dimensions}
                WITH NO DATA;
            ");

            migrationBuilder.Sql($@"CREATE INDEX IX_{name}_window ON statistics.{name} ({dimensions}, bucket_hour);");
        }
    }
}
