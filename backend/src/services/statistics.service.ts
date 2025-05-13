import { DateTime } from "luxon";
import fs from "node:fs";
import path from "node:path";
import os from "node:os";
import { t } from "elysia";
import { DateTimeObject } from "../types/datetime";
import { HttpError } from "../response/error";
import { HttpStatus } from "../response/status";
import { getCollection } from "../db/mongodb/mongodb";
import { type ConnectionDocument } from "../db/mongodb/data.schema";
import { type StopAnalytics } from "navigator-core/src/models/station";
import { type Connection } from "navigator-core/src/models/connection";
import calculateDuration from "../lib/time";

type StatisticManifest = {
	relatedEvaNumbers: number[];
	finished: boolean;
	totalCount?: number;
	processStartedAt: DateTime;
	processFinishedAt?: DateTime;
};

class StatisticsService {
	// timetable change
	readonly START_DATE: DateTime = DateTime.fromObject({ day: 17, month: 12, year: 2024 }).startOf("day");
	private readonly BASE_PATH: string = path.join(os.tmpdir(), "navigator", "query-station-stats");
	private readonly CHUNK_SIZE: number = 2500;

	private cleanupTimers: Map<string, Timer> = new Map();

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

	constructor() {
		const cleanup = () => {
			console.log("Cleaning up tmp directory...");

			if (fs.existsSync(this.BASE_PATH)) fs.rmSync(this.BASE_PATH, { recursive: true });
			console.log("Deleted tmp directory");

			this.cleanupTimers.values().forEach((timer) => clearTimeout(timer));
			this.cleanupTimers.clear();
		};

		process.on("SIGINT", () => cleanup());
		process.on("SIGTERM", () => cleanup());
	}

	private scheduleDeletion = (dirPath: string): void => {
		const cleanupTimer = this.cleanupTimers.get(dirPath);
		if (cleanupTimer) clearTimeout(cleanupTimer);

		// 60min
		const MAX_CACHE_TIME = 60 * 60 * 1000;
		const timer: Timer = setTimeout(() => {
			if (!fs.existsSync(dirPath)) return;
			fs.rmSync(dirPath, { recursive: true });

			this.cleanupTimers.delete(dirPath);
		}, MAX_CACHE_TIME);

		this.cleanupTimers.set(dirPath, timer);
	};

	startEvaluation = async (body: typeof this.statsBody.static, evaNumbers: number[]): Promise<StopAnalytics> => {
		const saveDir = path.join(this.BASE_PATH, evaNumbers.join("-"));
		const manifestPath = path.join(saveDir, "manifest.json");

		// statistics are already processed
		if (fs.existsSync(saveDir)) {
			const manifest = JSON.parse(fs.readFileSync(manifestPath, { encoding: "utf8" })) as StatisticManifest;
			if (!manifest.finished) throw new HttpError(HttpStatus.HTTP_409_CONFLICT, "Statistics are still being processed.");

			const startedAt = DateTime.now();

			const analytics = await this.analyzeStation(evaNumbers, saveDir, body);
			analytics.foundByQuery = manifest.totalCount;
			analytics.executionTime = Math.round(DateTime.now().diff(startedAt).milliseconds ?? 0);
			return analytics;
		}

		// statistics are not processed yet
		let manifest: StatisticManifest = {
			relatedEvaNumbers: evaNumbers,
			finished: false,
			processStartedAt: DateTime.now()
		};

		fs.mkdirSync(saveDir, { recursive: true });
		fs.writeFileSync(manifestPath, JSON.stringify(manifest, null, 4));

		await this.gatherChunks(evaNumbers, saveDir);
		this.scheduleDeletion(saveDir);

		const analyzed = await this.analyzeStation(evaNumbers, saveDir, body);

		manifest = {
			...manifest,
			finished: true,
			processFinishedAt: DateTime.now(),
			totalCount: analyzed.foundByQuery
		};
		fs.writeFileSync(manifestPath, JSON.stringify(manifest, null, 4));

		analyzed.foundByQuery = manifest.totalCount;
		analyzed.executionTime = Math.round(
			manifest.processFinishedAt?.diff(manifest.processStartedAt).as("milliseconds") ?? 0
		);
		return analyzed;
	};

	private gatherChunks = async (evaNumbers: number[], saveDir: string): Promise<void> => {
		const totalCount = await this.getTripsCount(evaNumbers);
		const totalChunks = Math.ceil(totalCount / this.CHUNK_SIZE);

		let processedSoFar = 0;

		const processChunk = async (chunkIndex: number, evaNumbers: number[], skip: number): Promise<number> => {
			const collection = await getCollection("trips");
			const result = (await collection
				.find({ "viaStops.evaNumber": { $in: evaNumbers } })
				.skip(skip)
				.limit(this.CHUNK_SIZE)
				.toArray()) as ConnectionDocument[];

			const chunkFile = path.join(saveDir, `chunk-${chunkIndex}.json`);
			fs.writeFileSync(chunkFile, JSON.stringify(result, null, 4));

			processedSoFar += result.length;
			const progress = ((processedSoFar / totalCount) * 100).toFixed(2);
			console.log(`Processed ${processedSoFar}/${totalCount} connections (${progress}%)`);

			return result.length;
		};

		let array = [];
		for (let index = 0; index < totalChunks; index++) {
			const skip = index * this.CHUNK_SIZE;
			array.push(processChunk(index + 1, evaNumbers, skip));
		}

		await Promise.all(array);
		console.log(`Completed processing all ${totalCount} connections and saved to ${saveDir}`);
	};

	private analyzeStation = async (
		evaNumbers: number[],
		saveDir: string,
		body: typeof this.statsBody.static
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
			return this.analyzeConnections(evaNumbers, JSON.parse(content) as Connection[], body);
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
						Math.round((results.reduce((sum, item) => sum + item.arrival.averageDelay, 0) / totalArrivals) * 100) /
						100,
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
						Math.round(
							(results.reduce((sum, item) => sum + item.departure.averageDelay, 0) / totalDepartures) * 100
						) / 100,
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

	private analyzeConnections = async (
		relevantEvaNumbers: number[],
		connections: ConnectionDocument[],
		body: typeof this.statsBody.static
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

		const isInBetween = (toCheck: DateTime, startDate: DateTime, endDate: DateTime): boolean => {
			if (!toCheck.isValid || !startDate.isValid || !endDate.isValid) return false;
			return startDate <= toCheck && toCheck <= endDate;
		};

		connections.forEach((connection: ConnectionDocument) => {
			const date = DateTime.fromFormat(connection?._id?.toString().split("-")[0] ?? "", "yyyyMMdd");
			if (!isInBetween(date, body.startDate!, body.endDate!)) return;

			const product = connection?.lineInformation?.type || "UNKNOWN";

			if (body.filter.products.length > 0 && !body.filter.products.includes(product)) return;
			if (body.filter.lineName.length > 0 && !body.filter.lineName.includes(connection?.lineInformation?.lineName ?? ""))
				return;
			if (
				body.filter.lineNumber.length > 0 &&
				!body.filter.lineNumber.includes(connection?.lineInformation?.fahrtNr ?? "")
			)
				return;

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
				else if (delay <= body.filter.delayThreshold) analytics.arrival.totalPunctual++;
				else if (delay > body.filter.delayThreshold) analytics.arrival.totalDelayed++;
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
				else if (delay <= body.filter.delayThreshold) analytics.departure.totalPunctual++;
				else if (delay > body.filter.delayThreshold) analytics.departure.totalDelayed++;
				else analytics.departure.totalPunctual++;

				if (relevantStop?.departure?.plannedPlatform !== relevantStop?.departure?.actualPlatform)
					analytics.departure.totalPlatformChanges++;

				analytics.departure.total++;
			}

			analytics.parsingSucceeded++;
		});
		return analytics;
	};

	getTripsCount = async (evaNumbers: number[]) => {
		const collection = await getCollection("trips");
		return await collection.countDocuments({ "viaStops.evaNumber": { $in: evaNumbers } });
	};
}

export { StatisticsService };
