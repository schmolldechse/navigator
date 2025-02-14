import { DateTime } from "luxon";
import { RequestType, retrieveBahnhofJourneys, retrieveCombinedConnections } from "./requests.ts";
import type { Connection, Journey } from "../../models/connection.ts";
import { isMatching } from "../../lib/merge.ts";
import calculateDuration from "../../lib/time.ts";
import { Controller, Get, Queries, Res, Route, Tags, type TsoaResponse } from "tsoa";

class CombinedQuery {
	evaNumber!: string;
	type!: RequestType;
	when?: string;
	duration?: number = 60;
	results?: number = 1000;
	locale?: string = "en";
}

@Route("timetable/combined")
@Tags("Combined")
export class CombinedController extends Controller {
	@Get()
	async getCombinedJourneys(
		@Queries() query: CombinedQuery,
		@Res() badRequestResponse: TsoaResponse<400, { reason: string }>
	): Promise<Journey[]> {
		query = {
			...query,
			when: query.when ?? DateTime.now().set({ second: 0, millisecond: 0 }).toISO()
		};

		if (!DateTime.fromISO(query.when!).isValid) {
			return badRequestResponse(400, { reason: "Invalid date" });
		}

		const now = DateTime.now().set({ second: 0, millisecond: 0 });
		const diffToStart: number = calculateDuration(DateTime.fromISO(query.when!), now, "minutes");
		const diffToEnd: number = calculateDuration(
			now,
			DateTime.fromISO(query.when!).plus({ minutes: query.duration! }),
			"minutes"
		);

		// Bahnhof API only allows requests for the next 6 hours, so there won't be any data for requests outside of this range
		if (diffToStart < -60 || diffToEnd > 360) {
			return (await retrieveCombinedConnections(query)).map((connection) => ({
				connections: [connection]
			}));
		}

		// effectiveDuration is the duration for the Bahnhof API as we can't provide a date to look for
		const effectiveDuration: number = calculateDuration(
			DateTime.fromISO(query.when!).plus({ minutes: query.duration! }),
			now,
			"minutes"
		);
		const [bahnhof, vendo] = await Promise.all([
			retrieveBahnhofJourneys({ ...query, duration: effectiveDuration }),
			retrieveCombinedConnections(query)
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
								...matching.lineInformation
							}
						}
					: connectionA;
			});

			return {
				...journey,
				connections: updated
			};
		});

		// add all Connections which are not in the updatedConnections but have a earlier date than the first Journey from Bahnhof API
		const firstJourney = getFirst(updatedConnections, query.type);
		const additionalConnections = vendo
			.filter((connectionB: Connection) => {
				const connectionBTime = DateTime.fromISO(
					connectionB[query.type === RequestType.DEPARTURES ? "departure" : "arrival"]!.plannedTime
				);
				return (
					!updatedConnections.some((journey: Journey) =>
						journey.connections.some((connectionA: Connection) => isMatching(connectionA, connectionB, query.type))
					) && connectionBTime <= firstJourney
				);
			})
			.map((connection) => ({ connections: [connection] }));

		// sort
		return [...updatedConnections, ...additionalConnections].sort((a, b) => {
			const dir = query.type === "departures" ? "departure" : "arrival";
			const getTime = (j: Journey) =>
				DateTime.fromISO(j.connections[0][dir]?.actualTime ?? j.connections[0][dir]!.plannedTime).toMillis();
			return getTime(a) - getTime(b);
		});
	}
}

const getFirst = (journeys: Journey[], type: "departures" | "arrivals"): DateTime => {
	const allTimes = journeys
		.flatMap((journey) =>
			journey.connections.map((connection) =>
				type === "departures" ? connection?.departure?.plannedTime : connection?.arrival?.plannedTime
			)
		)
		.filter(Boolean) as string[];
	return DateTime.fromISO(allTimes.reduce((a, b) => (a < b ? a : b), allTimes[0]));
};
