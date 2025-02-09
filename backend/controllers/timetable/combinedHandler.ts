// @ts-types="npm:@types/express"
import express from "npm:express";
import { RequestType } from "./vendoHandler.ts";
import { DateTime } from "npm:luxon";

type Query = {
	evaNumber: string;
	type: RequestType;
	when?: string;
	duration?: number;
	results?: number;
}

export class CombinedHandler {
	async handleRequest(
		req: express.Request,
		res: express.Response,
	): Promise<void> {
		let query: Query = req.query as unknown as Query;
		query = {
			...req.query as unknown as Query,
			when: query.when ?? DateTime.now().set({ seconds: 0, milliseconds: 0 }).toISO(),
			duration: query.duration ?? 60,
			results: query.results ?? 1000,
		}

		if (!query.evaNumber) {
			res.status(400).json({error: "Station's evaNumber is required"});
			return;
		}

		if (!query.type) {
			res.status(400).json({
				error:
					"Type is required. Expected 'departures' or 'arrivals'",
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
			// TODO: somehow url is not correct
			const request = await fetch(`http://localhost:8000/api/v1/timetable/vendo?evaNumber=${query.evaNumber}&type=${query.type}&when=${encodeURIComponent(query.when!)}&duration=${query.duration}&results=${query.results}&profile=combined`, { method: "GET" });
			if (!request.ok) {
				res.status(404).json({ error: "No data found: " + JSON.stringify(request.body) });
				return;
			}

			const data = await request.json();
			if (!data[query.type] || !Array.isArray(data[query.type])) {
				res.status(204);
				return;
			}

			res.status(200).json(data);
			return;
		}

		res.status(200).json({ ok: "OK" });
		return;
	}
}