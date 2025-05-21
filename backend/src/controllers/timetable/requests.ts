import { DateTime } from "luxon";
import { mergeConnections } from "../../lib/merge.ts";
import { mapConnection } from "../../lib/mapping.ts";
import type { Connection, Journey } from "../../models/connection.ts";

export const retrieveConnections = async (query: Query): Promise<Connection[]> => {
	const request = await fetch(
		`https://vendo-prof-${query.profile}.voldechse.wtf/stops/${query.evaNumber}/${query.type}?when=${encodeURIComponent(
			query.when!
		)}&duration=${query.duration}&results=${query.results}&stopovers=true`,
		{ method: "GET" }
	);
	if (!request.ok) return [];

	const data = await request.json();
	if (!data[query.type] || !Array.isArray(data[query.type])) return [];

	const map = new Map<string, Connection>();
	data[query.type].forEach((connectionRaw: any) => {
		const tripId = connectionRaw?.tripId;
		if (!tripId || map.has(tripId)) return;

		const connection: Connection = mapConnection(connectionRaw, query.type, false);
		if (!connection) return;

		map.set(tripId, connection);
	});

	return Array.from(map.values());
};

export const retrieveCombinedConnections = async (
	query: Query,
	destinationOriginCriteria: boolean = false
): Promise<Connection[]> => {
	const [db, dbweb] = await Promise.all([
		retrieveConnections({ ...query, profile: Profile.DB }),
		retrieveConnections({ ...query, profile: Profile.DBWEB })
	]);

	const connections = mergeConnections(db, dbweb, query.type, destinationOriginCriteria);
	if (!connections) return [];

	return connections.sort((a, b) => {
		const dir = query.type === "departures" ? "departure" : "arrival";
		const getTime = (c: Connection) => {
			const time = c[dir]?.actualTime ?? c[dir]?.plannedTime;
			return time ? DateTime.fromISO(time).toMillis() : 0;
		};
		return getTime(a) - getTime(b);
	});
};