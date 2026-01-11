import { query } from "$app/server";
import { env } from "$env/dynamic/public";
import { MetricQueryType, type MetricQueryRequest, type MetricSeries } from "$lib/api";
import { DateTime } from "luxon";
import * as v from "valibot";

const loadMetric = query(
	v.object({
		start: v.optional(v.date()),
		end: v.optional(v.date()),
		metric: v.enum(MetricQueryType),
		userIp: v.optional(v.string())
	}),
	async (schema) => {
		if (!env.PUBLIC_API_URL) throw new Error("API URL is not defined");

		const headers: HeadersInit = {
			"Content-Type": "application/json"
		};

		if (schema.userIp) headers["X-Forwarded-For"] = schema.userIp;

		const response = await fetch(`${env.PUBLIC_API_URL}/api/v1/statistics/metrics`, {
			method: "POST",
			body: JSON.stringify({
				start: schema?.start
					? DateTime.fromJSDate(schema.start).set({ millisecond: 0 }).toISO({ suppressMilliseconds: true })
					: undefined,
				end: schema?.end
					? DateTime.fromJSDate(schema.end).set({ millisecond: 0 }).toISO({ suppressMilliseconds: true })
					: undefined,
				queryType: schema.metric,
				cumulativeValues: true
			} as MetricQueryRequest),
			headers
		});

		if (!response.ok) throw new Error(`Error fetching metric: ${response.status} ${response.statusText}`);
		return (await response.json()) as MetricSeries[];
	}
);

export { loadMetric };
