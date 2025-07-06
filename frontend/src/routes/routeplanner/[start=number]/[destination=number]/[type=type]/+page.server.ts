import { env } from "$env/dynamic/private";
import type { RouteDetails, Station } from "$models/models";
import { error } from "@sveltejs/kit";
import type { PageServerLoad } from "./$types";
import { DateTime } from "luxon";

const loadStation = async (evaNumber: number): Promise<Station> => {
	const request = await fetch(`${env.PRIVATE_BACKEND_URL}/api/station/${evaNumber}`, {
		method: "GET"
	});
	if (!request.ok) throw error(400, `Could not load the station: ${evaNumber}`);
	return (await request.json()) as Station;
};

const loadRoute = async (
	start: number,
	destination: number,
	type: "departures" | "arrivals",
	when: DateTime,
	disabledProducts: string[] = []
): Promise<RouteDetails> => {
	const request = await fetch(`${env.PRIVATE_BACKEND_URL}/api/route/`, {
		method: "POST",
		headers: {
			"Content-Type": "application/json"
		},
		body: JSON.stringify({
			from: start,
			to: destination,
			type: type === "departures" ? "departure" : "arrival",
			when: when.toISO(),
			filter: {
				disabledProducts,
				changeover: {
					maxAmount: -1,
					minDuration: -1
				},
				onlyFastRoutes: false
			},
			reference: ""
		})
	});

	if (!request.ok) throw error(400, `Could not load retrieve the routes: ${request.statusText}`);
	return (await request.json()) as RouteDetails;
};

export const load: PageServerLoad = async ({
	params,
	url
}): Promise<{
	stations: Promise<{ start: Station; destination: Station }>;
	route: Promise<RouteDetails>;
	filter: {
		disabledProducts: string[];
		date: string;
		type: "departures" | "arrivals";
	}
}> => {
	let when = DateTime.fromISO(url.searchParams.get("when") || DateTime.now().set({ second: 0, millisecond: 0 }).toISO());
	if (!when.isValid) when = DateTime.now().set({ second: 0, millisecond: 0 });

	return {
		stations: Promise.all([loadStation(Number(params.start)), loadStation(Number(params.destination))]).then(
			([start, destination]) => ({
				start,
				destination
			})
		),
		route: loadRoute(Number(params.start), Number(params.destination), params.type as "departures" | "arrivals", when, []),
		filter: {
			disabledProducts: [],
			date: when.toISO(),
			type: params.type as "departures" | "arrivals"
		}
	};
};
