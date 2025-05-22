import { DateTime } from "luxon";
import { type Connection } from "navigator-core/src/models/connection";
import { type Stop } from "navigator-core/src/models/station";
import { type Time } from "navigator-core/src/models/time";
import { type Message } from "navigator-core/src/models/message";
import { mapToProduct } from "navigator-core/src/models/products";
import calculateDuration from "./time";
import { extractProduct } from "./regex";

const mapConnection = (
	entry: any,
	type: "departures" | "arrivals" | "both",
	options: {
		isBahnhofProfile?: boolean;
		parseTimesInStopovers?: boolean;
	} = { isBahnhofProfile: false, parseTimesInStopovers: false }
): Connection => {
	/**
	 * journeyID : bahnhof profile
	 * train?.journeyId : ris profile
	 */
	const journeyId = entry?.journeyID ?? entry?.train?.journeyId;
	const isIdentifiableAsHAFAS = journeyId?.startsWith("2|#") ?? false;

	const connection: Connection = {
		/**
		 * journeyID : bahnhof profile
		 * train?.journeyId : ris profile
		 */
		ris_journeyId: isIdentifiableAsHAFAS ? undefined : journeyId,
		hafas_journeyId: isIdentifiableAsHAFAS ? journeyId : undefined,
		// destination
		destination: entry?.destination ? mapStop(entry?.destination)! : undefined,
		actualDestination: entry?.actualDestination ? mapStop(entry?.actualDestination)! : undefined,
		direction: options.isBahnhofProfile ? undefined : undefined, // TODO
		// origin
		origin: entry?.origin ? mapStop(entry?.origin)! : undefined,
		provenance: options.isBahnhofProfile ? undefined : undefined, // TODO
		// departure/ arrival time
		departure: type !== "arrivals" ? mapTime(entry, "departure") : undefined,
		arrival: type !== "departures" ? mapTime(entry, "arrival") : undefined,
		// lineInformation
		lineInformation: entry?.walking
			? undefined
			: {
					/**
					 * type : bahnhof profile
					 * train?.type : ris profile
					 */
					type: mapToProduct(entry?.type ?? entry?.train?.type).value ?? undefined,
					product: options.isBahnhofProfile ? extractProduct(entry?.lineName) : entry?.train?.category, // TODO
					journeyNumber: entry?.line?.fahrtNr ?? entry?.train?.no ?? undefined,
					/**
					 * lineName : bahnhof profile
					 * (train?.category + train?.lineName) : ris profile
					 */
					journeyName: entry?.lineName ?? (entry?.train?.category + " " + entry?.train?.lineName), // TODO
					additionalJourneyName: !options.isBahnhofProfile ? undefined : entry?.additionalLineName,
					/**
					 * line?.fahrtNr : bahnhof profile
					 * train?.no : ris profile
					 */
					operator: options.isBahnhofProfile
						? undefined
						: {
								id: entry?.administration?.id,
								name: entry?.administration?.operatorName
							} // TODO
				},
		viaStops: (entry?.viaStops ?? []).map((rawStop: any) =>
			mapStop(rawStop, isIdentifiableAsHAFAS && options.parseTimesInStopovers)
		),
		messages: mapMessages(entry?.messages, isIdentifiableAsHAFAS) ?? undefined, // TODO
		cancelled: !entry?.walking ? (entry?.canceled ?? false) : undefined, // TODO
		walking: entry?.walking ? entry?.walking : undefined,
		distance: entry?.walking ? entry?.distance : undefined,
		loadFactor: undefined // TODO
	};

	/**
	 * as our viaStops entry may contain the destination and origin as well, we need to remove them from the viaStops
	 * in addition, we update the cancelled status of the destination and origin
	 */
	if ((connection?.viaStops ?? []).length > 0) {
		const matchingDestination: Stop | undefined = connection?.viaStops?.find(
			(stop: Stop) => stop?.evaNumber === connection?.destination?.evaNumber
		);
		const matchingOrigin: Stop | undefined = connection?.viaStops?.find(
			(stop: Stop) => stop?.evaNumber === connection?.origin?.evaNumber
		);

		if (matchingDestination) {
			connection.destination!.cancelled = matchingDestination?.cancelled ?? false;
			connection.destination!.messages = matchingDestination?.messages ?? undefined;

			connection.viaStops = connection?.viaStops?.filter((stop: Stop) => stop !== matchingDestination);
		}
		if (matchingOrigin) {
			connection.origin!.cancelled = matchingOrigin?.cancelled ?? false;
			connection.origin!.messages = matchingOrigin?.messages ?? undefined;

			connection.viaStops = connection?.viaStops?.filter((stop: Stop) => stop !== matchingOrigin);
		}
	}

	return connection;
};

/**
 * actualTime
 * timeDelayed : bahnhof profile
 * timePredicted : ris profile
 *
 * plannedTime
 * timeSchedule : bahnhof profile
 * time : ris profile
 */
const mapTime = (entry: any, type: "departure" | "arrival"): Time => {
	const isDeparture = type === "departure";

	const plannedTime = DateTime.fromISO(entry?.timeSchedule ?? entry?.time);
	let actualTime = DateTime.fromISO(entry?.timeDelayed ?? entry?.timePredicted);
	if (!actualTime.isValid) actualTime = plannedTime;

	const delay: number = calculateDuration(actualTime, plannedTime, ["seconds"]).seconds;

	return {
		plannedTime: plannedTime.toISO()!,
		actualTime: actualTime.toISO()!,
		delay: !entry?.walking ? delay : undefined,
		plannedPlatform: !entry?.walking ? entry?.platform : undefined,
		actualPlatform: !entry?.walking ? entry?.platform : undefined
	};
};

const mapStop = (entry: any, parseTimeInfo: boolean = false): Stop | null => {
	if (!entry) return null;

	return {
		/**
		 * evaNumber : bahnhof profile
		 * evaNo : ris profile
		 */
		evaNumber: Number(entry?.evaNumber ?? entry?.evaNo),
		name: entry?.name,
		cancelled: entry?.canceled ?? false,
		additional: entry?.additional ?? undefined,
		separation: entry?.separation ?? undefined,
		nameParts:
			entry?.nameParts?.map((rawPart: any) => ({
				type: rawPart?.type,
				value: rawPart?.value
			})) ?? undefined,
		departure: parseTimeInfo ? mapTime(entry, "departure") : undefined,
		arrival: parseTimeInfo ? mapTime(entry, "arrival") : undefined,
		messages: parseTimeInfo ? mapMessages(entry?.remarks, true) : undefined
	};
};

const mapMessages = (entry: any, isIdentifiableAsHAFAS: boolean = false): Message[] => {
	if (!entry) return [];

	if (isIdentifiableAsHAFAS)
		return entry.map((message: any) => ({
			type: message?.type,
			text: message?.text,
			summary: message?.summary
		}));

	return (entry?.common || [])
		.concat(entry?.delay || [])
		.concat(entry?.cancelation || [])
		.concat(entry?.destination || [])
		.concat(entry?.via || [])
		.map((message: any) => ({
			type: message?.type,
			text: message?.text,
			links: message?.links ?? undefined
		}));
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
		legs: rawJourney?.legs?.map((rawLeg: any) => mapConnection(rawLeg, "both", false, true)),
		messages: mapMessages(rawJourney?.remarks, true),
		refreshToken: rawJourney?.refreshToken
	}))
});

export { mapConnection, mapCoachSequence, mapToRoute };
