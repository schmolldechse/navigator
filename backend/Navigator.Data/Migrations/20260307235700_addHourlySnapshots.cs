using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Navigator.Data.Migrations
{
    /// <inheritdoc />
    public partial class addHourlySnapshots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Global Stop Summary
            migrationBuilder.Sql(@"
                CREATE MATERIALIZED VIEW IF NOT EXISTS statistics.hourly_transport_snapshots AS
                SELECT
                    DATE_TRUNC('hour', journey_stop_places.planned_time) AS bucket_hour,
                    journey_transports.transport_type AS transport_type,
                    COUNT(*) FILTER (WHERE journey_stop_places.schedule_type = 'ARRIVAL') AS arrival_count,
                    COUNT(*) FILTER (WHERE journey_stop_places.schedule_type = 'ARRIVAL' AND journey_stop_places.cancelled IS TRUE) AS arrival_cancellation_count,
                    COALESCE(SUM(journey_stop_places.delay) FILTER (WHERE journey_stop_places.schedule_type = 'ARRIVAL' AND journey_stop_places.cancelled IS NOT TRUE), 0) AS arrival_delay_sum,
                    COUNT(*) FILTER (WHERE journey_stop_places.schedule_type = 'DEPARTURE') AS departure_count,
                    COUNT(*) FILTER (WHERE journey_stop_places.schedule_type = 'DEPARTURE' AND journey_stop_places.cancelled IS TRUE) AS departure_cancellation_count,
                    COALESCE(SUM(journey_stop_places.delay) FILTER (WHERE journey_stop_places.schedule_type = 'DEPARTURE' AND journey_stop_places.cancelled IS NOT TRUE), 0) AS departure_delay_sum,
                FROM core.journey_stop_places AS journey_stop_places
                JOIN core.journey_transports AS journey_transports ON journey_stop_places.journey_id = journey_transports.journey_id
                GROUP BY
                    bucket_hour,
                    transport_type;
            ");

            migrationBuilder.Sql(@"
                CREATE UNIQUE INDEX IX_hourly_transport_snapshots_bucket_hour_transport_type
                ON statistics.hourly_transport_snapshots (bucket_hour, transport_type);
            ");

            // Station -specific Stop Summary
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
                    COALESCE(SUM(journey_stop_places.delay) FILTER (WHERE journey_stop_places.schedule_type = 'DEPARTURE' AND journey_stop_places.cancelled IS NOT TRUE), 0) AS departure_delay_sum,
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP MATERIALIZED VIEW IF EXISTS statistics.hourly_transport_snapshots;");
            migrationBuilder.Sql(@"DROP MATERIALIZED VIEW IF EXISTS statistics.hourly_station_snapshots;");
        }
    }
}
