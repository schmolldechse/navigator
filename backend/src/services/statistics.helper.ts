import {
	CompleteMeasurementSchema,
	DelayByDateMeasurementSchema,
	MeasurementByDateSchema,
	TimeStatisticsSchema
} from "../models/elysia/analytics.model";
import { DateTime } from "luxon";
import { journeyViaStops } from "../db/core.schema";

type MeasurementByDate = typeof MeasurementByDateSchema.static;
type CompleteMeasurement = typeof CompleteMeasurementSchema.static;

type TimeStatistics = typeof TimeStatisticsSchema.static;

type DelayMeasurement = typeof TimeStatisticsSchema.static.delay;
type DelayByDateMeasurement = typeof DelayByDateMeasurementSchema.static;

class StatisticsHelper {
	initMeasurement = (): CompleteMeasurement => ({
		totalMeasured: 0,
		measurements: []
	});

	initDelayMeasurement = (): DelayMeasurement => ({
		average: 0,
		minimum: Number.MAX_SAFE_INTEGER,
		maximum: Number.MIN_SAFE_INTEGER,
		measurements: []
	});

	getOrCreateDateMeasurement = (dateMeasurements: MeasurementByDate[], date: DateTime): MeasurementByDate => {
		const dateFormat = date.toFormat("yyyy-MM-dd");
		if (!dateMeasurements.some((measurement) => measurement.date === dateFormat))
			dateMeasurements.push({ date: dateFormat, totalMeasured: 0 });
		return dateMeasurements.find((measurement) => measurement.date === dateFormat)!;
	};

	getOrCreateDelayMeasurement = (dateMeasurements: DelayByDateMeasurement[], date: DateTime): DelayByDateMeasurement => {
		const dateFormat = date.toFormat("yyyy-MM-dd");
		if (!dateMeasurements.some((measurement) => measurement.date === dateFormat))
			dateMeasurements.push({
				date: dateFormat,
				totalMeasured: 0,
				average: 0,
				minimum: Number.MAX_SAFE_INTEGER,
				maximum: Number.MIN_SAFE_INTEGER
			});
		return dateMeasurements.find((measurement) => measurement.date === dateFormat)!;
	};

	calculateTotalFromMeasurements = (completeMeasurement: CompleteMeasurement): CompleteMeasurement => {
		return {
			...completeMeasurement,
			totalMeasured: completeMeasurement.measurements.reduce((acc, measurement) => acc + measurement.totalMeasured, 0)
		};
	};

	private normalizeValues = (delayMeasurement: DelayMeasurement): DelayMeasurement => ({
		...delayMeasurement,
		average: isNaN(delayMeasurement.average) ? 0 : delayMeasurement.average,
		minimum: delayMeasurement.minimum === Number.MAX_SAFE_INTEGER ? 0 : delayMeasurement.minimum,
		maximum: delayMeasurement.maximum === Number.MIN_SAFE_INTEGER ? 0 : delayMeasurement.maximum,
		measurements: delayMeasurement.measurements.map((measurement) => ({
			...measurement,
			average: isNaN(measurement.average) ? 0 : measurement.average,
			minimum: measurement.minimum === Number.MAX_SAFE_INTEGER ? 0 : measurement.minimum,
			maximum: measurement.maximum === Number.MIN_SAFE_INTEGER ? 0 : measurement.maximum
		}))
	});

	private processDelayMeasurements = (delayMeasurement: DelayMeasurement): DelayMeasurement => {
		const totalMeasured = delayMeasurement.measurements.reduce((acc, measurement) => acc + measurement.totalMeasured, 0);

		const copy: DelayMeasurement = { ...delayMeasurement };
		copy.average = delayMeasurement.measurements.reduce((acc, measurement) => acc + measurement.average, 0) / totalMeasured;
		copy.minimum = delayMeasurement.measurements.reduce(
			(acc, measurement) => Math.min(acc, measurement.minimum),
			Number.MAX_SAFE_INTEGER
		);
		copy.maximum = delayMeasurement.measurements.reduce(
			(acc, measurement) => Math.max(acc, measurement.maximum),
			Number.MIN_SAFE_INTEGER
		);

		copy.measurements = delayMeasurement.measurements.map((measurement) => ({
			...measurement,
			average: measurement.totalMeasured === 0 ? measurement.average : measurement.average / measurement.totalMeasured
		}));

		return this.normalizeValues(copy);
	};

	calculateTimeStatistics = (
		delayThreshold: number,
		viaStops: (typeof journeyViaStops)[],
		data: (stop: typeof journeyViaStops) => {
			actualTime: string;
			plannedTime: string;
			delay: number;
			actualPlatform?: string;
			plannedPlatform?: string;
		}
	): TimeStatistics => {
		const result: TimeStatistics = {
			total: this.initMeasurement(),
			delay: this.initDelayMeasurement(),
			tooEarly: this.initMeasurement(),
			punctual: this.initMeasurement(),
			delayed: this.initMeasurement(),
			platformChanges: this.initMeasurement()
		};

		for (const viaStop of viaStops) {
			if (viaStop.cancelled) continue;
			if (!(data(viaStop).actualTime ?? data(viaStop).plannedTime)) continue;

			const date = DateTime.fromFormat(String(viaStop.journeyId).substring(0, 8), "yyyyMMdd");
			if (!date.isValid) continue;

			// total
			const totalMeasurement = this.getOrCreateDateMeasurement(result.total.measurements, date);
			totalMeasurement.totalMeasured += 1;

			// delay
			const delayMeasurement = this.getOrCreateDelayMeasurement(result.delay.measurements, date);
			delayMeasurement.totalMeasured += 1;
			delayMeasurement.average += data(viaStop).delay;
			delayMeasurement.minimum = Math.min(delayMeasurement.minimum, data(viaStop).delay);
			delayMeasurement.maximum = Math.max(delayMeasurement.maximum, data(viaStop).delay);

			// tooEarly
			if (data(viaStop).delay < 0) {
				const tooEarlyMeasurement = this.getOrCreateDateMeasurement(result.tooEarly.measurements, date);
				tooEarlyMeasurement.totalMeasured += 1;
			}

			// punctual
			if (data(viaStop).delay >= 0 && data(viaStop).delay <= delayThreshold) {
				const punctualMeasurement = this.getOrCreateDateMeasurement(result.punctual.measurements, date);
				punctualMeasurement.totalMeasured += 1;
			}

			// delayed
			if (data(viaStop).delay > delayThreshold) {
				const delayedMeasurement = this.getOrCreateDateMeasurement(result.delayed.measurements, date);
				delayedMeasurement.totalMeasured += 1;
			}

			// platform changes
			if (
				data(viaStop).actualPlatform &&
				data(viaStop).plannedPlatform &&
				data(viaStop).actualPlatform !== data(viaStop).plannedPlatform
			) {
				const platformChangeMeasurement = this.getOrCreateDateMeasurement(result.platformChanges.measurements, date);
				platformChangeMeasurement.totalMeasured += 1;
			}
		}

		// calculate totalMeasured for each measurement
		result.total = this.calculateTotalFromMeasurements(result.total);
		result.delay = this.processDelayMeasurements(result.delay);
		result.tooEarly = this.calculateTotalFromMeasurements(result.tooEarly);
		result.punctual = this.calculateTotalFromMeasurements(result.punctual);
		result.delayed = this.calculateTotalFromMeasurements(result.delayed);
		result.platformChanges = this.calculateTotalFromMeasurements(result.platformChanges);

		// calculate averages for delay
		result.delay = this.processDelayMeasurements(result.delay);

		return result;
	};
}

export { StatisticsHelper };
