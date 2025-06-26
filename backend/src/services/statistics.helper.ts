import { DateTime } from "luxon";
import {
	CancellationSchema,
	DelayMeasurementStatisticSchema,
	GeneralMeasurementSchema,
	ProductOccurrenceInMeasurementSchema,
	ProductOccurrenceSchema,
	SingleProductDelayStatisticSchema,
	SingleProductInGeneralMeasurementSchema,
	TimeStatisticsSchema
} from "../models/elysia/analytics.model";

type StatisticsDatabaseResult = {
	date: string;
	product_type: string;
	stop_count: number;
	unique_journeys: number;
	arrival_count: number;
	arrival_delay_sum: number;
	arrival_delay_avg: number;
	arrival_delay_min: number;
	arrival_delay_max: number;
	arrival_platform_changes: number;
	arrival_too_early_count: number;
	arrival_punctual_count: number;
	arrival_delayed_count: number;
	departure_count: number;
	departure_delay_sum: number;
	departure_delay_avg: number;
	departure_delay_min: number;
	departure_delay_max: number;
	departure_platform_changes: number;
	departure_too_early_count: number;
	departure_punctual_count: number;
	departure_delayed_count: number;
	cancellations: number;
};

type OverallDelayCalculation = {
	date: DateTime;
	type: "arrival" | "departure";
	delaySum: number;
	totalCount: number;
};

class StatisticsHelper {
	calculateStatistics = (
		rows: StatisticsDatabaseResult[]
	): {
		products: typeof ProductOccurrenceSchema.static;
		arrival: typeof TimeStatisticsSchema.static;
		departure: typeof TimeStatisticsSchema.static;
		cancellations: typeof CancellationSchema.static;
	} => {
		const products: typeof ProductOccurrenceSchema.static = {
			totalStops: 0,
			totalUniqueJourneys: 0,
			measurements: []
		};
		let arrival: typeof TimeStatisticsSchema.static = {
			total: this.initMeasurement(),
			delay: this.initDelayMeasurement(),
			tooEarly: this.initMeasurement(),
			punctual: this.initMeasurement(),
			delayed: this.initMeasurement(),
			platformChanges: this.initMeasurement()
		};
		let departure: typeof TimeStatisticsSchema.static = {
			total: this.initMeasurement(),
			delay: this.initDelayMeasurement(),
			tooEarly: this.initMeasurement(),
			punctual: this.initMeasurement(),
			delayed: this.initMeasurement(),
			platformChanges: this.initMeasurement()
		};
		const cancellations: typeof CancellationSchema.static = this.initMeasurement();

		// used to calculate the average delay for each date & overall
		const delays: OverallDelayCalculation[] = [];

		for (const row of rows) {
			const date = DateTime.fromFormat(row.date, "yyyy-MM-dd");
			if (!date.isValid) continue;

			// product occurrences
			this.processProduct(products.measurements, date, row.product_type, row.stop_count, row.unique_journeys);

			// arrival statistics
			this.processTimeStatistics(arrival, "arrival", row, date);
			delays.push({ date, type: "arrival", delaySum: row.arrival_delay_sum, totalCount: row.arrival_count });

			// departure statistics
			this.processTimeStatistics(departure, "departure", row, date);
			delays.push({
				date,
				type: "departure",
				delaySum: row.departure_delay_sum,
				totalCount: row.departure_count
			});

			// cancellations
			this.processCancellations(cancellations, date, row.product_type, row.cancellations);
		}

		// sum up total values
		products.measurements.forEach((measurement) => {
			measurement.totalStops = measurement.products.reduce((acc, product) => acc + product.stopCount, 0);
			measurement.totalUniqueJourneys = measurement.products.reduce((acc, product) => acc + product.uniqueJourneys, 0);
		});
		products.totalStops = products.measurements.reduce((acc, measurement) => acc + measurement.totalStops, 0);
		products.totalUniqueJourneys = products.measurements.reduce(
			(acc, measurement) => acc + measurement.totalUniqueJourneys,
			0
		);

		arrival = this.normalizeTimeStatistics(
			arrival,
			delays.filter((delay) => delay.type === "arrival")
		);
		departure = this.normalizeTimeStatistics(
			departure,
			delays.filter((delay) => delay.type === "departure")
		);

		cancellations.measurements = cancellations.measurements.map((measurement) =>
			this.calculateTotalFromMeasurements(measurement)
		);
		cancellations.total = cancellations.measurements.reduce((acc, measurement) => acc + measurement.total, 0);

		return {
			products,
			arrival,
			departure,
			cancellations
		};
	};

	private initMeasurement = (): typeof TimeStatisticsSchema.static.total => ({
		total: 0,
		measurements: []
	});

	private initDelayMeasurement = (): typeof TimeStatisticsSchema.static.delay => ({
		average: 0,
		minimum: Number.MAX_SAFE_INTEGER,
		maximum: Number.MIN_SAFE_INTEGER,
		measurements: []
	});

	private processCancellations = (
		measurements: typeof CancellationSchema.static,
		date: DateTime,
		productType: string,
		cancellations: number
	): typeof CancellationSchema.static => {
		const dateFormat = date.toFormat("yyyy-MM-dd");

		// check if the date already exists
		if (!measurements.measurements.some((dateMeasurement) => dateMeasurement.date === dateFormat))
			measurements.measurements.push({ date: dateFormat, total: 0, products: [] });

		const dateMeasurement = measurements.measurements.find((dateMeasurement) => dateMeasurement.date === dateFormat)!;

		// check if the product type already exists
		if (!dateMeasurement.products.some((product) => product.productType === productType)) {
			dateMeasurement.products.push({
				productType,
				total: 0
			});
		}

		// update the product measurement
		const productMeasurement = dateMeasurement.products.find((product) => product.productType === productType)!;
		productMeasurement.total += cancellations;
		return measurements;
	};

	private processProduct = (
		measurements: (typeof ProductOccurrenceInMeasurementSchema.static)[],
		date: DateTime,
		productType: string,
		stopCount: number,
		uniqueJourneys: number
	): typeof ProductOccurrenceInMeasurementSchema.static => {
		const dateFormat = date.toFormat("yyyy-MM-dd");

		// check if the date already exists
		if (!measurements.some((dateMeasurement) => dateMeasurement.date === dateFormat))
			measurements.push({ date: dateFormat, totalStops: 0, totalUniqueJourneys: 0, products: [] });

		const dateMeasurement = measurements.find((dateMeasurement) => dateMeasurement.date === dateFormat)!;

		// check if the product type already exists
		if (!dateMeasurement.products.some((product) => product.productType === productType)) {
			dateMeasurement.products.push({
				productType,
				stopCount: 0,
				uniqueJourneys: 0
			});
		}

		// update the product measurement
		const productMeasurement = dateMeasurement.products.find((product) => product.productType === productType)!;
		productMeasurement.stopCount += stopCount;
		productMeasurement.uniqueJourneys += uniqueJourneys;
		return dateMeasurement;
	};

	private processTimeStatistics = (
		timeStatistics: typeof TimeStatisticsSchema.static,
		type: "arrival" | "departure",
		row: StatisticsDatabaseResult,
		date: DateTime
	) => {
		const isDeparture = type === "departure";

		// total
		const totalMeasurement = this.getOrCreateDateMeasurement(timeStatistics.total.measurements, date);
		if (isDeparture ? row.departure_count : row.arrival_count) {
			const productInMeasurement = this.getOrCreateSingleProductInMeasurement(
				totalMeasurement.products,
				row.product_type
			);
			productInMeasurement.total += isDeparture ? row.departure_count : row.arrival_count;
		}

		const delayMeasurement = this.getOrCreateDelayMeasurement(timeStatistics.delay.measurements, date);
		if (isDeparture ? row.departure_delay_sum : row.arrival_delay_sum) {
			const productInMeasurement = this.getOrCreateSingleProductDelayMeasurement(
				delayMeasurement.products,
				row.product_type
			);
			productInMeasurement.average = isDeparture ? row.departure_delay_avg : row.arrival_delay_avg;
			productInMeasurement.minimum = isDeparture ? row.departure_delay_min : row.arrival_delay_min;
			productInMeasurement.maximum = isDeparture ? row.departure_delay_max : row.arrival_delay_max;
		}

		// tooEarly
		const tooEarlyMeasurement = this.getOrCreateDateMeasurement(timeStatistics.tooEarly.measurements, date);
		if (isDeparture ? row.departure_too_early_count : row.arrival_too_early_count) {
			const productInMeasurement = this.getOrCreateSingleProductInMeasurement(
				tooEarlyMeasurement.products,
				row.product_type
			);
			productInMeasurement.total += isDeparture ? row.departure_too_early_count : row.arrival_too_early_count;
		}

		// punctual
		const punctualMeasurement = this.getOrCreateDateMeasurement(timeStatistics.punctual.measurements, date);
		if (isDeparture ? row.departure_punctual_count : row.arrival_punctual_count) {
			const productInMeasurement = this.getOrCreateSingleProductInMeasurement(
				punctualMeasurement.products,
				row.product_type
			);
			productInMeasurement.total += isDeparture ? row.departure_punctual_count : row.arrival_punctual_count;
		}

		// delayed
		const delayedMeasurement = this.getOrCreateDateMeasurement(timeStatistics.delayed.measurements, date);
		if (isDeparture ? row.departure_delayed_count : row.arrival_delayed_count) {
			const productInMeasurement = this.getOrCreateSingleProductInMeasurement(
				delayedMeasurement.products,
				row.product_type
			);
			productInMeasurement.total += isDeparture ? row.departure_delayed_count : row.arrival_delayed_count;
		}

		// platform changes
		const platformChangesMeasurement = this.getOrCreateDateMeasurement(timeStatistics.platformChanges.measurements, date);
		if (isDeparture ? row.departure_platform_changes : row.arrival_platform_changes) {
			const productInMeasurement = this.getOrCreateSingleProductInMeasurement(
				platformChangesMeasurement.products,
				row.product_type
			);
			productInMeasurement.total += isDeparture ? row.departure_platform_changes : row.arrival_platform_changes;
		}
	};

	getOrCreateSingleProductInMeasurement = (
		productsInMeasurement: (typeof SingleProductInGeneralMeasurementSchema.static)[],
		productType: string
	): typeof SingleProductInGeneralMeasurementSchema.static => {
		if (!productsInMeasurement.some((product) => product.productType === productType))
			productsInMeasurement.push({ productType, total: 0 });
		return productsInMeasurement.find((product) => product.productType === productType)!;
	};

	getOrCreateDateMeasurement = (
		dateMeasurements: (typeof GeneralMeasurementSchema.static)[],
		date: DateTime
	): typeof GeneralMeasurementSchema.static => {
		const dateFormat = date.toFormat("yyyy-MM-dd");
		if (!dateMeasurements.some((measurement) => measurement.date === dateFormat))
			dateMeasurements.push({ date: dateFormat, total: 0, products: [] });
		return dateMeasurements.find((measurement) => measurement.date === dateFormat)!;
	};

	getOrCreateSingleProductDelayMeasurement = (
		productsInMeasurement: (typeof SingleProductDelayStatisticSchema.static)[],
		productType: string
	): typeof SingleProductDelayStatisticSchema.static => {
		if (!productsInMeasurement.some((product) => product.productType === productType))
			productsInMeasurement.push({
				productType,
				average: 0,
				minimum: Number.MAX_SAFE_INTEGER,
				maximum: Number.MIN_SAFE_INTEGER
			});
		return productsInMeasurement.find((product) => product.productType === productType)!;
	};

	getOrCreateDelayMeasurement = (
		dateMeasurements: (typeof DelayMeasurementStatisticSchema.static)[],
		date: DateTime
	): typeof DelayMeasurementStatisticSchema.static => {
		const dateFormat = date.toFormat("yyyy-MM-dd");
		if (!dateMeasurements.some((measurement) => measurement.date === dateFormat))
			dateMeasurements.push({
				date: dateFormat,
				minimum: Number.MAX_SAFE_INTEGER,
				maximum: Number.MIN_SAFE_INTEGER,
				average: 0,
				products: []
			});
		return dateMeasurements.find((measurement) => measurement.date === dateFormat)!;
	};

	calculateTotalFromMeasurements = (
		dateMeasurements: typeof GeneralMeasurementSchema.static
	): typeof GeneralMeasurementSchema.static => ({
		...dateMeasurements,
		total: dateMeasurements.products.reduce((acc, product) => acc + product.total, 0)
	});

	private normalizeTimeStatistics = (
		timeStatistics: typeof TimeStatisticsSchema.static,
		delays: OverallDelayCalculation[]
	): typeof TimeStatisticsSchema.static => {
		timeStatistics.total.measurements = timeStatistics.total.measurements.map((measurement) =>
			this.calculateTotalFromMeasurements(measurement)
		);
		timeStatistics.total.total = timeStatistics.total.measurements.reduce((acc, measurement) => acc + measurement.total, 0);

		timeStatistics.delay.measurements = timeStatistics.delay.measurements.map((measurement) => {
			const delaysOnDate = delays.filter((delay) => delay.date.toFormat("yyyy-MM-dd") === measurement.date);

			measurement.average =
				delaysOnDate.reduce((acc, delay) => acc + delay.delaySum, 0) /
				delaysOnDate.reduce((acc, delay) => acc + delay.totalCount, 0);
			measurement.minimum = measurement.products.reduce(
				(acc, product) => Math.min(acc, product.minimum),
				Number.MAX_SAFE_INTEGER
			);
			measurement.maximum = measurement.products.reduce(
				(acc, product) => Math.max(acc, product.maximum),
				Number.MIN_SAFE_INTEGER
			);
			return measurement;
		});
		timeStatistics.delay.average =
			delays.reduce((acc, delay) => acc + delay.delaySum, 0) / delays.reduce((acc, delay) => acc + delay.totalCount, 0);
		timeStatistics.delay.minimum = timeStatistics.delay.measurements.reduce(
			(acc, measurement) => Math.min(acc, measurement.minimum),
			Number.MAX_SAFE_INTEGER
		);
		timeStatistics.delay.maximum = timeStatistics.delay.measurements.reduce(
			(acc, measurement) => Math.max(acc, measurement.maximum),
			Number.MIN_SAFE_INTEGER
		);

		// normalize
		timeStatistics.delay.measurements.forEach((measurement) => {
			measurement.products = measurement.products.map((product) => ({
				...product,
				average: isNaN(product.average) ? 0 : product.average,
				minimum: product.minimum === Number.MAX_SAFE_INTEGER ? 0 : product.minimum,
				maximum: product.maximum === Number.MIN_SAFE_INTEGER ? 0 : product.maximum
			}));

			measurement.average = isNaN(measurement.average) ? 0 : measurement.average;
			measurement.minimum = measurement.minimum === Number.MAX_SAFE_INTEGER ? 0 : measurement.minimum;
			measurement.maximum = measurement.maximum === Number.MIN_SAFE_INTEGER ? 0 : measurement.maximum;
		});
		timeStatistics.delay.average = isNaN(timeStatistics.delay.average) ? 0 : timeStatistics.delay.average;
		timeStatistics.delay.minimum =
			timeStatistics.delay.minimum === Number.MAX_SAFE_INTEGER ? 0 : timeStatistics.delay.minimum;
		timeStatistics.delay.maximum =
			timeStatistics.delay.maximum === Number.MIN_SAFE_INTEGER ? 0 : timeStatistics.delay.maximum;

		timeStatistics.tooEarly.measurements = timeStatistics.tooEarly.measurements.map((measurement) =>
			this.calculateTotalFromMeasurements(measurement)
		);
		timeStatistics.tooEarly.total = timeStatistics.tooEarly.measurements.reduce(
			(acc, measurement) => acc + measurement.total,
			0
		);

		timeStatistics.punctual.measurements = timeStatistics.punctual.measurements.map((measurement) =>
			this.calculateTotalFromMeasurements(measurement)
		);
		timeStatistics.punctual.total = timeStatistics.punctual.measurements.reduce(
			(acc, measurement) => acc + measurement.total,
			0
		);

		timeStatistics.delayed.measurements = timeStatistics.delayed.measurements.map((measurement) =>
			this.calculateTotalFromMeasurements(measurement)
		);
		timeStatistics.delayed.total = timeStatistics.delayed.measurements.reduce(
			(acc, measurement) => acc + measurement.total,
			0
		);

		timeStatistics.platformChanges.measurements = timeStatistics.platformChanges.measurements.map((measurement) =>
			this.calculateTotalFromMeasurements(measurement)
		);
		timeStatistics.platformChanges.total = timeStatistics.platformChanges.measurements.reduce(
			(acc, measurement) => acc + measurement.total,
			0
		);

		return timeStatistics;
	};
}

export { StatisticsDatabaseResult, StatisticsHelper };
