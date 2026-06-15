<script lang="ts">
	import type { EventTimeSeriesPoint, JourneyTimeSeriesPoint } from "@lib/api";
	import MetricComparisonList from "@lib/components/statistics/dashboard/MetricComparisonList.svelte";
	import MetricHeatmap from "@lib/components/statistics/dashboard/MetricHeatmap.svelte";
	import MetricKpiGrid from "@lib/components/statistics/dashboard/MetricKpiGrid.svelte";
	import MetricRankingList from "@lib/components/statistics/dashboard/MetricRankingList.svelte";
	import MetricTrendChart, { type TrendMetricDefinition } from "@lib/components/statistics/dashboard/MetricTrendChart.svelte";
	import NetworkHotspotMap from "@lib/components/statistics/dashboard/NetworkHotspotMap.svelte";
	import StatisticsFilterBar from "@lib/components/statistics/dashboard/StatisticsFilterBar.svelte";
	import { rateToPercent } from "@lib/components/statistics/dashboard/statistics-dashboard";
	import type { PageProps } from "./$types";

	let { data }: PageProps = $props();

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

	const journeyTrendMetrics: TrendMetricDefinition[] = [
		{
			key: "journeyCompletionRate",
			label: "Completed",
			color: "var(--color-accent)",
			format: "percent",
			value: (item) => rateToPercent((item as JourneyTimeSeriesPoint).journeyMetrics.journeyCompletionRate)
		},
		{
			key: "destinationPunctuality5Rate",
			label: "Destination <= 5:59",
			color: "var(--color-foreground)",
			format: "percent",
			value: (item) => rateToPercent((item as JourneyTimeSeriesPoint).journeyMetrics.destinationPunctuality5Rate)
		},
		{
			key: "fullCancellationRate",
			label: "Fully cancelled",
			color: "#dc2626",
			format: "percent",
			value: (item) => rateToPercent((item as JourneyTimeSeriesPoint).journeyMetrics.fullCancellationRate)
		}
	];
</script>

<svelte:head>
	<title>Network Statistics - Navigator</title>
</svelte:head>

<main class="container mx-auto flex flex-col gap-y-8 p-4 sm:py-8">
	<section class="relative">
		<div class="bg-accent absolute top-1 bottom-1 w-0.5 sm:-left-4 sm:w-1"></div>

		<div class="flex flex-col gap-y-2 pl-4 sm:pl-8">
			<h1 class="text-3xl font-bold text-balance sm:text-4xl">Network statistics</h1>
			<p class="text-foreground/60 text-sm font-semibold">{data.rangeLabel}</p>
		</div>
	</section>

	<StatisticsFilterBar filters={data.filters} />

	<MetricKpiGrid eventPromise={data.metrics.eventKpis} journeyPromise={data.metrics.journeyKpis} />

	<section class="grid gap-4 xl:grid-cols-[minmax(0,1.45fr)_minmax(22rem,0.55fr)]">
		<NetworkHotspotMap promise={data.mapHotspots} />
		<MetricComparisonList
			title="Transport families"
			description="Event and journey quality by transport type."
			promise={data.metrics.transportTypeComparison}
			mode="transportComparison"
		/>
	</section>

	<section class="grid gap-4 xl:grid-cols-2">
		<MetricTrendChart
			title="Stop-event reliability"
			description="Reliability, broader punctuality and cancellations over time."
			promise={data.metrics.eventTimeSeries}
			metrics={eventTrendMetrics}
			yDomain={[0, 100]}
		/>

		<MetricTrendChart
			title="Journey outcomes"
			description="Completed journeys and destination quality over time."
			promise={data.metrics.journeyTimeSeries}
			metrics={journeyTrendMetrics}
			yDomain={[0, 100]}
		/>
	</section>

	<section class="grid gap-4 xl:grid-cols-[minmax(0,1fr)_minmax(22rem,0.8fr)]">
		<MetricHeatmap
			title="Weekday and hour"
			description="Stop-event reliability by local weekday and hour."
			promise={data.metrics.weekdayHourHeatmap}
		/>
		<MetricRankingList
			title="Station delay pressure"
			description="Stations ranked by accumulated positive delay minutes."
			promise={data.metrics.stationRanking}
			mode="networkStations"
		/>
	</section>

	<section class="grid gap-4 xl:grid-cols-2">
		<MetricRankingList
			title="Line delay pressure"
			description="Lines ranked by accumulated destination delay minutes."
			promise={data.metrics.lineRanking}
			mode="networkLines"
		/>
	</section>
</main>
