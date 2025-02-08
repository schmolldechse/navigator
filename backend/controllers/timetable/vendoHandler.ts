// @ts-types="npm:@types/express"
import express from "npm:express";
import { Connection } from "../../models/connection.ts";
import mapConnection from "../../lib/mapping.ts";
import { DateTime } from "npm:luxon";
import merge from "../../lib/merge.ts";
import Conn = Deno.Conn;

export type RequestType = "departures" | "arrivals";
// fetches boards; db = RIS id, dbnav = HAFAS, all = both
enum Profile {
	DB = "db",
	DBNAV = "dbnav",
	COMBINED = "combined",
}

type Query = {
	evaNumber: string;
	type: RequestType;
	profile: Profile;
	when?: string;
	duration?: number;
	results?: number;
}

export class VendoHandler {
	async handleSingle(
		req: express.Request,
		res: express.Response,
	): Promise<void> {
		let query: Query = req.query as unknown as Query;
		query = {
			...req.query as unknown as Query,
			when: query.when ?? DateTime.local().toISO(),
			duration: query.duration ?? 60,
			results: query.results ?? 1000,
		}

		if (!query?.evaNumber) {
			res.status(400).send("Station's evaNumber is required");
			return;
		}

		if (!query?.type) {
			res.status(400).send(
				"Type is required. Expected 'departures' or 'arrivals'",
			);
			return;
		}

		if (!query?.profile) {
			res.status(400).send(
				"Profile is required. Expected 'db' or 'dbnav'",
			);
			return;
		}

		if (!DateTime.fromISO(query.when).isValid) {
			res.status(400).send("Invalid date");
			return;
		}

		if (query.profile === Profile.COMBINED) {
			const [db, dbnav] = await Promise.all([
				this.retrieveConnections({ ...query, profile: Profile.DB }),
				this.retrieveConnections({ ...query, profile: Profile.DBNAV }),
			]);

			const connections = merge(db, dbnav, query.type);
			if (!connections) {
				res.status(200).send([]);
				return;
			}

			res.status(200).send(connections.sort((a, b) => {
				const dir = query.type === "departures" ? "departure" : "arrival";
				const getTime = (c: Connection) => DateTime.fromISO(c[dir]?.actualTime ?? c[dir]?.plannedTime);
				return getTime(a) - getTime(b);
			}));
			return;
		}

		res.status(200).send(await this.retrieveConnections(query));
	}

	private async retrieveConnections(query: Query): Promise<Connection[]> {
		const request = await fetch(`https://vendo-prof-${query.profile}.voldechse.wtf/stops/${query.evaNumber}/${query.type}?when${query.when}&duration=${query.duration}&results=${query.results}`, { method: 'GET' });
		if (!request.ok) return [];

		const data = await request.json();
		if (!data[query.type] || !Array.isArray(data[query.type])) return [];

		const map = new Map<string, Connection>();
		data[query.type].forEach((connectionRaw: any) => {
			const tripId = connectionRaw?.tripId;
			if (!tripId || map.has(tripId)) return;

			const connection: Connection = mapConnection(
				connectionRaw,
				query.type,
				query.profile.toString() as "db" | "dbnav",
			);
			if (!connection) return;
			map.set(tripId, connection);
		});

		return Array.from(map.values());
	}
}