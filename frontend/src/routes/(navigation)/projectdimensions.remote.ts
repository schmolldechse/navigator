import { query } from "$app/server";
import { env } from "$env/dynamic/public";
import type { MeasuredTimerangeStatistic } from "$lib/api";
import { DateTime } from "luxon";
import * as v from "valibot";

const estimateDatabaseSize = query(v.object({ start: v.date(), end: v.date() }), async (schema) => {
	if (!env.PUBLIC_API_URL) throw new Error("API URL is not defined");

	const response = await fetch(`${env.PUBLIC_API_URL}/api/v1/statistics/estimate-size`, {
		method: "POST",
		body: JSON.stringify({
			start: DateTime.fromJSDate(schema.start).set({ millisecond: 0 }).toISO({ suppressMilliseconds: true }),
			end: DateTime.fromJSDate(schema.end).set({ millisecond: 0 }).toISO({ suppressMilliseconds: true })
		}),
		headers: {
			"Content-Type": "application/json"
		}
	});

	if (!response.ok) throw new Error(`Error fetching database size: ${response.status} ${response.statusText}`);
	return (await response.json()) as MeasuredTimerangeStatistic;
});

const estimateRisIds = query(v.object({ start: v.date(), end: v.date() }), async (schema) => {
	if (!env.PUBLIC_API_URL) throw new Error("API URL is not defined");

	const response = await fetch(`${env.PUBLIC_API_URL}/api/v1/statistics/estimate-ris-ids`, {
		method: "POST",
		body: JSON.stringify({
			start: DateTime.fromJSDate(schema.start).set({ millisecond: 0 }).toISO({ suppressMilliseconds: true }),
			end: DateTime.fromJSDate(schema.end).set({ millisecond: 0 }).toISO({ suppressMilliseconds: true })
		}),
		headers: {
			"Content-Type": "application/json"
		}
	});

	if (!response.ok) throw new Error(`Error fetching RIS ID count: ${response.status} ${response.statusText}`);
	return (await response.json()) as MeasuredTimerangeStatistic;
});

export { estimateDatabaseSize, estimateRisIds };
