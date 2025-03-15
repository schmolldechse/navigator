import type { Station } from "$models/station";
import { error } from "@sveltejs/kit";
import { env } from "$env/dynamic/private";
import type { LayoutServerLoad } from "./$types";

export const load: LayoutServerLoad = async ({ params }): Promise<{ station: Station }> => {
	const station = await loadStation(params.evaNumber);

	return { station };
};

const loadStation = async (evaNumber: string): Promise<Station> => {
	const response = await fetch(`${env.BACKEND_DOCKER_BASE_URL}/api/v1/stations/${evaNumber}`, {
		method: "GET"
	});

	if (!response.ok) {
		throw error(404, `No station found for ${evaNumber}. Maybe try a different one?`);
	}

	const station = (await response.json()) as Station;
	if (station.evaNumber === undefined) {
		throw error(404, `No station found for ${evaNumber}. Maybe try a different one?`);
	}
	return station;
};
