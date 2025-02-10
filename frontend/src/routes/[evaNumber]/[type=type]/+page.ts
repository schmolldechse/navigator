import type { PageLoad } from "./$types";
import type { Journey } from "$models/connection";

export const load: PageLoad = async ({ fetch, params, data }): Promise<{ journeys: Journey[] } | undefined> => {
	const queryString = new URLSearchParams({
		evaNumber: params.evaNumber,
		type: params.type
	}).toString();

	const response = await fetch(`http://localhost:8000/api/v1/timetable/combined?${queryString}`, {
		method: "GET"
	});
	if (!response.ok) return;

	const jsonData = await response.json();
	if (!Array.isArray(jsonData)) return;

	return { ...data, journeys: jsonData as Journey[] };
};
