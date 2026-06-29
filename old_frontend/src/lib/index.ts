// place files you want to import through the `$lib` alias in this folder.
import { DateTime, Duration, type DurationUnit } from "luxon";
import type { Time } from "$models/time";

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

export { calculateDuration, formatDuration };
