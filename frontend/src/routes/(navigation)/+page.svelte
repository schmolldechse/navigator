<script lang="ts">
	import { goto } from "$app/navigation";
	import type { MeasuredTimerangeStatistic } from "$lib/api/types.gen.js";
	import PillSelector from "$lib/components/interactable/PillSelector.svelte";
	import TimePickerDialog from "$lib/components/interactable/TimePickerDialog.svelte";
	import StatisticCard from "$lib/components/statistics/StatisticCard.svelte";
	import StatisticChartDialog from "$lib/components/statistics/StatisticChartDialog.svelte";
	import type { TimerangeOption } from "$lib/types/timerange.js";
	import Info from "@lucide/svelte/icons/info";
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
	let selectedTimerange: TimerangeOption = $state(timeranges[0]);
	let previouslySelectedTimerange: TimerangeOption = $state(timeranges[0]);

	let pickingCustomRange: boolean = $state(false);

	const updateTimerange = async (start: DateTime, end: DateTime) => {
		const url = new URL(window.location.href);
		url.searchParams.set("start", start.toISO()!);
		url.searchParams.set("end", end.toISO()!);

		await goto(url, { replaceState: true, keepFocus: true, noScroll: true });
		// invalidate("project:dimensions");
	};

	let statistics = [{ id: "DATABASE_ESTIMATION", label: "Total Database Size", promise: () => data.dimensions.databaseSize }];

	let selectedStatistic: { statistic: MeasuredTimerangeStatistic; label: string } | null = $state(null);
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
		<div class="mb-6 flex flex-col gap-y-2 sm:flex-row sm:items-center sm:justify-between sm:gap-0">
			<h2 class="text-2xl font-medium">Project Dimensions</h2>

			<div class="w-full sm:w-auto">
				<PillSelector
					selected={selectedTimerange.id}
					items={timeranges.map((timerangeOption: TimerangeOption) => ({
						id: timerangeOption.id,
						label: timerangeOption.label
					}))}
					onselect={async (id: string) => {
						const timerange = timeranges.find((timerangeOption: TimerangeOption) => timerangeOption.id === id);
						if (!timerange) return;

						if (timerange.isCustom) {
							previouslySelectedTimerange = selectedTimerange;
							selectedTimerange = timerange;
							pickingCustomRange = true;
							return;
						}

						selectedTimerange = timerange;
						previouslySelectedTimerange = timerange;

						if (!selectedTimerange.value) return;
						await updateTimerange(selectedTimerange.value.start, selectedTimerange.value.end);
					}}
				/>
			</div>

			<TimePickerDialog
				bind:isVisible={pickingCustomRange}
				title="Select Custom Time Range"
				multiSelect
				startDate={DateTime.now().minus({ days: 1 })}
				endDate={DateTime.now()}
				onchange={async ({ start, end }) => {
					pickingCustomRange = false;
					if (!start || !end) {
						selectedTimerange = previouslySelectedTimerange;
						return;
					}

					selectedTimerange = timeranges.find((timerangeOption: TimerangeOption) => timerangeOption.id === "CUSTOM")!;
					previouslySelectedTimerange = selectedTimerange;

					await updateTimerange(start.startOf("day"), end.endOf("day"));
				}}
				onclose={() => {
					pickingCustomRange = false;
					selectedTimerange = previouslySelectedTimerange;
				}}
			/>
		</div>

		<div class="grid grid-cols-1 gap-4 sm:grid-cols-2 sm:gap-6 lg:grid-cols-3">
			{#each statistics as localStatistic}
				<StatisticCard
					title={localStatistic.label}
					statisticPromise={localStatistic.promise()}
					onselect={(statistic: MeasuredTimerangeStatistic) => (selectedStatistic = { statistic, label: localStatistic.label })}
				/>
			{/each}
		</div>

		{#if selectedStatistic}
			<StatisticChartDialog
				isVisible={true}
				title={selectedStatistic.label}
				measuredStatistic={selectedStatistic.statistic}
				onclose={() => (selectedStatistic = null)}
			/>
		{/if}
	</section>
</main>

<style>
	:global(p.about span) {
		color: var(--color-accent);
		font-weight: 600;
	}
</style>
