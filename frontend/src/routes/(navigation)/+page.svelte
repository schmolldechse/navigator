<script lang="ts" module>
	interface UnknownTypeTitle<T> {
		type: T;
		title: string;
	}

	interface MetricCardData {
		title: string;
		requestMetricType: MetricQueryType;
		promise: () => Promise<MetricSeries[]>;
		typeTitles: UnknownTypeTitle<any>[];
		chartDialogDetails: {
			isOpenable: boolean;
			chartType?: ChartType;
		};
	}

	export type { MetricCardData, UnknownTypeTitle };
</script>

<script lang="ts">
	import { goto } from "$app/navigation";
	import { MetricSeriesType, TransportType, type MetricSeries, MetricQueryType } from "$lib/api/types.gen.js";
	import TimePickerDialog from "@lib/components/interactable/TimePickerDialog.svelte";
	import MetricCard from "@lib/components/statistics/MetricCard.svelte";
	import MetricChartDialog from "@lib/components/statistics/MetricChartDialog.svelte";
	import { ChartType } from "@lib/util/chart.js";
	import type { TimerangeOption } from "@lib/util/timerange.js";
	import Info from "@lucide/svelte/icons/info";
	import Calendar from "@lucide/svelte/icons/calendar";
	import { DateTime } from "luxon";

	let { data } = $props();

	let timeranges: TimerangeOption[] = [
		{
			id: "24H",
			label: "24 Hours",
			value: { start: DateTime.now().minus({ days: 1 }), end: DateTime.now() }
		},
		{
			id: "7D",
			label: "7 Days",
			value: { start: DateTime.now().minus({ days: 7 }), end: DateTime.now() }
		},
		{
			id: "30D",
			label: "30 Days",
			value: { start: DateTime.now().minus({ days: 30 }), end: DateTime.now() }
		},
		{
			id: "CUSTOM",
			label: "Custom Range",
			isCustom: true
		}
	];

	let selectingTimerange: boolean = $state(true);
	let selectedTimerange: TimerangeOption = $state(timeranges[0]);

	const updateTimerange = async (start: DateTime, end: DateTime) => {
		const url = new URL(window.location.href);
		url.searchParams.set("start", start.toISO()!);
		url.searchParams.set("end", end.toISO()!);

		await goto(url, { replaceState: true, keepFocus: true, noScroll: true });
		// invalidate("project:dimensions");
	};

	let metricCards: MetricCardData[] = [
		{
			title: "Total Database Size",
			requestMetricType: data.dimensions.databaseSize.requestMetricType,
			promise: () => data.dimensions.databaseSize.promise,
			typeTitles: [{ type: MetricSeriesType.DATABASE_SIZE, title: "Database Size" }],
			chartDialogDetails: {
				isOpenable: true,
				chartType: ChartType.AREA
			}
		},
		{
			title: "Distribution of Transport Types",
			requestMetricType: data.dimensions.transportTypes.requestMetricType,
			promise: () => data.dimensions.transportTypes.promise,
			typeTitles: [
				{ type: TransportType.HIGH_SPEED_TRAIN, title: "Long-distance transport" },
				{ type: TransportType.INTERCITY_TRAIN, title: "Intercity train" },
				{ type: TransportType.INTER_REGIONAL_TRAIN, title: "Inter-regional transport" },
				{ type: TransportType.REGIONAL_TRAIN, title: "Regional transport" },
				{ type: TransportType.CITY_TRAIN, title: "City train" },
				{ type: TransportType.TRAM, title: "Tram" },
				{ type: TransportType.BUS, title: "Bus" },
				{ type: TransportType.UNKNOWN, title: "Unknown" }
			],
			chartDialogDetails: { isOpenable: false }
		},
		{
			title: "Recorded RIS IDs",
			requestMetricType: data.dimensions.totalRisIds.requestMetricType,
			promise: () => data.dimensions.totalRisIds.promise,
			typeTitles: [
				{ type: MetricSeriesType.RIS_IDS_ACTIVE, title: "Active RIS IDs" },
				{ type: MetricSeriesType.RIS_IDS_INACTIVE, title: "Inactive RIS IDs" }
			],
			chartDialogDetails: { isOpenable: true, chartType: ChartType.AREA }
		},
		{
			title: "Recorded Journeys",
			requestMetricType: data.dimensions.totalJourneys.requestMetricType,
			promise: () => data.dimensions.totalJourneys.promise,
			typeTitles: [{ type: MetricSeriesType.JOURNEY_TOTAL, title: "Total Journeys" }],
			chartDialogDetails: { isOpenable: true, chartType: ChartType.AREA }
		}
	];

	let chartDialogOpen: boolean = $state(false);
	let selectedMetricCard: {
		title: string;
		metrics: MetricSeries[];
		typeTitles: UnknownTypeTitle<any>[];
		chartType?: ChartType;
	} | null = $state(null);

	/**
	 * TODO:
	 * 1) Change metricCards which hold the Card Component as Snippet, uses MetricCard as base
	 */
</script>

<svelte:head>
	<title>Navigator</title>
</svelte:head>

<main class="container mx-auto flex flex-col space-y-8 p-4 sm:py-8">
	<!-- Hero Section -->
	<section class="mx-auto max-w-7xl">
		<div class="relative mb-4">
			<div class="bg-accent absolute top-0 bottom-0 w-0.5 sm:-left-4 sm:w-1"></div>
			<h1 class="pl-4 text-3xl font-bold text-balance sm:mb-6 sm:pl-8 sm:text-4xl md:text-6xl">
				Visualizing Public Transport Performance
			</h1>
		</div>

		<p class="text-muted-foreground max-w-3xl pl-4 text-base sm:pl-8 sm:text-lg lg:text-xl">
			Clear insights. <span class="text-accent font-semibold">Transparency</span> for Germany's Public Transport.
		</p>
	</section>

	<!-- Project Overview -->
	<section class="mx-auto max-w-7xl">
		<div class="border-accent/20 flex flex-col gap-y-6 rounded-lg border-2 p-4 backdrop-blur">
			<div class="flex items-baseline gap-x-4">
				<Info />
				<h2 class="text-3xl font-bold">About the Project</h2>
			</div>

			<div class="flex flex-col space-y-2 px-10 text-pretty">
				<p class="about text-sm leading-relaxed sm:text-base lg:text-lg">
					Detailed and freely accessible statistics on the <span>punctuality</span> of German rail transport are not available from
					Deutsche Bahn. There is no way to view the performance of specific stations for a user-definable period, as only an annual
					summary report exists.
				</p>

				<p class="about text-sm leading-relaxed sm:text-base lg:text-lg">
					<span>Navigator</span> addresses this lack of <span>transparency</span> by specifically collecting journey data. Since
					<span>March 2025</span>, all relevant information on train services, including
					<span>delays</span>
					and <span>cancellations</span>, has been systematically recorded, starting in the Stuttgart area.
				</p>

				<p class="about text-sm leading-relaxed sm:text-base lg:text-lg">
					The dataset is continuously expanding to gradually cover more stations. The long-term goal is to achieve <span
						>nationwide coverage</span
					>. In this way, Navigator creates <span>data-based transparency</span> that allows users to get a clear picture of the reliability
					of rail transport.
				</p>
			</div>
		</div>
	</section>

	<!-- Project Dimensions -->
	<section class="mx-auto w-full max-w-7xl overflow-hidden">
		<div class="flex items-center justify-between gap-y-2">
			<h2 class="text-2xl font-medium">Project Dimensions</h2>

			<div class="relative">
				<button
					class="bg-muted/70 border-muted-foreground/20 hover:bg-muted-foreground/20 flex cursor-pointer items-center justify-center gap-x-2 rounded-md border px-2 py-1"
					onclick={() => (selectingTimerange = true)}
				>
					<Calendar size={20} />

					<div class="hidden gap-x-1 md:flex">
						<span>{data.timerange.start?.toLocaleString(DateTime.DATE_MED)}</span>
						<span>&nbsp;–&nbsp;</span>
						<span>{data.timerange.end?.toLocaleString(DateTime.DATE_MED)}</span>
					</div>
				</button>

				<TimePickerDialog
					bind:isVisible={selectingTimerange}
					title="Select Custom Time Range"
					multiSelect
					startDate={DateTime.now().minus({ days: 1 })}
					endDate={DateTime.now()}
					onchange={async ({ start, end }) => {
						selectingTimerange = false;
						/*
					if (!start || !end) {
						selectedTimerange = previouslySelectedTimerange;
						return;
					}

					selectedTimerange = timeranges.find((timerangeOption: TimerangeOption) => timerangeOption.id === "CUSTOM")!;
					previouslySelectedTimerange = selectedTimerange;
					*/

						await updateTimerange(start.startOf("day"), end!.endOf("day"));
					}}
					onclose={() => {
						selectingTimerange = false;
					}}
					class="mt-2"
				/>
			</div>
		</div>

		<div class="columns-1 gap-4 sm:columns-2 xl:columns-3">
			{#each metricCards as metricCard}
				{@const title = metricCard.title}
				{@const promise = metricCard.promise()}
				{@const typeTitles = metricCard.typeTitles}

				<MetricCard
					{title}
					{promise}
					onselect={(metrics: MetricSeries[]) => {
						if (!metricCard.chartDialogDetails.isOpenable) return;

						chartDialogOpen = true;
						selectedMetricCard = {
							title: metricCard.title,
							metrics,
							typeTitles: metricCard.typeTitles,
							chartType: metricCard.chartDialogDetails.chartType
						};
					}}
					{typeTitles}
					class="mb-4 w-full"
				/>
			{/each}
		</div>

		<MetricChartDialog
			bind:isVisible={chartDialogOpen}
			title={selectedMetricCard?.title!}
			metrics={selectedMetricCard?.metrics}
			typeTitles={selectedMetricCard?.typeTitles}
			onclose={() => {
				selectedMetricCard = null;
			}}
		/>
	</section>
</main>

<style>
	:global(p.about span) {
		color: var(--color-accent);
		font-weight: 600;
	}
</style>
