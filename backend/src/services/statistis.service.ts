import { DateTime } from "luxon";

class StatistisService {
	// timetable change
	readonly START_DATE: DateTime = DateTime.fromObject({ day: 17, month: 12, year: 2024 }).startOf("day");
}

export { StatistisService };
