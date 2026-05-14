using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Navigator.Data.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20260506193000_AddDashboardMetricSnapshots")]
    public partial class AddDashboardMetricSnapshots : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            UnscheduleCronJob(migrationBuilder, "refresh_station_snapshots");

            migrationBuilder.Sql(@"DROP MATERIALIZED VIEW IF EXISTS statistics.hourly_station_snapshots;");

            migrationBuilder.Sql(@"
                CREATE MATERIALIZED VIEW statistics.hourly_station_snapshots AS
                SELECT
                    date_trunc('hour', stop_place.planned_time) AS bucket_hour,
                    stop_place.station_eva_number AS eva_number,
                    journey_transports.transport_type AS transport_type,

                    (count(*) FILTER (WHERE stop_place.schedule_type = 'ARRIVAL'))::integer AS arrival_count,
                    (count(*) FILTER (WHERE stop_place.schedule_type = 'ARRIVAL' AND stop_place.cancelled IS TRUE))::integer AS arrival_cancellation_count,
                    coalesce(sum(stop_place.delay) FILTER (WHERE stop_place.schedule_type = 'ARRIVAL' AND stop_place.cancelled IS NOT TRUE), 0)::double precision AS arrival_delay_sum,
                    (count(*) FILTER (WHERE stop_place.schedule_type = 'ARRIVAL' AND stop_place.cancelled IS NOT TRUE))::integer AS arrival_delay_sample_count,
                    (count(*) FILTER (WHERE stop_place.schedule_type = 'ARRIVAL' AND stop_place.cancelled IS NOT TRUE AND stop_place.delay <= 359))::integer AS arrival_punctual_count,
                    (count(*) FILTER (WHERE stop_place.schedule_type = 'ARRIVAL' AND stop_place.cancelled IS NOT TRUE AND stop_place.delay BETWEEN 360 AND 899))::integer AS arrival_delay_minor_count,
                    (count(*) FILTER (WHERE stop_place.schedule_type = 'ARRIVAL' AND stop_place.cancelled IS NOT TRUE AND stop_place.delay BETWEEN 900 AND 3599))::integer AS arrival_delay_major_count,
                    (count(*) FILTER (WHERE stop_place.schedule_type = 'ARRIVAL' AND stop_place.cancelled IS NOT TRUE AND stop_place.delay >= 3600))::integer AS arrival_delay_severe_count,
                    (count(*) FILTER (WHERE stop_place.schedule_type = 'ARRIVAL' AND stop_place.cancelled IS NOT TRUE AND stop_place.planned_platform IS NOT NULL AND stop_place.actual_platform IS NOT NULL AND stop_place.planned_platform <> stop_place.actual_platform))::integer AS arrival_platform_change_count,
                    (count(*) FILTER (WHERE stop_place.schedule_type = 'ARRIVAL' AND stop_place.additional IS TRUE))::integer AS arrival_additional_count,
                    (count(*) FILTER (WHERE stop_place.schedule_type = 'ARRIVAL' AND stop_place.demand IS TRUE))::integer AS arrival_demand_count,
                    (count(*) FILTER (WHERE stop_place.schedule_type = 'ARRIVAL' AND stop_place.no_passenger_change IS TRUE))::integer AS arrival_no_passenger_change_count,

                    (count(*) FILTER (WHERE stop_place.schedule_type = 'DEPARTURE'))::integer AS departure_count,
                    (count(*) FILTER (WHERE stop_place.schedule_type = 'DEPARTURE' AND stop_place.cancelled IS TRUE))::integer AS departure_cancellation_count,
                    coalesce(sum(stop_place.delay) FILTER (WHERE stop_place.schedule_type = 'DEPARTURE' AND stop_place.cancelled IS NOT TRUE), 0)::double precision AS departure_delay_sum,
                    (count(*) FILTER (WHERE stop_place.schedule_type = 'DEPARTURE' AND stop_place.cancelled IS NOT TRUE))::integer AS departure_delay_sample_count,
                    (count(*) FILTER (WHERE stop_place.schedule_type = 'DEPARTURE' AND stop_place.cancelled IS NOT TRUE AND stop_place.delay <= 359))::integer AS departure_punctual_count,
                    (count(*) FILTER (WHERE stop_place.schedule_type = 'DEPARTURE' AND stop_place.cancelled IS NOT TRUE AND stop_place.delay BETWEEN 360 AND 899))::integer AS departure_delay_minor_count,
                    (count(*) FILTER (WHERE stop_place.schedule_type = 'DEPARTURE' AND stop_place.cancelled IS NOT TRUE AND stop_place.delay BETWEEN 900 AND 3599))::integer AS departure_delay_major_count,
                    (count(*) FILTER (WHERE stop_place.schedule_type = 'DEPARTURE' AND stop_place.cancelled IS NOT TRUE AND stop_place.delay >= 3600))::integer AS departure_delay_severe_count,
                    (count(*) FILTER (WHERE stop_place.schedule_type = 'DEPARTURE' AND stop_place.cancelled IS NOT TRUE AND stop_place.planned_platform IS NOT NULL AND stop_place.actual_platform IS NOT NULL AND stop_place.planned_platform <> stop_place.actual_platform))::integer AS departure_platform_change_count,
                    (count(*) FILTER (WHERE stop_place.schedule_type = 'DEPARTURE' AND stop_place.additional IS TRUE))::integer AS departure_additional_count,
                    (count(*) FILTER (WHERE stop_place.schedule_type = 'DEPARTURE' AND stop_place.demand IS TRUE))::integer AS departure_demand_count,
                    (count(*) FILTER (WHERE stop_place.schedule_type = 'DEPARTURE' AND stop_place.no_passenger_change IS TRUE))::integer AS departure_no_passenger_change_count
                FROM core.journey_stop_places AS stop_place
                JOIN core.journey_transports AS journey_transports
                    ON stop_place.journey_id = journey_transports.journey_id
                    AND stop_place.date = journey_transports.date
                GROUP BY
                    bucket_hour,
                    eva_number,
                    transport_type;
            ");

            migrationBuilder.Sql(@"CREATE UNIQUE INDEX UX_hourly_station_snapshots ON statistics.hourly_station_snapshots (bucket_hour, eva_number, transport_type);");

            migrationBuilder.Sql(@"
                CREATE MATERIALIZED VIEW statistics.daily_journey_service_snapshots AS
                WITH stop_stats AS (
                    SELECT
                        stop_place.journey_id,
                        stop_place.date,
                        count(*)::integer AS stop_count,
                        (count(*) FILTER (WHERE stop_place.schedule_type = 'ARRIVAL'))::integer AS arrival_stop_count,
                        (count(*) FILTER (WHERE stop_place.schedule_type = 'DEPARTURE'))::integer AS departure_stop_count,
                        (count(*) FILTER (WHERE stop_place.cancelled IS TRUE))::integer AS cancelled_stop_count,
                        coalesce(sum(stop_place.delay) FILTER (WHERE stop_place.cancelled IS NOT TRUE), 0)::double precision AS delay_sum,
                        (count(*) FILTER (WHERE stop_place.cancelled IS NOT TRUE))::integer AS delay_sample_count
                    FROM core.journey_stop_places AS stop_place
                    GROUP BY stop_place.journey_id, stop_place.date
                ),
                message_stats AS (
                    SELECT
                        message.journey_id,
                        message.date,
                        count(*)::integer AS message_count,
                        (count(*) FILTER (WHERE message.message_type = 'DISRUPTION'))::integer AS disruption_message_count
                    FROM core.journey_messages AS message
                    GROUP BY message.journey_id, message.date
                )
                SELECT
                    date_trunc('day', journey.date::timestamp) AS bucket_day,
                    transport.transport_type AS transport_type,
                    journey.journey_type AS journey_type,
                    administration.operator_code AS operator_code,
                    administration.operator_name AS operator_name,
                    count(*)::integer AS journey_count,
                    (count(*) FILTER (WHERE journey.cancelled IS TRUE))::integer AS journey_cancellation_count,
                    (count(*) FILTER (WHERE transport.replacement_transport_type IS NOT NULL))::integer AS replacement_transport_count,
                    coalesce(sum(stop_stats.stop_count), 0)::integer AS stop_count,
                    coalesce(sum(stop_stats.arrival_stop_count), 0)::integer AS arrival_stop_count,
                    coalesce(sum(stop_stats.departure_stop_count), 0)::integer AS departure_stop_count,
                    coalesce(sum(stop_stats.cancelled_stop_count), 0)::integer AS cancelled_stop_count,
                    coalesce(sum(stop_stats.delay_sum), 0)::double precision AS delay_sum,
                    coalesce(sum(stop_stats.delay_sample_count), 0)::integer AS delay_sample_count,
                    coalesce(sum(message_stats.message_count), 0)::integer AS message_count,
                    coalesce(sum(message_stats.disruption_message_count), 0)::integer AS disruption_message_count
                FROM core.journeys AS journey
                JOIN core.journey_transports AS transport
                    ON journey.id = transport.journey_id
                    AND journey.date = transport.date
                JOIN core.journey_administrations AS administration
                    ON journey.administration_id = administration.id
                LEFT JOIN stop_stats
                    ON journey.id = stop_stats.journey_id
                    AND journey.date = stop_stats.date
                LEFT JOIN message_stats
                    ON journey.id = message_stats.journey_id
                    AND journey.date = message_stats.date
                GROUP BY
                    bucket_day,
                    transport_type,
                    journey_type,
                    operator_code,
                    operator_name;
            ");

            migrationBuilder.Sql(@"CREATE UNIQUE INDEX UX_daily_journey_service_snapshots ON statistics.daily_journey_service_snapshots (bucket_day, transport_type, journey_type, operator_code, operator_name);");

            migrationBuilder.Sql(@"
                CREATE MATERIALIZED VIEW statistics.daily_station_message_snapshots AS
                SELECT
                    date_trunc('day', stop_place.planned_time) AS bucket_day,
                    stop_place.station_eva_number AS eva_number,
                    transport.transport_type AS transport_type,
                    message.message_type AS message_type,
                    coalesce(message.code, '') AS message_code,
                    coalesce(message.disruption_cause, '') AS disruption_cause,
                    coalesce(message.disruption_effect, '') AS disruption_effect,
                    coalesce(message.note_category, '') AS note_category,
                    (count(DISTINCT (message.id, message.date)))::integer AS message_count,
                    (count(DISTINCT (stop_place.id, stop_place.date)))::integer AS affected_stop_place_count,
                    (count(DISTINCT (stop_place.journey_id, stop_place.date)))::integer AS affected_journey_count
                FROM core.journey_stop_place_messages AS stop_place_message
                JOIN core.journey_stop_places AS stop_place
                    ON stop_place_message.journey_stop_place_id = stop_place.id
                    AND stop_place_message.date = stop_place.date
                JOIN core.journey_messages AS message
                    ON stop_place_message.journey_message_id = message.id
                    AND stop_place_message.date = message.date
                JOIN core.journey_transports AS transport
                    ON stop_place.journey_id = transport.journey_id
                    AND stop_place.date = transport.date
                GROUP BY
                    bucket_day,
                    eva_number,
                    transport_type,
                    message_type,
                    message_code,
                    disruption_cause,
                    disruption_effect,
                    note_category;
            ");

            migrationBuilder.Sql(@"CREATE UNIQUE INDEX UX_daily_station_message_snapshots ON statistics.daily_station_message_snapshots (bucket_day, eva_number, transport_type, message_type, message_code, disruption_cause, disruption_effect, note_category);");

            ScheduleCronJob(migrationBuilder, "refresh_station_snapshots", "0 */6 * * *", "REFRESH MATERIALIZED VIEW CONCURRENTLY statistics.hourly_station_snapshots");
            ScheduleCronJob(migrationBuilder, "refresh_journey_service_snapshots", "15 */6 * * *", "REFRESH MATERIALIZED VIEW CONCURRENTLY statistics.daily_journey_service_snapshots");
            ScheduleCronJob(migrationBuilder, "refresh_station_message_snapshots", "30 */6 * * *", "REFRESH MATERIALIZED VIEW CONCURRENTLY statistics.daily_station_message_snapshots");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            UnscheduleCronJob(migrationBuilder, "refresh_station_message_snapshots");
            UnscheduleCronJob(migrationBuilder, "refresh_journey_service_snapshots");
            UnscheduleCronJob(migrationBuilder, "refresh_station_snapshots");

            migrationBuilder.Sql(@"DROP MATERIALIZED VIEW IF EXISTS statistics.daily_station_message_snapshots;");
            migrationBuilder.Sql(@"DROP MATERIALIZED VIEW IF EXISTS statistics.daily_journey_service_snapshots;");
            migrationBuilder.Sql(@"DROP MATERIALIZED VIEW IF EXISTS statistics.hourly_station_snapshots;");

            migrationBuilder.Sql(@"
                CREATE MATERIALIZED VIEW statistics.hourly_station_snapshots AS
                SELECT
                    date_trunc('hour', stop_place.planned_time) AS bucket_hour,
                    stop_place.station_eva_number AS eva_number,
                    journey_transports.transport_type AS transport_type,
                    (count(*) FILTER (WHERE stop_place.schedule_type = 'ARRIVAL'))::integer AS arrival_count,
                    (count(*) FILTER (WHERE stop_place.schedule_type = 'ARRIVAL' AND stop_place.cancelled IS TRUE))::integer AS arrival_cancellation_count,
                    coalesce(sum(stop_place.delay) FILTER (WHERE stop_place.schedule_type = 'ARRIVAL' AND stop_place.cancelled IS NOT TRUE), 0)::double precision AS arrival_delay_sum,
                    (count(*) FILTER (WHERE stop_place.schedule_type = 'DEPARTURE'))::integer AS departure_count,
                    (count(*) FILTER (WHERE stop_place.schedule_type = 'DEPARTURE' AND stop_place.cancelled IS TRUE))::integer AS departure_cancellation_count,
                    coalesce(sum(stop_place.delay) FILTER (WHERE stop_place.schedule_type = 'DEPARTURE' AND stop_place.cancelled IS NOT TRUE), 0)::double precision AS departure_delay_sum
                FROM core.journey_stop_places AS stop_place
                JOIN core.journey_transports AS journey_transports
                    ON stop_place.journey_id = journey_transports.journey_id
                    AND stop_place.date = journey_transports.date
                GROUP BY
                    bucket_hour,
                    eva_number,
                    transport_type;
            ");

            migrationBuilder.Sql(@"CREATE UNIQUE INDEX UX_hourly_station_snapshots ON statistics.hourly_station_snapshots (bucket_hour, eva_number, transport_type);");
            ScheduleCronJob(migrationBuilder, "refresh_station_snapshots", "0 */6 * * *", "REFRESH MATERIALIZED VIEW CONCURRENTLY statistics.hourly_station_snapshots");
        }

        private static void UnscheduleCronJob(MigrationBuilder migrationBuilder, string jobName)
        {
            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF EXISTS (SELECT 1 FROM cron.job WHERE jobname = '{jobName}') THEN
                        PERFORM cron.unschedule('{jobName}');
                    END IF;
                END $$;
            ");
        }

        private static void ScheduleCronJob(MigrationBuilder migrationBuilder, string jobName, string schedule, string command)
        {
            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM cron.job WHERE jobname = '{jobName}') THEN
                        PERFORM cron.schedule('{jobName}', '{schedule}', '{command}');
                    END IF;
                END $$;
            ");
        }
    }
}
