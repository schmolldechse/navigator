import { GroupedTimetableEntrySchema } from "../../models/elysia/timetable.model";
import { Timetable } from "../timetable";

class RisTimetable extends Timetable {
	private apiUrl: string = "https://apis.deutschebahn.com/db/apis/ris-boards/v1/public/";

	async fetchTimetable(): Promise<(typeof GroupedTimetableEntrySchema.static)[]> {
		const request = await fetch(
			"https://apis.deutschebahn.com/db/apis/ris-boards/v1/public/departures/8000096?filterTransports=HIGH_SPEED_TRAIN,INTERCITY_TRAIN,INTER_REGIONAL_TRAIN,REGIONAL_TRAIN&includeStationGroup=false&maxViaStops=4&timeEnd=2025-07-25T18:43:40Z&sortBy=TIME_SCHEDULE",
			{ method: "GET" }
		);
		if (!request.ok) return [];
		return Promise.resolve([]); // Placeholder for actual implementation
	}
}
