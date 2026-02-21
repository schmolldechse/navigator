import { query } from "$app/server";
import { env } from "$env/dynamic/public";
import type { MetricSeries } from "@lib/api";
import { vBaseMetricRequest } from "@lib/api/valibot.gen";
import * as v from "valibot";

const loadMetric = query(
	v.object({ request: vBaseMetricRequest, userIp: v.optional(v.pipe(v.string(), v.ip())) }),
	async ({ request, userIp }) => {
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
			throw new Error(`Error fetching metric: ${response.status} - ${error}`);
		}

		return (await response.json()) as MetricSeries[];
	}
);

export { loadMetric };
