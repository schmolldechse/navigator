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
	import { DateTime } from "luxon";
	import { MetricSeriesType, TransportType } from "@lib/api";
	import StationHeatmap, { type StationHeatmapPoint } from "@lib/components/statistics/global/StationHeatmap.svelte";
	import StationHeatmapSettingsDialog, {
		type StationHeatmapSettings
	} from "@lib/components/statistics/global/StationHeatmapSettingsDialog.svelte";
	import { interpolateTurbo } from "d3-scale-chromatic";
	import { Legend } from "layerchart";
	import { scaleSequential } from "d3-scale";
	import { Tween } from "svelte/motion";
	import { cubicOut } from "svelte/easing";
	import Button from "@lib/components/interactable/button/Button.svelte";
	import { goto } from "$app/navigation";
	import type { PageProps } from "./$types";
	import { setContext } from "svelte";

	let { data }: PageProps = $props();

	let heatmapPointsLoading: boolean = $state(true);
	let stationHeatmapPoints: StationHeatmapPoint[] = $state([]);
	const heatmapExtent = new Tween<[number, number]>([0, 100], {
		duration: 500,
		easing: cubicOut
	});

	$effect(() => {
		data.heatmapPoints
			.then((points: StationHeatmapPoint[]) => (stationHeatmapPoints = points))
			.finally(() => (heatmapPointsLoading = false));
	});

	// dialog
	let isSettingsDialogOpen: boolean = $state(false);

	let seriesType: MetricSeriesType | undefined = $state(undefined);

	$effect(() => {
		data.seriesTypes.then((seriesTypes: MetricSeriesType[]) => {
			if (!seriesTypes || seriesTypes.length === 0) return;
			seriesType = seriesTypes[0];
			setContext("METRIC_SERIES_TYPE", () => seriesType);
		});
	});
</script>

<svelte:head>
	<title>Statistics - Navigator</title>
</svelte:head>

<main class="container mx-auto flex flex-col space-y-8 p-4 sm:py-8">
	<section class="mx-auto w-full max-w-7xl space-y-4">
		<!-- Heatmap Header -->
		<div class="relative flex flex-row items-center justify-between">
			<h2 class="text-2xl font-medium">Station Heatmap</h2>

			<Button
				mode="secondary"
				disabled={heatmapPointsLoading}
				onclick={(event: MouseEvent) => {
					event.stopPropagation();
					isSettingsDialogOpen = !isSettingsDialogOpen;
				}}
			>
				<SlidersHorizontal size={18} />

				<div class="hidden items-baseline gap-x-1 md:flex">
					<span class="text-sm tracking-tight">{data.settings.dates.start.toLocaleString(DateTime.DATE_MED)}</span>
					<span>&nbsp;–&nbsp;</span>
					<span class="text-sm tracking-tight">{data.settings.dates.end.toLocaleString(DateTime.DATE_MED)}</span>
				</div>
			</Button>

			<StationHeatmapSettingsDialog
				bind:isVisible={isSettingsDialogOpen}
				initialSettings={data.settings}
				onapply={async (settings: StationHeatmapSettings) => {
					const url = new URL(window.location.href);
					url.searchParams.set("start", settings.dates.start.startOf("day").toISO() as string);
					url.searchParams.set("end", settings.dates.end.endOf("day").toISO() as string);

					url.searchParams.delete("transportTypes");
					settings.transportTypes.forEach((transportType: TransportType) =>
						url.searchParams.append("transportTypes", transportType)
					);

					url.searchParams.set("scheduleType", settings.scheduleType);
					url.searchParams.set("plotType", settings.plotType);

					heatmapPointsLoading = true;
					await goto(url, { invalidateAll: true, replaceState: true, keepFocus: true, noScroll: true });
				}}
				class="mt-14 self-start justify-self-end"
			/>
		</div>

		<div class="flex flex-col gap-y-4">
			<div class="relative h-[650px] w-full transition-all duration-300">
				<StationHeatmap
					bind:scale={heatmapExtent.target}
					{stationHeatmapPoints}
					class="border-muted-foreground/10 z-10 h-full w-full rounded-xl border"
				/>

				{#await data.heatmapPoints}
					<div
						class="bg-background/50 absolute inset-0 z-20 flex flex-col items-center justify-center rounded-xl backdrop-blur-sm"
					>
						<LoaderCircle size={32} class="text-accent animate-spin" />
						<span class="text-text mt-2 text-sm font-medium">Generating heatmap...</span>
					</div>
				{/await}
			</div>

			<Legend
				scale={scaleSequential(heatmapExtent.current, interpolateTurbo)}
				title={translations.find((translation: Translations) => translation.seriesType === seriesType)?.title ||
					"Requested metric"}
				tickFormat={(value: number) => value.toLocaleString()}
				class="transition-all duration-300"
			/>
		</div>
	</section>
</main>
