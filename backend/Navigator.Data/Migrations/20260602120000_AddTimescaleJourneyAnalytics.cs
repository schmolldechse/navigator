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

            migrationBuilder.Sql(@"CREATE TABLE IF NOT EXISTS core.journey_messages (
                            id uuid NOT NULL DEFAULT gen_random_uuid(),
                            journey_id character varying(82) NOT NULL,
                            date date NOT NULL,
                            message_type core.message_type NOT NULL,
                            code character varying(64),
                            text character varying(2048),
                            text_short character varying(2048),
                            disruption_cause character varying(128),
                            disruption_effect character varying(128),
                            note_category character varying(128),
                            CONSTRAINT PK_journey_messages PRIMARY KEY (id, date)
                        );");

            migrationBuilder.Sql(@"SELECT create_hypertable('core.journey_messages', 'date', chunk_time_interval => INTERVAL '1 month', if_not_exists => TRUE);");

            migrationBuilder.Sql(@"CREATE TABLE IF NOT EXISTS core.journey_stop_place_messages (
                            journey_stop_place_id uuid NOT NULL,
                            journey_message_id uuid NOT NULL,
                            date date NOT NULL,
                            CONSTRAINT PK_journey_stop_place_messages PRIMARY KEY (journey_stop_place_id, journey_message_id, date)
                        );");

            migrationBuilder.Sql(@"SELECT create_hypertable('core.journey_stop_place_messages', 'date', chunk_time_interval => INTERVAL '14 days', if_not_exists => TRUE);");
            #endregion

            #region Journey Raw Indexes
            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS IX_journey_stop_places_station_date_planned_time
                        ON core.journey_stop_places (station_eva_number, date, planned_time)
                        INCLUDE (id, journey_id, schedule_type, cancelled, delay);");

            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS IX_journey_stop_places_journey_date_planned_time
                        ON core.journey_stop_places (journey_id, date, planned_time)
                        INCLUDE (station_eva_number, schedule_type, cancelled, delay);");

            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS IX_journey_messages_journey_date
                        ON core.journey_messages (journey_id, date);");

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
                            station_eva_number integer NOT NULL,
                            schedule_type core.schedule_type NOT NULL,
                            administration_id uuid NOT NULL,
                            transport_type core.transport_type NOT NULL,
                            journey_description character varying(64) NOT NULL,
                            number integer NOT NULL,
                            is_replacement_transport boolean NOT NULL,
                            origin_eva_number integer NOT NULL,
                            destination_eva_number integer NOT NULL,
                            is_station_line_event boolean NOT NULL,
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
                        ON statistics.journey_event_quality_facts (station_eva_number, planned_time, journey_description, number, transport_type)
                        WHERE is_station_line_event;");

            migrationBuilder.Sql(@"CREATE TABLE IF NOT EXISTS statistics.journey_route_quality_facts (
                            journey_id character varying(82) NOT NULL,
                            date date NOT NULL,
                            journey_start_time timestamp with time zone NOT NULL,
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

            #region Continuous Aggregates
            migrationBuilder.Sql(@"
                CREATE MATERIALIZED VIEW IF NOT EXISTS statistics.station_event_quality_hourly
                WITH (timescaledb.continuous) AS
                SELECT
                    time_bucket(INTERVAL '1 hour', planned_time) AS bucket_hour,
                    station_eva_number,
                    schedule_type,
                    administration_id,
                    transport_type,
                    is_replacement_transport,
                    count(*)::bigint AS event_count,
                    count(*) FILTER (WHERE cancelled IS TRUE)::bigint AS cancelled_count,
                    count(*) FILTER (WHERE cancelled IS NOT TRUE)::bigint AS delay_sample_count,
                    coalesce(sum(delay) FILTER (WHERE cancelled IS NOT TRUE), 0)::bigint AS delay_sum_seconds,
                    count(*) FILTER (WHERE cancelled IS NOT TRUE AND delay < 300)::bigint AS punctual_5_count,
                    count(*) FILTER (WHERE cancelled IS NOT TRUE AND delay < 900)::bigint AS punctual_15_count
                FROM statistics.journey_event_quality_facts
                GROUP BY
                    bucket_hour,
                    station_eva_number,
                    schedule_type,
                    administration_id,
                    transport_type,
                    is_replacement_transport
                WITH NO DATA;
            ");

            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS IX_station_event_quality_hourly_network
                        ON statistics.station_event_quality_hourly (
                            schedule_type,
                            transport_type,
                            is_replacement_transport,
                            bucket_hour
                        );");

            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS IX_station_event_quality_hourly_station
                        ON statistics.station_event_quality_hourly (
                            station_eva_number,
                            schedule_type,
                            transport_type,
                            is_replacement_transport,
                            bucket_hour
                        );");

            migrationBuilder.Sql(@"
                CREATE MATERIALIZED VIEW IF NOT EXISTS statistics.station_line_route_quality_hourly
                WITH (timescaledb.continuous) AS
                SELECT
                    time_bucket(INTERVAL '1 hour', planned_time) AS bucket_hour,
                    station_eva_number,
                    administration_id,
                    transport_type,
                    journey_description,
                    number,
                    is_replacement_transport,
                    origin_eva_number,
                    destination_eva_number,
                    count(*)::bigint AS event_count,
                    count(*) FILTER (WHERE cancelled IS TRUE)::bigint AS cancelled_count,
                    count(*) FILTER (WHERE cancelled IS NOT TRUE)::bigint AS delay_sample_count,
                    coalesce(sum(delay) FILTER (WHERE cancelled IS NOT TRUE), 0)::bigint AS delay_sum_seconds,
                    count(*) FILTER (WHERE cancelled IS NOT TRUE AND delay < 300)::bigint AS punctual_5_count,
                    count(*) FILTER (WHERE cancelled IS NOT TRUE AND delay < 900)::bigint AS punctual_15_count
                FROM statistics.journey_event_quality_facts
                WHERE is_station_line_event IS TRUE
                GROUP BY
                    bucket_hour,
                    station_eva_number,
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
                    count(*)::bigint AS journey_count,
                    count(*) FILTER (WHERE journey_cancelled IS TRUE)::bigint AS journey_cancelled_count,
                    count(*) FILTER (WHERE journey_cancelled IS NOT TRUE AND terminal_delay_seconds IS NOT NULL)::bigint AS delay_sample_count,
                    coalesce(sum(terminal_delay_seconds) FILTER (
                        WHERE journey_cancelled IS NOT TRUE AND terminal_delay_seconds IS NOT NULL
                    ), 0)::bigint AS delay_sum_seconds,
                    count(*) FILTER (WHERE journey_cancelled IS NOT TRUE AND terminal_delay_seconds < 300)::bigint AS punctual_5_count,
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

            migrationBuilder.Sql(@"SELECT add_continuous_aggregate_policy('statistics.station_event_quality_hourly',
                        start_offset => INTERVAL '210 days',
                        end_offset => INTERVAL '1 hour',
                        schedule_interval => INTERVAL '1 hour',
                        if_not_exists => TRUE);");

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

            migrationBuilder.Sql(@"ALTER TABLE core.journey_stop_place_messages SET (
                        timescaledb.compress,
                        timescaledb.compress_orderby = 'date'
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
            migrationBuilder.Sql(@"SELECT add_compression_policy('core.journey_stop_place_messages', INTERVAL '240 days', if_not_exists => TRUE);");
            migrationBuilder.Sql(@"SELECT add_compression_policy('statistics.journey_event_quality_facts', INTERVAL '240 days', if_not_exists => TRUE);");
            migrationBuilder.Sql(@"SELECT add_compression_policy('statistics.journey_route_quality_facts', INTERVAL '240 days', if_not_exists => TRUE);");
            #endregion
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"SELECT remove_continuous_aggregate_policy('statistics.station_event_quality_hourly', if_exists => TRUE);");
            migrationBuilder.Sql(@"SELECT remove_continuous_aggregate_policy('statistics.station_line_route_quality_hourly', if_exists => TRUE);");
            migrationBuilder.Sql(@"SELECT remove_continuous_aggregate_policy('statistics.journey_route_quality_hourly', if_exists => TRUE);");

            migrationBuilder.Sql(@"DROP INDEX IF EXISTS _timescaledb_internal.IX_journey_route_quality_hourly_window;");
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS _timescaledb_internal.IX_station_line_route_quality_hourly_station;");
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS _timescaledb_internal.IX_station_event_quality_hourly_station;");
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS _timescaledb_internal.IX_station_event_quality_hourly_network;");

            migrationBuilder.Sql(@"DROP MATERIALIZED VIEW IF EXISTS statistics.journey_route_quality_hourly;");
            migrationBuilder.Sql(@"DROP MATERIALIZED VIEW IF EXISTS statistics.station_line_route_quality_hourly;");
            migrationBuilder.Sql(@"DROP MATERIALIZED VIEW IF EXISTS statistics.station_event_quality_hourly;");

            migrationBuilder.Sql(@"SELECT remove_compression_policy('statistics.journey_route_quality_facts', if_exists => TRUE);");
            migrationBuilder.Sql(@"SELECT remove_compression_policy('statistics.journey_event_quality_facts', if_exists => TRUE);");
            migrationBuilder.Sql(@"SELECT remove_compression_policy('core.journey_stop_place_messages', if_exists => TRUE);");
            migrationBuilder.Sql(@"SELECT remove_compression_policy('core.journey_stop_places', if_exists => TRUE);");

            migrationBuilder.Sql(@"DROP TABLE IF EXISTS statistics.journey_route_quality_facts CASCADE;");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS statistics.journey_event_quality_facts CASCADE;");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS core.journey_stop_place_messages CASCADE;");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS core.journey_messages CASCADE;");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS core.journey_stop_places CASCADE;");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS core.journey_transports CASCADE;");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS core.journeys CASCADE;");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS core.journey_administrations CASCADE;");
        }
    }
}
