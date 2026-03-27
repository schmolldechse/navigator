import type { PageServerLoad } from "./$types";
import { DateTime } from "luxon";
import { loadMetric } from "@lib/remote/metrics.remote";
import type { MetricSeries } from "@lib/api";

export const load: PageServerLoad = async ({
	url,
	getClientAddress
}): Promise<{
	dimensions: {
		databaseSize: Promise<MetricSeries>;
		risIdDistribution: Promise<MetricSeries[]>;
		recordedJourneys: Promise<MetricSeries>;
		transportTypeDistribution: Promise<MetricSeries>;
	};
	timerange: { start: DateTime; end: DateTime };
}> => {
	const userDefinedTimerange = url.searchParams.has("start") && url.searchParams.has("end");
	let start: DateTime, end: DateTime;

	if (userDefinedTimerange) {
		const parsedStart = DateTime.fromISO(url.searchParams.get("start")!);
		const parsedEnd = DateTime.fromISO(url.searchParams.get("end")!);

		start = parsedStart.isValid ? parsedStart : DateTime.now().minus({ days: 7 });
		end = parsedEnd.isValid ? parsedEnd : DateTime.now();
	} else {
		start = DateTime.now().minus({ days: 7 });
		end = DateTime.now();
	}

	return {
		dimensions: {
			databaseSize: loadMetric({
				request: { queryType: "DATABASE_SIZE_SNAPSHOT", start: start.toISO()!, end: end.toISO()! },
				userIp: getClientAddress()
			}),
			risIdDistribution: Promise.all([
				loadMetric({
					request: { queryType: "RIS_ID_SNAPSHOT", seriesType: "RIS_IDS_ACTIVE", start: start.toISO()!, end: end.toISO()! },
					userIp: getClientAddress()
				}),
				loadMetric({
					request: { queryType: "RIS_ID_SNAPSHOT", seriesType: "RIS_IDS_INACTIVE", start: start.toISO()!, end: end.toISO()! },
					userIp: getClientAddress()
				})
			]),
			recordedJourneys: loadMetric({
				request: { queryType: "JOURNEY_SNAPSHOT", start: start.toISO()!, end: end.toISO()! },
				userIp: getClientAddress()
			}),
			transportTypeDistribution: loadMetric({
				request: { queryType: "TRANSPORT_TYPE_DISTRIBUTION", end: end.toISO()! },
				userIp: getClientAddress()
			})
		},
		timerange: { start, end }
	};
};
