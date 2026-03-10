<script lang="ts">
	import SlidersHorizontal from "@lucide/svelte/icons/sliders-horizontal";
	import LoaderCircle from "@lucide/svelte/icons/loader-circle";
	import { DateTime } from "luxon";
	import { loadMetric } from "../projectdimensions.remote";
	import {
		StationSnapshotType,
		type BaseMetricDataPoint,
		type BaseMetricDataPointStationDataPoint,
		type BaseStation,
		type MetricSeries
	} from "@lib/api";
	import { env } from "$env/dynamic/public";
	import StationHeatmap from "@lib/components/statistics/global/StationHeatmap.svelte";
	import StationHeatmapSettingsDialog, {
		type StationHeatmapPlotType,
		type StationHeatmapScheduleType
	} from "@lib/components/statistics/global/StationHeatmapSettingsDialog.svelte";
	import type { HeatPoint } from "@lib/leafletImplementation/heatmapLayer";

	// dialog
	let isSettingsDialogOpen: boolean = $state(false);

	// options
	let heatmapTimerange: { start: DateTime; end: DateTime } = $state({
		start: DateTime.now().startOf("year"),
		end: DateTime.now()
	});

	// promise
	let heatmapPromise: Promise<HeatPoint[]> | null = $state(null);

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

	const loadHeatmap = async (metrics: MetricSeries[]): Promise<HeatPoint[]> => {
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

		// aggregate values per evaNumber (sum across transport types)
		const valueByEva = new Map<number, number>();
		for (const dataPoint of dataPoints) {
			const eva = Number(dataPoint.evaNumber);
			valueByEva.set(eva, (valueByEva.get(eva) ?? 0) + Number(dataPoint.value));
		}

		const stations = await getStationBatch({ evaNumbers });

		// build lookup map
		const stationMap = new Map<number, BaseStation>();
		for (const station of stations) {
			stationMap.set(Number(station.evaNumber), station);
		}

		// build heat points
		const points: HeatPoint[] = [];
		for (const [eva, value] of valueByEva) {
			const station = stationMap.get(eva);
			if (!station) continue;

			points.push([Number(station.position.latitude), Number(station.position.longitude), value]);
		}

		return points;
	};

	const getStationBatch = async ({ evaNumbers }: { evaNumbers: number[] }) => {
		const stations = await fetch(`${env.PUBLIC_API_URL}/api/v1/stations/batch`, {
			method: "POST",
			headers: {
				"Content-Type": "application/json",
				Accept: "*/*"
			},
			body: JSON.stringify(evaNumbers)
		});

		if (!stations.ok) throw new Error(`Error fetching stations: ${stations.status} ${stations.statusText}`);
		return (await stations.json()) as BaseStation[];
	};
</script>

<svelte:head>
	<title>Statistics - Navigator</title>
</svelte:head>

<main class="container mx-auto flex flex-col space-y-8 p-4 sm:py-8">
	<section class="mx-auto w-full max-w-7xl space-y-4">
		<!-- Heatmap Header -->
		<div class="relative flex flex-row items-center justify-between">
			<h2 class="text-2xl font-medium">Station Heatmap</h2>

			<button
				class="bg-muted/70 border-muted-foreground/20 hover:bg-muted-foreground/20 flex w-fit cursor-pointer items-center justify-center gap-x-2 rounded-md border px-4 py-2 transition-colors"
				onclick={(event: MouseEvent) => {
					event.stopPropagation();
					isSettingsDialogOpen = !isSettingsDialogOpen;
				}}
			>
				<SlidersHorizontal size={18} />

				<div class="hidden items-baseline gap-x-1 md:flex">
					<span class="text-sm tracking-tight">{heatmapTimerange.start?.toLocaleString(DateTime.DATE_MED)}</span>
					<span>&nbsp;–&nbsp;</span>
					<span class="text-sm tracking-tight">{heatmapTimerange.end?.toLocaleString(DateTime.DATE_MED)}</span>
				</div>
			</button>

			<StationHeatmapSettingsDialog
				bind:isVisible={isSettingsDialogOpen}
				initialDates={heatmapTimerange}
				initialTransportTypes={[]}
				initialScheduleType="arrivals"
				initialPlotType="count"
				onapply={({ start, end, transportTypes, scheduleType, plotType }) => {
					heatmapPromise = loadMetric({
						request: {
							queryType: "STATION_SUMMARY",
							snapshot: getSnapshot(scheduleType, plotType),
							start: start.toISO(),
							end: end.toISO()!,
							transportTypes
						}
					}).then(loadHeatmap);
				}}
				class="mt-14 self-start justify-self-end"
			/>
		</div>

		<div class="h-[650px] transition-all duration-300">
			{#await heatmapPromise}
				<div class="bg-muted/30 border-muted-foreground/10 flex h-full items-center justify-center rounded-xl border">
					<div class="flex flex-col items-center gap-y-3">
						<LoaderCircle size={32} class="text-accent animate-spin" />
						<span class="text-muted-foreground text-sm">Loading heatmap data…</span>
					</div>
				</div>
			{:then heatPoints}
				<StationHeatmap heatPoints={heatPoints!} class="border-muted-foreground/10 z-10 h-full rounded-xl border" />
			{:catch}
				<div class="bg-muted/30 border-muted-foreground/10 flex h-full items-center justify-center rounded-xl border">
					<div class="flex flex-col items-center gap-y-3">
						<span class="text-muted-foreground text-sm">Failed to load heatmap</span>
					</div>
				</div>
			{/await}
		</div>
	</section>
</main>
