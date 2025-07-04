import type { PageServerLoad } from "./$types";
import { error } from "@sveltejs/kit";
import type { TimetableEntry } from "$models/models";
import { DateTime } from "luxon";
import { env } from "$env/dynamic/private";

export const load: PageServerLoad = async ({ params, url }): Promise<{ timetable: TimetableEntry[] }> => {
	if (params.type !== "departures" && params.type !== "arrivals")
		throw error(400, `Parameter 'type' must be either 'departures' or 'arrivals'.`);

	const when = url.searchParams.get("startDate") ?? DateTime.now().set({ second: 0, millisecond: 0 }).toISO();

	const response = await fetch(
		`${env.PRIVATE_BACKEND_URL}/api/timetable/${params.evaNumber}/${params.type}`,
		{
			method: "GET",
			body: JSON.stringify({
				when,
				duration: 60,
				language: "en"
			})
		}
	);
	if (!response.ok) throw error(404, `No journeys found for ${params.evaNumber}.`);

	const jsonData = await response.json();
	if (!Array.isArray(jsonData)) throw error(500, `No journeys found for ${params.evaNumber}.`);

	return { timetable: jsonData as TimetableEntry[] };
};
