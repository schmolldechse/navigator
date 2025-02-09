// @ts-types="npm:@types/express"
import express from "npm:express";
import { Connection, Journey } from "../../models/connection.ts";
import mapConnection from "../../lib/mapping.ts";
import { RequestType } from "./vendoHandler.ts";

export class BahnhofHandler {
	async handleRequest(
		req: express.Request,
		res: express.Response,
	): Promise<void> {
		const {
			evaNumber,
			duration = 60,
			locale = "en",
			type,
		} = req.query as unknown as {
			evaNumber: string;
			duration?: number;
			locale?: string;
			type: RequestType;
		};
		if (!evaNumber) {
			res.status(400).json({ error: "Station's evaNumber is required" });
			return;
		}

		if (!type) {
			res.status(400).json({error:
				"Type is required. Expected 'departures' or 'arrivals'",
			});
			return;
		}

		const request = await fetch(
			`https://bahnhof.de/api/boards/${type}?evaNumbers=${evaNumber}&duration=${duration}&locale=${locale}`,
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
					type,
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