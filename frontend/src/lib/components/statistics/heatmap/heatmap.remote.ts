import { query } from "$app/server";
import type { BaseMetricDataPointStationDataPoint, BaseStation, MetricSeries } from "@lib/api";
import { vBaseMetricRequestStationEventQualitySummaryMetricRequest } from "@lib/api/valibot.gen";
import type { HeatmapMetricSeries, StationHeatmapPoint } from "@lib/components/statistics/heatmap/StationMetricHeatmap.svelte";
import { loadMetric } from "@lib/remote/metrics.remote";
import { getStationBatch } from "@lib/remote/station.remote";
import * as v from "valibot";

const loadHeatmap = query(
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
	async ({ request, userIp }): Promise<HeatmapMetricSeries> => {
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

		const valueByStation = new Map<number, number>();
		for (const baseDataPoint of metric.dataPoints) {
			if (!("station" in baseDataPoint) || !baseDataPoint.station) continue;

			const dataPoint = baseDataPoint as BaseMetricDataPointStationDataPoint;
			const evaNumber = Number(dataPoint.station.evaNumber);
			if (!Number.isFinite(evaNumber) || valueByStation.has(evaNumber)) continue;

			valueByStation.set(evaNumber, Number(dataPoint.value));
		}

		const evaNumbers = [...valueByStation.keys()];
		if (evaNumbers.length === 0) return { ...metric, points: [] };

		const fetchedStations = await getStationBatch({ evaNumbers });
		const stationByEva = new Map<number, BaseStation>();
		for (const station of fetchedStations) {
			stationByEva.set(Number(station.evaNumber), station);
		}

		const points: StationHeatmapPoint[] = [];
		for (const [evaNumber, value] of valueByStation) {
			const station = stationByEva.get(evaNumber);
			if (!station) continue;

			points.push({ station, value });
		}

		return { ...metric, points };
	}
);

export { loadHeatmap };
