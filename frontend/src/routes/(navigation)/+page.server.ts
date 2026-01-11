import { DateTime } from "luxon";
import type { PageServerLoad } from "./maps/$types";
import { MetricQueryType, type MetricSeries } from "@lib/api";
import { loadMetric } from "./projectdimensions.remote";

export const load: PageServerLoad = async ({
	url,
	getClientAddress
}): Promise<{
	dimensions: {
		databaseSize: {
			requestMetricType: MetricQueryType;
			promise: Promise<MetricSeries[]>;
		};
		totalRisIds: {
			requestMetricType: MetricQueryType;
			promise: Promise<MetricSeries[]>;
		};
		totalJourneys: {
			requestMetricType: MetricQueryType;
			promise: Promise<MetricSeries[]>;
		};
		transportTypes: {
			requestMetricType: MetricQueryType;
			promise: Promise<MetricSeries[]>;
		};
	};
	timerange: { start: DateTime; end: DateTime };
}> => {
	const startParam = url.searchParams.get("start");
	const endParam = url.searchParams.get("end");

	const start = startParam ? DateTime.fromISO(startParam) : DateTime.now().minus({ days: 1 });
	const end = endParam ? DateTime.fromISO(endParam) : DateTime.now();

	return {
		dimensions: {
			databaseSize: {
				requestMetricType: MetricQueryType.DATABASE_SIZE,
				promise: loadMetric({
					start: start.toJSDate(),
					end: end.toJSDate(),
					metric: MetricQueryType.DATABASE_SIZE,
					userIp: getClientAddress()
				})
			},
			totalRisIds: {
				requestMetricType: MetricQueryType.RIS_IDS,
				promise: loadMetric({
					start: start.toJSDate(),
					end: end.toJSDate(),
					metric: MetricQueryType.RIS_IDS,
					userIp: getClientAddress()
				})
			},
			totalJourneys: {
				requestMetricType: MetricQueryType.JOURNEYS,
				promise: loadMetric({
					start: start.toJSDate(),
					end: end.toJSDate(),
					metric: MetricQueryType.JOURNEYS,
					userIp: getClientAddress()
				})
			},
			transportTypes: {
				requestMetricType: MetricQueryType.TRANSPORT_TYPES,
				promise: loadMetric({ metric: MetricQueryType.TRANSPORT_TYPES, userIp: getClientAddress() })
			}
		},
		timerange: { start, end }
	};
};
