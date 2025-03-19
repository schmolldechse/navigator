import { type DateTime, Duration, type DurationUnit } from "luxon";

const calculateDuration = (
	startDate: DateTime,
	endDate: DateTime,
	unit: DurationUnit | DurationUnit[]
): Duration => {
	if (!startDate.isValid || !endDate.isValid) {
		throw new Error("Invalid DateTime objects provided");
	}
	return startDate.diff(endDate, unit);
};

export default calculateDuration;
