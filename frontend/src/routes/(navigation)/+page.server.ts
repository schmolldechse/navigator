import { DateTime } from "luxon";
import type { PageServerLoad } from "./maps/$types";
import { estimateDatabaseSize } from "./projectdimensions.remote";
import type { MeasuredTimerangeStatistic } from "$lib/api";

export const load: PageServerLoad = async ({
	url
}): Promise<{
	dimensions: { databaseSize: Promise<MeasuredTimerangeStatistic> };
}> => {
	const startParam = url.searchParams.get("start");
	const endParam = url.searchParams.get("end");

	const start = startParam ? DateTime.fromISO(startParam) : DateTime.now().minus({ days: 1 });
	const end = endParam ? DateTime.fromISO(endParam) : DateTime.now();

	return {
		dimensions: {
			databaseSize: estimateDatabaseSize({ start: start.toJSDate(), end: end.toJSDate() })
		}
	};
};
