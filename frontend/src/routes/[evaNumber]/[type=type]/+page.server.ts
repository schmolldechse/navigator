import type { PageServerLoad } from "./$types";
import type { Station } from "$models/station";
import { error } from "@sveltejs/kit";
import type { Journey } from "$models/connection";
import { DateTime } from "luxon";

export const load: PageServerLoad = async ({ params, url }): Promise<{ station: Station; journeys: Journey[] }> => {
	const [station, journeys] = await Promise.all([
		loadStation(params.evaNumber),
		loadJourneys(
			params.evaNumber,
			params.type,
			url.searchParams.get("startDate") ?? DateTime.now().set({ second: 0, millisecond: 0 }).toISO()
		)
	]);
	return { station, journeys };
};

const loadStation = async (evaNumber: string): Promise<Station> => {
	const response = await fetch(`http://navigator-backend:8000/api/v1/stations/${evaNumber}`, {
		method: "GET"
	});
	if (!response.ok) {
		throw error(404, `No station found for ${evaNumber}`);
	}

	const station = (await response.json()) as Station;
	if (station.evaNumber === undefined) {
		throw error(404, `No station found for ${evaNumber}`);
	}
	return station;
};

const loadJourneys = async (evaNumber: string, type: string, startDate: string): Promise<Journey[]> => {
	const queryString = new URLSearchParams({
		evaNumber: evaNumber,
		type: type,
		when: startDate
	}).toString();

	const response = await fetch(`http://navigator-backend:8000/api/v1/timetable/combined?${queryString}`, {
		method: "GET"
	});
	if (!response.ok) {
		throw error(404, `No journeys found for ${evaNumber}`);
	}

	const jsonData = await response.json();
	if (!Array.isArray(jsonData)) {
		throw error(500, `No journeys found for ${evaNumber}`);
	}

	return jsonData as Journey[];
};
