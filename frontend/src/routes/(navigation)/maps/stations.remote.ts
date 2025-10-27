import { query } from "$app/server";
import { env } from "$env/dynamic/public";
import type { StationGatheringInfoDTO } from "$lib/models/StationGatheringInfoDTO";
import type { StationSummaryDTO } from "$lib/models/StationSummaryDTO";
import * as v from "valibot";

const getStations = query(
	v.object({ latitude: v.number(), longitude: v.number(), radius: v.optional(v.number()), limit: v.optional(v.number()) }),
	async (schema) => {
		if (!env.PUBLIC_API_URL) throw new Error("API URL is not defined");

		const response = await fetch(`${env.PUBLIC_API_URL}/stations/nearby`, {
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
		return (await response.json()) as StationSummaryDTO[];
	}
);

const getStationGatheringInfo = query(v.object({ evaNumber: v.number() }), async (schema) => {
	if (!env.PUBLIC_API_URL) throw new Error("API URL is not defined");

	const response = await fetch(`${env.PUBLIC_API_URL}/stations/gathering/${schema.evaNumber}`, {
		method: "GET",
		headers: {
			"Content-Type": "application/json",
			Accept: "*/*"
		}
	});

	if (!response.ok) throw new Error(`Error fetching station gathering info: ${response.status} ${response.statusText}`);
	return (await response.json()) as StationGatheringInfoDTO;
});

export { getStations, getStationGatheringInfo };
