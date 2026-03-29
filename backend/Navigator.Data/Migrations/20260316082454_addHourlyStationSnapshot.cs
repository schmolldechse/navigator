using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Navigator.Data.Migrations
{
    /// <inheritdoc />
    public partial class addHourlyStationSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:core.journey_type", "EXTRA,REGULAR,RELIEF,REPLACEMENT")
                .Annotation("Npgsql:Enum:core.message_reference_type", "ATTACHMENT,IMAGE,LINK")
                .Annotation("Npgsql:Enum:core.message_type", "ATTRIBUTE,DISRUPTION,NOTE,RIS_CAUSE,RIS_QUALITY_DEVIATION")
                .Annotation("Npgsql:Enum:core.schedule_type", "ARRIVAL,DEPARTURE")
                .Annotation("Npgsql:Enum:core.time_type", "PREVIEW,REAL,SCHEDULE")
                .Annotation("Npgsql:Enum:core.transport_type", "BIKE,BUS,CAR,CITY_TRAIN,FERRY,FLIGHT,HIGH_SPEED_TRAIN,INTERCITY_TRAIN,INTER_REGIONAL_TRAIN,REGIONAL_TRAIN,SCOOTER,SHUTTLE,SUBWAY,TAXI,TRAM,UNKNOWN,WALK")
                .Annotation("Npgsql:PostgresExtension:cube", ",,")
                .Annotation("Npgsql:PostgresExtension:earthdistance", ",,")
                .Annotation("Npgsql:PostgresExtension:pg_cron", ",,")
                .OldAnnotation("Npgsql:Enum:core.journey_type", "EXTRA,REGULAR,RELIEF,REPLACEMENT")
                .OldAnnotation("Npgsql:Enum:core.message_reference_type", "ATTACHMENT,IMAGE,LINK")
                .OldAnnotation("Npgsql:Enum:core.message_type", "ATTRIBUTE,DISRUPTION,NOTE,RIS_CAUSE,RIS_QUALITY_DEVIATION")
                .OldAnnotation("Npgsql:Enum:core.schedule_type", "ARRIVAL,DEPARTURE")
                .OldAnnotation("Npgsql:Enum:core.time_type", "PREVIEW,REAL,SCHEDULE")
                .OldAnnotation("Npgsql:Enum:core.transport_type", "BIKE,BUS,CAR,CITY_TRAIN,FERRY,FLIGHT,HIGH_SPEED_TRAIN,INTERCITY_TRAIN,INTER_REGIONAL_TRAIN,REGIONAL_TRAIN,SCOOTER,SHUTTLE,SUBWAY,TAXI,TRAM,UNKNOWN,WALK")
                .OldAnnotation("Npgsql:PostgresExtension:cube", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:earthdistance", ",,");

            // station snapshot
            migrationBuilder.Sql(@"
                CREATE MATERIALIZED VIEW IF NOT EXISTS statistics.hourly_station_snapshots AS
                SELECT
                    DATE_TRUNC('hour', journey_stop_places.planned_time) AS bucket_hour,
                    journey_stop_places.station_eva_number AS eva_number,
                    journey_transports.transport_type AS transport_type,
                    COUNT(*) FILTER (WHERE journey_stop_places.schedule_type = 'ARRIVAL') AS arrival_count,
                    COUNT(*) FILTER (WHERE journey_stop_places.schedule_type = 'ARRIVAL' AND journey_stop_places.cancelled IS TRUE) AS arrival_cancellation_count,
                    COALESCE(SUM(journey_stop_places.delay) FILTER (WHERE journey_stop_places.schedule_type = 'ARRIVAL' AND journey_stop_places.cancelled IS NOT TRUE), 0) AS arrival_delay_sum,
                    COUNT(*) FILTER (WHERE journey_stop_places.schedule_type = 'DEPARTURE') AS departure_count,
                    COUNT(*) FILTER (WHERE journey_stop_places.schedule_type = 'DEPARTURE' AND journey_stop_places.cancelled IS TRUE) AS departure_cancellation_count,
                    COALESCE(SUM(journey_stop_places.delay) FILTER (WHERE journey_stop_places.schedule_type = 'DEPARTURE' AND journey_stop_places.cancelled IS NOT TRUE), 0) AS departure_delay_sum
                FROM core.journey_stop_places journey_stop_places
                JOIN core.journey_transports journey_transports ON journey_stop_places.journey_id = journey_transports.journey_id
                GROUP BY 
                    bucket_hour, 
                    eva_number, 
                    transport_type;
            ");

            migrationBuilder.Sql(@"
                CREATE UNIQUE INDEX IX_hourly_station_snapshots_bucket_hour_eva_number_transport_ty
                ON statistics.hourly_station_snapshots (bucket_hour, eva_number, transport_type);
            ");

            // refresh materialized view using pg_cron
            migrationBuilder.Sql(@"
                SELECT cron.schedule('refresh_station_snapshot', '0 */6 * * *',
                'REFRESH MATERIALIZED VIEW CONCURRENTLY statistics.hourly_station_snapshots');
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:core.information_type", "DISRUPTION,JOURNEY_ATTRIBUTE,MESSAGE,RIS_CAUSE_REASON,RIS_QUALITY_DEVIATION")
                .Annotation("Npgsql:Enum:core.journey_type", "EXTRA,REGULAR,RELIEF,REPLACEMENT")
                .Annotation("Npgsql:Enum:core.message_reference_type", "ATTACHMENT,IMAGE,LINK")
                .Annotation("Npgsql:Enum:core.message_type", "ATTRIBUTE,DISRUPTION,NOTE,RIS_CAUSE,RIS_QUALITY_DEVIATION")
                .Annotation("Npgsql:Enum:core.schedule_type", "ARRIVAL,DEPARTURE")
                .Annotation("Npgsql:Enum:core.time_type", "PREVIEW,REAL,SCHEDULE")
                .Annotation("Npgsql:Enum:core.transport_type", "BIKE,BUS,CAR,CITY_TRAIN,FERRY,FLIGHT,HIGH_SPEED_TRAIN,INTERCITY_TRAIN,INTER_REGIONAL_TRAIN,REGIONAL_TRAIN,SCOOTER,SHUTTLE,SUBWAY,TAXI,TRAM,UNKNOWN,WALK")
                .Annotation("Npgsql:PostgresExtension:cube", ",,")
                .Annotation("Npgsql:PostgresExtension:earthdistance", ",,")
                .OldAnnotation("Npgsql:Enum:core.information_type", "DISRUPTION,JOURNEY_ATTRIBUTE,MESSAGE,RIS_CAUSE_REASON,RIS_QUALITY_DEVIATION")
                .OldAnnotation("Npgsql:Enum:core.journey_type", "EXTRA,REGULAR,RELIEF,REPLACEMENT")
                .OldAnnotation("Npgsql:Enum:core.message_reference_type", "ATTACHMENT,IMAGE,LINK")
                .OldAnnotation("Npgsql:Enum:core.message_type", "ATTRIBUTE,DISRUPTION,NOTE,RIS_CAUSE,RIS_QUALITY_DEVIATION")
                .OldAnnotation("Npgsql:Enum:core.schedule_type", "ARRIVAL,DEPARTURE")
                .OldAnnotation("Npgsql:Enum:core.time_type", "PREVIEW,REAL,SCHEDULE")
                .OldAnnotation("Npgsql:Enum:core.transport_type", "BIKE,BUS,CAR,CITY_TRAIN,FERRY,FLIGHT,HIGH_SPEED_TRAIN,INTERCITY_TRAIN,INTER_REGIONAL_TRAIN,REGIONAL_TRAIN,SCOOTER,SHUTTLE,SUBWAY,TAXI,TRAM,UNKNOWN,WALK")
                .OldAnnotation("Npgsql:PostgresExtension:cube", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:earthdistance", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:pg_cron", ",,");

            migrationBuilder.Sql(@"DROP MATERIALIZED VIEW IF EXISTS statistics.hourly_station_snapshots;");

            migrationBuilder.Sql(@"SELECT cron.unschedule('refresh_station_snapshot');");
        }
    }
}
