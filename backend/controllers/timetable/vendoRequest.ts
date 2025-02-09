import { Connection } from "../../models/connection.ts";
import mapConnection from "../../lib/mapping.ts";
import merge from "../../lib/merge.ts";
import { DateTime } from "npm:luxon";

export type RequestType = "departures" | "arrivals";
// fetches boards; db = RIS id, dbnav = HAFAS, all = both
export enum Profile {
	DB = "db",
	DBNAV = "dbnav",
	COMBINED = "combined",
}

export type Query = {
	evaNumber: string;
	type: RequestType;
	profile?: Profile;
	when?: string;
	duration?: number;
	results?: number;
};

export const retrieveConnections = async (query: Query): Promise<Connection[]> => {
	const request = await fetch(
		`https://vendo-prof-${query.profile}.voldechse.wtf/stops/${query.evaNumber}/${query.type}?when=${
			encodeURIComponent(query.when!)
		}&duration=${query.duration}&results=${query.results}`,
		{ method: "GET" },
	);
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
			query.profile!.toString() as "db" | "dbnav",
		);
		if (!connection) return;
		map.set(tripId, connection);
	});

	return Array.from(map.values());
};

export const retrieveCombinedConnections = async (query: Query): Promise<Connection[]> => {
	const [db, dbnav] = await Promise.all([
		retrieveConnections({ ...query, profile: Profile.DB }),
		retrieveConnections({ ...query, profile: Profile.DBNAV }),
	]);

	const connections = merge(db, dbnav, query.type);
	if (!connections) return [];

	return connections.sort((a, b) => {
		const dir = query.type === "departures" ? "departure" : "arrival";
		const getTime = (c: Connection) => DateTime.fromISO(c[dir]?.actualTime ?? c[dir]?.plannedTime);
		return getTime(a) - getTime(b);
	});
};
