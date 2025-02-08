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
			when: query.when ?? DateTime.now().toISO(),
			duration: query.duration ?? 60,
			results: query.results ?? 1000,
		}

		if (!query.evaNumber) {
			res.status(400).send("Station's evaNumber is required");
			return;
		}

		if (!query.type) {
			res.status(400).send(
				"Type is required. Expected 'departures' or 'arrivals'",
			);
			return;
		}

		if (!DateTime.fromISO(query.when).isValid) {
			res.status(400).send("Invalid date");
			return;
		}

		const now = DateTime.now().set({ second: 0, millisecond: 0 });
		const diffToStart = Math.round(DateTime.fromISO(query.when).diff(now, "hours").hours);
		const diffToEnd = Math.round((DateTime.fromISO(query.when).diff(now, "hours").minutes + query.duration) / 60);

		// Bahnhof API only allows requests for the next 6 hours
		if (diffToStart < -1 || diffToEnd > 6) {
			// TODO: somehow url is not correct
			const request = await fetch(`http://localhost:8000/api/v1/timetable/vendo?evaNumber=${query.evaNumber}&type=${query.type}&when=${encodeURIComponent(query.when as string)}&duration=${query.duration}&results=${query.results}`, { method: "GET" });
			if (!request.ok) {
				res.status(200).send(request.body);
				return;
			}

			const data = await request.json();
			if (!data[query.type] || !Array.isArray(data[query.type])) {
				res.status(200).send([]);
				return;
			}

			res.status(200).send(data);
			return;
		}

		res.status(200).send("OK");
		return;
	}
}