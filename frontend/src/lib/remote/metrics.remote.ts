import { getRequestEvent, query } from "$app/server";
import type { MetricSeries } from "@lib/api";
import { vBaseMetricRequest } from "@lib/api/valibot.gen";
import * as v from "valibot";
import { env } from "$env/dynamic/public";

const loadMetric = query(
	v.object({
		request: vBaseMetricRequest,
		userIp: v.optional(v.string())
	}),
	async ({ request, userIp }): Promise<MetricSeries> => {
		if (!env.PUBLIC_API_URL) throw new Error("API URL is not defined");

		const headers: HeadersInit = {
			"Content-Type": "application/json"
		};
		if (userIp) headers["X-Forwarded-For"] = userIp;

		const response = await fetch(`${env.PUBLIC_API_URL}/api/v1/statistics/metrics`, {
			method: "POST",
			body: JSON.stringify(request),
			headers
		});

		if (!response.ok) {
			const error = await response.text();
			throw new Error(`Error fetching metric ${request.queryType}: ${response.status} - ${error}`);
		}

		return (await response.json()) as MetricSeries;
	}
);

export { loadMetric };
