import type { DateTime, DurationObjectUnits, DurationUnit } from "luxon";

const calculateDuration = (
	startDate: DateTime,
	endDate: DateTime,
	unit: DurationUnit | DurationUnit[]
): DurationObjectUnits => {
	if (!startDate.isValid || !endDate.isValid) {
		throw new Error("Invalid DateTime objects provided");
	}
	return startDate.diff(endDate, unit).toObject();
};

export default calculateDuration;
