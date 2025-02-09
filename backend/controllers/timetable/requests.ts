import { Connection, Journey } from "../../models/connection.ts";
import mapConnection from "../../lib/mapping.ts";
import { DateTime } from "npm:luxon";
import { mergeConnections } from "../../lib/merge.ts";

export type RequestType = "departures" | "arrivals";

/**
 * specifies the profile used in boards
 * db = RIS id
 * dbnav = HAFAS
 * combined = both
 */
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
	locale?: string;
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

	const connections = mergeConnections(db, dbnav, query.type);
	if (!connections) return [];

	return connections.sort((a, b) => {
		const dir = query.type === "departures" ? "departure" : "arrival";
		const getTime = (c: Connection) => DateTime.fromISO(c[dir]?.actualTime ?? c[dir]?.plannedTime);
		return getTime(a) - getTime(b);
	});
};

export const retrieveBahnhofConnections = async (query: Query): Promise<Journey[]> => {
	const request = await fetch(
		`https://bahnhof.de/api/boards/${query.type}?evaNumbers=${query.evaNumber}&duration=${query.duration}&locale=${query.locale}`,
	);
	if (!request.ok) return [];

	const response = await request.json();
	if (!response?.entries || !Array.isArray(response?.entries)) return [];

	return Object.values(response?.entries)
		.filter(Array.isArray)
		.map((journeyRaw) => ({
			connections: journeyRaw
				.filter((connectionRaw) => connectionRaw?.journeyID)
				.map((connectionRaw) => mapConnection(connectionRaw, query.type, "db")),
		}));
};
