// @ts-types="npm:@types/express"
import express from "npm:express";
import { Connection, Journey } from "../models/connection.ts";

export class BahnhofController {
	public router = express.Router();

	constructor() {
		this.router.get("/api/v1/proxy", this.handleRequest.bind(this));
	}

	async handleRequest(
		req: express.Request,
		res: express.Response,
	): Promise<void> {
		const { evaNr, duration = 60, locale = "en", type } = req.query;
		if (!evaNr) {
			res.status(400).send("Station's evaNr is required");
			return;
		}

		if (!type) {
			res.status(400).send(
				"Type is required. Expected 'departures' or 'arrivals'",
			);
			return;
		}

		const request = await fetch(
			`https://bahnhof.de/api/boards/${type}?evaNumbers=${evaNr}&duration=${duration}&locale=${locale}`,
		);
		if (!request.ok) {
			res.status(200);
			return;
		}

		const response = await request.json();
		if (!response?.entries || !Array.isArray(response?.entries)) {
			res.status(200);
			return;
		}

		Object.values(response?.entries).forEach((journeyRaw) => {
			// a journey may hold multiple Connection's, therefore it's an array
			if (!Array.isArray(journeyRaw)) return;

			const journey: Journey = {};
			journeyRaw.forEach((connectionRaw) => {
				const risId = connectionRaw?.journeyID;
				if (!risId) return;

				const connection: Connection = mapConnection(
					connectionRaw,
					type,
				);
			});
		});

		res.status(200).send(response?.entries);
	}
}

const mapConnection = (
	entry: any,
	type: "departures" | "arrivals",
): Connection => {
	const delay: number = calculateDuration(
		DateTime.fromISO(entry?.scheduledTime),
		DateTime.fromISO(entry?.realTime),
		"minutes",
	);
	const isDeparture = type === "departures";
};
