import type { PageServerLoad } from "./$types";
import type { Station } from "$models/station";
import { error } from "@sveltejs/kit";

export const load: PageServerLoad = async ({ fetch, params }): Promise<{ station: Station }> => {
	const response = await fetch(`http://navigator-backend:8000/api/v1/stations/${params.evaNumber}`, {
		method: "GET"
	});

	if (!response.ok) {
		throw error(404, `No station found for ${params.evaNumber}`);
	}

	const station = (await response.json()) as Station;

	if (station.evaNumber === undefined) {
		throw error(404, `No station found for ${params.evaNumber}`);
	}

	return { station: station };
};
