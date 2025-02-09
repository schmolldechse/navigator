// @ts-types="npm:@types/express"
import express from "npm:express";
import { Query, retrieveBahnhofConnections } from "./requests.ts";

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

		res.status(200).json(await retrieveBahnhofConnections(query));
	}
}
