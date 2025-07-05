import type { Station, TimetableEntry } from "$models/models";
import { error } from "@sveltejs/kit";
import { env } from "$env/dynamic/private";
import { DateTime } from "luxon";
import type { PageServerLoad } from "./$types";

const loadStation = async (evaNumber: number): Promise<Station> => {
	const request = await fetch(`${env.PRIVATE_BACKEND_URL}/api/station/${evaNumber}`, {
		method: "GET"
	});
	if (!request.ok) throw error(400, `Could not load the station: ${evaNumber}`);
	return (await request.json()) as Station;
};

const loadTimetable = async (evaNumber: number, type: "departures" | "arrivals", when: DateTime): Promise<TimetableEntry[]> => {
	const request = await fetch(`${env.PRIVATE_BACKEND_URL}/api/timetable/${evaNumber}/${type}?when=${encodeURIComponent(when.toISO()!)}`, {
        method: "GET"
    });
	if (!request.ok) throw error(400, `Could not load the timetable for ${evaNumber} (${type}) at ${when.toISO()}`);

	const jsonData = await request.json();
	if (!Array.isArray(jsonData)) throw error(500, `No journeys found for ${evaNumber} (${type}) at ${when.toISO()}.`);

	return jsonData as TimetableEntry[];
};

export const load: PageServerLoad = async ({
	params, url
}): Promise<{ station: Promise<Station>; timetable: Promise<TimetableEntry[]> }> => {
	let when = DateTime.fromISO(url.searchParams.get("when") || DateTime.now().set({ second: 0, millisecond: 0 }).toISO());
	if (!when.isValid) when = DateTime.now().set({ second: 0, millisecond: 0 });

	return {
		station: loadStation(Number(params.evaNumber)),
		timetable: loadTimetable(Number(params.evaNumber), params.type as "departures" | "arrivals", when)
	}
};
