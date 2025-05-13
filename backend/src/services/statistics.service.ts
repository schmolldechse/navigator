import { DateTime } from "luxon";
import fs from "node:fs";
import path from "node:path";
import os from "node:os";
import { t } from "elysia";
import { DateTimeObject } from "../types/datetime";

class StatisticsService {
	// timetable change
	readonly START_DATE: DateTime = DateTime.fromObject({ day: 17, month: 12, year: 2024 }).startOf("day");
	private readonly BASE_PATH: string = path.join(os.tmpdir(), "navigator", "query-station-stats");

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

	analyze = (body: typeof this.statsBody.static) => {

	};
}

export { StatisticsService };
