import { Body, Controller, Path, Post, Route, Tags } from "tsoa";
import { HttpError } from "../../lib/errors/HttpError.ts";
import { getCollection, type StationDocument } from "../../lib/db/mongo-data-db.ts";
import type { ConnectionDocument } from "../../db/mongodb/station.schema.ts";
import { getCachedStation } from "./request.ts";
import * as fs from "node:fs";
import path from "node:path";
import { DateTime } from "luxon";
import * as os from "node:os";
import { analyzeStation } from "./analytics.ts";
import type { StopAnalytics } from "../../models/station.ts";

interface StatisticManifest {
	relatedEvaNumbers: number[];
	finished: boolean;
	totalCount?: number;
	processStartedAt: DateTime;
	processFinishedAt?: DateTime;
}

interface StatsInDto {
	/**
	 * @format date
	 */
	startDate?: string;
	/**
	 * @format date
	 */
	endDate?: string;

	/**
	 * filter options
	 */
	filter?: {
		/**
		 * defines the threshold in seconds after which a delay is considered
		 * @example 60
		 */
		delayThreshold?: number;

		products?: string[];
		lineName?: string[];
		lineNumber?: string[];
	};
}

@Route("stations")
@Tags("Stations")
export class StatisticsController extends Controller {
	private readonly CHUNK_SIZE: number = 2500;
	private readonly BASE_PATH: string = path.join(os.tmpdir(), "navigator", "query-station-stats");

	private static cleanupTimers: Map<string, Timer> = new Map();

	constructor() {
		super();

		const cleanup = () => {
			console.log("Cleaning up tmp directory...");

			if (fs.existsSync(this.BASE_PATH)) fs.rmSync(this.BASE_PATH, { recursive: true });
			console.log("Deleted tmp directory");

			StatisticsController.cleanupTimers.values().forEach((timer) => clearTimeout(timer));
			StatisticsController.cleanupTimers.clear();
		};

		process.on("SIGINT", () => cleanup());
		process.on("SIGTERM", () => cleanup());
	}

	@Post("/stats/{evaNumber}")
	async getStationStatistics(@Path() evaNumber: number, @Body() body: StatsInDto): Promise<StopAnalytics> {
		const startDate: DateTime = body?.startDate ? DateTime.fromISO(body.startDate).startOf("day") : this.START_DATE;
		const endDate: DateTime = body?.endDate ? DateTime.fromISO(body.endDate).endOf("day") : DateTime.now().endOf("day");
		if (startDate > endDate) throw new HttpError(400, "Start date can't be after end date");

		const cachedStation = await getCachedStation(evaNumber);
		if (!cachedStation) throw new HttpError(400, "Station not found");

		const evaNumbers = await this.getRelatedEvaNumbers(cachedStation);
		if (evaNumbers.length === 0) throw new HttpError(400, "Station not found");

		const filter = {
			delayThreshold: 60,
			products: [],
			lineName: [],
			lineNumber: [],
			...body.filter
		};
		if (filter.delayThreshold < 60) filter.delayThreshold = 60;

		const saveDir = path.join(this.BASE_PATH, evaNumbers.join("-"));
		const manifestPath = path.join(saveDir, "manifest.json");
		if (fs.existsSync(saveDir)) {
			const manifest = JSON.parse(fs.readFileSync(manifestPath, { encoding: "utf8" })) as StatisticManifest;
			if (!manifest.finished) throw new HttpError(202, "Statistics are still being processed");

			const startedAt = DateTime.now();

			const analytics = await analyzeStation(saveDir, evaNumbers, { startDate, endDate }, filter);
			analytics.foundByQuery = manifest.totalCount;
			analytics.executionTime = Math.round(DateTime.now().diff(startedAt).as("milliseconds") ?? 0);
			return analytics;
		}

		let manifest: StatisticManifest = {
			relatedEvaNumbers: evaNumbers,
			finished: false,
			processStartedAt: DateTime.now()
		};

		fs.mkdirSync(saveDir, { recursive: true });
		fs.writeFileSync(manifestPath, JSON.stringify(manifest, null, 4));

		const tripsCollection = await getCollection("trips");
		const query = { "viaStops.evaNumber": { $in: evaNumbers } };
		const totalCount = await tripsCollection.countDocuments(query);

		await this.startProcess(evaNumbers, totalCount, saveDir);

		console.log(`Completed processing all ${totalCount} connections and saved to ${saveDir}`);
		this.scheduleDeletion(saveDir);

		const analyzed = await analyzeStation(saveDir, evaNumbers, { startDate, endDate }, filter);

		manifest = {
			...manifest,
			finished: true,
			processFinishedAt: DateTime.now(),
			totalCount
		};
		fs.writeFileSync(manifestPath, JSON.stringify(manifest, null, 4));

		analyzed.foundByQuery = totalCount;
		analyzed.executionTime = Math.round(
			manifest.processFinishedAt?.diff(manifest.processStartedAt).as("milliseconds") ?? 0
		);
		return analyzed;
	}

	private scheduleDeletion = (dirPath: string): void => {
		const cleanupTimer = StatisticsController.cleanupTimers.get(dirPath);
		if (cleanupTimer) clearTimeout(cleanupTimer);

		// 60min
		const MAX_CACHE_TIME = 60 * 60 * 1000;
		const timer: Timer = setTimeout(() => {
			if (!fs.existsSync(dirPath)) return;
			fs.rmSync(dirPath, { recursive: true });

			StatisticsController.cleanupTimers.delete(dirPath);
			console.log(`Completed deletion of ${dirPath}`);
		}, MAX_CACHE_TIME);

		StatisticsController.cleanupTimers.set(dirPath, timer);
	};

	/**
	 * since a station can have several evaNumbers, we try to access all evaNumbers using the "ril100" identifier
	 * this can occur at larger stations, for example "Stuttgart Hbf" & "Hauptbf (Arnulf-Klett-Platz), Stuttgart"
	 */
	private getRelatedEvaNumbers = async (station: StationDocument): Promise<number[]> => {
		if ((station?.ril100 || []).length > 0) {
			const collection = await getCollection("stations");
			return (await collection.find({ ril100: { $in: station.ril100 } }).toArray()).map(
				(station: StationDocument) => station?.evaNumber
			);
		}
		return [station?.evaNumber];
	};

	private startProcess = async (evaNumbers: number[], totalCount: number, saveDir: string): Promise<void> => {
		const totalChunks = Math.ceil(totalCount / this.CHUNK_SIZE);
		let processedSoFar = 0;

		const processChunk = async (
			evaNumbers: number[],
			chunkIndex: number,
			skip: number,
			saveDir: string,
			total: number
		): Promise<number> => {
			const result = await acquireTrips(evaNumbers, this.CHUNK_SIZE, skip, total);

			const chunkFile = path.join(saveDir, `chunk-${chunkIndex}.json`);
			fs.writeFileSync(chunkFile, JSON.stringify(result.connections, null, 4));

			return result.connections.length;
		};

		for (let index = 0; index < totalChunks; index++) {
			const skip = index * this.CHUNK_SIZE;
			const count = await processChunk(evaNumbers, index + 1, skip, saveDir, totalCount);

			processedSoFar += count;

			console.log(`Saved chunk ${index} with ${count} connections`);
			console.log(
				`Processed ${processedSoFar}/${totalCount} connections (${((processedSoFar / totalCount) * 100).toFixed(2)}%)`
			);
		}
	};
}

const acquireTrips = async (
	evaNumbers: number[],
	limit: number = 2500,
	skip: number = 0,
	providedTotalCount?: number
): Promise<{
	connections: ConnectionDocument[];
	hasMore: boolean;
	total: number;
}> => {
	const collection = await getCollection("trips");

	const query = { "viaStops.evaNumber": { $in: evaNumbers } };
	const total = providedTotalCount || (await collection.countDocuments(query));

	const connections = (await collection.find(query).skip(skip).limit(limit).toArray()) as ConnectionDocument[];

	const hasMore = skip + connections.length < total;
	return { connections, hasMore, total };
};
