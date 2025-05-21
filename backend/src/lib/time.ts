import { DateTime, Duration, DurationUnit } from "luxon";

const calculateDuration = (
	startDate: DateTime,
	endDate: DateTime,
	unit: DurationUnit | DurationUnit[]
): Duration => {
	if (!startDate.isValid || !endDate.isValid) return Duration.fromMillis(-1);
	return startDate.diff(endDate, unit);
};

export default calculateDuration;
