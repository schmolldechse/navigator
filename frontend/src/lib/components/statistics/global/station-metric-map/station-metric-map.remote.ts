import { query } from "$app/server";
import type { BaseMetricDataPointStationDataPoint, BaseStation, MetricSample } from "@lib/api";
import { vBaseMetricRequestStationEventQualitySummaryMetricRequest } from "@lib/api/valibot.gen";
import type {
	StationMetricMapPoint,
	StationMetricMapSeries
} from "@lib/components/statistics/global/station-metric-map/StationMetricMap.svelte";
import { loadMetric } from "@lib/remote/metrics.remote";
import { getStationBatch } from "@lib/remote/station.remote";
import * as v from "valibot";

const loadStationMetricMap = query(
	v.object({
		request: v.pick(vBaseMetricRequestStationEventQualitySummaryMetricRequest, [
			"seriesType",
			"scheduleType",
			"start",
			"end",
			"transportTypes"
		]),
		userIp: v.optional(v.pipe(v.string(), v.ip()))
	}),
	async ({ request, userIp }): Promise<StationMetricMapSeries> => {
		const metric = await loadMetric({
			request: {
				queryType: "STATION_EVENT_QUALITY_SUMMARY",
				seriesType: request.seriesType,
				scheduleType: request.scheduleType,
				start: request.start,
				end: request.end,
				transportTypes: request.transportTypes
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

		const fetchedStations = await getStationBatch({ evaNumbers });
		const stationByEva = new Map<number, BaseStation>();
		for (const station of fetchedStations) {
			stationByEva.set(Number(station.evaNumber), station);
		}

		const points: StationMetricMapPoint[] = [];
		for (const [evaNumber, dataPoint] of valueByStation) {
			const station = stationByEva.get(evaNumber);
			if (!station) continue;

			points.push({ station, value: dataPoint.value, sample: dataPoint.sample });
		}

		return { ...metric, points };
	}
);

export { loadStationMetricMap };
