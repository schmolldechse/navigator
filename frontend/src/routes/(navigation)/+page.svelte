<script lang="ts">
	import { goto } from "$app/navigation";
	import Info from "@lucide/svelte/icons/info";
	import SlidersHorizontal from "@lucide/svelte/icons/sliders-horizontal";
	import { DateTime } from "luxon";
	import type { Component } from "svelte";
	import type { PageProps } from "./$types";
	import Button from "@lib/components/ui/Button.svelte";
	import Separator from "@lib/components/ui/Separator.svelte";
	import TimePickerDialog from "@lib/components/ui/timepicker/TimePickerDialog.svelte";
	import DatabaseSize from "@lib/components/statistics/projectdimensions/DatabaseSize.svelte";
	import TransportTypeDistribution from "@lib/components/statistics/projectdimensions/TransportTypeDistribution.svelte";
	// import RecordedRisIds from "@lib/components/projectdimensions/risids/RecordedRisIds.svelte";
	import RecordedJourneys from "@lib/components/statistics/projectdimensions/RecordedJourneys.svelte";
	import type { TimePickerValue, TimePickerRange } from "@lib/components/ui/timepicker/TimePicker.svelte";

	let { data }: PageProps = $props();

	// settings
	let isSettingsOpen: boolean = $state(false);
	let selectedTimerange: { start: DateTime; end: DateTime } = $state({
		start: data.timerange.start,
		end: data.timerange.end
	});

	type MetricCardData = {
		metricComponent: Component<any>;
		props: Record<string, unknown>;
		scale: "small" | "medium" | "large";
	};
	let metricCards: MetricCardData[] = $derived([
		{
			metricComponent: DatabaseSize,
			props: {
				promise: data.dimensions.databaseSize
			},
			scale: "medium"
		},
		{
			metricComponent: TransportTypeDistribution,
			props: {
				promise: data.dimensions.transportTypeDistribution
			},
			scale: "small"
		},
		/*
		{
			metricComponent: RecordedRisIds,
			props: {
				promise: data.dimensions.risIdDistribution
			},
			scale: "large"
		},
		*/
		{
			metricComponent: RecordedJourneys,
			props: {
				promise: data.dimensions.recordedJourneys
			},
			scale: "large"
		}
	]);
</script>

<svelte:head>
	<title>Navigator</title>
</svelte:head>

<main class="container mx-auto flex flex-col space-y-8 p-4 sm:py-8">
	<!-- Hero Section -->
	<section>
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
	<section>
		<div class="border-accent/40 flex flex-col gap-y-6 rounded-xl border-2 p-4 backdrop-blur">
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

	<Separator orientation="horizontal" />

	<!-- Project Dimensions -->
	<section class="space-y-4">
		<div class="relative flex flex-row items-center justify-between">
			<h2 class="text-2xl font-medium">Project Dimensions</h2>

			<Button
				mode="secondary"
				onclick={(event: MouseEvent) => {
					event.stopPropagation();
					isSettingsOpen = !isSettingsOpen;
				}}
				class="flex items-center gap-x-2"
			>
				<SlidersHorizontal size={18} />

				<div class="hidden items-baseline gap-x-1 md:flex">
					<span class="text-sm tracking-tight">{selectedTimerange.start?.toLocaleString(DateTime.DATE_MED)}</span>
					<span>&nbsp;–&nbsp;</span>
					<span class="text-sm tracking-tight">{selectedTimerange.end?.toLocaleString(DateTime.DATE_MED)}</span>
				</div>
			</Button>

			<TimePickerDialog
				bind:isVisible={isSettingsOpen}
				isModal={false}
				isRange
				value={selectedTimerange}
				onchange={async (value: TimePickerValue) => {
					const { start, end } = value as TimePickerRange;

					selectedTimerange = { start, end: end! };

					const url = new URL(window.location.href);
					url.searchParams.set("start", start.startOf("day").toISO() as string);
					url.searchParams.set("end", end!.endOf("day").toISO() as string);

					await goto(url, { replaceState: true, keepFocus: true, noScroll: true });
				}}
				class="absolute top-full z-50 mt-2 justify-self-end"
			/>
		</div>

		<div class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
			{#each metricCards as metricCard}
				{@const MetricComponent = metricCard.metricComponent}
				{@const cardScale = metricCard.scale}
				<MetricComponent
					{...metricCard.props}
					class={[cardScale === "medium" && "sm:col-span-2", cardScale === "large" && "sm:col-span-2 lg:col-span-3"]}
				/>
			{/each}
		</div>
	</section>
</main>

<style>
	@reference "../app.css";

	:global(p.about span) {
		@apply text-accent font-semibold;
	}
</style>
