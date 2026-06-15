<script lang="ts">
	import type { EventTimeSeriesPoint } from "@lib/api";
	import LineHourMatrix from "@lib/components/statistics/dashboard/LineHourMatrix.svelte";
	import MetricComparisonList from "@lib/components/statistics/dashboard/MetricComparisonList.svelte";
	import MetricHeatmap from "@lib/components/statistics/dashboard/MetricHeatmap.svelte";
	import MetricKpiGrid from "@lib/components/statistics/dashboard/MetricKpiGrid.svelte";
	import MetricRankingList from "@lib/components/statistics/dashboard/MetricRankingList.svelte";
	import MetricTrendChart, { type TrendMetricDefinition } from "@lib/components/statistics/dashboard/MetricTrendChart.svelte";
	import StationEventDetailsTable from "@lib/components/statistics/dashboard/StationEventDetailsTable.svelte";
	import StatisticsFilterBar from "@lib/components/statistics/dashboard/StatisticsFilterBar.svelte";
	import {
		createSearchParamsFromStatisticsFilters,
		rateToPercent,
		transportTypeShortLabel
	} from "@lib/components/statistics/dashboard/statistics-dashboard";
	import Button from "@lib/components/ui/Button.svelte";
	import ArrowLeft from "@lucide/svelte/icons/arrow-left";
	import RadioTower from "@lucide/svelte/icons/radio-tower";
	import type { PageProps } from "./$types";

	let { data }: PageProps = $props();
	const networkHref = $derived(`/statistics?${createSearchParamsFromStatisticsFilters(data.filters).toString()}`);

	const eventTrendMetrics: TrendMetricDefinition[] = [
		{
			key: "customerReliability5Rate",
			label: "Reliable <= 5:59",
			color: "var(--color-accent)",
			format: "percent",
			value: (item) => rateToPercent((item as EventTimeSeriesPoint).eventMetrics.customerReliability5Rate)
		},
		{
			key: "customerReliability15Rate",
			label: "Reliable <= 14:59",
			color: "var(--color-foreground)",
			format: "percent",
			value: (item) => rateToPercent((item as EventTimeSeriesPoint).eventMetrics.customerReliability15Rate)
		},
		{
			key: "cancellationRate",
			label: "Cancelled",
			color: "#dc2626",
			format: "percent",
			value: (item) => rateToPercent((item as EventTimeSeriesPoint).eventMetrics.cancellationRate)
		}
	];
</script>

<svelte:head>
	<title>{data.station.name} Statistics - Navigator</title>
</svelte:head>

<main class="container mx-auto flex flex-col gap-y-8 p-4 sm:py-8">
	<section class="relative">
		<div class="bg-accent absolute top-1 bottom-1 w-0.5 sm:-left-4 sm:w-1"></div>

		<div class="flex flex-col gap-4 pl-4 sm:pl-8 lg:flex-row lg:items-start lg:justify-between">
			<div class="min-w-0">
				<p class="text-foreground/55 text-sm font-semibold">EVA {data.evaNumber}</p>
				<h1 class="text-3xl font-bold text-balance sm:text-4xl">{data.station.name}</h1>
				<p class="text-foreground/60 mt-2 text-sm font-semibold">{data.rangeLabel}</p>

				<div class="mt-3 flex flex-wrap gap-2">
					{#each data.station.transports as transportType (transportType)}
						<span class="border-border bg-secondary rounded-lg border px-2 py-1 text-xs font-bold">
							{transportTypeShortLabel(transportType)}
						</span>
					{/each}

					{#await data.gatheringPromise}
						<span class="border-border bg-secondary rounded-lg border px-2 py-1 text-xs font-bold">Loading status</span>
					{:then gathering}
						<span
							class={[
								"inline-flex items-center gap-1 rounded-lg border px-2 py-1 text-xs font-bold",
								gathering.queryingEnabled
									? "border-emerald-500/30 bg-emerald-500/10 text-emerald-600"
									: "border-border bg-secondary text-foreground/60"
							]}
						>
							<RadioTower size={13} />
							{gathering.queryingEnabled ? "Gathering active" : "Gathering paused"}
						</span>
					{/await}
				</div>
			</div>

			<Button href={networkHref} mode="secondary" class="inline-flex items-center gap-2">
				<ArrowLeft size={16} />
				Network
			</Button>
		</div>
	</section>

	<StatisticsFilterBar filters={data.filters} />

	<MetricKpiGrid eventPromise={data.metrics.eventKpis} benchmarkPromise={data.metrics.benchmark} />

	<section class="grid gap-4 xl:grid-cols-[minmax(0,1.25fr)_minmax(22rem,0.75fr)]">
		<MetricTrendChart
			title="Station stop-event reliability"
			description="Reliability, broader punctuality and cancellations over time."
			promise={data.metrics.timeSeries}
			metrics={eventTrendMetrics}
			yDomain={[0, 100]}
		/>

		<MetricComparisonList
			title="Arrival and departure"
			description="Stop-event quality split by schedule type."
			promise={data.metrics.arrivalDepartureComparison}
			mode="arrivalDeparture"
		/>
	</section>

	<section class="grid gap-4 xl:grid-cols-[minmax(0,1fr)_minmax(22rem,0.8fr)]">
		<MetricHeatmap
			title="Weekday and hour"
			description="Station reliability by local weekday and hour."
			promise={data.metrics.weekdayHourHeatmap}
		/>
		<MetricComparisonList
			title="Transport mix"
			description="Station activity share and reliability by transport type."
			promise={data.metrics.transportTypeMix}
			mode="transportMix"
		/>
	</section>

	<section class="grid gap-4 xl:grid-cols-2">
		<MetricRankingList
			title="Line delay pressure"
			description="Lines at this station ranked by accumulated delay minutes."
			promise={data.metrics.lineRanking}
			mode="stationLines"
		/>
		<MetricRankingList
			title="Directions"
			description="Connected origins and destinations ranked by delay pressure."
			promise={data.metrics.directions}
			mode="stationDirections"
		/>
	</section>

	<LineHourMatrix promise={data.metrics.lineHourMatrix} />

	<StationEventDetailsTable promise={data.metrics.eventDetails} />
</main>
