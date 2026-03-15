import { DateTime } from "luxon";
import type { PageServerLoad } from "./$types";
import {
	TransportType,
	StationSnapshotType,
	type MetricSeries,
	type BaseStation,
	type BaseMetricDataPoint,
	type BaseMetricDataPointStationDataPoint,
	MetricSeriesType
} from "@lib/api";
import {
	type StationHeatmapSettings,
	type StationHeatmapScheduleType,
	type StationHeatmapPlotType
} from "@lib/components/statistics/global/StationHeatmapSettingsDialog.svelte";
import type { StationHeatmapPoint } from "@lib/components/statistics/global/StationHeatmap.svelte";
import { loadMetric } from "../projectdimensions.remote";
import { getStationBatch } from "@lib/remote/station.remote";

const getSnapshot = (scheduleType: StationHeatmapScheduleType, plotType: StationHeatmapPlotType): StationSnapshotType => {
	if (scheduleType === "arrivals") {
		if (plotType === "count") return StationSnapshotType.ARRIVALS;
		if (plotType === "cancellations") return StationSnapshotType.ARRIVAL_CANCELLATIONS;
		if (plotType === "delay_avg") return StationSnapshotType.ARRIVAL_DELAY_AVG;
		return StationSnapshotType.ARRIVALS;
	}

	if (scheduleType === "departures") {
		if (plotType === "count") return StationSnapshotType.DEPARTURES;
		if (plotType === "cancellations") return StationSnapshotType.DEPARTURE_CANCELLATIONS;
		if (plotType === "delay_avg") return StationSnapshotType.DEPARTURE_DELAY_AVG;
		return StationSnapshotType.DEPARTURES;
	}

	throw new Error("Invalid schedule type or plot type");
};

export const load: PageServerLoad = async ({
	url,
	getClientAddress
}): Promise<{
	heatmapPoints: Promise<StationHeatmapPoint[]>;
	seriesTypes: Promise<MetricSeriesType[]>;
	settings: StationHeatmapSettings;
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

	const scheduleType = (url.searchParams.get("scheduleType") as StationHeatmapScheduleType) ?? "arrivals";
	const plotType = (url.searchParams.get("plotType") as StationHeatmapPlotType) ?? "count";

	const metrics = loadMetric({
		request: {
			queryType: "STATION_SUMMARY",
			start: start.toISO(),
			end: end.toISO()!,
			...(transportTypesFilter.length > 0 ? { transportTypes: transportTypesFilter } : {}),
			snapshot: getSnapshot(scheduleType, plotType)
		},
		userIp: getClientAddress()
	});

	return {
		heatmapPoints: metrics.then(async (metrics: MetricSeries[]) => {
			// extract data points with evaNumber and value
			const dataPoints = metrics
				.flatMap((metric: MetricSeries) => metric.dataPoints)
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
		}),
		seriesTypes: metrics.then((metrics: MetricSeries[]) => [
			...new Set(metrics.map((metric: MetricSeries) => metric.seriesType))
		]),
		settings: {
			dates: {
				start,
				end
			},
			transportTypes: transportTypesFilter,
			scheduleType,
			plotType
		}
	};
};
