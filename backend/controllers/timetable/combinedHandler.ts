// @ts-types="npm:@types/express"
import express from "npm:express";
import { DateTime } from "npm:luxon";
import { Profile, Query, retrieveCombinedConnections } from "./requests.ts";

export class CombinedHandler {
	async handleRequest(
		req: express.Request,
		res: express.Response,
	): Promise<void> {
		let query: Query = req.query as unknown as Query;
		query = {
			...req.query as unknown as Query,
			profile: Profile.COMBINED,
			when: query.when ?? DateTime.now().set({ seconds: 0, milliseconds: 0 }).toISO(),
			duration: query.duration ?? 60,
			results: query.results ?? 1000,
		};

		if (!query.evaNumber) {
			res.status(400).json({ error: "Station's evaNumber is required" });
			return;
		}

		if (!query.type) {
			res.status(400).json({
				error: "Type is required. Expected 'departures' or 'arrivals'",
			});
			return;
		}

		if (!DateTime.fromISO(query.when).isValid) {
			res.status(400).json({ error: "Invalid date" });
			return;
		}

		const now = DateTime.now().set({ second: 0, millisecond: 0 });
		const diffToStart = Math.round(DateTime.fromISO(query.when).diff(now, "hours").hours);
		const diffToEnd = Math.round((DateTime.fromISO(query.when).diff(now, "hours").minutes + query.duration) / 60);

		// Bahnhof API only allows requests for the next 6 hours, so there won't be any data for requests outside of this range
		if (diffToStart < -1 || diffToEnd > 6) {
			res.status(200).json(await retrieveCombinedConnections(query));
			return;
		}

		res.status(200).json({ ok: "OK" });
		return;
	}
}
