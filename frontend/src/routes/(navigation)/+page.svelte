<script lang="ts">
	import Info from "@lucide/svelte/icons/info";
	import type { Component } from "svelte";
	import type { PageProps } from "./$types";
	import DatabaseSizeKpi from "@lib/components/statistics/projectdimensions/DatabaseSizeKpi.svelte";
	import RecordedRisIds from "@lib/components/statistics/projectdimensions/RecordedRisIds.svelte";
	import RecordedJourneys from "@lib/components/statistics/projectdimensions/RecordedJourneys.svelte";
	import * as Accordion from "@lib/components/ui/accordion";

	let { data }: PageProps = $props();
	let faqValues: string[] = $state([]);

	type MetricCardData = {
		metricComponent: Component<any>;
		promise: Promise<unknown>;
	};
	let metricCards: MetricCardData[] = $derived([
		{
			metricComponent: DatabaseSizeKpi,
			promise: data.databaseSize
		},
		{
			metricComponent: RecordedRisIds,
			promise: data.risIdDistribution
		},
		{
			metricComponent: RecordedJourneys,
			promise: data.recordedJourneys
		}
	]);
</script>

<svelte:head>
	<title>Navigator</title>
</svelte:head>

<main class="container mx-auto flex flex-col gap-y-8 p-4 sm:py-8">
	<!-- Hero Section -->
	<section>
		<div class="relative mb-4">
			<div class="bg-accent absolute top-0 bottom-0 w-0.5 sm:-left-4 sm:w-1"></div>
			<h1 class="pl-4 text-3xl font-bold text-balance sm:mb-6 sm:pl-8 sm:text-4xl md:text-6xl">
				Visualizing Public Transport Performance
			</h1>
		</div>

		<p class="text-foreground/60 max-w-3xl pl-4 text-base sm:pl-8 sm:text-lg lg:text-xl">
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
					Detailed and freely accessible statistics on the
					<span class="text-accent font-semibold">punctuality</span>
					of German rail transport are not available from Deutsche Bahn. There is no way to view the performance of specific stations
					for a user-definable period, as only an annual summary report exists.
				</p>

				<p class="about text-sm leading-relaxed sm:text-base lg:text-lg">
					<span class="text-accent font-semibold">Navigator</span>
					addresses this lack of
					<span class="text-accent font-semibold">transparency</span>
					by specifically collecting journey data. Since
					<span class="text-accent font-semibold">March 2025</span>, all relevant information on train services, including
					<span class="text-accent font-semibold">delays</span>
					and <span class="text-accent font-semibold">cancellations</span>, has been systematically recorded, starting in the Stuttgart
					area.
				</p>

				<p class="about text-sm leading-relaxed sm:text-base lg:text-lg">
					The dataset is continuously expanding to gradually cover more stations. The long-term goal is to achieve
					<span class="text-accent font-semibold">nationwide coverage</span>. In this way, Navigator creates
					<span class="text-accent font-semibold">data-based transparency</span>
					that allows users to get a clear picture of the reliability of rail transport.
				</p>
			</div>
		</div>
	</section>

	<!-- Project Dimensions -->
	<section class="space-y-4">
		<h2 class="text-2xl font-medium">Project Dimensions</h2>

		<div class="grid grid-cols-1 items-start gap-4 sm:grid-cols-2 lg:grid-cols-3">
			{#each metricCards as metricCard}
				{@const MetricComponent = metricCard.metricComponent}
				<MetricComponent promise={metricCard.promise} class="mb-4 break-inside-avoid" />
			{/each}
		</div>
	</section>

	<!-- FAQ -->
	<section class="space-y-4">
		<div class="flex flex-col gap-y-1">
			<h2 class="text-2xl font-medium">FAQ</h2>
			<p class="text-foreground/60 max-w-3xl text-sm sm:text-base">
				Answers to the most important questions about Navigator's data collection.
			</p>
		</div>

		<Accordion.Root type="multiple" bind:value={faqValues}>
			<Accordion.Item value="data-source">
				<Accordion.Trigger>Where do the data come from?</Accordion.Trigger>
				<Accordion.Content>
					<p class="leading-relaxed text-pretty">
						Navigator pulls real-time transit data directly from the Deutsche Bahn API Marketplace. The system continuously
						monitors station boards to discover active services and then performs deep-dive queries to capture full journey
						details. This allows tracking everything from precise stop times and platform changes to real-time delays and
						cancellations across the entire network.
					</p>
				</Accordion.Content>
			</Accordion.Item>

			<Accordion.Item value="ris-id-matching">
				<Accordion.Trigger>How are journeys identified?</Accordion.Trigger>
				<Accordion.Content>
					<p class="leading-relaxed text-pretty">
						Official Journey IDs change every day because they include a date prefix. Navigator strips this prefix to isolate a
						stable RIS ID. By focusing on this permanent identifier, recurring services can be tracked across multiple days.
						This makes it possible to analyze the performance of specific lines over time.
					</p>
				</Accordion.Content>
			</Accordion.Item>
		</Accordion.Root>
	</section>
</main>
