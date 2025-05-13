import { DateTime } from "luxon";
import fs from "node:fs";
import path from "node:path";
import os from "node:os";
import { t } from "elysia";
import { DateTimeObject } from "../types/datetime";
import { HttpError } from "../response/error";
import { HttpStatus } from "../response/status";
import { getCollection } from "../db/mongodb/mongodb";
import { ConnectionDocument } from "../db/mongodb/data.schema";

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

	startEvaluation = async (body: typeof this.statsBody.static, evaNumbers: number[]) => {
		const saveDir = path.join(this.BASE_PATH, evaNumbers.join("-"));
		const manifestPath = path.join(saveDir, "manifest.json");

		// statistics are already processed
		if (fs.existsSync(saveDir)) {
			const manifest = JSON.parse(fs.readFileSync(manifestPath, { encoding: "utf8" })) as StatisticManifest;
			if (!manifest.finished) throw new HttpError(HttpStatus.HTTP_409_CONFLICT, "Statistics are still being processed.");

			/**
			const startedAt = DateTime.now();

			const analytics = await this.analyzeStation(saveDir, evaNumbers, body);
			analytics.foundByQuery = manifest.totalCount;
			analytics.executionTime = Math.round(DateTime.now().diff(startedAt).milliseconds ?? 0);
			return analytics;
				*/
			return 1;
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

		/**
		const analyzed = await this.analyzeStation(saveDir, evaNumbers, body);

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
			**/
		return 1;
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

	getTripsCount = async (evaNumbers: number[]) => {
		const collection = await getCollection("trips");
		return await collection.countDocuments({ "viaStops.evaNumber": { $in: evaNumbers } });
	};
}

export { StatisticsService };
