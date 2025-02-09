// @ts-types="npm:@types/express"
import express from "npm:express";
import { DateTime } from "npm:luxon";
import { Profile, Query, retrieveBahnhofConnections, retrieveCombinedConnections } from "./requests.ts";
import { Connection, Journey } from "../../models/connection.ts";
import { isMatching } from "../../lib/merge.ts";
import calculateDuration from "../../lib/time.ts";

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
			locale: query.locale ?? "en",
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
		const diffToStart: number = calculateDuration(DateTime.fromISO(query.when), now, "minutes");
		const diffToEnd: number = calculateDuration(now, DateTime.fromISO(query.when).plus({ minutes: query.duration! }), "minutes");

		// Bahnhof API only allows requests for the next 6 hours, so there won't be any data for requests outside of this range
		if (diffToStart < -60 || diffToEnd > 360) {
			res.status(200).json((await retrieveCombinedConnections(query)).map((connection) => ({
				connections: [connection],
			})));
			return;
		}

		// effectiveDuration is the duration for the Bahnhof API as we can't provide a date to look for
		const effectiveDuration: number = calculateDuration(
			DateTime.fromISO(query.when!).plus({ minutes: query.duration! }),
			now,
			"minutes",
		);
		const [bahnhof, vendo] = await Promise.all([
			retrieveBahnhofConnections({ ...query, duration: effectiveDuration }),
			retrieveCombinedConnections(query),
		]);

		// merge Connections from Bahnhof with Vendo
		const updatedConnections: Journey[] = bahnhof.flatMap((journey: Journey) => {
			const updated = journey?.connections.map((connectionA: Connection) => {
				const matching = vendo.find((connectionB: Connection) =>
					isMatching(connectionA, connectionB, query.type, journey?.connections.length > 1)
				);
				return matching
					? {
						...connectionA,
						...matching,
						lineInformation: {
							...connectionA.lineInformation,
							...matching.lineInformation,
						},
					}
					: connectionA;
			});

			return {
				...journey,
				connections: updated,
			};
		});

		// add all Connections which are not in the updatedConnections but have a earlier date than the first Journey from Bahnhof API
		const firstJourney = this.getFirst(updatedConnections, query.type);
		const additionalConnections = vendo.filter((connectionB: Connection) => {
			const connectionBTime = DateTime.fromISO(
				connectionB[query.type === "departures" ? "departure" : "arrival"]?.plannedTime,
			);
			return !updatedConnections.some((journey: Journey) =>
				journey.connections.some((connectionA: Connection) => isMatching(connectionA, connectionB, query.type))
			) && connectionBTime < firstJourney;
		}).map((connection) => ({ connections: [connection] }));

		// sort
		res.status(200).json([...updatedConnections, ...additionalConnections].sort((a, b) => {
			const dir = query.type === "departures" ? "departure" : "arrival";
			const getTime = (j: Journey) =>
				DateTime.fromISO(j.connections[0][dir]?.actualTime ?? j.connections[0][dir]?.plannedTime);
			return getTime(a) - getTime(b);
		}));
		return;
	}

	private getFirst(journeys: Journey[], type: "departures" | "arrivals"): DateTime {
		const allTimes = journeys.flatMap(journey => journey.connections.map(connection => type === "departures" ? connection?.departure?.plannedTime : connection?.arrival?.plannedTime))
			.filter(Boolean) as string[];
		return DateTime.fromISO(allTimes.reduce((a, b) => a < b ? a : b));
	}
}
