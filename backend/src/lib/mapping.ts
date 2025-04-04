import calculateDuration from "./time.ts";
import { DateTime } from "luxon";
import type { Connection } from "../models/connection.ts";
import { mapToProduct } from "../models/products.ts";
import type { Stop } from "../models/station.ts";
import type { Message } from "../models/message.ts";
import type { Sequence } from "../models/sequence.ts";
import type { RouteData } from "../models/route.ts";
import type { Time } from "../models/time.ts";

const mapConnection = (
	entry: any,
	type: "departures" | "arrivals" | "both",
	queriedFromBahnhof: boolean = false,
	parseStopovers: boolean = false,
	parseTimesInStopovers: boolean = false,
): Connection => {
	const isIdentifiableAsHAFAS = (entry?.journeyID ?? entry?.tripId ?? "").startsWith("2|#");

	return {
		// journeyId
		ris_journeyId: isIdentifiableAsHAFAS ? undefined : (entry?.journeyID ?? entry?.tripId) ?? undefined,
		hafas_journeyId: isIdentifiableAsHAFAS ? entry?.tripId : undefined,
		// destination
		destination: entry?.destination ? mapStops(entry?.destination)![0] : undefined,
		actualDestination: entry?.actualDestination ? mapStops(entry?.actualDestination)![0] : undefined,
		direction: queriedFromBahnhof ? undefined : entry?.direction ?? undefined, // somehow, "direction" shows either "departure" / "arrival" at the Bahnhof API
		// origin
		origin: entry?.origin ? mapStops(entry?.origin)![0] : undefined,
		provenance: entry?.provenance ?? undefined,
		// departure/ arrival time
		departure: (type === "both" || type === "departures") ? mapTime(entry, "departure") : undefined,
		arrival: (type === "both" || type === "arrivals") ? mapTime(entry, "arrival") : undefined,
		// lineInformation
		lineInformation: !entry?.walking ? {
			type: mapToProduct(entry?.type ?? entry?.line?.product).value ?? undefined,
			replacementServiceType: entry?.replacementServiceType ?? undefined,
			product: entry?.line?.productName ?? undefined,
			lineName: entry?.lineName ?? entry?.line?.name,
			additionalLineName: entry?.additionalLineName ?? undefined,
			fahrtNr: entry?.line?.fahrtNr ?? undefined,
			operator: {
				id: entry?.line?.operator?.id ?? undefined,
				name: entry?.line?.operator?.name ?? undefined
			}
		} : undefined,
		viaStops: mapStops(entry.viaStops ?? entry?.nextStopovers ?? entry?.previousStopovers ?? entry?.stopovers, parseStopovers, (isIdentifiableAsHAFAS && parseTimesInStopovers)) ?? undefined,
		// cancelledStopsAfterActualDestination: mapStops(entry?.canceledStopsAfterActualDestination) ?? undefined,
		// additionalStops: mapStops(entry?.additionalStops) ?? undefined,
		// cancelledStops: mapStops(entry?.canceledStops) ?? undefined,
		messages: mapMessages(entry?.messages ?? entry?.remarks, isIdentifiableAsHAFAS) ?? undefined,
		cancelled: !entry?.walking ? (entry?.canceled ?? entry?.cancelled ?? false) : undefined,
		providesVehicleSequence: !entry?.walking ? (entry?.providesVehicleSequence ?? false) : undefined,
		walking: entry?.walking ? entry?.walking : undefined,
		distance: entry?.walking ? entry?.distance : undefined,
		loadFactor: entry?.loadFactor ?? undefined
	};
};

const mapTime = (entry: any, type: "departure" | "arrival"): Time => {
	const isDeparture = type === "departure";

	const delay = (): number => calculateDuration(
		DateTime.fromISO(entry?.timeDelayed ?? entry?.when ?? entry?.plannedWhen ?? (isDeparture ? entry?.departure : entry?.arrival)),
		DateTime.fromISO(entry?.timeSchedule ?? entry?.plannedWhen ?? (isDeparture ? entry?.plannedDeparture : entry?.plannedArrival)),
		"seconds"
	);

	return {
		plannedTime:
			entry?.timeSchedule ??
			entry?.plannedWhen ??
			(isDeparture ? entry?.plannedDeparture : entry?.plannedArrival) ??
			undefined,
		actualTime: entry?.timeDelayed ?? entry?.when ?? (isDeparture ? entry?.departure : entry?.arrival) ?? undefined,
		delay: !entry?.walking ? ((isDeparture ? entry?.departureDelay : entry?.arrivalDelay) ?? delay()) : undefined,
		plannedPlatform: !entry?.walking
			? (entry?.platformSchedule ??
				entry?.plannedPlatform ??
				(isDeparture ? entry?.plannedDeparturePlatform : entry?.plannedArrivalPlatform))
			: undefined,
		actualPlatform: !entry?.walking
			? (entry?.platform ?? (isDeparture ? entry?.departurePlatform : entry?.arrivalPlatform))
			: undefined
	};
};

const mapStops = (entry: any, parseStopoversFromHAFAS: boolean = false, parseTimeInfo: boolean = false): Stop[] | null => {
	if (!entry) return null;
	if (!Array.isArray(entry)) entry = [entry];

	/**
	 * when parsing further information (departure, arrival & messages), it skips the first & last element of viaStops
	 * this is because HAFAS returns the origin & destination stops in viaStops
	 */
	if (parseStopoversFromHAFAS) {
		entry?.shift(); // removes the origin stop
		entry?.pop(); // removes the destination stop
	}

	return entry.map((rawStop: any) => ({
		evaNumber: rawStop?.evaNumber ?? rawStop?.id ?? rawStop?.stop?.id,
		name: rawStop?.name ?? rawStop?.stop?.name,
		cancelled: rawStop?.canceled ?? rawStop?.cancelled ?? false,
		additional: rawStop?.additional ?? undefined,
		separation: rawStop?.separation ?? undefined,
		nameParts:
			rawStop?.nameParts?.map((rawPart: any) => ({
				type: rawPart?.type,
				value: rawPart?.value
			})) ?? undefined,
		departure: (parseStopoversFromHAFAS && parseTimeInfo) ? mapTime(rawStop, "departure") : undefined,
		arrival: (parseStopoversFromHAFAS && parseTimeInfo) ? mapTime(rawStop, "arrival") : undefined,
		messages: (parseStopoversFromHAFAS && parseTimeInfo) ? mapMessages(rawStop?.remarks, true) : undefined
	}));
};

const mapMessages = (entry: any, isIdentifiableAsHAFAS: boolean = false): Message[] | [] => {
	if (!entry) return [];

	if (isIdentifiableAsHAFAS) return entry.map((message: any) => {
		let transformed = { ...message };

		/**
		 * this ensures it matches the types used by RIS
		 * - "warning" is somehow for cancelled trips. why would you?
		 */
		if (transformed?.type === "warning" || transformed?.type === "status") transformed.type = "general-warning";
		return transformed;
	}).map((message: any) => ({
		type: message?.type,
		text: message?.text ?? message?.summary
	}));

	return (entry?.common || [])
		.concat(entry?.delay || [])
		.concat(entry?.cancelation || [])
		.concat(entry?.destination || [])
		.concat(entry?.via || [])
		.map((message: any) => message as Message);
};

const mapCoachSequence = (entry: any): Sequence => {
	return {
		track: {
			start: { position: entry?.gleis?.start?.position },
			end: { position: entry?.gleis?.ende?.position },
			sections: entry?.gleis?.sektoren?.map((rawSection: any) => ({
				name: rawSection?.bezeichnung,
				start: { position: rawSection?.start?.position },
				end: { position: rawSection?.ende?.position },
				gleisabschnittswuerfelPosition: rawSection?.gleisabschnittswuerfelPosition,
				firstClass: rawSection?.ersteKlasse
			})),
			name: entry?.gleis?.bezeichnung
		},
		vehicleGroup: entry?.fahrzeuggruppen?.map((rawGroup: any) => ({
			vehicles: rawGroup?.fahrzeuge?.map((rawVehicle: any) => ({
				vehicleType: {
					category: rawVehicle?.fahrzeugtyp?.fahrzeugkategorie,
					model: rawVehicle?.fahrzeugtyp?.baureihe,
					firstClass: rawVehicle?.fahrzeugtyp?.ersteKlasse,
					secondClass: rawVehicle?.fahrzeugtyp?.zweiteKlasse
				},
				status: rawVehicle?.status,
				orientation: rawVehicle?.orientierung,
				positionOnTrack: {
					start: { position: rawVehicle?.positionAmGleis?.start?.position },
					end: { position: rawVehicle?.positionAmGleis?.ende?.position },
					section: rawVehicle?.positionAmGleis?.sektor
				},
				equipment: rawVehicle?.ausstattungsmerkmale?.map((rawEquipment: any) => ({
					type: rawEquipment?.art,
					status: rawEquipment?.status
				})),
				orderNumber: rawVehicle?.ordnungsnummer
			})),
			tripReference: {
				type: rawGroup?.fahrtreferenz?.typ,
				line: rawGroup?.fahrtreferenz?.linie,
				destination: { name: rawGroup?.fahrtreferenz?.ziel.bezeichnung },
				category: rawGroup?.fahrtreferenz?.gattung,
				fahrtNr: rawGroup?.fahrtreferenz?.fahrtnummer
			},
			designation: rawGroup?.bezeichnung
		})),
		direction: entry?.fahrtrichtung === "RECHTS" ? "RIGHT" : "LEFT",
		plannedTrack: entry?.gleisSoll,
		actualTrack: entry?.gleisVorschau
	};
};

const mapToRoute = (entry: any): RouteData => ({
	earlierRef: entry?.earlierRef,
	laterRef: entry?.laterRef,
	journeys: entry?.journeys?.map((rawJourney: any) => ({
		legs: rawJourney?.legs?.map((rawLeg: any) => mapConnection(rawLeg, "both", false, true, true)),
		refreshToken: rawJourney?.refreshToken
	}))
});

export { mapConnection, mapCoachSequence, mapToRoute };
