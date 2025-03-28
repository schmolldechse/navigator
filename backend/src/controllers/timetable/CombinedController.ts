import { DateTime } from "luxon";
import { Controller, Get, Queries, Route, Tags } from "tsoa";
import type { Connection, Journey } from "../../models/connection.ts";
import calculateDuration from "../../lib/time.ts";
import { RequestType, retrieveBahnhofJourneys, retrieveCombinedConnections } from "./requests.ts";
import { isMatching, merge } from "../../lib/merge.ts";
import { HttpError } from "../../lib/errors/HttpError.ts";

class CombinedQuery {
	evaNumber!: number;
	type!: RequestType;
	when?: string;
	duration?: number = 60;
	results?: number = 1000;
	locale?: string = "en";
}

@Route("timetable/combined")
@Tags("Timetable")
export class CombinedController extends Controller {
	/**
	 * Returns journeys through db-vendo-client@6.5.1 with multiple ID profiles: 'db' (RIS IDs), 'dbweb' (HAFAS IDs), and 'combined' (merged journey elements)
	 */
	@Get()
	async getCombinedJourneys(@Queries() query: CombinedQuery): Promise<Journey[]> {
		query = {
			...query,
			when: query.when ?? DateTime.now().set({ second: 0, millisecond: 0 }).toISO()
		};

		if (!DateTime.fromISO(query.when!).isValid) {
			throw new HttpError(400, "Invalid date");
		}

		const now = DateTime.now().set({ second: 0, millisecond: 0 });
		// duration to the requested time (when)
		const diffToWhen: number = calculateDuration(DateTime.fromISO(query.when!), now, "minutes");
		// duration to the requested time (when) + duration
		const diffToWhenPlusDuration: number = calculateDuration(
			DateTime.fromISO(query.when!).plus({ minutes: query.duration! }),
			now,
			"minutes"
		);

		// Bahnhof API only allows requests for the next 6 hours, so there won't be any data for requests outside of this range
		if (diffToWhen < -60 || diffToWhenPlusDuration > 360) {
			return (await retrieveCombinedConnections(query)).map((connection) => ({
				connections: [connection]
			}));
		}

		const [bahnhof, vendo] = await Promise.all([
			retrieveBahnhofJourneys({ ...query, duration: diffToWhenPlusDuration }),
			retrieveCombinedConnections(query)
		]);

		// merge Connections from Bahnhof with Vendo
		const updatedConnections: Journey[] = bahnhof.flatMap((journey: Journey) => {
			const updated = journey?.connections.map((connectionA: Connection) => {
				const matching = vendo.find((connectionB: Connection) =>
					isMatching(connectionA, connectionB, query.type, journey?.connections.length > 1)
				);

				if (!matching) return connectionA;

				const merged = merge(connectionA, matching, query.type);
				return {
					...merged,
					lineInformation: {
						...merged?.lineInformation,
						lineName: matching?.lineInformation?.lineName
					}
				};
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
