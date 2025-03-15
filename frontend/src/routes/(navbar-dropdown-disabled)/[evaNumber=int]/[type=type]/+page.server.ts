import type { PageServerLoad } from "./$types";
import { error } from "@sveltejs/kit";
import type { Journey } from "$models/connection";
import { DateTime } from "luxon";
import { env } from "$env/dynamic/private";

export const load: PageServerLoad = async ({ params, url }): Promise<{ journeys: Promise<Journey[]> }> => {
	const journeys = loadJourneys(
		params.evaNumber,
		params.type,
		url.searchParams.get("startDate") ?? DateTime.now().set({ second: 0, millisecond: 0 }).toISO()
	);
	return { journeys };
};

const loadJourneys = async (evaNumber: string, type: string, startDate: string): Promise<Journey[]> => {
	const queryString = new URLSearchParams({
		evaNumber: evaNumber,
		type: type,
		when: startDate
	}).toString();

	const response = await fetch(`${env.BACKEND_DOCKER_BASE_URL}/api/v1/timetable/combined?${queryString}`, {
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
