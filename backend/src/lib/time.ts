import { DateTime } from "luxon";

const calculateDuration = (
	startDate: DateTime,
	endDate: DateTime,
	unit: "milliseconds" | "seconds" | "minutes" | "hours" | "days"
): number => {
	if (!startDate.isValid || !endDate.isValid) {
		throw new Error("Invalid DateTime objects provided");
	}
	return startDate.diff(endDate).as(unit);
};

export default calculateDuration;
