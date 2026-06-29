import { query } from "$app/server";
import { env } from "$env/dynamic/public";
import type { BaseStation, Station, StationGatheringInfo } from "$lib/api";
import { vStationByGeographicCoordinatesRequest, vStationBySerchtermRequest } from "@lib/api/valibot.gen";
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

const getStationByEvaNumber = query(
	v.object({ evaNumber: v.number(), userIp: v.optional(v.string()) }),
	async ({ evaNumber, userIp }) => {
		if (!env.PUBLIC_API_URL) throw new Error("API URL is not defined");

		const headers: HeadersInit = {
			"Content-Type": "application/json",
			Accept: "*/*"
		};
		if (userIp) headers["X-Forwarded-For"] = userIp;

		const response = await fetch(`${env.PUBLIC_API_URL}/api/v1/stations/${evaNumber}`, {
			method: "GET",
			headers
		});

		if (!response.ok) throw new Error(`Error fetching station: ${response.status} ${response.statusText}`);
		return (await response.json()) as Station;
	}
);

const searchStations = query(
	v.object({
		request: vStationBySerchtermRequest,
		userIp: v.optional(v.string())
	}),
	async ({ request, userIp }) => {
		if (!env.PUBLIC_API_URL) throw new Error("API URL is not defined");

		const headers: HeadersInit = {
			"Content-Type": "application/json",
			Accept: "*/*"
		};
		if (userIp) headers["X-Forwarded-For"] = userIp;

		const response = await fetch(`${env.PUBLIC_API_URL}/api/v1/stations/search`, {
			method: "POST",
			body: JSON.stringify(request),
			headers
		});

		if (!response.ok) throw new Error(`Error searching stations: ${response.status} ${response.statusText}`);
		return (await response.json()) as Station[];
	}
);

const getStationBatch = query(
	v.object({ evaNumbers: v.array(v.number()), userIp: v.optional(v.string()) }),
	async ({ evaNumbers, userIp }) => {
		if (!env.PUBLIC_API_URL) throw new Error("API URL is not defined");

		const headers: HeadersInit = {
			"Content-Type": "application/json"
		};
		if (userIp) headers["X-Forwarded-For"] = userIp;

		const response = await fetch(`${env.PUBLIC_API_URL}/api/v1/stations/batch`, {
			method: "POST",
			body: JSON.stringify(evaNumbers),
			headers: {
				"Content-Type": "application/json",
				Accept: "*/*",
				...headers
			}
		});

		if (!response.ok) throw new Error(`Error fetching station batch: ${response.status} ${response.statusText}`);
		return (await response.json()) as BaseStation[];
	}
);

export { findNearbyStations, getStationGatheringInfo, getStationByEvaNumber, getStationBatch, searchStations };
