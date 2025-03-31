// place files you want to import through the `$lib` alias in this folder.
import type { Stop } from "$models/station";
import { DateTime, Duration, type DurationUnit } from "luxon";
import type { Connection } from "$models/connection";
import type { Time } from "$models/time";

const writeStop = (stop?: Stop, fallbackName: string = ""): string => {
	if (!stop) return fallbackName;
	if (!stop.name) return fallbackName;
	if (!stop.nameParts || stop.nameParts.length === 0) return stop.name;
	return stop.nameParts
		.map((part) => part.value)
		.join("")
		.trim();
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
		nameParts:
			rawStop?.nameParts?.map((rawPart: any) => ({
				type: rawPart?.type,
				value: rawPart?.value
			})) ?? undefined
	}));
};

const calculateDuration = (startDate: DateTime, endDate: DateTime, unit: DurationUnit | DurationUnit[]): Duration => {
	if (!startDate.isValid || !endDate.isValid) {
		throw new Error("Invalid DateTime objects provided");
	}
	return startDate.diff(endDate, unit);
};

const durationOfConnection = (connection: Connection): number =>
	calculateDuration(
		DateTime.fromISO(connection?.arrival?.actualTime ?? connection?.arrival?.plannedTime ?? ""),
		DateTime.fromISO(connection?.departure?.actualTime ?? connection?.departure?.plannedTime ?? ""),
		["minutes"]
	).as("minutes");

const formatDuration = (end?: Time, start?: Time): string => {
	const duration = calculateDuration(
		DateTime.fromISO(end?.actualTime ?? end?.plannedTime ?? ""),
		DateTime.fromISO(start?.actualTime ?? start?.plannedTime ?? ""),
		["minutes"]
	).as("minutes");

	if (duration > 60) {
		const hours = Math.floor(duration / 60);
		const minutes = duration % 60;
		return `${hours} h ${minutes} min`;
	} else return `${Math.floor(duration)} min`;
};

export { writeStop, mapStops, calculateDuration, durationOfConnection, formatDuration };
