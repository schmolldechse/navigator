import type { PageLoad } from "./$types";
import type { Journey } from "$models/connection";
import { error } from "@sveltejs/kit";
import {DateTime} from "luxon";

export const load: PageLoad = async ({ fetch, params, data, url }): Promise<{ stationName: string; journeys: Journey[] }> => {
	const queryString = new URLSearchParams({
		evaNumber: params.evaNumber,
		type: params.type,
		when: url.searchParams.get("startDate") ?? DateTime.now().set({ second: 0, minute: 0 }).toISO()
	}).toString();

	const response = await fetch(`http://localhost:8000/api/v1/timetable/combined?${queryString}`, {
		method: "GET"
	});
	if (!response.ok) {
		throw error(404, `No journeys found for ${params.evaNumber}`);
	}

	const jsonData = await response.json();
	if (!Array.isArray(jsonData)) {
		throw error(500, `No journeys found for ${params.evaNumber}`);
	}

	return { ...data, journeys: jsonData as Journey[] };
};
