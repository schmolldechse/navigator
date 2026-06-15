import { query } from "$app/server";
import { env } from "$env/dynamic/public";
import type {
	GeoJsonFeatureCollection,
	NetworkMapHotspotsRequest,
	NetworkStatisticsMetricRequest,
	StationStatisticsMetricRequest,
	StatisticsMetricResponse
} from "@lib/api";
import {
	vNetworkMapHotspotsRequest,
	vNetworkStatisticsMetricRequest,
	vStationStatisticsMetricRequest
} from "@lib/api/valibot.gen";
import * as v from "valibot";

const createHeaders = (userIp?: string, accept = "application/json"): HeadersInit => {
	const headers: HeadersInit = {
		"Content-Type": "application/json",
		Accept: accept
	};
	if (userIp) headers["X-Forwarded-For"] = userIp;

	return headers;
};

const postJson = async <TResponse>(path: string, request: unknown, userIp?: string, accept?: string): Promise<TResponse> => {
	if (!env.PUBLIC_API_URL) throw new Error("API URL is not defined");

	const response = await fetch(`${env.PUBLIC_API_URL}${path}`, {
		method: "POST",
		body: JSON.stringify(request),
		headers: createHeaders(userIp, accept)
	});

	if (!response.ok) {
		const message = await response.text();
		throw new Error(`Error fetching statistics ${path}: ${response.status} - ${message}`);
	}

	return (await response.json()) as TResponse;
};

const loadNetworkMetric = query(
	v.object({
		request: vNetworkStatisticsMetricRequest,
		userIp: v.optional(v.string())
	}),
	async ({ request, userIp }): Promise<StatisticsMetricResponse> =>
		postJson<StatisticsMetricResponse>("/api/v1/statistics/network", request as NetworkStatisticsMetricRequest, userIp)
);

const loadNetworkMapHotspots = query(
	v.object({
		request: vNetworkMapHotspotsRequest,
		userIp: v.optional(v.string())
	}),
	async ({ request, userIp }): Promise<GeoJsonFeatureCollection> =>
		postJson<GeoJsonFeatureCollection>(
			"/api/v1/statistics/network/map-hotspots.geojson",
			request as NetworkMapHotspotsRequest,
			userIp,
			"application/geo+json"
		)
);

const loadStationMetric = query(
	v.object({
		request: vStationStatisticsMetricRequest,
		userIp: v.optional(v.string())
	}),
	async ({ request, userIp }): Promise<StatisticsMetricResponse> =>
		postJson<StatisticsMetricResponse>("/api/v1/statistics/stations", request as StationStatisticsMetricRequest, userIp)
);

export { loadNetworkMapHotspots, loadNetworkMetric, loadStationMetric };
