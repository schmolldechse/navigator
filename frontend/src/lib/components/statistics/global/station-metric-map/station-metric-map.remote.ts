import { query } from "$app/server";
import { vBaseMetricRequestStationEventQualitySummaryMetricRequest } from "@lib/api/valibot.gen";
import type { StationMetricMapSeries } from "./station-metric-map-types";
import * as v from "valibot";
import type { BaseMetricDataPointStationDataPoint, BaseStation, MetricSample } from "@lib/api";
import { loadMetric } from "@lib/remote/metrics.remote";
import { getStationBatch } from "@lib/remote/station.remote";

const loadStationMetricMap = query(
	v.object({
		request: v.pick(vBaseMetricRequestStationEventQualitySummaryMetricRequest, [
			"seriesType",
			"scheduleType",
			"start",
			"end",
			"transportTypes",
			"includeReplacementTransport"
		]),
		userIp: v.optional(v.string())
	}),
	async ({ request, userIp }): Promise<StationMetricMapSeries> => {
		const metric = await loadMetric({
			request: {
				queryType: "STATION_EVENT_QUALITY_SUMMARY",
				seriesType: request.seriesType,
				scheduleType: request.scheduleType,
				start: request.start,
				end: request.end,
				transportTypes: request.transportTypes,
				includeReplacementTransport: request.includeReplacementTransport
			},
			userIp
		});

		const valueByStation = new Map<number, { value: number; sample?: MetricSample | null }>();
		for (const baseDataPoint of metric.dataPoints) {
			if (!("station" in baseDataPoint) || !baseDataPoint.station) continue;

			const dataPoint = baseDataPoint as BaseMetricDataPointStationDataPoint;
			const evaNumber = Number(dataPoint.station.evaNumber);
			if (!Number.isFinite(evaNumber) || valueByStation.has(evaNumber)) continue;

			valueByStation.set(evaNumber, {
				value: Number(dataPoint.value),
				sample: dataPoint.sample
			});
		}

		const evaNumbers = [...valueByStation.keys()];
		if (evaNumbers.length === 0) return { ...metric, points: [] };

		const fetchedStations = await getStationBatch({ evaNumbers, userIp });
		const stationByEva = new Map<number, BaseStation>();
		for (const station of fetchedStations) {
			stationByEva.set(Number(station.evaNumber), station);
		}

		const points = [...valueByStation.entries()].flatMap(([evaNumber, dataPoint]) => {
			const station = stationByEva.get(evaNumber);
			return station ? [{ station, value: dataPoint.value, sample: dataPoint.sample }] : [];
		});

		return { ...metric, points };
	}
);

export { loadStationMetricMap };
