import { DateTime } from "luxon";
import { t } from "elysia";
import { DateTimeObject } from "../types/datetime";
import { database } from "../db/postgres";
import { sql } from "drizzle-orm";
import { StatisticsDatabaseResult, StatisticsHelper } from "./statistics.helper";
import { Products } from "../models/core/products";
import { HttpError } from "../response/error";
import { HttpStatus } from "../response/status";
import { JourneyStatisticsSchema } from "../models/elysia/analytics.model";

class StatisticsService {
	private readonly helper: StatisticsHelper = new StatisticsHelper();

	// timetable changes
	readonly TIMETABLE_CHANGES: DateTime[] = [
		DateTime.fromObject({ day: 17, month: 12, year: 2024 }).startOf("day"),
		DateTime.fromObject({ day: 15, month: 6, year: 2025 }).startOf("day")
	];

	public getLastTimetableChange = (compareTo?: DateTime): DateTime => {
		compareTo ??= DateTime.now();

		const filteredChanges = this.TIMETABLE_CHANGES.filter((change) => change <= compareTo);
		if (filteredChanges.length === 0)
			return this.TIMETABLE_CHANGES.reduce((min, date) => (date < min ? date : min), this.TIMETABLE_CHANGES[0]);
		return filteredChanges.reduce((max, date) => (date > max ? date : max), filteredChanges[0]);
	};

	readonly statsBody = t.Object({
		evaNumbers: t.Array(t.Number({ description: "The evaNumber of a station." }), {
			description: "List of evaNumbers to filter the statistics by.",
			uniqueItems: true,
			default: [],
			error: () => {
				throw new HttpError(
					HttpStatus.HTTP_400_BAD_REQUEST,
					"Invalid evaNumbers provided. Must be an array of unique numbers."
				);
			}
		}),
		startDate: t.Optional(
			DateTimeObject({
				fieldName: "startDate",
				default: this.getLastTimetableChange().toFormat("yyyy-MM-dd"),
				description: "Start date of the filter (YYYY-MM-DD). Defaults to the last timetable change date.",
				error: "Parameter 'startDate' could not be parsed as a date."
			})
		),
		endDate: t.Optional(
			DateTimeObject({
				fieldName: "endDate",
				default: DateTime.now().toFormat("yyyy-MM-dd"),
				description: "End date of the filter (YYYY-MM-DD). Defaults to the current date.",
				error: "Parameter 'endDate' could not be parsed as a date."
			})
		),
		filter: t.Object({
			delayThreshold: t.Number({
				default: 60,
				description: "Defines the threshold in seconds after which a delay is considered.",
				minimum: 0,
				maximum: Number.MAX_SAFE_INTEGER
			}),
			products: t.Array(
				t.String({
					description: "The name of the product type which should be included in the statistics."
				}),
				{
					description: "List of product types to filter by. For example, `HOCHGESCHWINDIGKEITSZUEGE`.",
					default: [],
					uniqueItems: true
				}
			),
			lineName: t.String({ description: "An REGEX pattern which lineNames should be included in the statistics." }),
			lineNumber: t.String({ description: "An REGEX pattern which lineNumbers should be included in the statistics." })
		}),
		options: t.Object({
			enableRilLookup: t.Boolean({
				description:
					"Enables automatic inclusion of semantically related stations in journey searches. When activated, this option captures journeys from all stations sharing the same functional significance, such as both `Stuttgart Hbf` (8000096) and `Stuttgart Hbf (tief)` (8098096). This ensures comprehensive coverage of all transport services, including suburban rail journeys that primarly operate from subsidiary station levels.",
				default: true
			})
		})
	});

	startEvaluation = async (body: typeof this.statsBody.static): Promise<typeof JourneyStatisticsSchema.static> => {
		const startExecutionTime = DateTime.now();

		const startDate = body.startDate!.toFormat("yyyy-MM-dd");
		const endDate = body.endDate!.toFormat("yyyy-MM-dd");

		const evaNumberSQL =
			body.evaNumbers.length > 0
				? sql.raw(`ARRAY[${body.evaNumbers.map((evaNumber) => evaNumber).join(",")}]::integer[]`)
				: sql`NULL`;
		const productSQL =
			body.filter.products.length > 0
				? sql.raw(`ARRAY[${body.filter.products.map((product) => `'${product}'`).join(",")}]::varchar[]`)
				: sql`NULL`;

		const query = await database.execute(sql`
			WITH date_range AS (
    			SELECT generate_series(${startDate}::date, ${endDate}::date, '1 day'::interval)::date AS date
			),
			product_types AS (
    			SELECT DISTINCT product_type
    			FROM core.journeys
    			WHERE journey_date BETWEEN ${startDate}::date AND ${endDate}::date
    			AND (
        			${productSQL}::varchar[] IS NULL
        			OR coalesce(array_length(${productSQL}::varchar[], 1), 0) = 0
        			OR UPPER(product_type) = ANY(SELECT UPPER(unnest(${productSQL}::varchar[])))
    			)
				AND (
					${body.filter.lineName}::varchar IS NULL
				   	OR ${body.filter.lineName} = ''
				   	OR journeys.journey_name ~ ${body.filter.lineName}
				)
				AND (
					${body.filter.lineNumber}::varchar IS NULL
				   	OR ${body.filter.lineNumber} = ''
				   	OR journeys.journey_number ~ ${body.filter.lineNumber}
				)
			),
			date_product_combinations AS (
    			SELECT
        			range.date,
        			products.product_type
    			FROM date_range range
    			CROSS JOIN product_types products
			),
			statistics AS (
    			SELECT
        			journeys.journey_date AS date,
        			journeys.product_type AS product_type,
        			COUNT(via_stops.journey_id) AS stop_count,
        			COUNT(DISTINCT journeys.journey_id) AS unique_journeys,
    			    -- Arrival
					SUM(CASE
						WHEN via_stops.arrival_planned_time IS NOT NULL
						AND via_stops.arrival_actual_time IS NOT NULL
						THEN 1
						ELSE 0
					END) AS arrival_count,
					SUM(via_stops.arrival_delay) AS arrival_delay_sum,
					AVG(via_stops.arrival_delay) AS arrival_delay_avg,
					MIN(via_stops.arrival_delay) AS arrival_delay_min,
					MAX(via_stops.arrival_delay) AS arrival_delay_max,
					SUM(CASE
						WHEN via_stops.arrival_actual_platform IS NOT NULL
						AND via_stops.arrival_planned_platform IS NOT NULL
						AND via_stops.arrival_actual_platform <> via_stops.arrival_planned_platform
						THEN 1
						ELSE 0
					END) AS arrival_platform_changes,
					SUM(CASE
						WHEN via_stops.arrival_delay < 0
						THEN 1
						ELSE 0
					END) AS arrival_too_early_count,
					SUM(CASE
						WHEN via_stops.arrival_delay >= 0 AND via_stops.arrival_delay <= ${body.filter.delayThreshold}::integer
						THEN 1
						ELSE 0
					END) AS arrival_punctual_count,
					SUM(CASE
						WHEN via_stops.arrival_delay > ${body.filter.delayThreshold}::integer
						THEN 1
						ELSE 0
					END) AS arrival_delayed_count,
					-- Departure
					SUM(CASE
						WHEN via_stops.departure_planned_time IS NOT NULL
						AND via_stops.departure_actual_time IS NOT NULL
						THEN 1
						ELSE 0
					END) AS departure_count,
					SUM(via_stops.departure_delay) AS departure_delay_sum,
					AVG(via_stops.departure_delay) AS departure_delay_avg,
					MIN(via_stops.departure_delay) AS departure_delay_min,
					MAX(via_stops.departure_delay) AS departure_delay_max,
					SUM(CASE
						WHEN via_stops.departure_actual_platform IS NOT NULL
						AND via_stops.departure_planned_platform IS NOT NULL
						AND via_stops.departure_actual_platform <> via_stops.departure_planned_platform
						THEN 1
						ELSE 0
					END) AS departure_platform_changes,
					SUM(CASE
						WHEN via_stops.departure_delay < 0
						THEN 1
						ELSE 0
					END) AS departure_too_early_count,
					SUM(CASE
						WHEN via_stops.departure_delay >= 0 AND via_stops.departure_delay <= ${body.filter.delayThreshold}::integer
						THEN 1
						ELSE 0
					END) AS departure_punctual_count,
					SUM(CASE
						WHEN via_stops.departure_delay > ${body.filter.delayThreshold}::integer
						THEN 1
						ELSE 0
					END) AS departure_delayed_count,
					-- Core
					SUM(CASE
						WHEN via_stops.cancelled IS TRUE
						THEN 1
						ELSE 0
					END) AS cancellations
				FROM core.journeys AS journeys
				JOIN core."journey_via-stops" AS via_stops ON journeys.journey_date = via_stops.journey_date
    			AND journeys.journey_id = via_stops.journey_id
    			WHERE journeys.journey_date BETWEEN ${startDate}::date AND ${endDate}::date
    			AND (
					${evaNumberSQL}::integer[] IS NULL
        			OR coalesce(array_length(${evaNumberSQL}::integer[], 1), 0) = 0
        			OR via_stops.eva_number = ANY(${evaNumberSQL}::integer[])
    			)
    			GROUP BY journeys.journey_date, journeys.product_type
			)
			SELECT
				-- General
    			combinations.date,
			    combinations.product_type,
			    -- Product occurrences
    			COALESCE(statistics.stop_count, 0)::integer AS stop_count,
    			COALESCE(statistics.unique_journeys, 0)::integer AS unique_journeys,
				-- Arrival
				COALESCE(statistics.arrival_count, 0)::integer AS arrival_count,
				COALESCE(statistics.arrival_delay_sum, 0)::integer AS arrival_delay_sum,
				COALESCE(statistics.arrival_delay_avg, 0)::double precision AS arrival_delay_avg,
				COALESCE(statistics.arrival_delay_min, 0)::double precision AS arrival_delay_min,
				COALESCE(statistics.arrival_delay_max, 0)::double precision AS arrival_delay_max,
				COALESCE(statistics.arrival_platform_changes, 0)::integer AS arrival_platform_changes,
				COALESCE(statistics.arrival_too_early_count, 0)::integer AS arrival_too_early_count,
				COALESCE(statistics.arrival_punctual_count, 0)::integer AS arrival_punctual_count,
				COALESCE(statistics.arrival_delayed_count, 0)::integer AS arrival_delayed_count,
				-- Departure
				COALESCE(statistics.departure_count, 0)::integer AS departure_count,
				COALESCE(statistics.departure_delay_sum, 0)::integer AS departure_delay_sum,
				COALESCE(statistics.departure_delay_avg, 0)::double precision AS departure_delay_avg,
				COALESCE(statistics.departure_delay_min, 0)::double precision AS departure_delay_min,
				COALESCE(statistics.departure_delay_max, 0)::double precision AS departure_delay_max,
				COALESCE(statistics.departure_platform_changes, 0)::integer AS departure_platform_changes,
				COALESCE(statistics.departure_too_early_count, 0)::integer AS departure_too_early_count,
				COALESCE(statistics.departure_punctual_count, 0)::integer AS departure_punctual_count,
				COALESCE(statistics.departure_delayed_count, 0)::integer AS departure_delayed_count,
				-- Core Statistics
				COALESCE(statistics.cancellations, 0)::integer AS cancellations
			FROM date_product_combinations combinations
			LEFT JOIN statistics ON (combinations.date = statistics.date) AND (combinations.product_type = statistics.product_type)
			ORDER BY combinations.date, combinations.product_type;
		`);
		if (!query.rows || query.rows.length === 0)
			throw new HttpError(HttpStatus.HTTP_502_BAD_GATEWAY, "No data found for the given parameters.");

		const rows = query.rows as unknown as StatisticsDatabaseResult[];
		const statistics = this.helper.calculateStatistics(rows);

		return {
			executionTime: DateTime.now().diff(startExecutionTime, ["milliseconds"]).milliseconds,
			products: statistics.products,
			arrival: statistics.arrival,
			departure: statistics.departure,
			cancellations: statistics.cancellations
		};
	};
}

export { StatisticsService };
