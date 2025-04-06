import { Controller, Path, Post, Route, Tags } from "tsoa";
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
		}

		process.on("SIGINT", () => cleanup());
		process.on("SIGTERM", () => cleanup());
	}

	@Post("/stats/{evaNumber}")
	async getStationStatistics(@Path() evaNumber: number): Promise<StopAnalytics> {
		const cachedStation = await getCachedStation(evaNumber);
		if (!cachedStation) throw new HttpError(400, "Station not found");

		const evaNumbers = await this.getRelatedEvaNumbers(cachedStation);
		if (evaNumbers.length === 0) throw new HttpError(400, "Station not found");

		const saveDir = path.join(this.BASE_PATH, evaNumbers.join("-"));
		if (!fs.existsSync(saveDir)) fs.mkdirSync(saveDir, { recursive: true });

		const tripsCollection = await getCollection("trips");
		const query = { "viaStops.evaNumber": { $in: evaNumbers } };
		const totalCount = await tripsCollection.countDocuments(query);

		await this.startProcess(evaNumbers, totalCount, saveDir);

		const manifestPath = path.join(saveDir, "manifest.json");
		fs.writeFileSync(manifestPath, JSON.stringify({
			relatedEvaNumbers: evaNumbers,
			totalCount: totalCount,
			timestamp: DateTime.now()
		}, null, 4));

		console.log(`Completed processing all ${totalCount} connections and saved to ${saveDir}`);
		this.scheduleDeletion(saveDir);

		return (await analyzeStation(saveDir, evaNumbers));
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
			return (await collection.find({ ril100: { $in: station.ril100 } })
				.toArray())
				.map((station: StationDocument) => station?.evaNumber);
		}
		return [station?.evaNumber];
	};

	private startProcess = async (evaNumbers: number[], totalCount: number, saveDir: string): Promise<void> => {
		const totalChunks = Math.ceil(totalCount / this.CHUNK_SIZE);
		let processedSoFar = 0;

		const processChunk = async (evaNumbers: number[], chunkIndex: number, skip: number, saveDir: string, total: number): Promise<number> => {
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
			console.log(`Processed ${processedSoFar}/${totalCount} connections (${(processedSoFar / totalCount * 100).toFixed(2)}%)`);
		}
	};
}

const acquireTrips = async (evaNumbers: number[], limit: number = 2500, skip: number = 0, providedTotalCount?: number): Promise<{
	connections: ConnectionDocument[],
	hasMore: boolean,
	total: number
}> => {
	const collection = await getCollection("trips");

	const query = { "viaStops.evaNumber": { $in: evaNumbers } };
	const total = providedTotalCount || await collection.countDocuments(query);

	const connections = await collection
		.find(query)
		.skip(skip)
		.limit(limit)
		.toArray() as ConnectionDocument[];

	const hasMore = skip + connections.length < total;
	return { connections, hasMore, total };
};
