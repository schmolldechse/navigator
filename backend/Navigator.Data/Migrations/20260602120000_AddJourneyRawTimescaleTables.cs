using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Navigator.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddJourneyRawTimescaleTables : Migration
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
            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS IX_journeys_date_id_projection
                        ON core.journeys (date, id)
                        INCLUDE (administration_id, cancelled, journey_type);");

            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS IX_journey_stop_places_station_date_planned_time
                        ON core.journey_stop_places (station_eva_number, date, planned_time)
                        INCLUDE (id, journey_id, schedule_type, cancelled, delay);");

            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS IX_journey_stop_places_journey_date_planned_time
                        ON core.journey_stop_places (journey_id, date, planned_time)
                        INCLUDE (station_eva_number, schedule_type, cancelled, delay);");

            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS IX_journey_transports_number_date
                        ON core.journey_transports (number, date)
                        INCLUDE (journey_id, transport_type, replacement_transport_type, journey_description, category, line);");
            #endregion

            #region Compression
            migrationBuilder.Sql(@"ALTER TABLE core.journey_stop_places SET (
                        timescaledb.compress,
                        timescaledb.compress_segmentby = 'station_eva_number',
                        timescaledb.compress_orderby = 'date, planned_time'
                    );");

            migrationBuilder.Sql(@"SELECT add_compression_policy('core.journey_stop_places', INTERVAL '240 days', if_not_exists => TRUE);");
            #endregion
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"SELECT remove_compression_policy('core.journey_stop_places', if_exists => TRUE);");

            migrationBuilder.Sql(@"DROP TABLE IF EXISTS core.journey_stop_places CASCADE;");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS core.journey_transports CASCADE;");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS core.journeys CASCADE;");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS core.journey_administrations CASCADE;");
        }
    }
}
