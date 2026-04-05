using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Navigator.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddJourneyRawSql : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:core.journey_type", "EXTRA,REGULAR,RELIEF,REPLACEMENT")
                .Annotation("Npgsql:Enum:core.message_type", "ATTRIBUTE,DISRUPTION,NOTE,RIS_CAUSE,RIS_QUALITY_DEVIATION")
                .Annotation("Npgsql:Enum:core.schedule_type", "ARRIVAL,DEPARTURE")
                .Annotation("Npgsql:Enum:core.time_type", "PREVIEW,REAL,SCHEDULE")
                .Annotation("Npgsql:Enum:core.transport_type", "BIKE,BUS,CAR,CITY_TRAIN,FERRY,FLIGHT,HIGH_SPEED_TRAIN,INTERCITY_TRAIN,INTER_REGIONAL_TRAIN,REGIONAL_TRAIN,SCOOTER,SHUTTLE,SUBWAY,TAXI,TRAM,UNKNOWN,WALK")
                .Annotation("Npgsql:PostgresExtension:cube", ",,")
                .Annotation("Npgsql:PostgresExtension:earthdistance", ",,")
                .Annotation("Npgsql:PostgresExtension:pg_cron", ",,")
                .Annotation("Npgsql:PostgresExtension:partman.pg_partman", ",,")
                .OldAnnotation("Npgsql:Enum:core.journey_type", "EXTRA,REGULAR,RELIEF,REPLACEMENT")
                .OldAnnotation("Npgsql:Enum:core.message_type", "ATTRIBUTE,DISRUPTION,NOTE,RIS_CAUSE,RIS_QUALITY_DEVIATION")
                .OldAnnotation("Npgsql:Enum:core.schedule_type", "ARRIVAL,DEPARTURE")
                .OldAnnotation("Npgsql:Enum:core.time_type", "PREVIEW,REAL,SCHEDULE")
                .OldAnnotation("Npgsql:Enum:core.transport_type", "BIKE,BUS,CAR,CITY_TRAIN,FERRY,FLIGHT,HIGH_SPEED_TRAIN,INTERCITY_TRAIN,INTER_REGIONAL_TRAIN,REGIONAL_TRAIN,SCOOTER,SHUTTLE,SUBWAY,TAXI,TRAM,UNKNOWN,WALK")
                .OldAnnotation("Npgsql:PostgresExtension:cube", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:earthdistance", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:pg_partman", ",,");

            #region Partman Extension Setup
            migrationBuilder.Sql(@"CREATE SCHEMA IF NOT EXISTS partman;");
            migrationBuilder.Sql(@"CREATE EXTENSION IF NOT EXISTS pg_partman SCHEMA partman;");
            #endregion

            #region Journey Administration
            migrationBuilder.Sql(@"CREATE TABLE IF NOT EXISTS core.journey_administrations (
                            id uuid NOT NULL DEFAULT gen_random_uuid(),
                            administration_id character varying(32) NOT NULL,
                            operator_code character varying(32) NOT NULL,
                            operator_name character varying(128) NOT NULL,
                            CONSTRAINT PK_journey_administrations PRIMARY KEY (id)
                        );");

            migrationBuilder.Sql(@"CREATE UNIQUE INDEX IF NOT EXISTS IX_journey_administrations_unique ON core.journey_administrations (administration_id, operator_code, operator_name);");
            #endregion

            #region Journey
            migrationBuilder.Sql(@"CREATE TABLE IF NOT EXISTS core.journeys (
                            id character varying(82) NOT NULL,
                            date date NOT NULL,
                            inserted_at timestamp with time zone NOT NULL DEFAULT now(),
                            administration_id uuid NOT NULL,
                            cancelled boolean NOT NULL,
                            journey_type core.journey_type NOT NULL,
                            CONSTRAINT PK_journeys PRIMARY KEY (id, date),
                            CONSTRAINT FK_journeys_administrations FOREIGN KEY (administration_id) REFERENCES core.journey_administrations (id) ON DELETE RESTRICT
                        ) PARTITION BY RANGE (date);");

            migrationBuilder.Sql(@"SELECT partman.create_partition(
                            p_parent_table := 'core.journeys'::text
                            , p_control := 'date'::text
                            , p_interval := '3 months'::text
                            , p_type := 'range'::text
                            , p_default_table := true
                            , p_premake := 2
                        );");
            #endregion

            #region Journey Transports
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
                            CONSTRAINT PK_journey_transports PRIMARY KEY (journey_id, date),
                            CONSTRAINT FK_journey_transports_journeys FOREIGN KEY (journey_id, date) REFERENCES core.journeys (id, date) ON DELETE CASCADE
                        ) PARTITION BY RANGE (date);");

            migrationBuilder.Sql(@"SELECT partman.create_partition(
                            p_parent_table := 'core.journey_transports',
                            p_control := 'date',
                            p_interval := '3 months',
                            p_type := 'range',
                            p_default_table := true,
                            p_premake := 2
                        );");
            #endregion

            #region Journey Stop Places
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
                            CONSTRAINT PK_journey_stop_places PRIMARY KEY (journey_id, date),
                            CONSTRAINT UK_journey_stop_places_principal UNIQUE (id, date),
                            CONSTRAINT FK_journey_stop_places_journeys FOREIGN KEY (journey_id, date) REFERENCES core.journeys (id, date) ON DELETE CASCADE
                        ) PARTITION BY RANGE (date);");

            migrationBuilder.Sql(@"SELECT partman.create_partition(
                            p_parent_table := 'core.journey_stop_places',
                            p_control := 'date',
                            p_interval := '3 months',
                            p_type := 'range',
                            p_default_table := true,
                            p_premake := 2
                        );");
            #endregion

            #region Journey Messages
            migrationBuilder.Sql(@"CREATE TABLE IF NOT EXISTS core.journey_messages (
                            id uuid NOT NULL DEFAULT gen_random_uuid(),
                            journey_id character varying(82) NOT NULL,
                            date date NOT NULL,
                            message_type core.message_type NOT NULL,
                            code character varying(64) NOT NULL,
                            text character varying(2048) NOT NULL,
                            text_short character varying(2048) NOT NULL,
                            disruption_cause character varying(128),
                            disruption_effect character varying(128),
                            note_category character varying(128),
                            CONSTRAINT PK_journey_messages PRIMARY KEY (id, date),
                            CONSTRAINT UK_journey_messages_principal UNIQUE (id, date),
                            CONSTRAINT FK_journey_messages_journeys FOREIGN KEY (journey_id, date) REFERENCES core.journeys (id, date) ON DELETE CASCADE
                        ) PARTITION BY RANGE (date);");

            migrationBuilder.Sql(@"SELECT partman.create_partition(
                            p_parent_table := 'core.journey_messages',
                            p_control := 'date',
                            p_interval := '3 months',
                            p_type := 'range',
                            p_default_table := true,
                            p_premake := 2
                        );");
            #endregion

            #region Journey Stop Place Messages
            migrationBuilder.Sql(@"CREATE TABLE IF NOT EXISTS core.journey_stop_place_messages (
                            journey_stop_place_id uuid NOT NULL,
                            journey_message_id uuid NOT NULL,
                            date date NOT NULL,
                            CONSTRAINT PK_journey_stop_place_messages PRIMARY KEY (journey_stop_place_id, journey_message_id, date),
                            CONSTRAINT FK_stop_place_messages_stop_places FOREIGN KEY (journey_stop_place_id, date) REFERENCES core.journey_stop_places (id, date) ON DELETE CASCADE,
                            CONSTRAINT FK_stop_place_messages_journey_messages FOREIGN KEY (journey_message_id, date) REFERENCES core.journey_messages (id, date) ON DELETE CASCADE
                        ) PARTITION BY RANGE (date);");

            migrationBuilder.Sql(@"SELECT partman.create_partition(
                            p_parent_table := 'core.journey_stop_place_messages',
                            p_control := 'date',
                            p_interval := '3 months',
                            p_type := 'range',
                            p_default_table := true,
                            p_premake := 2
                        );");
            #endregion

            migrationBuilder.Sql(@"SELECT cron.schedule('maintenance_journeys', '0 0 * * *', 'CALL partman.run_maintenance_proc();');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"SELECT cron.unschedule('maintenance_journeys');");

            migrationBuilder.Sql(@"DELETE FROM parent.part_config WHERE parent_table IN (
                'core.journeys',
                'core.journey_transports',
                'core.journey_stop_places',
                'core.journey_messages',
                'core.journey_stop_place_messages'
            );");

            migrationBuilder.Sql(@"DROP TABLE IF EXISTS core.journey_stop_place_messages CASCADE;");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS core.journey_messages CASCADE;");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS core.journey_stop_places CASCADE;");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS core.journey_transports CASCADE;");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS core.journeys CASCADE;");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS core.journey_administrations CASCADE;");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:core.journey_type", "EXTRA,REGULAR,RELIEF,REPLACEMENT")
                .Annotation("Npgsql:Enum:core.message_type", "ATTRIBUTE,DISRUPTION,NOTE,RIS_CAUSE,RIS_QUALITY_DEVIATION")
                .Annotation("Npgsql:Enum:core.schedule_type", "ARRIVAL,DEPARTURE")
                .Annotation("Npgsql:Enum:core.time_type", "PREVIEW,REAL,SCHEDULE")
                .Annotation("Npgsql:Enum:core.transport_type", "BIKE,BUS,CAR,CITY_TRAIN,FERRY,FLIGHT,HIGH_SPEED_TRAIN,INTERCITY_TRAIN,INTER_REGIONAL_TRAIN,REGIONAL_TRAIN,SCOOTER,SHUTTLE,SUBWAY,TAXI,TRAM,UNKNOWN,WALK")
                .Annotation("Npgsql:PostgresExtension:cube", ",,")
                .Annotation("Npgsql:PostgresExtension:earthdistance", ",,")
                .Annotation("Npgsql:PostgresExtension:pg_partman", ",,")
                .OldAnnotation("Npgsql:Enum:core.journey_type", "EXTRA,REGULAR,RELIEF,REPLACEMENT")
                .OldAnnotation("Npgsql:Enum:core.message_type", "ATTRIBUTE,DISRUPTION,NOTE,RIS_CAUSE,RIS_QUALITY_DEVIATION")
                .OldAnnotation("Npgsql:Enum:core.schedule_type", "ARRIVAL,DEPARTURE")
                .OldAnnotation("Npgsql:Enum:core.time_type", "PREVIEW,REAL,SCHEDULE")
                .OldAnnotation("Npgsql:Enum:core.transport_type", "BIKE,BUS,CAR,CITY_TRAIN,FERRY,FLIGHT,HIGH_SPEED_TRAIN,INTERCITY_TRAIN,INTER_REGIONAL_TRAIN,REGIONAL_TRAIN,SCOOTER,SHUTTLE,SUBWAY,TAXI,TRAM,UNKNOWN,WALK")
                .OldAnnotation("Npgsql:PostgresExtension:cube", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:earthdistance", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:pg_cron", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:partman.pg_partman", ",,");
        }
    }
}
