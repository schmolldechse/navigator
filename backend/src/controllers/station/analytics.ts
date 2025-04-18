import * as fs from "node:fs";
import path from "node:path";
import type { Connection } from "../../models/connection.ts";
import type { StopAnalytics } from "../../models/station.ts";
import { DateTime } from "luxon";
import calculateDuration from "../../lib/time.ts";
import type { ConnectionDocument } from "../../db/mongodb/station.schema.ts";

const analyzeStation = async (
	saveDir: string,
	evaNumbers: number[],
	options: { startDate: DateTime; endDate: DateTime },
	filter: { delayThreshold: number, products: string[]; lineName: string[]; lineNumber: string[] }
): Promise<StopAnalytics> => {
	if (!fs.existsSync(saveDir)) throw new Error(`Statistics for ${evaNumbers} do not exist`);

	const files = fs
		.readdirSync(saveDir)
		.filter((file) => file.endsWith(".json"))
		.filter((file) => file !== "manifest.json")
		.map((file) => path.join(saveDir, file));
	console.log(`Found ${files.length} JSON files to analyze in ${saveDir}`);

	const filePromises = files.map(async (filePath) => {
		const content = fs.readFileSync(filePath, { encoding: "utf8" });
		return analyzeConnections(evaNumbers, JSON.parse(content) as Connection[], options, filter);
	});
	const results: StopAnalytics[] = await Promise.all(filePromises);

	const mergedProducts: Record<string, number> = {};
	results.forEach((result) =>
		Object.entries(result.products).forEach(
			([product, count]) => (mergedProducts[product] = (mergedProducts[product] ?? 0) + count)
		)
	);

	const totalArrivals = results.reduce((sum, item) => sum + item.arrival.total, 0);
	const totalDepartures = results.reduce((sum, item) => sum + item.departure.total, 0);

	return new Promise((resolve, reject) =>
		resolve({
			relatedEvaNumbers: evaNumbers,
			parsingSucceeded: results.reduce((sum, item) => sum + item.parsingSucceeded, 0),
			parsingFailed: results.reduce((sum, item) => sum + item.parsingFailed, 0),
			products: mergedProducts,
			arrival: {
				total: totalArrivals,
				averageDelay:
					Math.round((results.reduce((sum, item) => sum + item.arrival.averageDelay, 0) / totalArrivals) * 100) / 100,
				minimumDelay: Math.min(...results.flatMap((item) => item.arrival.minimumDelay)),
				maximumDelay: Math.max(...results.flatMap((item) => item.arrival.maximumDelay)),
				totalTooEarly: results.reduce((sum, item) => sum + item.arrival.totalTooEarly, 0),
				totalPunctual: results.reduce((sum, item) => sum + item.arrival.totalPunctual, 0),
				totalDelayed: results.reduce((sum, item) => sum + item.arrival.totalDelayed, 0),
				totalPlatformChanges: results.reduce((sum, item) => sum + item.arrival.totalPlatformChanges, 0)
			},
			departure: {
				total: totalDepartures,
				averageDelay:
					Math.round((results.reduce((sum, item) => sum + item.departure.averageDelay, 0) / totalDepartures) * 100) /
					100,
				minimumDelay: Math.min(...results.flatMap((item) => item.departure.minimumDelay)),
				maximumDelay: Math.max(...results.flatMap((item) => item.departure.maximumDelay)),
				totalTooEarly: results.reduce((sum, item) => sum + item.departure.totalTooEarly, 0),
				totalPunctual: results.reduce((sum, item) => sum + item.departure.totalPunctual, 0),
				totalDelayed: results.reduce((sum, item) => sum + item.departure.totalDelayed, 0),
				totalPlatformChanges: results.reduce((sum, item) => sum + item.departure.totalPlatformChanges, 0)
			},
			cancellations: results.reduce((sum, item) => sum + item.cancellations, 0)
		})
	);
};

const analyzeConnections = async (
	relevantEvaNumbers: number[],
	connections: ConnectionDocument[],
	options: { startDate: DateTime; endDate: DateTime },
	filter: { delayThreshold: number, products: string[]; lineName: string[]; lineNumber: string[] }
): Promise<StopAnalytics> => {
	const analytics: StopAnalytics = {
		products: {},
		relatedEvaNumbers: relevantEvaNumbers,
		parsingFailed: 0,
		parsingSucceeded: 0,
		arrival: {
			total: 0,
			averageDelay: 0,
			minimumDelay: 0,
			maximumDelay: 0,
			totalTooEarly: 0,
			totalPunctual: 0,
			totalDelayed: 0,
			totalPlatformChanges: 0
		},
		departure: {
			total: 0,
			averageDelay: 0,
			minimumDelay: 0,
			maximumDelay: 0,
			totalTooEarly: 0,
			totalPunctual: 0,
			totalDelayed: 0,
			totalPlatformChanges: 0
		},
		cancellations: 0
	};

	const isInBetween = (check: DateTime, options: { startDate: DateTime; endDate: DateTime }): boolean => {
		if (!check.isValid) return false;
		return options.startDate <= check && check <= options.endDate;
	};

	connections.forEach((connection: ConnectionDocument) => {
		const date = DateTime.fromFormat(connection?._id?.toString().split("-")[0] ?? "", "yyyyMMdd");
		if (!isInBetween(date, options)) return;

		const product = connection?.lineInformation?.type || "UNKNOWN";

		if (filter.products.length > 0 && !filter.products.includes(product)) return;
		if (filter.lineName.length > 0 && !filter.lineName.includes(connection?.lineInformation?.lineName ?? "")) return;
		if (filter.lineNumber.length > 0 && !filter.lineNumber.includes(connection?.lineInformation?.fahrtNr ?? "")) return;

		analytics.products[product] = (analytics.products[product] ?? 0) + 1;

		const relevantStop = connection?.viaStops?.find(
			(stop) => stop?.evaNumber && relevantEvaNumbers.includes(stop?.evaNumber)
		);
		if (!relevantStop) {
			analytics.parsingFailed++;
			return;
		}

		if (relevantStop?.cancelled) {
			analytics.cancellations++;
			analytics.parsingSucceeded++;
			return;
		}

		if (
			relevantStop?.arrival &&
			(DateTime.fromISO(relevantStop?.arrival?.plannedTime).isValid ||
				DateTime.fromISO(relevantStop?.arrival?.actualTime).isValid)
		) {
			const delay = calculateDuration(
				DateTime.fromISO(relevantStop?.arrival?.actualTime ?? relevantStop?.arrival?.plannedTime),
				DateTime.fromISO(relevantStop?.arrival?.plannedTime),
				"seconds"
			);
			analytics.arrival.averageDelay += delay;
			analytics.arrival.minimumDelay = Math.min(analytics.arrival.minimumDelay, delay);
			analytics.arrival.maximumDelay = Math.max(analytics.arrival.maximumDelay, delay);

			if (delay < 0) analytics.arrival.totalTooEarly++;
			else if (delay <= 60) analytics.arrival.totalPunctual++;
			else if (delay > filter.delayThreshold) analytics.arrival.totalDelayed++;
			else analytics.arrival.totalPunctual++;

			if (relevantStop?.arrival?.plannedPlatform !== relevantStop?.arrival?.actualPlatform)
				analytics.arrival.totalPlatformChanges++;

			analytics.arrival.total++;
		}

		if (
			relevantStop?.departure &&
			(DateTime.fromISO(relevantStop?.departure?.plannedTime).isValid ||
				DateTime.fromISO(relevantStop?.departure?.actualTime).isValid)
		) {
			const plannedDeparture = DateTime.fromISO(relevantStop?.departure?.plannedTime);
			const actualDeparture = DateTime.fromISO(relevantStop?.departure?.actualTime);
			if (!plannedDeparture.isValid || !actualDeparture.isValid) return;

			const delay = calculateDuration(
				DateTime.fromISO(relevantStop?.departure?.actualTime ?? relevantStop?.departure?.plannedTime),
				DateTime.fromISO(relevantStop?.departure?.plannedTime),
				"seconds"
			);
			analytics.departure.averageDelay += delay;
			analytics.departure.minimumDelay = Math.min(analytics.departure.minimumDelay, delay);
			analytics.departure.maximumDelay = Math.max(analytics.departure.maximumDelay, delay);

			if (delay < 0) analytics.departure.totalTooEarly++;
			else if (delay <= 60) analytics.departure.totalPunctual++;
			else if (delay > filter.delayThreshold) analytics.departure.totalDelayed++;
			else analytics.departure.totalPunctual++;

			if (relevantStop?.departure?.plannedPlatform !== relevantStop?.departure?.actualPlatform)
				analytics.departure.totalPlatformChanges++;

			analytics.departure.total++;
		}

		analytics.parsingSucceeded++;
	});
	return analytics;
};

export { analyzeStation };
