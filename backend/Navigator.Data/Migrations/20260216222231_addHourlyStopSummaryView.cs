using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Navigator.Data.Migrations
{
    /// <inheritdoc />
    public partial class addHourlyStopSummaryView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE MATERIALIZED VIEW statistics.hourly_stop_summary AS
                SELECT
                    DATE_TRUNC('hour', journey_stop_places.planned_time) AS bucket_hour,
                    journey_transports.transport_type AS transport_type,
                    COUNT(*) FILTER (WHERE journey_stop_places.schedule_type = 'ARRIVAL') AS arrivals_count,
                    COUNT(*) FILTER (WHERE journey_stop_places.schedule_type = 'ARRIVAL' AND journey_stop_places.cancelled IS TRUE) AS arrival_cancellations_count,
                    COALESCE(SUM(journey_stop_places.delay) FILTER (WHERE journey_stop_places.schedule_type = 'ARRIVAL' AND journey_stop_places.cancelled IS NOT TRUE), 0) AS arrival_delay_sum,
                    COALESCE(AVG(journey_stop_places.delay) FILTER (WHERE journey_stop_places.schedule_type = 'ARRIVAL' AND journey_stop_places.cancelled IS NOT TRUE), 0) AS arrival_delay_avg,
                    COUNT(*) FILTER (WHERE journey_stop_places.schedule_type = 'DEPARTURE') AS departures_count,
                    COUNT(*) FILTER (WHERE journey_stop_places.schedule_type = 'DEPARTURE' AND journey_stop_places.cancelled IS TRUE) AS departure_cancellations_count,
                    COALESCE(SUM(journey_stop_places.delay) FILTER (WHERE journey_stop_places.schedule_type = 'DEPARTURE' AND journey_stop_places.cancelled IS NOT TRUE), 0) AS departure_delay_sum,
                    COALESCE(AVG(journey_stop_places.delay) FILTER (WHERE journey_stop_places.schedule_type = 'DEPARTURE' AND journey_stop_places.cancelled IS NOT TRUE), 0) AS departure_delay_avg
                FROM core.journey_stop_places AS journey_stop_places
                JOIN core.journey_transports AS journey_transports ON journey_stop_places.journey_id = journey_transports.journey_id
                GROUP BY
                    bucket_hour,
                    transport_type;
            ");

            migrationBuilder.Sql(@"
                CREATE UNIQUE INDEX IX_hourly_stop_summary_bucket_hour_transport_type
                ON statistics.hourly_stop_summary (bucket_hour, transport_type);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP MATERIALIZED VIEW IF EXISTS statistics.hourly_stop_summary;");
        }
    }
}
