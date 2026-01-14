import { DateTime } from "luxon";
import type { PageServerLoad } from "./maps/$types";
import { MetricQueryType, type MetricSeries } from "@lib/api";
import { loadMetric } from "./projectdimensions.remote";

export const load: PageServerLoad = async ({
	url,
	getClientAddress
}): Promise<{
	dimensions: {
		databaseSize: Promise<MetricSeries[]>;
		totalRisIds: Promise<MetricSeries[]>;
		totalJourneys: Promise<MetricSeries[]>;
		transportTypes: Promise<MetricSeries[]>;
	};
	timerange: { start: DateTime; end: DateTime };
}> => {
	const startParam = url.searchParams.get("start");
	const endParam = url.searchParams.get("end");

	const start = startParam ? DateTime.fromISO(startParam) : DateTime.now().minus({ days: 1 });
	const end = endParam ? DateTime.fromISO(endParam) : DateTime.now();

	return {
		dimensions: {
			databaseSize: loadMetric({
				start: start.toJSDate(),
				end: end.toJSDate(),
				metric: MetricQueryType.DATABASE_SIZE,
				userIp: getClientAddress()
			}),
			totalRisIds: loadMetric({
				start: start.toJSDate(),
				end: end.toJSDate(),
				metric: MetricQueryType.RIS_IDS
			}),
			totalJourneys: loadMetric({
				start: start.toJSDate(),
				end: end.toJSDate(),
				metric: MetricQueryType.JOURNEYS,
				userIp: getClientAddress()
			}),
			transportTypes: loadMetric({ metric: MetricQueryType.TRANSPORT_TYPES, userIp: getClientAddress() })
		},
		timerange: { start, end }
	};
};
