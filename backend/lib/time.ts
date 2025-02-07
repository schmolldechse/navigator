// @ts-types="npm:@types/luxon"
import { DateTime } from "npm:luxon";

const calculateDuration = (
	startDate: DateTime,
	endDate: DateTime,
	unit: "milliseconds" | "seconds" | "minutes" | "hours" | "days",
): number => {
	if (!startDate.isValid || !endDate.isValid) {
		throw new Error("Invalid DateTime objects provided");
	}
	return endDate.diff(startDate, unit).toObject()[unit];
};

export default { calculateDuration };
