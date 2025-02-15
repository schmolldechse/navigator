import type { Connection } from "../models/connection.ts";
import { RequestType } from "../controllers/timetable/requests.ts";

const mergeConnections = (
	connectionsA: Connection[],
	connectionsB: Connection[],
	type: RequestType,
	destinationOriginCriteria: boolean = false
): Connection[] => {
	const merge = (connectionA: Connection, connectionB: Connection): Connection => {
		return {
			ris_journeyId: connectionA?.ris_journeyId ?? undefined,
			hafas_journeyId: connectionB?.hafas_journeyId ?? undefined,
			direction: type === "departures" ? ((connectionA?.direction || connectionB?.direction) ?? undefined) : undefined,
			provenance: type === "arrivals" ? ((connectionA?.provenance || connectionB?.provenance) ?? undefined) : undefined,
			destination: type === "departures" ? (connectionA?.destination ?? undefined) : undefined, // RIS (`db` profile) does contain this
			origin: type === "arrivals" ? (connectionA?.origin ?? undefined) : undefined, // RIS (`db` profile) does contain this
			departure: type === "departures" ? connectionA?.departure || connectionB?.departure : undefined,
			arrival: type === "arrivals" ? connectionA?.arrival || connectionB?.arrival : undefined,
			lineInformation: {
				fahrtNr: (connectionA?.lineInformation?.fahrtNr || connectionB?.lineInformation?.fahrtNr) ?? undefined, // HAFAS (`dbnav` profile) contains the line number (e.g. MEX 12)
				lineName: (connectionA?.lineInformation?.lineName || connectionB?.lineInformation?.lineName) ?? undefined,
				operator: connectionA?.lineInformation?.operator ?? undefined
			},
			cancelled: (connectionA?.cancelled || connectionB?.cancelled) ?? false
		};
	};

	const merged: Connection[] = [];
	connectionsB.forEach((connectionB: Connection) => {
		/**
		 * additional check for destination/ origin is needed here!
		 * for example: a wing-train got 2 connections with the same name ("RE 6") but different locations.
		 * it wasn't able to merge one of them, because they had the same name
		 */
		const matching = connectionsA.find((connectionA: Connection) =>
			isMatching(connectionA, connectionB, type, destinationOriginCriteria)
		);
		if (!matching) merged.push(connectionB);
		else merged.push(merge(matching, connectionB));
	});
	return merged;
};

const isMatching = (
	connectionA: Connection,
	connectionB: Connection,
	type: "departures" | "arrivals",
	destinationOriginCriteria: boolean = false
): boolean => {
	if (connectionA?.ris_journeyId === connectionB?.ris_journeyId) return true;
	if (connectionA?.hafas_journeyId === connectionB?.hafas_journeyId) return true;

	const aFullName = normalize(connectionA.lineInformation?.lineName);
	const bFullName = normalize(connectionB.lineInformation?.lineName);

	const platformMatch =
		type === "departures"
			? connectionA.departure?.plannedPlatform && connectionB.departure?.plannedPlatform
				? connectionA.departure?.plannedPlatform === connectionB.departure?.plannedPlatform
				: true
			: connectionA.arrival?.plannedPlatform && connectionB.arrival?.plannedPlatform
				? connectionA.arrival?.plannedPlatform === connectionB.arrival?.plannedPlatform
				: true;

	const timeMatch =
		type === "departures"
			? connectionA.departure?.plannedTime === connectionB.departure?.plannedTime
			: connectionA.arrival?.plannedTime === connectionB.arrival?.plannedTime;

	// matches line by lineName or by comparing fahrtNr
	// necessary for Vendo, as the destinationOriginMatch is not sufficient. Vendo may contain a different stationsName when comparing
	const lineMatch =
		aFullName === bFullName ||
		aFullName === normalize(connectionB.lineInformation?.fahrtNr) ||
		connectionA?.lineInformation?.fahrtNr === connectionB?.lineInformation?.fahrtNr;

	const destinationOriginMatch = destinationOriginCriteria
		? type === "departures"
			? connectionA?.destination?.evaNumber === connectionB?.destination?.evaNumber ||
				connectionA?.destination?.name === connectionB?.direction
			: connectionA?.origin?.evaNumber === connectionB?.origin?.evaNumber ||
				connectionA?.origin?.name === connectionB?.provenance
		: true;

	return platformMatch && timeMatch && lineMatch && destinationOriginMatch;
};

const normalize = (name?: string): string | undefined => {
	return name?.replace(/[^a-zA-Z0-9]/g, "").toLowerCase();
};

export { mergeConnections, isMatching };
