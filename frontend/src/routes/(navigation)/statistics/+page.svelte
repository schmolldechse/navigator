<script lang="ts">
	import type { EventTimeSeriesPoint, JourneyTimeSeriesPoint } from "@lib/api";
	import NetworkDelayDistributionChart from "@lib/components/statistics/network/NetworkDelayDistributionChart.svelte";
	import NetworkFilterPanel from "@lib/components/statistics/network/NetworkFilterPanel.svelte";
	import NetworkHeatmap from "@lib/components/statistics/network/NetworkHeatmap.svelte";
	import NetworkHotspotMap from "@lib/components/statistics/network/NetworkHotspotMap.svelte";
	import NetworkKpiGrid from "@lib/components/statistics/network/NetworkKpiGrid.svelte";
	import NetworkRankingList from "@lib/components/statistics/network/NetworkRankingList.svelte";
	import NetworkTransportComparisonChart from "@lib/components/statistics/network/NetworkTransportComparisonChart.svelte";
	import NetworkTrendChart, { type TrendMetricDefinition } from "@lib/components/statistics/network/NetworkTrendChart.svelte";
	import { getStatisticsDashboardContext } from "@lib/components/statistics/shared/statistics-context.svelte";
	import MetricBucketControl from "@lib/components/statistics/shared/MetricBucketControl.svelte";
	import {
		createNetworkEventDelayDistributionRequest,
		createNetworkEventSummaryRequest,
		createNetworkEventTimeSeriesRequest,
		createNetworkJourneySummaryRequest,
		createNetworkJourneyTimeSeriesRequest,
		createNetworkLineRankingRequest,
		createNetworkMapHotspotsRequest,
		createNetworkStationRankingRequest,
		createNetworkTransportTypeComparisonRequest,
		createNetworkWeekdayHourHeatmapRequest,
		createPreviousGlobalScope,
		createRangeLabel,
		getStatisticsBucketOptions,
		rateToPercent,
		transportTypeShortLabel
	} from "@lib/components/statistics/shared/statistics-dashboard";
	import { loadNetworkMapHotspots, loadNetworkMetric } from "@lib/remote/statistics.remote";

	const statistics = getStatisticsDashboardContext();
	const rangeLabel = $derived(createRangeLabel(statistics.global));
	const previousGlobal = $derived(createPreviousGlobalScope(statistics.global));
	const bucketOptions = $derived(getStatisticsBucketOptions(statistics.global));
	const activeFilterChips = $derived.by(() => {
		const chips = [rangeLabel];
		if (statistics.global.transportTypes.length > 0) {
			chips.push(...statistics.global.transportTypes.map((transportType) => transportTypeShortLabel(transportType)));
		} else {
			chips.push("All rail services");
		}
		if (statistics.event.scheduleType) chips.push(statistics.event.scheduleType === "ARRIVAL" ? "Arrivals" : "Departures");
		if (!statistics.global.includeReplacement) chips.push("No replacement services");

		return chips;
	});

	const eventSummary = $derived(
		loadNetworkMetric({ request: createNetworkEventSummaryRequest(statistics.global, statistics.event) })
	);
	const journeySummary = $derived(loadNetworkMetric({ request: createNetworkJourneySummaryRequest(statistics.global) }));
	const previousEventSummary = $derived(
		loadNetworkMetric({ request: createNetworkEventSummaryRequest(previousGlobal, statistics.event) })
	);
	const previousJourneySummary = $derived(loadNetworkMetric({ request: createNetworkJourneySummaryRequest(previousGlobal) }));
	const eventTimeSeries = $derived(
		loadNetworkMetric({
			request: createNetworkEventTimeSeriesRequest(statistics.global, statistics.event, statistics.network.eventTimeSeries)
		})
	);
	const journeyTimeSeries = $derived(
		loadNetworkMetric({
			request: createNetworkJourneyTimeSeriesRequest(statistics.global, statistics.network.journeyTimeSeries)
		})
	);
	const weekdayHourHeatmap = $derived(
		loadNetworkMetric({ request: createNetworkWeekdayHourHeatmapRequest(statistics.global, statistics.event) })
	);
	const transportTypeComparison = $derived(
		loadNetworkMetric({ request: createNetworkTransportTypeComparisonRequest(statistics.global, statistics.event) })
	);
	const stationRanking = $derived(
		loadNetworkMetric({
			request: createNetworkStationRankingRequest(statistics.global, statistics.event, statistics.network.stationRanking)
		})
	);
	const lineRanking = $derived(
		loadNetworkMetric({ request: createNetworkLineRankingRequest(statistics.global, statistics.network.lineRanking) })
	);
	const mapHotspots = $derived(
		loadNetworkMapHotspots({
			request: createNetworkMapHotspotsRequest(statistics.global, statistics.event, statistics.network.mapHotspots)
		})
	);
	const delayDistribution = $derived(
		loadNetworkMetric({ request: createNetworkEventDelayDistributionRequest(statistics.global, statistics.event) })
	);

	const eventTrendMetrics: TrendMetricDefinition[] = [
		{
			key: "customerReliability5Rate",
			label: "Reliable < 6 min",
			color: "var(--color-accent)",
			format: "percent",
			value: (item) => rateToPercent((item as EventTimeSeriesPoint).eventMetrics.customerReliability5Rate)
		},
		{
			key: "customerReliability15Rate",
			label: "Reliable < 15 min",
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
			label: "Destination < 6 min",
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
	<section class="border-border bg-secondary/10 rounded-xl border-2 p-5">
		<div class="flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between">
			<div class="min-w-0">
				<p class="text-accent text-xs font-bold tracking-wider uppercase">Network overview</p>
				<h1 class="text-foreground mt-1 text-3xl font-bold text-balance sm:text-4xl">Network statistics</h1>
				<p class="text-foreground/60 mt-2 max-w-3xl text-sm leading-relaxed font-semibold">
					Operational quality across German rail services, grouped by stops, journeys, time, transport families and high-impact
					hotspots.
				</p>
			</div>

			<div class="flex flex-wrap gap-2 lg:justify-end">
				{#each activeFilterChips as chip (chip)}
					<span class="border-border bg-background rounded-lg border px-2.5 py-1 text-xs font-semibold">
						{chip}
					</span>
				{/each}
			</div>
		</div>
	</section>

	<NetworkFilterPanel />

	<NetworkKpiGrid
		eventPromise={eventSummary}
		journeyPromise={journeySummary}
		previousEventPromise={previousEventSummary}
		previousJourneyPromise={previousJourneySummary}
	/>

	<section class="grid gap-4 xl:grid-cols-[minmax(0,1.45fr)_minmax(22rem,0.55fr)]">
		<NetworkHotspotMap promise={mapHotspots} />
		<NetworkTransportComparisonChart
			promise={transportTypeComparison}
			metric={statistics.network.transportTypeComparison.metric}
			onmetricchange={statistics.setNetworkComparisonMetric}
		/>
	</section>

	<section class="grid gap-4 xl:grid-cols-2">
		<NetworkTrendChart
			title="Stop-event reliability"
			description="Reliability, broader punctuality and cancellations over time."
			promise={eventTimeSeries}
			metrics={eventTrendMetrics}
			yDomain={[0, 100]}
		>
			{#snippet actions()}
				<MetricBucketControl
					value={statistics.network.eventTimeSeries.bucket}
					options={bucketOptions}
					onchange={statistics.setNetworkEventBucket}
				/>
			{/snippet}
		</NetworkTrendChart>

		<NetworkTrendChart
			title="Journey outcomes"
			description="Completed journeys and destination quality over time."
			promise={journeyTimeSeries}
			metrics={journeyTrendMetrics}
			yDomain={[0, 100]}
		>
			{#snippet actions()}
				<MetricBucketControl
					value={statistics.network.journeyTimeSeries.bucket}
					options={bucketOptions}
					onchange={statistics.setNetworkJourneyBucket}
				/>
			{/snippet}
		</NetworkTrendChart>
	</section>

	<section class="grid gap-4 xl:grid-cols-[minmax(0,1fr)_minmax(22rem,0.8fr)]">
		<NetworkHeatmap
			promise={weekdayHourHeatmap}
			metric={statistics.network.weekdayHourHeatmap.metric}
			onmetricchange={statistics.setNetworkHeatmapMetric}
		/>
		<NetworkRankingList
			title="Station delay pressure"
			description="Stations ranked by accumulated positive delay minutes."
			promise={stationRanking}
			mode="stations"
			onpagechange={statistics.setNetworkStationRankingOffset}
		/>
	</section>

	<section class="grid gap-4 xl:grid-cols-2">
		<NetworkRankingList
			title="Line delay pressure"
			description="Lines ranked by accumulated destination delay minutes."
			promise={lineRanking}
			mode="lines"
			onpagechange={statistics.setNetworkLineRankingOffset}
		/>
		<NetworkDelayDistributionChart
			promise={delayDistribution}
			mode={statistics.network.delayDistribution.mode}
			onmodechange={statistics.setNetworkDistributionMode}
		/>
	</section>
</main>
