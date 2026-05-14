import type { PageServerLoad } from "./$types";
import { loadMetric } from "@lib/remote/metrics.remote";
import type { MetricSeries } from "@lib/api";
import { DateTime } from "luxon";

export const load: PageServerLoad = async ({
	getClientAddress
}): Promise<{
	databaseSize: Promise<MetricSeries>;
	risIdDistribution: Promise<MetricSeries[]>;
	recordedJourneys: Promise<MetricSeries>;
}> => ({
	databaseSize: loadMetric({
		request: {
			queryType: "DATABASE_SIZE_SNAPSHOT",
			start: DateTime.now().minus({ days: 1 }).toISO(),
			end: DateTime.now().toISO()
		},
		userIp: getClientAddress()
	}),
	risIdDistribution: Promise.all([
		loadMetric({
			request: {
				queryType: "RIS_ID_SNAPSHOT",
				seriesType: "RIS_IDS_ACTIVE",
				start: DateTime.now().minus({ days: 1 }).toISO(),
				end: DateTime.now().toISO()
			},
			userIp: getClientAddress()
		}),
		loadMetric({
			request: {
				queryType: "RIS_ID_SNAPSHOT",
				seriesType: "RIS_IDS_INACTIVE",
				start: DateTime.now().minus({ days: 1 }).toISO(),
				end: DateTime.now().toISO()
			},
			userIp: getClientAddress()
		})
	]),
	recordedJourneys: loadMetric({
		request: {
			queryType: "JOURNEY_SNAPSHOT",
			start: DateTime.now().minus({ days: 1 }).toISO(),
			end: DateTime.now().toISO()
		},
		userIp: getClientAddress()
	})
});
