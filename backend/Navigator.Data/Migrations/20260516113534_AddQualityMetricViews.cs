using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Navigator.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddQualityMetricViews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            #region Journey Stop Places Indexes
            migrationBuilder.Sql(@"
                CREATE INDEX IF NOT EXISTS IX_journey_stop_places_station_date_planned_time
                ON core.journey_stop_places (station_eva_number, date, planned_time)
                INCLUDE (journey_id, schedule_type, cancelled, delay);
            ");

            migrationBuilder.Sql(@"
                CREATE INDEX IF NOT EXISTS IX_journey_stop_places_journey_date_planned_time
                ON core.journey_stop_places (journey_id, date, planned_time)
                INCLUDE (station_eva_number, schedule_type, cancelled, delay);
            ");

            migrationBuilder.Sql(@"
                CREATE INDEX IF NOT EXISTS IX_journey_transports_line_number_date
                ON core.journey_transports (line, number, date)
                INCLUDE (journey_id, journey_description, transport_type);
            ");
            #endregion

            #region JourneyRouteQualityHourly View
            migrationBuilder.Sql(@"
                CREATE MATERIALIZED VIEW IF NOT EXISTS statistics.journey_route_quality_hourly AS
                WITH journey_events AS (
                    SELECT
                        journey.id AS journey_id,
                        journey.date,
                        journey.administration_id,
                        journey.cancelled AS journey_cancelled,
                        transport.transport_type,
                        transport.journey_description,
                        coalesce(nullif(transport.line, ''), transport.number::text, 'UNKNOWN') AS line,
                        transport.number,
                        stop_place.station_eva_number,
                        stop_place.schedule_type,
                        stop_place.planned_time,
                        stop_place.cancelled AS event_cancelled,
                        stop_place.delay
                    FROM core.journeys AS journey
                        JOIN core.journey_transports AS transport
                            ON transport.journey_id = journey.id
                                AND transport.date = journey.date
                        JOIN core.journey_stop_places AS stop_place
                            ON stop_place.journey_id = journey.id
                                AND stop_place.date = journey.date
                ),
                journey_rollup AS (
                    SELECT
                        journey_id,
                        date,
                        administration_id,
                        journey_cancelled,
                        transport_type,
                        journey_description,
                        line,
                        number,
                        coalesce(
                            (array_agg(station_eva_number ORDER BY planned_time, station_eva_number) FILTER (WHERE schedule_type = 'DEPARTURE'))[1],
                            (array_agg(station_eva_number ORDER BY planned_time, station_eva_number))[1],
                            0
                        ) AS origin_eva_number,
                        coalesce(
                            (array_agg(station_eva_number ORDER BY planned_time DESC, station_eva_number DESC) FILTER (WHERE schedule_type = 'ARRIVAL'))[1],
                            (array_agg(station_eva_number ORDER BY planned_time DESC, station_eva_number DESC))[1],
                            0
                        ) AS destination_eva_number,
                        coalesce(
                            (array_agg(planned_time ORDER BY planned_time) FILTER (WHERE schedule_type = 'DEPARTURE'))[1],
                            (array_agg(planned_time ORDER BY planned_time))[1]
                        ) AS journey_start_time,
                        (array_agg(delay ORDER BY CASE WHEN schedule_type = 'ARRIVAL' THEN 0 ELSE 1 END, planned_time DESC) FILTER (WHERE event_cancelled IS NOT TRUE))[1] AS terminal_delay_seconds
                    FROM journey_events
                    GROUP BY
                        journey_id,
                        date,
                        administration_id,
                        journey_cancelled,
                        transport_type,
                        journey_description,
                        line,
                        number
                )
                SELECT
                    date_trunc('hour', journey_start_time) AS bucket_hour,
                    administration_id,
                    transport_type,
                    journey_description,
                    line,
                    number,
                    origin_eva_number,
                    destination_eva_number,
                    count(*)::bigint AS journey_count,
                    count(*) FILTER (WHERE journey_cancelled IS TRUE)::bigint AS journey_cancelled_count,
                    count(*) FILTER (
                        WHERE journey_cancelled IS NOT TRUE
                            AND terminal_delay_seconds IS NOT NULL
                    )::bigint AS delay_sample_count,
                    coalesce(sum(terminal_delay_seconds) FILTER (
                        WHERE journey_cancelled IS NOT TRUE
                            AND terminal_delay_seconds IS NOT NULL
                    ), 0)::bigint AS delay_sum_seconds,
                    count(*) FILTER (
                        WHERE journey_cancelled IS NOT TRUE
                            AND terminal_delay_seconds < 300
                    )::bigint AS punctual_5_count,
                    count(*) FILTER (
                        WHERE journey_cancelled IS NOT TRUE
                            AND terminal_delay_seconds < 900
                    )::bigint AS punctual_15_count
                FROM journey_rollup
                WHERE journey_start_time IS NOT NULL
                GROUP BY
                    bucket_hour,
                    administration_id,
                    transport_type,
                    journey_description,
                    line,
                    number,
                    origin_eva_number,
                    destination_eva_number;
            ");

            migrationBuilder.Sql(@"
                CREATE UNIQUE INDEX IF NOT EXISTS UX_journey_route_quality_hourly
                ON statistics.journey_route_quality_hourly (
                    bucket_hour,
                    administration_id,
                    transport_type,
                    journey_description,
                    line,
                    number,
                    origin_eva_number,
                    destination_eva_number
                );
            ");

            migrationBuilder.Sql(@"
                CREATE INDEX IF NOT EXISTS IX_journey_route_quality_hourly_bucket_administration
                ON statistics.journey_route_quality_hourly (bucket_hour, administration_id);
            ");

            migrationBuilder.Sql(@"
                CREATE INDEX IF NOT EXISTS IX_journey_route_quality_hourly_line_route
                ON statistics.journey_route_quality_hourly (line, number, bucket_hour);
            ");
            #endregion

            #region StationEventQualityHourly View
            migrationBuilder.Sql(@"
                CREATE MATERIALIZED VIEW IF NOT EXISTS statistics.station_event_quality_hourly AS
                SELECT
                    date_trunc('hour', stop_place.planned_time) AS bucket_hour,
                    stop_place.station_eva_number,
                    stop_place.schedule_type,
                    journey.administration_id,
                    transport.transport_type,
                    count(*)::bigint AS event_count,
                    count(*) FILTER (WHERE stop_place.cancelled IS TRUE)::bigint AS cancelled_count,
                    count(*) FILTER (WHERE stop_place.cancelled IS NOT TRUE)::bigint AS delay_sample_count,
                    coalesce(sum(stop_place.delay) FILTER (WHERE stop_place.cancelled IS NOT TRUE), 0)::bigint AS delay_sum_seconds,
                    count(*) FILTER (
                        WHERE stop_place.cancelled IS NOT TRUE
                            AND stop_place.delay < 300
                    )::bigint AS punctual_5_count,
                    count(*) FILTER (
                        WHERE stop_place.cancelled IS NOT TRUE
                            AND stop_place.delay < 900
                    )::bigint AS punctual_15_count
                FROM core.journey_stop_places AS stop_place
                    JOIN core.journeys AS journey
                        ON journey.id = stop_place.journey_id
                            AND journey.date = stop_place.date
                    JOIN core.journey_transports AS transport
                        ON transport.journey_id = stop_place.journey_id
                            AND transport.date = stop_place.date
                GROUP BY
                    bucket_hour,
                    stop_place.station_eva_number,
                    stop_place.schedule_type,
                    journey.administration_id,
                    transport.transport_type;
            ");

            migrationBuilder.Sql(@"
                CREATE UNIQUE INDEX IF NOT EXISTS UX_station_event_quality_hourly
                ON statistics.station_event_quality_hourly (
                    bucket_hour,
                    station_eva_number,
                    schedule_type,
                    administration_id,
                    transport_type
                );
            ");

            migrationBuilder.Sql(@"
                CREATE INDEX IF NOT EXISTS IX_station_event_quality_hourly_station_bucket
                ON statistics.station_event_quality_hourly (station_eva_number, bucket_hour);
            ");

            migrationBuilder.Sql(@"
                CREATE INDEX IF NOT EXISTS IX_station_event_quality_hourly_bucket_administration
                ON statistics.station_event_quality_hourly (bucket_hour, administration_id);
            ");
            #endregion

            migrationBuilder.Sql(@"SELECT cron.schedule('refresh_journey_route_quality_hourly', '0 */6 * * *', 'REFRESH MATERIALIZED VIEW CONCURRENTLY statistics.journey_route_quality_hourly');");
            migrationBuilder.Sql(@"SELECT cron.schedule('refresh_station_event_quality_hourly', '0 */6 * * *', 'REFRESH MATERIALIZED VIEW CONCURRENTLY statistics.station_event_quality_hourly');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"SELECT cron.unschedule(jobname) FROM cron.job WHERE jobname IN (
                'refresh_journey_route_quality_hourly',
                'refresh_station_event_quality_hourly'
            );");

            migrationBuilder.Sql(@"DROP MATERIALIZED VIEW IF EXISTS statistics.station_event_quality_hourly;");
            migrationBuilder.Sql(@"DROP MATERIALIZED VIEW IF EXISTS statistics.journey_route_quality_hourly;");

            migrationBuilder.Sql(@"DROP INDEX IF EXISTS core.IX_journey_transports_line_number_date;");
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS core.IX_journey_stop_places_journey_date_planned_time;");
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS core.IX_journey_stop_places_station_date_planned_time;");

        }
    }
}
