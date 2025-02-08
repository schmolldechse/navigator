import { Connection } from "../models/connection.ts";
import calculateDuration from "./time.ts";
// @ts-types="npm:@types/luxon"
import { DateTime } from "npm:luxon";
import { Stop } from "../models/station.ts";
import { Message } from "../models/message.ts";

const mapConnection = (
	entry: any,
	type: "departures" | "arrivals",
): Connection => {
	const delay: number = calculateDuration(
		DateTime.fromISO(entry?.timeDelayed),
		DateTime.fromISO(entry?.timeSchedule),
		"seconds",
	);
	const isDeparture = type === "departures";

	return {
		ris_journeyId: entry?.journeyID,
		destination: isDeparture ? mapStops(entry?.destination)[0] : undefined,
		actualDestination: entry?.actualDestination ? mapStops(entry?.actualDestination)[0] : undefined,
		origin: !isDeparture ? mapStops(entry?.destination)[0] : undefined,
		departure: isDeparture
			? {
				plannedTime: entry?.timeSchedule,
				actualTime: entry?.timeDelayed,
				delay: delay,
				plannedPlatform: entry?.platformSchedule,
				actualPlatform: entry?.platform,
			}
			: undefined,
		arrival: !isDeparture
			? {
				plannedTime: entry?.timeSchedule,
				actualTime: entry?.timeDelayed,
				delay: delay,
				plannedPlatform: entry?.platformSchedule,
				actualPlatform: entry?.platform,
			}
			: undefined,
		lineInformation: {
			type: entry?.type,
			replacementServiceType: entry?.replacementServiceType ?? undefined,
			lineName: entry?.lineName,
			additionalLineName: entry?.additionalLineName,
		},
		viaStops: mapStops(entry?.viaStops),
		cancelledStopsAfterActualDestination: mapStops(entry?.canceledStopsAfterActualDestination),
		additionalStops: mapStops(entry?.additionalStops),
		cancelledStops: mapStops(entry?.canceledStops),
		messages: mapMessages(entry?.messages),
		cancelled: entry?.canceled ?? false,
		providesVehicleSequence: entry?.providesVehicleSequence ?? false,
	};
};

const mapStops = (entry: any): Stop[] => {
	if (!Array.isArray(entry)) entry = [entry];
	return entry.map((rawStop: any) => ({
		evaNumber: rawStop?.evaNumber,
		name: rawStop?.name,
		cancelled: rawStop?.canceled ?? undefined,
		additional: rawStop?.additional ?? undefined,
		separation: rawStop?.separation ?? undefined,
		nameParts: rawStop?.nameParts?.map((rawPart: any) => ({
			type: rawPart?.type,
			value: rawPart?.value,
		})),
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
