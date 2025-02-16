import type { Connection } from "../models/connection.ts";
import calculateDuration from "./time.ts";
import { DateTime } from "luxon";
import type { Stop } from "../models/station.ts";
import type { Message } from "../models/message.ts";
import { mapToProduct } from "../models/products.ts";
import type { Sequence } from "../models/sequence.ts";

const mapConnection = (entry: any, type: "departures" | "arrivals", profile: "db" | "dbweb"): Connection => {
	const delay: number = calculateDuration(
		DateTime.fromISO(entry?.timeDelayed ?? entry?.when ?? entry?.plannedWhen),
		DateTime.fromISO(entry?.timeSchedule ?? entry?.plannedWhen),
		"seconds"
	);
	const isDeparture = type === "departures";
	const isRIS = profile === "db";

	return {
		ris_journeyId: isRIS ? (entry?.journeyID ?? entry?.tripId) : undefined,
		hafas_journeyId: !isRIS ? entry?.tripId : undefined,
		destination: isDeparture && isRIS ? mapStops(entry?.destination)![0] : undefined,
		actualDestination: entry?.actualDestination ? mapStops(entry?.actualDestination)![0] : undefined,
		direction: !isRIS ? entry?.direction : undefined,
		origin: !isDeparture && isRIS ? mapStops(entry?.origin)![0] : undefined,
		provenance: !isRIS ? entry?.provenance : undefined,
		departure: isDeparture
			? {
					plannedTime: entry?.timeSchedule ?? entry?.plannedWhen,
					actualTime: entry?.timeDelayed ?? entry?.when,
					delay: delay,
					plannedPlatform: entry?.platformSchedule ?? entry?.plannedPlatform,
					actualPlatform: entry?.platform
				}
			: undefined,
		arrival: !isDeparture
			? {
					plannedTime: entry?.timeSchedule ?? entry?.plannedWhen,
					actualTime: entry?.timeDelayed ?? entry?.when,
					delay: delay,
					plannedPlatform: entry?.platformSchedule ?? entry?.plannedPlatform,
					actualPlatform: entry?.platform
				}
			: undefined,
		lineInformation: {
			type: mapToProduct(entry?.type ?? entry?.line?.product).toString() ?? undefined,
			replacementServiceType: entry?.replacementServiceType ?? undefined,
			lineName: entry?.lineName ?? entry?.line?.name,
			additionalLineName: entry?.additionalLineName,
			fahrtNr: entry?.line?.fahrtNr ?? undefined,
			operator: {
				id: entry?.line?.operator?.id ?? undefined,
				name: entry?.line?.operator?.name ?? undefined
			}
		},
		viaStops: mapStops(entry.viaStops ?? entry?.nextStopovers) ?? undefined,
		cancelledStopsAfterActualDestination: mapStops(entry?.canceledStopsAfterActualDestination) ?? undefined,
		additionalStops: mapStops(entry?.additionalStops) ?? undefined,
		cancelledStops: mapStops(entry?.canceledStops) ?? undefined,
		messages: entry?.messages ? mapMessages(entry.messages) : undefined,
		cancelled: entry?.canceled ?? entry?.cancelled ?? false,
		providesVehicleSequence: entry?.providesVehicleSequence ?? false
	};
};

const mapStops = (entry: any): Stop[] | null => {
	if (!entry) return null;
	if (!Array.isArray(entry)) entry = [entry];
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
			})) ?? undefined
	}));
};

const mapMessages = (
	entry: any
): {
	common: Message[];
	delay: Message[];
	cancellation: Message[];
	destination: Message[];
	via: Message[];
} => {
	return {
		common: (entry.common || []).map((message: any) => message as Message),
		delay: (entry.delay || []).map((message: any) => message as Message),
		cancellation: (entry.cancellation || []).map((message: any) => message as Message),
		destination: (entry.destination || []).map((message: any) => message as Message),
		via: (entry.via || []).map((message: any) => message as Message)
	};
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

export { mapConnection, mapCoachSequence };
