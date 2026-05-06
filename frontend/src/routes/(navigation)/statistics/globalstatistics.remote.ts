import { query } from "$app/server";
import {
	type BaseStation,
	type BaseMetricDataPoint,
	type BaseMetricDataPointStationDataPoint,
	type MetricSeries
} from "@lib/api";
import { getStationBatch } from "@lib/remote/station.remote";
import * as v from "valibot";
import {
	vBaseMetricRequestHourlyTransportSnapshotMetricRequest,
	vBaseMetricRequestJourneyServiceMetricRequest,
	vBaseMetricRequestMessageSummaryMetricRequest,
	vBaseMetricRequestStationSummaryMetricRequest
} from "@lib/api/valibot.gen";
import { loadMetric } from "@lib/remote/metrics.remote";
import type { StationHeatmapPoint } from "@lib/components/statistics/heatmap/StationHeatmap.svelte";

const loadHourlyMetrics = query(
	v.object({
		request: v.pick(vBaseMetricRequestHourlyTransportSnapshotMetricRequest, ["start", "end", "transportTypes"]),
		userIp: v.optional(v.pipe(v.string(), v.ip()))
	}),
	async ({ request, userIp }): Promise<MetricSeries[]> =>
		Promise.all([
			loadMetric({
				request: {
					queryType: "HOURLY_TRANSPORT_SNAPSHOT",
					seriesType: "HOURLY_GLOBAL_ARRIVALS",
					start: request.start,
					end: request.end,
					transportTypes: request.transportTypes
				},
				userIp
			}),
			loadMetric({
				request: {
					queryType: "HOURLY_TRANSPORT_SNAPSHOT",
					seriesType: "HOURLY_GLOBAL_ARRIVAL_CANCELLATIONS",
					start: request.start,
					end: request.end,
					transportTypes: request.transportTypes
				},
				userIp
			}),
			loadMetric({
				request: {
					queryType: "HOURLY_TRANSPORT_SNAPSHOT",
					seriesType: "HOURLY_GLOBAL_ARRIVAL_DELAY_SUM",
					start: request.start,
					end: request.end,
					transportTypes: request.transportTypes
				},
				userIp
			}),
			loadMetric({
				request: {
					queryType: "HOURLY_TRANSPORT_SNAPSHOT",
					seriesType: "HOURLY_GLOBAL_ARRIVAL_DELAY_SAMPLE_COUNT",
					start: request.start,
					end: request.end,
					transportTypes: request.transportTypes
				},
				userIp
			}),
			loadMetric({
				request: {
					queryType: "HOURLY_TRANSPORT_SNAPSHOT",
					seriesType: "HOURLY_GLOBAL_ARRIVAL_PUNCTUAL_COUNT",
					start: request.start,
					end: request.end,
					transportTypes: request.transportTypes
				},
				userIp
			}),
			loadMetric({
				request: {
					queryType: "HOURLY_TRANSPORT_SNAPSHOT",
					seriesType: "HOURLY_GLOBAL_ARRIVAL_DELAY_MINOR_COUNT",
					start: request.start,
					end: request.end,
					transportTypes: request.transportTypes
				},
				userIp
			}),
			loadMetric({
				request: {
					queryType: "HOURLY_TRANSPORT_SNAPSHOT",
					seriesType: "HOURLY_GLOBAL_ARRIVAL_DELAY_MAJOR_COUNT",
					start: request.start,
					end: request.end,
					transportTypes: request.transportTypes
				},
				userIp
			}),
			loadMetric({
				request: {
					queryType: "HOURLY_TRANSPORT_SNAPSHOT",
					seriesType: "HOURLY_GLOBAL_ARRIVAL_DELAY_SEVERE_COUNT",
					start: request.start,
					end: request.end,
					transportTypes: request.transportTypes
				},
				userIp
			}),
			loadMetric({
				request: {
					queryType: "HOURLY_TRANSPORT_SNAPSHOT",
					seriesType: "HOURLY_GLOBAL_ARRIVAL_PLATFORM_CHANGES",
					start: request.start,
					end: request.end,
					transportTypes: request.transportTypes
				},
				userIp
			}),
			loadMetric({
				request: {
					queryType: "HOURLY_TRANSPORT_SNAPSHOT",
					seriesType: "HOURLY_GLOBAL_DEPARTURES",
					start: request.start,
					end: request.end,
					transportTypes: request.transportTypes
				},
				userIp
			}),
			loadMetric({
				request: {
					queryType: "HOURLY_TRANSPORT_SNAPSHOT",
					seriesType: "HOURLY_GLOBAL_DEPARTURE_CANCELLATIONS",
					start: request.start,
					end: request.end,
					transportTypes: request.transportTypes
				},
				userIp
			}),
			loadMetric({
				request: {
					queryType: "HOURLY_TRANSPORT_SNAPSHOT",
					seriesType: "HOURLY_GLOBAL_DEPARTURE_DELAY_SAMPLE_COUNT",
					start: request.start,
					end: request.end,
					transportTypes: request.transportTypes
				},
				userIp
			}),
			loadMetric({
				request: {
					queryType: "HOURLY_TRANSPORT_SNAPSHOT",
					seriesType: "HOURLY_GLOBAL_DEPARTURE_PUNCTUAL_COUNT",
					start: request.start,
					end: request.end,
					transportTypes: request.transportTypes
				},
				userIp
			}),
			loadMetric({
				request: {
					queryType: "HOURLY_TRANSPORT_SNAPSHOT",
					seriesType: "HOURLY_GLOBAL_DEPARTURE_DELAY_MINOR_COUNT",
					start: request.start,
					end: request.end,
					transportTypes: request.transportTypes
				},
				userIp
			}),
			loadMetric({
				request: {
					queryType: "HOURLY_TRANSPORT_SNAPSHOT",
					seriesType: "HOURLY_GLOBAL_DEPARTURE_DELAY_MAJOR_COUNT",
					start: request.start,
					end: request.end,
					transportTypes: request.transportTypes
				},
				userIp
			}),
			loadMetric({
				request: {
					queryType: "HOURLY_TRANSPORT_SNAPSHOT",
					seriesType: "HOURLY_GLOBAL_DEPARTURE_DELAY_SEVERE_COUNT",
					start: request.start,
					end: request.end,
					transportTypes: request.transportTypes
				},
				userIp
			}),
			loadMetric({
				request: {
					queryType: "HOURLY_TRANSPORT_SNAPSHOT",
					seriesType: "HOURLY_GLOBAL_DEPARTURE_PLATFORM_CHANGES",
					start: request.start,
					end: request.end,
					transportTypes: request.transportTypes
				},
				userIp
			}),
			loadMetric({
				request: {
					queryType: "HOURLY_TRANSPORT_SNAPSHOT",
					seriesType: "HOURLY_GLOBAL_DEPARTURE_DELAY_SUM",
					start: request.start,
					end: request.end,
					transportTypes: request.transportTypes
				},
				userIp
			})
		])
);

const loadHeatmapBySeriesType = query(
	v.object({
		request: v.pick(vBaseMetricRequestStationSummaryMetricRequest, [
			"seriesType",
			"start",
			"end",
			"transportTypes",
			"evaNumbers"
		]),
		userIp: v.optional(v.pipe(v.string(), v.ip()))
	}),
	async ({ request, userIp }): Promise<StationHeatmapPoint[]> => {
		const metrics = await loadMetric({
			request: {
				queryType: "STATION_SUMMARY",
				seriesType: request.seriesType,
				start: request.start,
				end: request.end,
				transportTypes: request.transportTypes,
				evaNumbers: request.evaNumbers
			},
			userIp
		});

		// extract data points with evaNumber and value
		const dataPoints = metrics.dataPoints
			.map((baseDataPoint: BaseMetricDataPoint) => {
				const dataPoint = baseDataPoint as BaseMetricDataPointStationDataPoint;
				if (!dataPoint.evaNumber) return;
				return dataPoint;
			})
			.filter((dataPoint: BaseMetricDataPointStationDataPoint | undefined) => dataPoint !== undefined);

		// get distinct evaNumbers
		const evaNumbers = [
			...new Set(dataPoints.map((dataPoint: BaseMetricDataPointStationDataPoint) => Number(dataPoint.evaNumber)))
		];

		const stations = new Map<number, BaseStation>();
		const fetchedStations = await getStationBatch({ evaNumbers });
		for (const station of fetchedStations) {
			stations.set(Number(station.evaNumber), station);
		}

		// aggregate values per evaNumber (sum across transport types)
		const valueByEva = new Map<number, number>();
		for (const dataPoint of dataPoints) {
			const eva = Number(dataPoint.evaNumber);
			valueByEva.set(eva, (valueByEva.get(eva) ?? 0) + Number(dataPoint.value));
		}

		// build heat points
		const points: StationHeatmapPoint[] = [];
		for (const [eva, value] of valueByEva) {
			const station = stations.get(eva);
			if (!station) continue;

			points.push({ station, value });
		}

		return points;
	}
);

const loadJourneyServiceMetrics = query(
	v.object({
		request: v.pick(vBaseMetricRequestJourneyServiceMetricRequest, [
			"start",
			"end",
			"transportTypes",
			"journeyTypes",
			"operatorCodes"
		]),
		userIp: v.optional(v.pipe(v.string(), v.ip()))
	}),
	async ({ request, userIp }): Promise<MetricSeries[]> =>
		Promise.all([
			loadMetric({
				request: {
					queryType: "JOURNEY_SERVICE",
					seriesType: "JOURNEY_SERVICE_OPERATOR_JOURNEYS",
					start: request.start,
					end: request.end,
					transportTypes: request.transportTypes,
					journeyTypes: request.journeyTypes,
					operatorCodes: request.operatorCodes
				},
				userIp
			}),
			loadMetric({
				request: {
					queryType: "JOURNEY_SERVICE",
					seriesType: "JOURNEY_SERVICE_OPERATOR_CANCELLATIONS",
					start: request.start,
					end: request.end,
					transportTypes: request.transportTypes,
					journeyTypes: request.journeyTypes,
					operatorCodes: request.operatorCodes
				},
				userIp
			}),
			loadMetric({
				request: {
					queryType: "JOURNEY_SERVICE",
					seriesType: "JOURNEY_SERVICE_JOURNEY_TYPE_JOURNEYS",
					start: request.start,
					end: request.end,
					transportTypes: request.transportTypes,
					journeyTypes: request.journeyTypes,
					operatorCodes: request.operatorCodes
				},
				userIp
			})
		])
);

const loadMessageSummaryMetrics = query(
	v.object({
		request: v.pick(vBaseMetricRequestMessageSummaryMetricRequest, [
			"start",
			"end",
			"transportTypes",
			"messageTypes",
			"evaNumbers",
			"limit"
		]),
		userIp: v.optional(v.pipe(v.string(), v.ip()))
	}),
	async ({ request, userIp }): Promise<MetricSeries[]> =>
		Promise.all([
			loadMetric({
				request: {
					queryType: "MESSAGE_SUMMARY",
					seriesType: "MESSAGE_DISRUPTION_CAUSES",
					start: request.start,
					end: request.end,
					transportTypes: request.transportTypes,
					messageTypes: request.messageTypes,
					evaNumbers: request.evaNumbers,
					limit: request.limit
				},
				userIp
			}),
			loadMetric({
				request: {
					queryType: "MESSAGE_SUMMARY",
					seriesType: "MESSAGE_DAILY_TYPES",
					start: request.start,
					end: request.end,
					transportTypes: request.transportTypes,
					messageTypes: request.messageTypes,
					evaNumbers: request.evaNumbers,
					limit: request.limit
				},
				userIp
			})
		])
);

export { loadHourlyMetrics, loadHeatmapBySeriesType, loadJourneyServiceMetrics, loadMessageSummaryMetrics };
