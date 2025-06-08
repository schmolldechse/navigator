import { DateTime } from "luxon";
import { t } from "elysia";
import { DateTimeObject } from "../types/datetime";
import { database } from "../db/postgres";
import { journeys, journeyViaStops } from "../db/core.schema";
import { and, eq, sql } from "drizzle-orm";
import { between, inArray } from "drizzle-orm/sql/expressions/conditions";
import { StopAnalytics } from "../models/core/models";
import { StopAnalyticsSchema } from "../models/elysia/analytics.model";
import { StatisticsHelper } from "./statistics.helper";

class StatisticsService {
	private readonly helper: StatisticsHelper = new StatisticsHelper();

	// timetable changes
	readonly TIMETABLE_CHANGES: DateTime[] = [
		DateTime.fromObject({ day: 17, month: 12, year: 2024 }).startOf("day"),
		DateTime.fromObject({ day: 9, month: 6, year: 2025 }).startOf("day"),
	]

	public getLastTimetableChange = (compareTo?: DateTime): DateTime => {
		compareTo ??= DateTime.now();

		const filteredChanges = this.TIMETABLE_CHANGES.filter(change => change <= compareTo);
		if (filteredChanges.length === 0) return this.TIMETABLE_CHANGES.reduce((min, date) => date < min ? date : min, this.TIMETABLE_CHANGES[0]);
		return filteredChanges.reduce((max, date) => date > max ? date : max, filteredChanges[0]);
	}

	readonly statsBody = t.Object({
		startDate: t.Optional(
			DateTimeObject({
				fieldName: "startDate",
				default: this.getLastTimetableChange().toFormat("yyyy-MM-dd"),
				description: "Start date of the filter",
				error: "Parameter 'startDate' could not be parsed as a date."
			})
		),
		endDate: t.Optional(
			DateTimeObject({
				fieldName: "endDate",
				default: DateTime.now().toFormat("yyyy-MM-dd"),
				description: "End date of the filter",
				error: "Parameter 'endDate' could not be parsed as a date."
			})
		),
		filter: t.Object({
			delayThreshold: t.Number({
				default: 60,
				description: "Defines the threshold in seconds after which a delay is considered",
				minimum: 0,
				maximum: Number.MAX_SAFE_INTEGER
			}),
			products: t.Array(t.String(), { default: [], description: "The products to look for" }),
			lineName: t.Array(t.String(), {
				default: [],
				description: "The lineName to look for, e.g. ´MEX 12´"
			}),
			lineNumber: t.Array(t.String(), {
				default: [],
				description: "The exact lineNumber to look for, e.g. ´19974´"
			})
		})
	});

	startEvaluation = async (body: typeof this.statsBody.static, evaNumbers: number[]): Promise<StopAnalytics> => {
		const startTime = DateTime.now();

		const query = database
			.select({ journey: journeys, viaStops: journeyViaStops })
			.from(journeys)
			.innerJoin(journeyViaStops, eq(journeys.journeyId, journeyViaStops.journeyId));

		const conditions = [];
		if (body.filter.products.length > 0) conditions.push(inArray(journeys.productType, body.filter.products));
		if (body.filter.lineName.length > 0) conditions.push(inArray(journeys.journeyName, body.filter.lineName));
		if (body.filter.lineNumber.length > 0) conditions.push(inArray(journeys.journeyNumber, body.filter.lineNumber));

		conditions.push(between(sql`TO_DATE(SUBSTR(${journeys.journeyId}, 1, 8), 'YYYYMMDD')`, body.startDate, body.endDate));
		conditions.push(inArray(journeyViaStops.evaNumber, evaNumbers));

		const result = (await query.where(and(...conditions))) as unknown as {
			journey: typeof journeys;
			viaStops: typeof journeyViaStops;
		}[];

		return {
			executionTime: DateTime.now().diff(startTime, "milliseconds").milliseconds,
			relatedEvaNumbers: evaNumbers,
			totalJourneys: result.length,
			departure: this.gatherDepartures(
				body.filter,
				result.map((item) => item.viaStops)
			),
			arrival: this.gatherArrivals(
				body.filter,
				result.map((item) => item.viaStops)
			),
			cancellations: this.gatherCancellations(result.map((item) => item.viaStops)),
			products: this.gatherProducts(result.map((item) => item.journey))
		};
	};

	private gatherProducts = (journeyList: (typeof journeys)[]): typeof StopAnalyticsSchema.static.products => {
		const products: typeof StopAnalyticsSchema.static.products = [];

		for (const journey of journeyList) {
			if (!products.some((product) => product.type.toLowerCase() === String(journey.productType).toLowerCase())) {
				products.push({
					type: String(journey.productType),
					totalMeasured: 0,
					measurements: []
				});
			}
			const product = products.find(
				(product) => product.type.toLowerCase() === String(journey.productType).toLowerCase()
			);
			product!.totalMeasured += 1;

			const date = DateTime.fromFormat(String(journey.journeyId).substring(0, 8), "yyyyMMdd");
			if (!date.isValid) continue;

			const measurement = this.helper.getOrCreateDateMeasurement(product!.measurements, date);
			measurement.totalMeasured += 1;
		}

		return products;
	};

	private gatherCancellations = (viaStops: (typeof journeyViaStops)[]): typeof StopAnalyticsSchema.static.cancellations => {
		const cancellations = this.helper.initMeasurement();

		for (const viaStop of viaStops) {
			if (!viaStop.cancelled) continue;

			const date = DateTime.fromFormat(String(viaStop.journeyId).substring(0, 8), "yyyyMMdd");
			const measurement = this.helper.getOrCreateDateMeasurement(cancellations.measurements, date);
			measurement.totalMeasured += 1;
		}

		return {
			...cancellations,
			totalMeasured: this.helper.calculateTotalFromMeasurements(cancellations).totalMeasured
		};
	};

	private gatherArrivals = (
		filter: typeof this.statsBody.static.filter,
		viaStops: (typeof journeyViaStops)[]
	): typeof StopAnalyticsSchema.static.arrival =>
		this.helper.calculateTimeStatistics(filter.delayThreshold, viaStops, (stop) => ({
			actualTime: String(stop.arrivalActualTime),
			plannedTime: String(stop.arrivalPlannedTime),
			delay: Number(stop.arrivalDelay),
			actualPlatform: String(stop.arrivalActualPlatform),
			plannedPlatform: String(stop.arrivalPlannedPlatform)
		}));

	private gatherDepartures = (
		filter: typeof this.statsBody.static.filter,
		viaStops: (typeof journeyViaStops)[]
	): typeof StopAnalyticsSchema.static.departure =>
		this.helper.calculateTimeStatistics(filter.delayThreshold, viaStops, (stop) => ({
			actualTime: String(stop.departureActualTime),
			plannedTime: String(stop.departurePlannedTime),
			delay: Number(stop.departureDelay),
			actualPlatform: String(stop.departureActualPlatform),
			plannedPlatform: String(stop.departurePlannedPlatform)
		}));
}

export { StatisticsService };