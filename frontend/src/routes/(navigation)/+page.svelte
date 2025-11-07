<script lang="ts">
	import GlobalStatisticCard from "$lib/components/statistics/GlobalStatisticCard.svelte";
	import StaticsticDialog from "$lib/components/statistics/StaticsticDialog.svelte";
	import type { MeasuredTimeframeStatisticDTO } from "$lib/models/MeasuredTimeframeStatisticDTO";
	import type { DateTime } from "luxon";
	import { getDatabaseSize } from "./globalstatistics.remote";

	let dialogRef: HTMLDialogElement;

	let selectedStatistic: { title?: string; measuredStatistic: MeasuredTimeframeStatisticDTO | null } = $state({
		measuredStatistic: null
	});

	let timerange: { start: DateTime; end: DateTime } | undefined = $state(undefined);
</script>

<svelte:head>
	<title>Navigator</title>
</svelte:head>

<main class="container mx-auto px-4 py-12">
	<!-- Hero Section -->
	<section class="mx-auto mb-12 max-w-6xl sm:mb-16 lg:mb-20">
		<div class="relative">
			<div class="absolute top-0 bottom-0 -left-2 w-0.5 bg-accent sm:-left-4 sm:w-1"></div>
			<h1 class="mb-4 pl-4 text-3xl font-bold text-balance sm:mb-6 sm:pl-8 sm:text-4xl md:text-6xl">
				Visualizing Public Transport Performance
			</h1>
		</div>

		<p class="mb-4 max-w-3xl pl-4 text-base text-muted-foreground sm:pl-8 sm:text-lg lg:text-xl">
			Clear insights. <span class="font-semibold text-accent">Transparency</span> for Germany's Public Transport.
		</p>
	</section>

	<!-- Project Overview -->
	<section class="mx-auto mb-12 max-w-6xl sm:mb-16 lg:mb-20">
		<div class="rounded-lg border-2 border-accent/20 backdrop-blur">
			<div class="p-4 sm:p-6">
				<h2 class="flex items-center gap-2 text-left text-2xl font-bold sm:gap-3 sm:text-3xl">
					<span class="text-accent">▸</span>About the Project
				</h2>
			</div>

			<div class="space-y-4 p-4 text-pretty sm:space-y-6 sm:p-6">
				<p class="about text-sm leading-relaxed sm:text-base lg:text-lg">
					Detailed and freely accessible statistics on the <span>punctuality</span> of German rail transport are not available
					from Deutsche Bahn. There is no way to view the performance of specific stations for a user-definable period,
					as only an annual summary report exists.
				</p>

				<p class="about text-sm leading-relaxed sm:text-base lg:text-lg">
					<span>Navigator</span> addresses this lack of <span>transparency</span> by specifically collecting journey
					data. Since <span>March 2025</span>, all relevant information on train services, including
					<span>delays</span>
					and <span>cancellations</span>, has been systematically recorded, starting in the Stuttgart area.
				</p>

				<p class="about text-sm leading-relaxed sm:text-base lg:text-lg">
					The dataset is continuously expanding to gradually cover more stations. The long-term goal is to achieve <span
						>nationwide coverage</span
					>. In this way, Navigator creates <span>data-based transparency</span> that allows users to get a clear picture
					of the reliability of rail transport.
				</p>
			</div>
		</div>
	</section>

	<!-- Global Statistics Cards -->
	<div class="mx-auto max-w-6xl">
		<div class="mb-6 border-b-2 border-accent/30 pb-4 sm:mb-8 sm:pb-6 lg:mb-10">
			<h2 class="flex items-center gap-2 text-2xl font-bold sm:gap-3 sm:text-3xl lg:text-4xl">
				<span class="text-accent">▸</span>Current Project Dimensions
			</h2>
		</div>

		<div class="grid grid-cols-1 gap-4 sm:grid-cols-2 sm:gap-6 lg:grid-cols-3">
			<GlobalStatisticCard
				title="Total Dataset size"
				fetch={async (params) => getDatabaseSize({ start: params.start.toJSDate(), end: params.end.toJSDate() })}
				onclick={() => dialogRef.showModal()}
				onmeasured={(data) => (selectedStatistic = { title: "Total Dataset size", measuredStatistic: data })}
				bind:timerange
			/>
		</div>

		<StaticsticDialog
			bind:dialog={dialogRef}
			title={selectedStatistic.title!}
			measuredStatistic={selectedStatistic.measuredStatistic}
			onclose={() => dialogRef.close()}
			ontimerangechange={(newTimerange) => (timerange = newTimerange)}
		/>
	</div>
</main>

<style>
	:global(p.about span) {
		color: var(--color-accent);
		font-weight: 600;
	}
</style>
