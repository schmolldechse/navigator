import type { PageServerLoad } from "./$types";
import { error } from "@sveltejs/kit";
import { env } from "$env/dynamic/private";
import type { RouteDetails, Station } from "$models/models";

export const load: PageServerLoad = async ({
	url
}): Promise<{
	stations: Promise<{ from: Station; to: Station }>;
	route: Promise<RouteDetails>;
}> => {
	const urlParams = new URLSearchParams(url.search);

	const fromEvaNumber: number = Number(urlParams.get("from"));
	const toEvaNumber: number = Number(urlParams.get("to"));

	if (!fromEvaNumber || !toEvaNumber) throw error(400, "Missing required 'from' or 'to' parameter");

	return {
		stations: Promise.all([loadStation(fromEvaNumber), loadStation(toEvaNumber)]).then(([from, to]) => ({ from, to })),
		route: loadRoute(fromEvaNumber, toEvaNumber)
	};
};

const loadStation = async (evaNumber: number): Promise<Station> => {
	const request = await fetch(`${env.BACKEND_DOCKER_BASE_URL}/api/station/${evaNumber}`, {
		method: "GET"
	});
	if (!request.ok) throw error(400, `Could not load the station: ${evaNumber}`);
	return (await request.json()) as Station;
};

const loadRoute = async (from: number, to: number): Promise<RouteDetails> => {
	const request = await fetch(`${env.BACKEND_DOCKER_BASE_URL}/api/route`, {
		method: "POST",
		body: JSON.stringify({
			from,
			to,
			type:
		})
	});
	if (!request.ok) throw error(400, "There was an error while loading the route data");
	return (await request.json()) as RouteDetails;
};
