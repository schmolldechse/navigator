<script module lang="ts">
	type Translations = {
		seriesType: MetricSeriesType;
		title: string;
	};
	const translations: Translations[] = [
		{ seriesType: MetricSeriesType.STATION_ARRIVALS, title: "Arrivals (amount)" },
		{ seriesType: MetricSeriesType.STATION_ARRIVAL_CANCELLATIONS, title: "Arrival cancellations (amount)" },
		{ seriesType: MetricSeriesType.STATION_ARRIVAL_DELAY_AVG, title: "Arrival avg delay (s)" },
		{ seriesType: MetricSeriesType.STATION_DEPARTURES, title: "Departures (amount)" },
		{ seriesType: MetricSeriesType.STATION_DEPARTURE_CANCELLATIONS, title: "Departure cancellations (amount)" },
		{ seriesType: MetricSeriesType.STATION_DEPARTURE_DELAY_AVG, title: "Departure avg delay (s)" }
	];
</script>

<script lang="ts">
	import SlidersHorizontal from "@lucide/svelte/icons/sliders-horizontal";
	import LoaderCircle from "@lucide/svelte/icons/loader-circle";
	import Layers from "@lucide/svelte/icons/layers";
	import { DateTime } from "luxon";
	import { loadMetric } from "../projectdimensions.remote";
	import {
		MetricSeriesType,
		StationSnapshotType,
		type BaseMetricDataPoint,
		type BaseMetricDataPointStationDataPoint,
		type BaseStation,
		type MetricSeries
	} from "@lib/api";
	import { env } from "$env/dynamic/public";
	import StationHeatmap, { type StationHeatmapPoint } from "@lib/components/statistics/global/StationHeatmap.svelte";
	import StationHeatmapSettingsDialog, {
		type StationHeatmapPlotType,
		type StationHeatmapScheduleType,
		type StationHeatmapSettings
	} from "@lib/components/statistics/global/StationHeatmapSettingsDialog.svelte";
	import { interpolateTurbo } from "d3-scale-chromatic";
	import { Legend } from "layerchart";
	import { scaleSequential } from "d3-scale";
	import Button from "@lib/components/interactable/button/Button.svelte";

	// dialog
	let isSettingsDialogOpen: boolean = $state(false);

	// settings
	let heatmapSettings: StationHeatmapSettings = $state({
		dates: {
			start: DateTime.now().startOf("year"),
			end: DateTime.now()
		},
		scheduleType: "arrivals",
		plotType: "count",
		transportTypes: []
	});
	let isGenerating: boolean = $state(false);

	let stationHeatmapPoints: StationHeatmapPoint[] = $state([]);
	const stations: Map<number, BaseStation> = new Map<number, BaseStation>();

	let heatmapMetrics: MetricSeries[] = $state([]);
	let heatmapValues = $derived(
		heatmapMetrics
			.flatMap((metric: MetricSeries) => metric.dataPoints)
			.map((dataPoint: BaseMetricDataPoint) => Number(dataPoint.value))
	);
	let heatmapExtent = $derived(heatmapValues.length > 0 ? [Math.min(...heatmapValues), Math.max(...heatmapValues)] : [0, 100]);

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

	const loadHeatmap = async () => {
		isGenerating = true;

		await loadMetric({
			request: {
				queryType: "STATION_SUMMARY",
				snapshot: getSnapshot(heatmapSettings.scheduleType, heatmapSettings.plotType),
				start: heatmapSettings.dates.start.toISO()!,
				end: heatmapSettings.dates.end.toISO()!,
				transportTypes: heatmapSettings.transportTypes
			}
		})
			.then(async (metrics: MetricSeries[]) => {
				heatmapMetrics = metrics;

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

				const missingEvaNumbers = evaNumbers.filter((evaNumber) => !stations.has(evaNumber));
				if (missingEvaNumbers.length > 0) {
					const fetchedStations = await getStationBatch({ evaNumbers: missingEvaNumbers });
					for (const station of fetchedStations) {
						stations.set(Number(station.evaNumber), station);
					}
				}

				// aggregate values per evaNumber (sum across transport types)
				const valueByEva = new Map<number, number>();
				for (const dataPoint of dataPoints) {
					const eva = Number(dataPoint.evaNumber);
					valueByEva.set(eva, (valueByEva.get(eva) ?? 0) + Number(dataPoint.value));
				}

				// build heat points
				const points: { station: BaseStation; value: number }[] = [];
				for (const [eva, value] of valueByEva) {
					const station = stations.get(eva);
					if (!station) continue;

					points.push({ station, value });
				}
				stationHeatmapPoints = points;
			})
			.catch(() => (heatmapMetrics = []))
			.finally(() => (isGenerating = false));
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

			<div class="flex items-center gap-x-2">
				<Button mode="primary" disabled={isGenerating} onclick={loadHeatmap}>
					{#if isGenerating}
						<LoaderCircle size={16} class="animate-spin" />
					{:else}
						<Layers size={16} />
					{/if}
					<span class="hidden font-medium md:block">Generate</span>
				</Button>

				<button
					class="bg-muted/70 border-muted-foreground/20 hover:bg-muted-foreground/20 flex w-fit cursor-pointer items-center justify-center gap-x-2 rounded-md border px-4 py-2 transition-colors"
					onclick={(event: MouseEvent) => {
						event.stopPropagation();
						isSettingsDialogOpen = !isSettingsDialogOpen;
					}}
				>
					<SlidersHorizontal size={18} />

					<div class="hidden items-baseline gap-x-1 md:flex">
						<span class="text-sm tracking-tight">{heatmapSettings.dates.start.toLocaleString(DateTime.DATE_MED)}</span>
						<span>&nbsp;–&nbsp;</span>
						<span class="text-sm tracking-tight">{heatmapSettings.dates.end.toLocaleString(DateTime.DATE_MED)}</span>
					</div>
				</button>
			</div>

			<StationHeatmapSettingsDialog
				bind:isVisible={isSettingsDialogOpen}
				initialSettings={heatmapSettings}
				onapply={(settings: StationHeatmapSettings) => (heatmapSettings = settings)}
				class="mt-14 self-start justify-self-end"
			/>
		</div>

		<div class="flex flex-col gap-y-4">
			<div class="relative h-[650px] w-full transition-all duration-300">
				<StationHeatmap {stationHeatmapPoints} class="border-muted-foreground/10 z-10 h-full w-full rounded-xl border" />

				{#if isGenerating}
					<div
						class="bg-background/50 absolute inset-0 z-20 flex flex-col items-center justify-center rounded-xl backdrop-blur-sm"
					>
						<LoaderCircle size={32} class="text-accent animate-spin" />
						<span class="text-text mt-2 text-sm font-medium">Generating heatmap...</span>
					</div>
				{/if}
			</div>

			{#if heatmapMetrics.length > 0}
				{@const legendTitle = translations.find(
					(translation: Translations) => translation.seriesType === heatmapMetrics[0].seriesType
				)?.title!}
				<Legend
					scale={scaleSequential(heatmapExtent, interpolateTurbo)}
					title={legendTitle}
					tickFormat={(value: number) => value.toLocaleString()}
				/>
			{/if}
		</div>
	</section>
</main>
