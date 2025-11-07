import { query } from "$app/server";
import { env } from "$env/dynamic/public";
import type { MeasuredTimeframeStatisticDTO } from "$lib/models/MeasuredTimeframeStatisticDTO";
import { DateTime } from "luxon";
import * as v from "valibot";

const getDatabaseSize = query(v.object({ start: v.date(), end: v.date() }), async (schema) => {
	if (!env.PUBLIC_API_URL) throw new Error("API URL is not defined");

	const response = await fetch(`${env.PUBLIC_API_URL}/api/v1/statistics/size/estimate`, {
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
	return (await response.json()) as MeasuredTimeframeStatisticDTO;
});

export { getDatabaseSize };
