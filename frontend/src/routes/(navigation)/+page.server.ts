import { DateTime } from "luxon";
import type { PageServerLoad } from "./maps/$types";
import { TransportType, type MetricSeries } from "@lib/api";
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
		stopSummary: Promise<MetricSeries[]>;
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

	const transportTypesFilter = url.searchParams
		.getAll("transportTypes")
		.filter((transportType: string): transportType is TransportType =>
			Object.values(TransportType).includes(transportType as TransportType)
		);

	return {
		dimensions: {
			databaseSize: loadMetric({
				request: { queryType: "DATABASE_SIZE_SNAPSHOT", start: start.toISO()!, end: end.toISO()! },
				userIp: getClientAddress()
			}),
			totalRisIds: loadMetric({
				request: { queryType: "RIS_ID_SNAPSHOT", start: start.toISO()!, end: end.toISO()! },
				userIp: getClientAddress()
			}),
			totalJourneys: loadMetric({
				request: { queryType: "JOURNEY_SNAPSHOT", start: start.toISO()!, end: end.toISO()! },
				userIp: getClientAddress()
			}),
			transportTypes: loadMetric({
				request: {
					queryType: "TRANSPORT_TYPE_DISTRIBUTION",
					end: end.toISO()!,
					...(transportTypesFilter.length > 0 ? { transportTypes: transportTypesFilter } : {})
				},
				userIp: getClientAddress()
			}),
			stopSummary: loadMetric({
				request: {
					queryType: "GLOBAL_STOP_SUMMARY",
					start: start.toISO()!,
					end: end.toISO()!,
					...(transportTypesFilter.length > 0 ? { transportTypes: transportTypesFilter } : {})
				},
				userIp: getClientAddress()
			})
		},
		timerange: { start, end }
	};
};
