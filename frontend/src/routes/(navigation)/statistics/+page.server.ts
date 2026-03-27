import { DateTime } from "luxon";
import type { PageServerLoad } from "./$types";
import { loadHeatmapBySeriesType, loadHourlyMetrics } from "./globalstatistics.remote";
import { MetricSeriesType, TransportType } from "@lib/api";

export const load: PageServerLoad = async ({
	getClientAddress
}): Promise<{
	streamed: {
		hourlyTransportMetrics: Promise<Awaited<ReturnType<typeof loadHourlyMetrics>>>;
		heatmapMetrics: Promise<Awaited<ReturnType<typeof loadHeatmapBySeriesType>>>;
	};
	seriesTypes: {
		heatmap: MetricSeriesType;
	};
	timerange: { start: DateTime; end: DateTime };
	transportTypes: TransportType[];
}> => {
	const start = DateTime.now().minus({ days: 7 });
	const end = DateTime.now();

	return {
		streamed: {
			hourlyTransportMetrics: loadHourlyMetrics({
				request: {
					start: start.toISO(),
					end: end.toISO()
				},
				userIp: getClientAddress()
			}),
			heatmapMetrics: loadHeatmapBySeriesType({
				request: {
					seriesType: "STATION_ARRIVALS",
					start: start.toISO(),
					end: end.toISO()
				},
				userIp: getClientAddress()
			})
		},
		seriesTypes: {
			heatmap: MetricSeriesType.STATION_ARRIVALS
		},
		timerange: { start, end },
		transportTypes: []
	};
};
