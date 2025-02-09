// @ts-types="npm:@types/express"
import express from "npm:express";
import { Connection, Journey } from "../../models/connection.ts";
import mapConnection from "../../lib/mapping.ts";
import { Query } from "./requests.ts";

export class BahnhofHandler {
	async handleRequest(
		req: express.Request,
		res: express.Response,
	): Promise<void> {
		let query: Query = req.query as unknown as Query;
		query = {
			...req.query as unknown as Query,
			duration: query.duration ?? 60,
			locale: query.locale ?? "en",
		};

		if (!query.evaNumber) {
			res.status(400).json({ error: "Station's evaNumber is required" });
			return;
		}

		if (!query.type) {
			res.status(400).json({ error: "Type is required. Expected 'departures' or 'arrivals'" });
			return;
		}

		const request = await fetch(
			`https://bahnhof.de/api/boards/${query.type}?evaNumbers=${query.evaNumber}&duration=${query.duration}&locale=${query.locale}`,
		);
		if (!request.ok) {
			res.status(404);
			return;
		}

		const response = await request.json();
		if (!response?.entries || !Array.isArray(response?.entries)) {
			res.status(204);
			return;
		}

		const journeys: Journey[] = [];
		Object.values(response?.entries).forEach((journeyRaw) => {
			// a journey may hold multiple Connection's
			if (!Array.isArray(journeyRaw)) return;

			const journey: Journey = {
				connections: [],
			};
			journeyRaw.forEach((connectionRaw) => {
				const risId = connectionRaw?.journeyID;
				if (!risId) return;

				const connection: Connection = mapConnection(
					connectionRaw,
					query.type,
					"db",
				);
				if (!connection) return;
				if (!journey.connections.some((c) => c.ris_journeyId === connection.ris_journeyId)) {
					journey.connections.push(connection);
				}
			});

			journeys.push(journey);
		});

		res.status(200).json(journeys);
	}
}
