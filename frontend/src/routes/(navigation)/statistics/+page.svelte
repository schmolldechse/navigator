<script module lang="ts">
	const getMetricSeriesTypeByHeatmapSettings = (
		scheduleType: StationHeatmapScheduleType,
		plotType: StationHeatmapPlotType
	): MetricSeriesType => {
		if (scheduleType === "arrivals") {
			if (plotType === "count") return MetricSeriesType.STATION_ARRIVALS;
			if (plotType === "cancellations") return MetricSeriesType.STATION_ARRIVAL_CANCELLATIONS;
			if (plotType === "delay_avg") return MetricSeriesType.STATION_ARRIVAL_DELAY_AVG;
			return MetricSeriesType.STATION_ARRIVALS;
		}

		if (scheduleType === "departures") {
			if (plotType === "count") return MetricSeriesType.STATION_DEPARTURES;
			if (plotType === "cancellations") return MetricSeriesType.STATION_DEPARTURE_CANCELLATIONS;
			if (plotType === "delay_avg") return MetricSeriesType.STATION_DEPARTURE_DELAY_AVG;
			return MetricSeriesType.STATION_DEPARTURES;
		}

		throw new Error("Invalid heatmap settings combination");
	};

	const getHeatmapSettingsByMetricSeriesType = (
		seriesType: MetricSeriesType
	): { scheduleType: StationHeatmapScheduleType; plotType: StationHeatmapPlotType } => {
		switch (seriesType) {
			case MetricSeriesType.STATION_ARRIVALS:
				return { scheduleType: "arrivals", plotType: "count" };
			case MetricSeriesType.STATION_ARRIVAL_CANCELLATIONS:
				return { scheduleType: "arrivals", plotType: "cancellations" };
			case MetricSeriesType.STATION_ARRIVAL_DELAY_AVG:
				return { scheduleType: "arrivals", plotType: "delay_avg" };
			case MetricSeriesType.STATION_DEPARTURES:
				return { scheduleType: "departures", plotType: "count" };
			case MetricSeriesType.STATION_DEPARTURE_CANCELLATIONS:
				return { scheduleType: "departures", plotType: "cancellations" };
			case MetricSeriesType.STATION_DEPARTURE_DELAY_AVG:
				return { scheduleType: "departures", plotType: "delay_avg" };
			default:
				throw new Error("Unsupported MetricSeriesType");
		}
	};
</script>

<script lang="ts">
	import { MetricSeriesType, type MetricSeries } from "@lib/api";
	import type { PageProps } from "./$types";
	import Button from "@lib/components/interactable/Button.svelte";
	import type {
		StationHeatmapPlotType,
		StationHeatmapScheduleType,
		StationHeatmapSettings
	} from "@lib/components/statistics/heatmap/settings/StationHeatmapSettingsDialog.svelte";
	import SlidersHorizontal from "@lucide/svelte/icons/sliders-horizontal";
	import { Tween } from "svelte/motion";
	import { cubicOut } from "svelte/easing";
	import { DateTime } from "luxon";
	import type { StationHeatmapPoint } from "@lib/components/statistics/heatmap/StationHeatmap.svelte";
	import StationHeatmapSettingsDialog from "@lib/components/statistics/heatmap/settings/StationHeatmapSettingsDialog.svelte";
	import {
		loadHeatmapBySeriesType,
		loadHourlyMetrics,
		loadJourneyServiceMetrics,
		loadMessageSummaryMetrics
	} from "./globalstatistics.remote";
	import StationHeatmap from "@lib/components/statistics/heatmap/StationHeatmap.svelte";
	import LoaderCircle from "@lucide/svelte/icons/loader-circle";
	import { Legend } from "layerchart";
	import { scaleSequential } from "d3-scale";
	import { interpolateTurbo } from "d3-scale-chromatic";
	import Separator from "@lib/components/interactable/Separator.svelte";
	import type { HourlyTransportSettings } from "@lib/components/statistics/global/settings/HourlyTransportSettingsDialog.svelte";
	import HourlyTransportSettingsDialog from "@lib/components/statistics/global/settings/HourlyTransportSettingsDialog.svelte";
	import type { Component } from "svelte";
	import HourlyStopRate from "@lib/components/statistics/global/charts/HourlyStopRate.svelte";
	import CancellationRate from "@lib/components/statistics/global/charts/cancellationrate/CancellationRate.svelte";
	import DelayAnalysis from "@lib/components/statistics/global/charts/delayanalysis/DelayAnalysis.svelte";
	import PunctualityRate from "@lib/components/statistics/global/charts/PunctualityRate.svelte";
	import DelaySeverityBuckets from "@lib/components/statistics/global/charts/DelaySeverityBuckets.svelte";
	import PlatformChangeRate from "@lib/components/statistics/global/charts/PlatformChangeRate.svelte";
	import OperatorReliability from "@lib/components/statistics/global/charts/OperatorReliability.svelte";
	import JourneyTypeShare from "@lib/components/statistics/global/charts/JourneyTypeShare.svelte";
	import DisruptionCauses from "@lib/components/statistics/global/charts/DisruptionCauses.svelte";

	let { data }: PageProps = $props();

	// heatmap settings
	let heatmapLoading: boolean = $state(true);
	let heatmapPromise: Promise<StationHeatmapPoint[]> = $state(data.streamed.heatmapMetrics);
	let heatmapPoints: StationHeatmapPoint[] = $state([]);
	let heatmapSettings: StationHeatmapSettings = $state({
		dates: {
			start: data.timerange.start,
			end: data.timerange.end
		},
		transportTypes: data.transportTypes,
		plotType: getHeatmapSettingsByMetricSeriesType(data.seriesTypes.heatmap).plotType,
		scheduleType: getHeatmapSettingsByMetricSeriesType(data.seriesTypes.heatmap).scheduleType
	});
	let heatmapSettingsDialogOpen: boolean = $state(false);

	const heatmapExtent = new Tween<[number, number]>([0, 100], {
		duration: 500,
		easing: cubicOut
	});

	$effect(() => {
		if (!heatmapPromise) return;

		heatmapLoading = true;
		heatmapPromise.then((points: StationHeatmapPoint[]) => (heatmapPoints = points)).finally(() => (heatmapLoading = false));
	});

	// hourly metrics
	let hourlyTransportMetricsLoading: boolean = $state(true);
	let hourlyTransportMetricsPromise: Promise<MetricSeries[]> = $state(data.streamed.hourlyTransportMetrics);
	let journeyServiceMetricsPromise: Promise<MetricSeries[]> = $state(data.streamed.journeyServiceMetrics);
	let messageSummaryMetricsPromise: Promise<MetricSeries[]> = $state(data.streamed.messageSummaryMetrics);
	let hourlyTransportSettings: HourlyTransportSettings = $state({
		dates: {
			start: data.timerange.start,
			end: data.timerange.end
		},
		transportTypes: data.transportTypes
	});
	let hourlyTransportSettingsDialogOpen: boolean = $state(false);

	$effect(() => {
		if (!hourlyTransportMetricsPromise) return;

		hourlyTransportMetricsLoading = true;
		hourlyTransportMetricsPromise.finally(() => (hourlyTransportMetricsLoading = false));
	});

	type MetricCardData = {
		metricComponent: Component<any>;
		props: Record<string, unknown>;
	};
	let hourlyMetricCards: MetricCardData[] = $derived([
		{
			metricComponent: HourlyStopRate,
			props: {
				promise: hourlyTransportMetricsPromise
			}
		},
		{
			metricComponent: PunctualityRate,
			props: {
				promise: hourlyTransportMetricsPromise
			}
		},
		{
			metricComponent: DelaySeverityBuckets,
			props: {
				promise: hourlyTransportMetricsPromise
			}
		},
		{
			metricComponent: PlatformChangeRate,
			props: {
				promise: hourlyTransportMetricsPromise
			}
		},
		{
			metricComponent: DelayAnalysis,
			props: {
				promise: hourlyTransportMetricsPromise,
				class: "xl:col-span-2"
			}
		},
		{
			metricComponent: CancellationRate,
			props: {
				promise: hourlyTransportMetricsPromise,
				class: "xl:col-span-2"
			}
		}
	]);
	let serviceMetricCards: MetricCardData[] = $derived([
		{
			metricComponent: OperatorReliability,
			props: {
				promise: journeyServiceMetricsPromise
			}
		},
		{
			metricComponent: JourneyTypeShare,
			props: {
				promise: journeyServiceMetricsPromise
			}
		},
		{
			metricComponent: DisruptionCauses,
			props: {
				promise: messageSummaryMetricsPromise,
				class: "xl:col-span-2"
			}
		}
	]);
</script>

<svelte:head>
	<title>Statistics - Navigator</title>
</svelte:head>

<main class="container mx-auto flex flex-col space-y-8 p-4 sm:py-8">
	<!-- Heatmap -->
	<section class="space-y-4">
		<div class="relative flex flex-row items-center justify-between">
			<h2 class="text-2xl font-medium">Station Heatmap</h2>

			<Button
				mode="secondary"
				disabled={heatmapLoading}
				onclick={(event: MouseEvent) => {
					event.stopPropagation();
					heatmapSettingsDialogOpen = !heatmapSettingsDialogOpen;
				}}
			>
				<SlidersHorizontal size={18} />

				<div class="hidden items-baseline gap-x-1 md:flex">
					<span class="text-sm tracking-tight">{heatmapSettings.dates.start.toLocaleString(DateTime.DATE_MED)}</span>
					<span>&nbsp;–&nbsp;</span>
					<span class="text-sm tracking-tight">{heatmapSettings.dates.end.toLocaleString(DateTime.DATE_MED)}</span>
				</div>
			</Button>

			<StationHeatmapSettingsDialog
				bind:isVisible={heatmapSettingsDialogOpen}
				initialSettings={heatmapSettings}
				onapply={async (settings: StationHeatmapSettings) => {
					heatmapSettings = settings;

					const seriesType = getMetricSeriesTypeByHeatmapSettings(settings.scheduleType, settings.plotType);
					heatmapPromise = loadHeatmapBySeriesType({
						request: {
							seriesType,
							start: settings.dates.start.startOf("day").toISO()!,
							end: settings.dates.end.endOf("day").toISO()!,
							...(settings.transportTypes.length > 0 ? { transportTypes: settings.transportTypes } : {})
						}
					});
				}}
				class="mt-14 self-start justify-self-end"
			/>
		</div>

		<div class="flex flex-col gap-y-4">
			<div class="relative h-[650px] w-full transition-all duration-300">
				<StationHeatmap
					bind:scale={heatmapExtent.target}
					stationHeatmapPoints={heatmapPoints}
					class="border-muted-foreground/10 z-10 h-full w-full rounded-xl border"
				/>

				{#if heatmapLoading}
					<div
						class="bg-background/50 absolute inset-0 z-20 flex flex-col items-center justify-center rounded-xl backdrop-blur-sm"
					>
						<LoaderCircle size={32} class="text-accent animate-spin" />
						<span class="text-text mt-2 text-sm font-medium">Generating heatmap...</span>
					</div>
				{/if}
			</div>

			<Legend
				scale={scaleSequential(heatmapExtent.current, interpolateTurbo)}
				title="Requested metric"
				tickFormat={(value: number) => value.toLocaleString()}
				class="transition-all duration-300"
			/>
		</div>
	</section>

	<Separator />

	<!-- Hourly Metrics by Transport Type -->
	<section class="space-y-4">
		<div class="relative flex flex-row items-center justify-between">
			<h2 class="text-2xl font-medium">Hourly Metrics by Transport Type</h2>

			<Button
				mode="secondary"
				disabled={heatmapLoading}
				onclick={(event: MouseEvent) => {
					event.stopPropagation();
					hourlyTransportSettingsDialogOpen = !hourlyTransportSettingsDialogOpen;
				}}
			>
				<SlidersHorizontal size={18} />

				<div class="hidden items-baseline gap-x-1 md:flex">
					<span class="text-sm tracking-tight">{hourlyTransportSettings.dates.start.toLocaleString(DateTime.DATE_MED)}</span>
					<span>&nbsp;–&nbsp;</span>
					<span class="text-sm tracking-tight">{hourlyTransportSettings.dates.end.toLocaleString(DateTime.DATE_MED)}</span>
				</div>
			</Button>

			<HourlyTransportSettingsDialog
				bind:isVisible={hourlyTransportSettingsDialogOpen}
				initialSettings={hourlyTransportSettings}
				onapply={async (settings: HourlyTransportSettings) => {
					hourlyTransportSettings = settings;

					hourlyTransportMetricsPromise = loadHourlyMetrics({
						request: {
							start: settings.dates.start.startOf("day").toISO()!,
							end: settings.dates.end.endOf("day").toISO()!,
							...(settings.transportTypes.length > 0 ? { transportTypes: settings.transportTypes } : {})
						}
					});
					journeyServiceMetricsPromise = loadJourneyServiceMetrics({
						request: {
							start: settings.dates.start.startOf("day").toISO()!,
							end: settings.dates.end.endOf("day").toISO()!,
							...(settings.transportTypes.length > 0 ? { transportTypes: settings.transportTypes } : {})
						}
					});
					messageSummaryMetricsPromise = loadMessageSummaryMetrics({
						request: {
							start: settings.dates.start.startOf("day").toISO()!,
							end: settings.dates.end.endOf("day").toISO()!,
							...(settings.transportTypes.length > 0 ? { transportTypes: settings.transportTypes } : {}),
							limit: 12
						}
					});
				}}
				class="mt-14 self-start justify-self-end"
			/>
		</div>

		<div class="grid grid-cols-1 gap-4 xl:grid-cols-2">
			{#each hourlyMetricCards as metricCard}
				{@const MetricComponent = metricCard.metricComponent}
				<MetricComponent {...metricCard.props} />
			{/each}
		</div>
	</section>

	<Separator />

	<section class="space-y-4">
		<div class="relative flex flex-row items-center justify-between">
			<h2 class="text-2xl font-medium">Service Transparency</h2>
		</div>

		<div class="grid grid-cols-1 gap-4 xl:grid-cols-2">
			{#each serviceMetricCards as metricCard}
				{@const MetricComponent = metricCard.metricComponent}
				<MetricComponent {...metricCard.props} />
			{/each}
		</div>
	</section>
</main>
