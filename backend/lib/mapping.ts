import { Connection } from "../models/connection.ts";
import calculateDuration from "./time.ts";
// @ts-types="npm:@types/luxon"
import { DateTime } from "npm:luxon";
import { Stop } from "../models/station.ts";
import { Message } from "../models/message.ts";

const mapConnection = (
	entry: any,
	type: "departures" | "arrivals",
	profile: "db" | "dbnav",
): Connection => {
	const delay: number = calculateDuration(
		DateTime.fromISO(entry?.timeDelayed ?? entry?.when ?? entry?.plannedWhen),
		DateTime.fromISO(entry?.timeSchedule ?? entry?.plannedWhen),
		"seconds",
	);
	const isDeparture = type === "departures";
	const isRIS = profile === "db";

	return {
		ris_journeyId: isRIS ? (entry?.journeyID ?? entry?.tripId) : undefined,
		hafas_journeyId: !isRIS ? entry?.tripId : undefined,
		destination: isDeparture && isRIS ? mapStops(entry?.destination)![0] : undefined,
		actualDestination: entry?.actualDestination ? mapStops(entry?.actualDestination)![0] : undefined,
		direction: entry?.direction ?? undefined,
		origin: !isDeparture && isRIS ? mapStops(entry?.origin)![0] : undefined,
		provenance: entry?.provenance ?? undefined,
		departure: isDeparture
			? {
				plannedTime: entry?.timeSchedule ?? entry?.plannedWhen,
				actualTime: entry?.timeDelayed ?? entry?.when,
				delay: delay,
				plannedPlatform: entry?.platformSchedule ?? entry?.plannedPlatform,
				actualPlatform: entry?.platform,
			}
			: undefined,
		arrival: !isDeparture
			? {
				plannedTime: entry?.timeSchedule ?? entry?.plannedWhen,
				actualTime: entry?.timeDelayed ?? entry?.when,
				delay: delay,
				plannedPlatform: entry?.platformSchedule ?? entry?.plannedPlatform,
				actualPlatform: entry?.platform,
			}
			: undefined,
		lineInformation: {
			type: entry?.type,
			replacementServiceType: entry?.replacementServiceType ?? undefined,
			lineName: entry?.lineName ?? entry?.line?.name,
			additionalLineName: entry?.additionalLineName,
			fahrtNr: entry?.line?.fahrtNr ?? undefined,
			operator: {
				id: entry?.line?.operator?.id ?? undefined,
				name: entry?.line?.operator?.name ?? undefined,
			},
		},
		viaStops: mapStops(entry?.viaStops) ?? undefined,
		cancelledStopsAfterActualDestination: mapStops(entry?.canceledStopsAfterActualDestination) ?? undefined,
		additionalStops: mapStops(entry?.additionalStops) ?? undefined,
		cancelledStops: mapStops(entry?.canceledStops) ?? undefined,
		messages: entry?.messages ? mapMessages(entry.messages) : undefined,
		cancelled: entry?.canceled ?? entry?.cancelled ?? false,
		providesVehicleSequence: entry?.providesVehicleSequence ?? false,
	};
};

const mapStops = (entry: any): Stop[] | null => {
	if (!entry) return null;
	if (!Array.isArray(entry)) entry = [entry];
	return entry.map((rawStop: any) => ({
		evaNumber: rawStop?.evaNumber ?? rawStop?.id,
		name: rawStop?.name,
		cancelled: rawStop?.canceled ?? rawStop?.cancelled ?? false,
		additional: rawStop?.additional ?? undefined,
		separation: rawStop?.separation ?? undefined,
		nameParts: rawStop?.nameParts?.map((rawPart: any) => ({
			type: rawPart?.type,
			value: rawPart?.value,
		})) ?? undefined,
	}));
};

const mapMessages = (entry: any): {
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
		via: (entry.via || []).map((message: any) => message as Message),
	};
};

export default mapConnection;
