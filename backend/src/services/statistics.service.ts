import { DateTime } from "luxon";
import { t } from "elysia";
import { DateTimeObject } from "../types/datetime";
import { database } from "../db/postgres";
import { journeys, journeyViaStops } from "../db/core.schema";
import { and, eq, sql } from "drizzle-orm";
import { between, inArray } from "drizzle-orm/sql/expressions/conditions";
import { StopAnalytics } from "../models/core/models";
import { StopAnalyticsSchema } from "../models/elysia/analytics.model";

class StatisticsService {
	// timetable change
	readonly START_DATE: DateTime = DateTime.fromObject({ day: 17, month: 12, year: 2024 }).startOf("day");

	readonly statsBody = t.Object({
		startDate: t.Optional(
			DateTimeObject({
				fieldName: "startDate",
				default: this.START_DATE.toFormat("yyyy-MM-dd"),
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
		const query = database
			.select({ journey: journeys, viaStops: journeyViaStops })
			.from(journeys)
			.innerJoin(journeyViaStops, eq(journeys.journeyId, journeyViaStops.journeyId));

		const conditions = [];
		if (body.filter.products.length > 0) conditions.push(inArray(journeys.productType, body.filter.products));
		if (body.filter.lineName.length > 0)
			conditions.push(inArray(journeys.journeyName, body.filter.lineName));
		if (body.filter.lineNumber.length > 0)
			conditions.push(inArray(journeys.journeyNumber, body.filter.lineNumber));

		conditions.push(between(sql`TO_DATE(SUBSTR(${journeys.journeyId}, 1, 8), 'YYYYMMDD')`, body.startDate, body.endDate));
		conditions.push(inArray(journeyViaStops.evaNumber, evaNumbers));

		const result = await query.where(and(...conditions)) as unknown as { journey: typeof journeys, viaStops: typeof journeyViaStops }[];

		// console.dir(this.gatherProducts(result.map((resultElement) => resultElement.journey)), { depth: null });
		console.dir(this.gatherCancellations(result.map((resultElement) => resultElement.viaStops)), { depth: null });
	};

	private gatherProducts = (journeyList: typeof journeys[]): typeof StopAnalyticsSchema.static.products => {
		const products: typeof StopAnalyticsSchema.static.products = [];

		for (const journey of journeyList) {
			if (!products.some(product => product.type.toLowerCase() === String(journey.productType).toLowerCase())) {
				products.push({
					type: String(journey.productType),
					amount: 0,
					measurements: []
				});
			}
			const product = products.find(product => product.type.toLowerCase() === String(journey.productType).toLowerCase());
			product!.amount += 1;

			const date = DateTime.fromFormat(String(journey.journeyId).substring(0, 8), "yyyyMMdd");
			if (!date.isValid) continue;

			if (!product!.measurements.some(measurement => measurement.date === date.toFormat("yyyy-MM-dd"))) {
				product!.measurements.push({
					date: date.toFormat("yyyy-MM-dd"),
					amount: 0
				});
			}
			const measurement = product!.measurements.find(measurement => measurement.date === date.toFormat("yyyy-MM-dd"));
			measurement!.amount += 1;
		}

		return products;
	};

	private gatherCancellations = (viaStops: typeof journeyViaStops[]): typeof StopAnalyticsSchema.static.cancellations => {
		const cancellations: typeof StopAnalyticsSchema.static.cancellations = {
			amount: 0,
			measurements: []
		};

		for (const viaStop of viaStops) {
			if (!viaStop.cancelled) continue;
			cancellations.amount += 1;

			const date = DateTime.fromFormat(String(viaStop.journeyId).substring(0, 8), "yyyyMMdd");
			if (!date.isValid) continue;

			if (!cancellations!.measurements.some(measurement => measurement.date === date.toFormat("yyyy-MM-dd"))) {
				cancellations!.measurements.push({
					date: date.toFormat("yyyy-MM-dd"),
					amount: 0
				});
			}
			const measurement = cancellations!.measurements.find(measurement => measurement.date === date.toFormat("yyyy-MM-dd"));
			measurement!.amount += 1;
		}

		return cancellations;
	}
}

export { StatisticsService };


