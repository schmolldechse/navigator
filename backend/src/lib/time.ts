import { DateTime } from "luxon";

const calculateDuration = (
	startDate: DateTime,
	endDate: DateTime,
	unit: "milliseconds" | "seconds" | "minutes" | "hours" | "days"
): number => {
	if (!startDate.isValid || !endDate.isValid) return -1;
	return startDate.diff(endDate).as(unit);
};

export default calculateDuration;
