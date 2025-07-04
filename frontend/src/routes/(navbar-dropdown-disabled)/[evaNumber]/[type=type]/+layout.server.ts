import type { Station } from "$models/models";
import { error } from "@sveltejs/kit";
import { env } from "$env/dynamic/private";
import type { LayoutServerLoad } from "./$types";

export const load: LayoutServerLoad = async ({ params }): Promise<{ station: Station }> => {
	const response = await fetch(`${env.PRIVATE_BACKEND_URL}/api/station/${params.evaNumber}`, {
		method: "GET"
	});
	if (!response.ok) throw error(404, `No station found for ${params.evaNumber}. Maybe try a different one?`);

	const station = (await response.json()) as Station;
	if (!station.evaNumber) throw error(404, `No station found for ${params.evaNumber}. Maybe try a different one?`);
	return { station };
};