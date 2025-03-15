import { error } from "@sveltejs/kit";
import { env } from "$env/dynamic/private";
import type { RouteData } from "$models/route";

export const load = async ({ url }): Promise<{ plannedRoute: Promise<RouteData> }> => {
	const from = url.searchParams.get("from");
	const to = url.searchParams.get("to");

	if (!from || !to) {
		throw error(400, "Missing required parameters");
	}

	const params = new URLSearchParams({
		from,
		to,
		...(url.searchParams.has("departure") && { departure: url.searchParams.get("departure")! }),
		...(url.searchParams.has("arrival") && { arrival: url.searchParams.get("arrival")! }),
		...(url.searchParams.has("results") && { results: url.searchParams.get("results")! }),
		...(url.searchParams.has("earlierThan") && { earlierThan: url.searchParams.get("earlierThan")! }),
		...(url.searchParams.has("laterThan") && { laterThan: url.searchParams.get("laterThan")! }),
	});

	const request = await fetch(`${env.BACKEND_DOCKER_BASE_URL}/api/v1/journey/route-planner?${params.toString()}`, { method: "GET" });
	if (!request.ok) {
		throw error(400, "Failed to fetch route");
	}

	return { plannedRoute: request.json() };
};
