import { query } from "$app/server";
import { PUBLIC_API_URL } from "$env/static/public";
import type { StationSummaryDTO } from "$lib/models/StationSummaryDTO";
import * as v from "valibot";

const getStations = query(
	v.object({ latitude: v.number(), longitude: v.number(), radius: v.optional(v.number()), limit: v.optional(v.number()) }),
	async (schema) => {
		if (!PUBLIC_API_URL) throw new Error("API URL is not defined");

		const response = await fetch(`${PUBLIC_API_URL}/stations/nearby`, {
			method: "POST",
			body: JSON.stringify({
				latitude: schema.latitude,
				longitude: schema.longitude,
				maxDistanceMeters: schema.radius,
				limit: schema.limit
			}),
			headers: {
				"Content-Type": "application/json",
				Accept: "*/*"
			}
		});

		if (!response.ok) throw new Error(`Error fetching stations: ${response.status} ${response.statusText}`);
		const stations = await response.json();
		console.log(stations.length);
		return stations as StationSummaryDTO[];
	}
);

export { getStations };
