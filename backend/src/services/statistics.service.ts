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
		const startTime = DateTime.now();

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

		return {
			executionTime: DateTime.now().diff(startTime, "milliseconds").milliseconds,
			relatedEvaNumbers: evaNumbers,
			totalJourneys: result.length,
			departure: this.gatherDepartures(body.filter, result.map(item => item.viaStops)),
			arrival: this.gatherArrivals(body.filter, result.map(item => item.viaStops)),
			cancellations: this.gatherCancellations(result.map(item => item.viaStops)),
			products: this.gatherProducts(result.map(item => item.journey))
		}
	};

	private gatherProducts = (journeyList: typeof journeys[]): typeof StopAnalyticsSchema.static.products => {
		const products: typeof StopAnalyticsSchema.static.products = [];

		for (const journey of journeyList) {
			if (!products.some(product => product.type.toLowerCase() === String(journey.productType).toLowerCase())) {
				products.push({
					type: String(journey.productType),
					totalMeasured: 0,
					measurements: []
				});
			}
			const product = products.find(product => product.type.toLowerCase() === String(journey.productType).toLowerCase());
			product!.totalMeasured += 1;

			const date = DateTime.fromFormat(String(journey.journeyId).substring(0, 8), "yyyyMMdd");
			if (!date.isValid) continue;

			if (!product!.measurements.some(measurement => measurement.date === date.toFormat("yyyy-MM-dd")))
				product!.measurements.push({
					date: date.toFormat("yyyy-MM-dd"),
					totalMeasured: 0
				});
			const measurement = product!.measurements.find(measurement => measurement.date === date.toFormat("yyyy-MM-dd"));
			measurement!.totalMeasured += 1;
		}

		return products;
	};

	private gatherCancellations = (viaStops: typeof journeyViaStops[]): typeof StopAnalyticsSchema.static.cancellations => {
		const cancellations: typeof StopAnalyticsSchema.static.cancellations = {
			totalMeasured: 0,
			measurements: []
		};

		for (const viaStop of viaStops) {
			if (!viaStop.cancelled) continue;
			cancellations.totalMeasured += 1;

			const date = DateTime.fromFormat(String(viaStop.journeyId).substring(0, 8), "yyyyMMdd");
			if (!date.isValid) continue;

			if (!cancellations!.measurements.some(measurement => measurement.date === date.toFormat("yyyy-MM-dd")))
				cancellations!.measurements.push({
					date: date.toFormat("yyyy-MM-dd"),
					totalMeasured: 0
				});
			const measurement = cancellations!.measurements.find(measurement => measurement.date === date.toFormat("yyyy-MM-dd"));
			measurement!.totalMeasured += 1;
		}

		return cancellations;
	}

	private gatherArrivals = (filter: typeof this.statsBody.static.filter, viaStops: typeof journeyViaStops[]): typeof StopAnalyticsSchema.static.arrival => {
		const arrival: typeof StopAnalyticsSchema.static.arrival = {
			total: {
				totalMeasured: 0,
				measurements: []
			},
			delay: {
				average: 0,
				minimum: Number.MAX_SAFE_INTEGER,
				maximum: Number.MIN_SAFE_INTEGER,
				measurements: []
			},
			tooEarly: {
				totalMeasured: 0,
				measurements: []
			},
			punctual: {
				totalMeasured: 0,
				measurements: []
			},
			delayed: {
				totalMeasured: 0,
				measurements: []
			},
			platformChanges: {
				totalMeasured: 0,
				measurements: []
			}
		};

		const calculateTotal = (date: DateTime): void => {
			if (!arrival.total!.measurements.some(measurement => measurement.date === date.toFormat("yyyy-MM-dd")))
				arrival.total!.measurements.push({
					date: date.toFormat("yyyy-MM-dd"),
					totalMeasured: 0
				});

			const measurement = arrival.total!.measurements.find(measurement => measurement.date === date.toFormat("yyyy-MM-dd"));
			measurement!.totalMeasured += 1;
		}
		const calculateDelay = (viaStop: typeof journeyViaStops, date: DateTime): void => {
			if(!arrival.delay!.measurements.some(measurement => measurement.date === date.toFormat("yyyy-MM-dd")))
				arrival.delay!.measurements.push({
					date: date.toFormat("yyyy-MM-dd"),
					totalMeasured: 0,
					average: 0,
					minimum: Number.MAX_SAFE_INTEGER,
					maximum: Number.MIN_SAFE_INTEGER
				});
			const measurement = arrival.delay!.measurements.find(measurement => measurement.date === date.toFormat("yyyy-MM-dd"));
			measurement!.totalMeasured += 1;
			measurement!.average += Number(viaStop.arrivalDelay);
			measurement!.minimum = Math.min(measurement!.minimum, Number(viaStop.arrivalDelay));
			measurement!.maximum = Math.max(measurement!.maximum, Number(viaStop.arrivalDelay));
		}
		const calculateTooEarly = (viaStop: typeof journeyViaStops, date: DateTime): void => {
			if(!arrival.tooEarly!.measurements.some(measurement => measurement.date === date.toFormat("yyyy-MM-dd")))
				arrival.tooEarly!.measurements.push({
					date: date.toFormat("yyyy-MM-dd"),
					totalMeasured: 0
				});

			if (Number(viaStop.arrivalDelay) > 0) return;
			const measurement = arrival.tooEarly!.measurements.find(measurement => measurement.date === date.toFormat("yyyy-MM-dd"));
			measurement!.totalMeasured += 1;
		}
		const calculatePunctual = (viaStop: typeof journeyViaStops, date: DateTime): void => {
			if(!arrival.punctual!.measurements.some(measurement => measurement.date === date.toFormat("yyyy-MM-dd")))
				arrival.punctual!.measurements.push({
					date: date.toFormat("yyyy-MM-dd"),
					totalMeasured: 0
				});

			if (Number(viaStop.arrivalDelay) < 0 || Number(viaStop.arrivalDelay) > filter.delayThreshold) return;
			const measurement = arrival.punctual!.measurements.find(measurement => measurement.date === date.toFormat("yyyy-MM-dd"));
			measurement!.totalMeasured += 1;
		}
		const calculateDelayed = (viaStop: typeof journeyViaStops, date: DateTime): void => {
			if(!arrival.delayed!.measurements.some(measurement => measurement.date === date.toFormat("yyyy-MM-dd")))
				arrival.delayed!.measurements.push({
					date: date.toFormat("yyyy-MM-dd"),
					totalMeasured: 0
				});

			if (Number(viaStop.arrivalDelay) <= filter.delayThreshold) return;
			const measurement = arrival.delayed!.measurements.find(measurement => measurement.date === date.toFormat("yyyy-MM-dd"));
			measurement!.totalMeasured += 1;
		}
		const calculatePlatformChanges = (viaStop: typeof journeyViaStops, date: DateTime): void => {
			if(!arrival.platformChanges!.measurements.some(measurement => measurement.date === date.toFormat("yyyy-MM-dd")))
				arrival.platformChanges!.measurements.push({
					date: date.toFormat("yyyy-MM-dd"),
					totalMeasured: 0
				});

			if (String(viaStop.arrivalPlannedPlatform) === String(viaStop.arrivalActualPlatform)) return;
			const measurement = arrival.platformChanges!.measurements.find(measurement => measurement.date === date.toFormat("yyyy-MM-dd"));
			measurement!.totalMeasured += 1;
		}

		for (const viaStop of viaStops) {
			if (!(viaStop.arrivalActualTime ?? viaStop.arrivalPlannedTime)) continue;

			const date = DateTime.fromFormat(String(viaStop.journeyId).substring(0, 8), "yyyyMMdd");
			if (!date.isValid) continue;

			calculateTotal(date);
			calculateDelay(viaStop, date);
			calculateTooEarly(viaStop, date);
			calculatePunctual(viaStop, date);
			calculateDelayed(viaStop, date);
			calculatePlatformChanges(viaStop, date);
		}

		// sum up totalMeasured for: total, tooEarly, punctual, delayed, platformChanges
		arrival.total!.totalMeasured = arrival.total!.measurements.reduce((acc, measurement) => acc + measurement.totalMeasured, 0);
		arrival.tooEarly!.totalMeasured = arrival.tooEarly!.measurements.reduce((acc, measurement) => acc + measurement.totalMeasured, 0);
		arrival.punctual!.totalMeasured = arrival.punctual!.measurements.reduce((acc, measurement) => acc + measurement.totalMeasured, 0);
		arrival.delayed!.totalMeasured = arrival.delayed!.measurements.reduce((acc, measurement) => acc + measurement.totalMeasured, 0);
		arrival.platformChanges!.totalMeasured = arrival.platformChanges!.measurements.reduce((acc, measurement) => acc + measurement.totalMeasured, 0);

		// calculate averages
		const delayTotalMeasured = arrival.delayed!.measurements.reduce((acc, measurement) => acc + measurement.totalMeasured, 0);
		arrival.delay!.average = arrival.delay!.measurements.reduce((acc, measurement) => acc + measurement.average, 0) / delayTotalMeasured;
		arrival.delay!.minimum = arrival.delay!.measurements.reduce((acc, measurement) => Math.min(acc, measurement.minimum), Number.MAX_SAFE_INTEGER);
		arrival.delay!.maximum = arrival.delay!.measurements.reduce((acc, measurement) => Math.max(acc, measurement.maximum), Number.MIN_SAFE_INTEGER);

		arrival.delay!.measurements.forEach(measurement => {
			if (measurement.totalMeasured === 0) return;
			measurement.average = measurement.average / measurement.totalMeasured;
		});

		// ensure non MIN_SAFE_INTEGER, MAX_SAFE_INTEGER & NaN values
		arrival.delay!.average = isNaN(arrival.delay!.average) ? 0 : arrival.delay!.average;
		arrival.delay!.minimum = arrival.delay!.minimum === Number.MAX_SAFE_INTEGER ? 0 : arrival.delay!.minimum;
		arrival.delay!.maximum = arrival.delay!.maximum === Number.MIN_SAFE_INTEGER ? 0 : arrival.delay!.maximum;
		arrival.delay!.measurements.forEach(measurement => {
			measurement.average = isNaN(measurement.average) ? 0 : measurement.average;
			measurement.minimum = measurement.minimum === Number.MAX_SAFE_INTEGER ? 0 : measurement.minimum;
			measurement.maximum = measurement.maximum === Number.MIN_SAFE_INTEGER ? 0 : measurement.maximum;
		});

		return arrival;
	}

	private gatherDepartures = (filter: typeof this.statsBody.static.filter, viaStops: typeof journeyViaStops[]): typeof StopAnalyticsSchema.static.departure => {
		const departure: typeof StopAnalyticsSchema.static.departure = {
			total: {
				totalMeasured: 0,
				measurements: []
			},
			delay: {
				average: 0,
				minimum: Number.MAX_SAFE_INTEGER,
				maximum: Number.MIN_SAFE_INTEGER,
				measurements: []
			},
			tooEarly: {
				totalMeasured: 0,
				measurements: []
			},
			punctual: {
				totalMeasured: 0,
				measurements: []
			},
			delayed: {
				totalMeasured: 0,
				measurements: []
			},
			platformChanges: {
				totalMeasured: 0,
				measurements: []
			}
		};

		const calculateTotal = (date: DateTime): void => {
			if (!departure.total!.measurements.some(measurement => measurement.date === date.toFormat("yyyy-MM-dd")))
				departure.total!.measurements.push({
					date: date.toFormat("yyyy-MM-dd"),
					totalMeasured: 0
				});

			const measurement = departure.total!.measurements.find(measurement => measurement.date === date.toFormat("yyyy-MM-dd"));
			measurement!.totalMeasured += 1;
		}
		const calculateDelay = (viaStop: typeof journeyViaStops, date: DateTime): void => {
			if(!departure.delay!.measurements.some(measurement => measurement.date === date.toFormat("yyyy-MM-dd")))
				departure.delay!.measurements.push({
					date: date.toFormat("yyyy-MM-dd"),
					totalMeasured: 0,
					average: 0,
					minimum: Number.MAX_SAFE_INTEGER,
					maximum: Number.MIN_SAFE_INTEGER
				});
			const measurement = departure.delay!.measurements.find(measurement => measurement.date === date.toFormat("yyyy-MM-dd"));
			measurement!.totalMeasured += 1;
			measurement!.average += Number(viaStop.arrivalDelay);
			measurement!.minimum = Math.min(measurement!.minimum, Number(viaStop.arrivalDelay));
			measurement!.maximum = Math.max(measurement!.maximum, Number(viaStop.arrivalDelay));
		}
		const calculateTooEarly = (viaStop: typeof journeyViaStops, date: DateTime): void => {
			if(!departure.tooEarly!.measurements.some(measurement => measurement.date === date.toFormat("yyyy-MM-dd")))
				departure.tooEarly!.measurements.push({
					date: date.toFormat("yyyy-MM-dd"),
					totalMeasured: 0
				});

			if (Number(viaStop.arrivalDelay) > 0) return;
			const measurement = departure.tooEarly!.measurements.find(measurement => measurement.date === date.toFormat("yyyy-MM-dd"));
			measurement!.totalMeasured += 1;
		}
		const calculatePunctual = (viaStop: typeof journeyViaStops, date: DateTime): void => {
			if(!departure.punctual!.measurements.some(measurement => measurement.date === date.toFormat("yyyy-MM-dd")))
				departure.punctual!.measurements.push({
					date: date.toFormat("yyyy-MM-dd"),
					totalMeasured: 0
				});

			if (Number(viaStop.arrivalDelay) < 0 || Number(viaStop.arrivalDelay) > filter.delayThreshold) return;
			const measurement = departure.punctual!.measurements.find(measurement => measurement.date === date.toFormat("yyyy-MM-dd"));
			measurement!.totalMeasured += 1;
		}
		const calculateDelayed = (viaStop: typeof journeyViaStops, date: DateTime): void => {
			if(!departure.delayed!.measurements.some(measurement => measurement.date === date.toFormat("yyyy-MM-dd")))
				departure.delayed!.measurements.push({
					date: date.toFormat("yyyy-MM-dd"),
					totalMeasured: 0
				});

			if (Number(viaStop.arrivalDelay) <= filter.delayThreshold) return;
			const measurement = departure.delayed!.measurements.find(measurement => measurement.date === date.toFormat("yyyy-MM-dd"));
			measurement!.totalMeasured += 1;
		}
		const calculatePlatformChanges = (viaStop: typeof journeyViaStops, date: DateTime): void => {
			if(!departure.platformChanges!.measurements.some(measurement => measurement.date === date.toFormat("yyyy-MM-dd")))
				departure.platformChanges!.measurements.push({
					date: date.toFormat("yyyy-MM-dd"),
					totalMeasured: 0
				});

			if (String(viaStop.arrivalPlannedPlatform) === String(viaStop.arrivalActualPlatform)) return;
			const measurement = departure.platformChanges!.measurements.find(measurement => measurement.date === date.toFormat("yyyy-MM-dd"));
			measurement!.totalMeasured += 1;
		}

		for (const viaStop of viaStops) {
			if (!(viaStop.departureActualTime ?? viaStop.departurePlannedTime)) continue;

			const date = DateTime.fromFormat(String(viaStop.journeyId).substring(0, 8), "yyyyMMdd");
			if (!date.isValid) continue;

			calculateTotal(date);
			calculateDelay(viaStop, date);
			calculateTooEarly(viaStop, date);
			calculatePunctual(viaStop, date);
			calculateDelayed(viaStop, date);
			calculatePlatformChanges(viaStop, date);
		}

		// sum up totalMeasured for: total, tooEarly, punctual, delayed, platformChanges
		departure.total!.totalMeasured = departure.total!.measurements.reduce((acc, measurement) => acc + measurement.totalMeasured, 0);
		departure.tooEarly!.totalMeasured = departure.tooEarly!.measurements.reduce((acc, measurement) => acc + measurement.totalMeasured, 0);
		departure.punctual!.totalMeasured = departure.punctual!.measurements.reduce((acc, measurement) => acc + measurement.totalMeasured, 0);
		departure.delayed!.totalMeasured = departure.delayed!.measurements.reduce((acc, measurement) => acc + measurement.totalMeasured, 0);
		departure.platformChanges!.totalMeasured = departure.platformChanges!.measurements.reduce((acc, measurement) => acc + measurement.totalMeasured, 0);

		// calculate averages
		const delayTotalMeasured = departure.delayed!.measurements.reduce((acc, measurement) => acc + measurement.totalMeasured, 0);
		departure.delay!.average = departure.delay!.measurements.reduce((acc, measurement) => acc + measurement.average, 0) / delayTotalMeasured;
		departure.delay!.minimum = departure.delay!.measurements.reduce((acc, measurement) => Math.min(acc, measurement.minimum), Number.MAX_SAFE_INTEGER);
		departure.delay!.maximum = departure.delay!.measurements.reduce((acc, measurement) => Math.max(acc, measurement.maximum), Number.MIN_SAFE_INTEGER);

		departure.delay!.measurements.forEach(measurement => {
			if (measurement.totalMeasured === 0) return;
			measurement.average = measurement.average / measurement.totalMeasured;
		});

		// ensure non MIN_SAFE_INTEGER, MAX_SAFE_INTEGER & NaN values
		departure.delay!.average = isNaN(departure.delay!.average) ? 0 : departure.delay!.average;
		departure.delay!.minimum = departure.delay!.minimum === Number.MAX_SAFE_INTEGER ? 0 : departure.delay!.minimum;
		departure.delay!.maximum = departure.delay!.maximum === Number.MIN_SAFE_INTEGER ? 0 : departure.delay!.maximum;
		departure.delay!.measurements.forEach(measurement => {
			measurement.average = isNaN(measurement.average) ? 0 : measurement.average;
			measurement.minimum = measurement.minimum === Number.MAX_SAFE_INTEGER ? 0 : measurement.minimum;
			measurement.maximum = measurement.maximum === Number.MIN_SAFE_INTEGER ? 0 : measurement.maximum;
		});

		return departure;
	}
}

export { StatisticsService };