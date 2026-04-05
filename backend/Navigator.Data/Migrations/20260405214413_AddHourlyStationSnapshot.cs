using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Navigator.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddHourlyStationSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE MATERIALIZED VIEW IF NOT EXISTS statistics.hourly_station_snapshots AS
                SELECT
                    date_trunc('hour', stop_place.planned_time) AS bucket_hour,
                    stop_place.station_eva_number AS eva_number,
                    journey_transports.transport_type AS transport_type,
                    count(*) FILTER (WHERE stop_place.schedule_type = 'ARRIVAL') AS arrival_count,
                    count(*) FILTER (WHERE stop_place.schedule_type = 'ARRIVAL' AND stop_place.cancelled IS TRUE) AS arrival_cancellation_count,
                    coalesce(sum(stop_place.delay) FILTER (WHERE stop_place.schedule_type = 'ARRIVAL' AND stop_place.cancelled IS NOT TRUE), 0) AS arrival_delay_sum,
                    count(*) FILTER (WHERE stop_place.schedule_type = 'DEPARTURE') AS departure_count,
                    count(*) FILTER (WHERE stop_place.schedule_type = 'DEPARTURE' AND stop_place.cancelled IS TRUE) AS departure_cancellation_count,
                    coalesce(sum(stop_place.delay) FILTER (WHERE stop_place.schedule_type = 'DEPARTURE' AND stop_place.cancelled IS NOT TRUE), 0) AS departure_delay_sum
                FROM core.journey_stop_places AS stop_place
                JOIN core.journey_transports AS journey_transports ON stop_place.journey_id = journey_transports.journey_id AND stop_place.date = journey_transports.date
                GROUP BY
                    bucket_hour,
                    eva_number,
                    transport_type;
            ");

            migrationBuilder.Sql(@"CREATE UNIQUE INDEX UX_hourly_station_snapshots ON statistics.hourly_station_snapshots (bucket_hour, eva_number, transport_type);");

            migrationBuilder.Sql(@"SELECT cron.schedule('refresh_station_snapshots', '0 */6 * * *', 'REFRESH MATERIALIZED VIEW CONCURRENTLY statistics.hourly_station_snapshots');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"SELECT cron.unschedule('refresh_station_snapshots');");
            migrationBuilder.Sql(@"DROP MATERIALIZED VIEW IF EXISTS statistics.hourly_station_snapshots;");
        }
    }
}
