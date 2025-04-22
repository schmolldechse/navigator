import type { PageServerLoad } from "./$types";
import type { Station } from "$models/station";
import type { RouteData } from "$models/route";
import { error } from "@sveltejs/kit";
import { env } from "$env/dynamic/private";

export const load: PageServerLoad = async ({
	url
}): Promise<{
	stations: Promise<{ from: Station; to: Station }>;
	route: Promise<RouteData>;
}> => {
	const urlParams = new URLSearchParams(url.search);

	const from = urlParams.get("from");
	const to = urlParams.get("to");

	if (!from || !to) throw error(400, "Missing required parameters");

	return {
		stations: Promise.all([loadStation(from), loadStation(to)]).then(([from, to]) => ({ from, to })),
		route: loadRoute(from, to, urlParams)
	};
};

const loadStation = async (evaNumber: string): Promise<Station> => {
	const request = await fetch(`${env.BACKEND_DOCKER_BASE_URL}/api/v1/stations/${evaNumber}`, {
		method: "GET"
	});
	if (!request.ok) throw error(400, "Failed to fetch station");
	return (await request.json()) as Station;
};

const loadRoute = async (from: string, to: string, urlParams: URLSearchParams): Promise<RouteData> => {
	const params = new URLSearchParams({
		from,
		to,
		...Object.fromEntries(
			["departure", "arrival", "disabledProducts", "results", "earlierThan", "laterThan"]
				.filter((key) => urlParams.has(key))
				.map((key) => [key, urlParams.get(key)!])
		)
	});
	const request = await fetch(`${env.BACKEND_DOCKER_BASE_URL}/api/v1/journey/route-planner?${params.toString()}`, {
		method: "GET"
	});
	if (!request.ok) throw error(400, "Failed to fetch route");
	return (await request.json()) as RouteData;
};
