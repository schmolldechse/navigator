// @ts-types="npm:@types/express"
import express from "npm:express";
import { DateTime } from "npm:luxon";
import { Profile, Query, retrieveCombinedConnections, retrieveConnections } from "./requests.ts";

export class VendoHandler {
	async handleSingle(
		req: express.Request,
		res: express.Response,
	): Promise<void> {
		let query: Query = req.query as unknown as Query;
		query = {
			...req.query as unknown as Query,
			when: query.when ?? DateTime.now().set({ seconds: 0, milliseconds: 0 }).toISO(),
			duration: query.duration ?? 60,
			results: query.results ?? 1000,
		};

		if (!query?.evaNumber) {
			res.status(400).json({ error: "Station's evaNumber is required" });
			return;
		}

		if (!query?.type) {
			res.status(400).json({
				error: "Type is required. Expected 'departures' or 'arrivals'",
			});
			return;
		}

		if (!query?.profile) {
			res.status(400).json({
				error: "Profile is required. Expected 'db' or 'dbnav'",
			});
			return;
		}

		if (!DateTime.fromISO(query.when).isValid) {
			res.status(400).json({ error: "Invalid date" });
			return;
		}

		if (query.profile === Profile.COMBINED) {
			res.status(200).json((await retrieveCombinedConnections(query)).map((connection) => ({
				connections: [connection],
			})));
			return;
		}

		res.status(200).json((await retrieveConnections(query)).map((connection) => ({
			connections: [connection],
		})));
	}
}
