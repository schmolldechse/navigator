import { error } from "@sveltejs/kit";
import { env } from "$env/dynamic/private";
import type { RouteData } from "$models/route";
import type { Station } from "$models/station";

export const load = async ({ url }): Promise<{ plannedRoute: Promise<RouteData>; stations: Promise<[Station, Station]> }> => {
	const from = url.searchParams.get("from");
	const to = url.searchParams.get("to");

	if (!from || !to) {
		throw error(400, "Missing required parameters");
	}

	return {
		plannedRoute: fetchRoutes(url, to, from),
		stations: Promise.all([fetchStation(Number(from)), fetchStation(Number(to))])
	};
};

const fetchStation = async (evaNumber: number): Promise<Station> => {
	const request = await fetch(`${env.BACKEND_DOCKER_BASE_URL}/api/v1/stations/${evaNumber}`, {
		method: "GET"
	});
	if (!request.ok) throw error(400, "Failed to fetch station");
	return (await request.json()) as Station;
};

const fetchRoutes = async (url: URL, to: string, from: string): Promise<RouteData> => {
	const params = new URLSearchParams({
		from,
		to,
		...(url.searchParams.has("departure") && { departure: url.searchParams.get("departure")! }),
		...(url.searchParams.has("arrival") && { arrival: url.searchParams.get("arrival")! }),
		...(url.searchParams.has("results") && { results: url.searchParams.get("results")! }),
		...(url.searchParams.has("earlierThan") && { earlierThan: url.searchParams.get("earlierThan")! }),
		...(url.searchParams.has("laterThan") && { laterThan: url.searchParams.get("laterThan")! })
	});
	const request = await fetch(`${env.BACKEND_DOCKER_BASE_URL}/api/v1/journey/route-planner?${params.toString()}`, {
		method: "GET"
	});
	if (!request.ok) throw error(400, "Failed to fetch route");
	return (await request.json()) as RouteData;
};
