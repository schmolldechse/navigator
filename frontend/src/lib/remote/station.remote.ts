import { query } from "$app/server";
import { env } from "$env/dynamic/public";
import type { BaseStation, StationGatheringInfo } from "$lib/api";
import { vStationByGeographicCoordinatesRequest } from "@lib/api/valibot.gen";
import * as v from "valibot";

const findNearbyStations = query(vStationByGeographicCoordinatesRequest, async (schema) => {
	if (!env.PUBLIC_API_URL) throw new Error("API URL is not defined");

	const response = await fetch(`${env.PUBLIC_API_URL}/api/v1/stations/nearby`, {
		method: "POST",
		body: JSON.stringify(schema),
		headers: {
			"Content-Type": "application/json"
		}
	});

	if (!response.ok) throw new Error(`Error fetching stations: ${response.status} ${response.statusText}`);
	return (await response.json()) as BaseStation[];
});

const getStationGatheringInfo = query(v.object({ evaNumber: v.number() }), async ({ evaNumber }) => {
	if (!env.PUBLIC_API_URL) throw new Error("API URL is not defined");

	const response = await fetch(`${env.PUBLIC_API_URL}/api/v1/stations/${evaNumber}/gathering`, {
		method: "GET",
		headers: {
			"Content-Type": "application/json",
			Accept: "*/*"
		}
	});

	if (!response.ok) throw new Error(`Error fetching station gathering info: ${response.status} ${response.statusText}`);
	return (await response.json()) as StationGatheringInfo;
});

const getStationBatch = query(v.object({ evaNumbers: v.array(v.number()) }), async ({ evaNumbers }) => {
	if (!env.PUBLIC_API_URL) throw new Error("API URL is not defined");

	const response = await fetch(`${env.PUBLIC_API_URL}/api/v1/stations/batch`, {
		method: "POST",
		body: JSON.stringify(evaNumbers),
		headers: {
			"Content-Type": "application/json",
			Accept: "*/*"
		}
	});

	if (!response.ok) throw new Error(`Error fetching station batch: ${response.status} ${response.statusText}`);
	return (await response.json()) as BaseStation[];
});

export { findNearbyStations, getStationGatheringInfo, getStationBatch };
