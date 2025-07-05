// place files you want to import through the `$lib` alias in this folder.
import type { Stop } from "$models/station";
import { DateTime, Duration, type DurationUnit } from "luxon";
import type { Time } from "$models/time";

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

export { writeStop, mapStops, calculateDuration, formatDuration };
