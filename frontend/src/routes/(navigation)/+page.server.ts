import { DateTime } from "luxon";
import type { PageServerLoad } from "./maps/$types";
import { estimateDatabaseSize, estimateJourneys, estimateRisIds } from "./projectdimensions.remote";
import type { MeasuredTimerangeStatistic } from "$lib/api";

export const load: PageServerLoad = async ({
	url
}): Promise<{
	dimensions: {
		databaseSize: Promise<MeasuredTimerangeStatistic>;
		risIds: Promise<MeasuredTimerangeStatistic>;
		journeys: Promise<MeasuredTimerangeStatistic>;
	};
	timerange: { start?: DateTime; end?: DateTime };
}> => {
	const startParam = url.searchParams.get("start");
	const endParam = url.searchParams.get("end");

	const start = startParam ? DateTime.fromISO(startParam) : DateTime.now().minus({ days: 1 });
	const end = endParam ? DateTime.fromISO(endParam) : DateTime.now();

	return {
		dimensions: {
			databaseSize: estimateDatabaseSize({ start: start.toJSDate(), end: end.toJSDate() }),
			risIds: estimateRisIds({ start: start.toJSDate(), end: end.toJSDate() }),
			journeys: estimateJourneys({ start: start.toJSDate(), end: end.toJSDate() })
		},
		timerange: {
			start: startParam ? start : undefined,
			end: endParam ? end : undefined
		}
	};
};
