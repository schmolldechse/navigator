<script lang="ts" module>
	interface MetricCardData {
		metricComponent: Component<any>;
		props: Record<string, any>;
		scale: CardScale;
	}

	export type { MetricCardData };
</script>

<script lang="ts">
	import { goto } from "$app/navigation";
	import TimePickerDialog from "@lib/components/interactable/TimePickerDialog.svelte";
	import Info from "@lucide/svelte/icons/info";
	import Calendar from "@lucide/svelte/icons/calendar";
	import { DateTime } from "luxon";
	import type { Component } from "svelte";
	import DatabaseSizeMetricCard from "@lib/components/statistics/projectdimensions/DatabaseSizeMetricCard.svelte";
	import RecordedRisIdsMetricCard from "@lib/components/statistics/projectdimensions/RecordedRisIdsMetricCard.svelte";
	import { CardScale } from "@lib/util/card.js";

	let { data } = $props();

	let selectingTimerange: boolean = $state(false);

	const updateTimerange = async (start: DateTime, end: DateTime) => {
		const url = new URL(window.location.href);
		url.searchParams.set("start", start.toISO()!);
		url.searchParams.set("end", end.toISO()!);

		await goto(url, { replaceState: true, keepFocus: true, noScroll: true });
		// invalidate("project:dimensions");
	};

	let metricCards: MetricCardData[] = $derived([
		{
			metricComponent: DatabaseSizeMetricCard,
			props: {
				promise: data.dimensions.databaseSize
			},
			scale: CardScale.MEDIUM
		},
		{
			metricComponent: RecordedRisIdsMetricCard,
			props: {
				promise: data.dimensions.totalRisIds
			},
			scale: CardScale.MEDIUM
		}
	]);

	const getSpanLength = (scale: MetricCardData): string => {
		switch (scale.scale) {
			case CardScale.SMALL:
				return "col-span-1";
			case CardScale.MEDIUM:
				return "col-span-2";
			case CardScale.LARGE:
				return "col-span-3";
			default:
				return "col-span-1";
		}
	};
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
	<section class="mx-auto w-full max-w-7xl space-y-2">
		<div class="flex items-center justify-between gap-y-2">
			<h2 class="text-2xl font-medium">Project Dimensions</h2>

			<div class="relative">
				<button
					class="bg-muted/70 border-muted-foreground/20 hover:bg-muted-foreground/20 flex cursor-pointer items-center justify-center gap-x-2 rounded-md border px-4 py-1"
					onclick={(event: MouseEvent) => {
						event.stopPropagation();
						selectingTimerange = !selectingTimerange;
					}}
				>
					<Calendar size={20} />

					<div class="hidden items-baseline gap-x-1 md:flex">
						<span class="text-sm tracking-tight">{data.timerange.start?.toLocaleString(DateTime.DATE_MED)}</span>
						<span>&nbsp;–&nbsp;</span>
						<span class="text-sm tracking-tight">{data.timerange.end?.toLocaleString(DateTime.DATE_MED)}</span>
					</div>
				</button>

				<TimePickerDialog
					bind:isVisible={selectingTimerange}
					multiSelect
					dates={{ start: data.timerange.start, end: data.timerange.end }}
					onchange={async ({ start, end }) => await updateTimerange(start.startOf("day"), end!.endOf("day"))}
					class="mt-2"
				/>
			</div>
		</div>

		<div class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
			{#each metricCards as metricCard}
				{@const MetricComponent = metricCard.metricComponent}
				<MetricComponent {...metricCard.props} class="col-span-3" />
			{/each}
		</div>
	</section>
</main>

<style>
	:global(p.about span) {
		color: var(--color-accent);
		font-weight: 600;
	}
</style>
