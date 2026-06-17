<script lang="ts">
	import type { EventTimeSeriesPoint } from "@lib/api";
	import { getStatisticsDashboardContext } from "@lib/components/statistics/shared/statistics-context.svelte";
	import MetricBucketControl from "@lib/components/statistics/shared/MetricBucketControl.svelte";
	import StatisticsFilterPanel from "@lib/components/statistics/shared/StatisticsFilterPanel.svelte";
	import {
		createRangeLabel,
		createStationArrivalDepartureComparisonRequest,
		createStationBenchmarkRequest,
		createStationDirectionsRequest,
		createStationEventDetailsRequest,
		createStationEventSummaryRequest,
		createStationLineHourMatrixRequest,
		createStationLineRankingRequest,
		createStationTimeSeriesRequest,
		createStationTransportTypeMixRequest,
		createStationWeekdayHourHeatmapRequest,
		rateToPercent,
		transportTypeShortLabel
	} from "@lib/components/statistics/shared/statistics-dashboard";
	import StationComparisonChart from "@lib/components/statistics/station/StationComparisonChart.svelte";
	import StationEventDetailsTable from "@lib/components/statistics/station/StationEventDetailsTable.svelte";
	import StationHeatmap from "@lib/components/statistics/station/StationHeatmap.svelte";
	import StationKpiGrid from "@lib/components/statistics/station/StationKpiGrid.svelte";
	import StationLineHourMatrix from "@lib/components/statistics/station/StationLineHourMatrix.svelte";
	import StationRankingList from "@lib/components/statistics/station/StationRankingList.svelte";
	import StationTrendChart, { type TrendMetricDefinition } from "@lib/components/statistics/station/StationTrendChart.svelte";
	import Button from "@lib/components/ui/Button.svelte";
	import { loadStationMetric } from "@lib/remote/statistics.remote";
	import ArrowLeft from "@lucide/svelte/icons/arrow-left";
	import RadioTower from "@lucide/svelte/icons/radio-tower";
	import type { PageProps } from "./$types";

	let { data }: PageProps = $props();
	const statistics = getStatisticsDashboardContext();
	const rangeLabel = $derived(createRangeLabel(statistics.global));

	const eventSummary = $derived(
		loadStationMetric({
			request: createStationEventSummaryRequest(statistics.global, statistics.event, data.evaNumber)
		})
	);
	const benchmark = $derived(
		loadStationMetric({
			request: createStationBenchmarkRequest(statistics.global, statistics.event, data.evaNumber)
		})
	);
	const timeSeries = $derived(
		loadStationMetric({
			request: createStationTimeSeriesRequest(
				statistics.global,
				statistics.event,
				data.evaNumber,
				statistics.station.timeSeries
			)
		})
	);
	const arrivalDepartureComparison = $derived(
		loadStationMetric({
			request: createStationArrivalDepartureComparisonRequest(statistics.global, statistics.event, data.evaNumber)
		})
	);
	const weekdayHourHeatmap = $derived(
		loadStationMetric({
			request: createStationWeekdayHourHeatmapRequest(statistics.global, statistics.event, data.evaNumber)
		})
	);
	const lineRanking = $derived(
		loadStationMetric({
			request: createStationLineRankingRequest(
				statistics.global,
				statistics.event,
				data.evaNumber,
				statistics.station.lineRanking
			)
		})
	);
	const directions = $derived(
		loadStationMetric({
			request: createStationDirectionsRequest(statistics.global, statistics.event, data.evaNumber)
		})
	);
	const transportTypeMix = $derived(
		loadStationMetric({
			request: createStationTransportTypeMixRequest(statistics.global, statistics.event, data.evaNumber)
		})
	);
	const lineHourMatrix = $derived(
		loadStationMetric({
			request: createStationLineHourMatrixRequest(statistics.global, statistics.event, data.evaNumber)
		})
	);
	const eventDetails = $derived(
		loadStationMetric({
			request: createStationEventDetailsRequest(
				statistics.global,
				statistics.event,
				data.evaNumber,
				statistics.station.eventDetails
			)
		})
	);

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
				<h1 class="text-3xl font-bold text-balance sm:text-4xl">{data.station.name}</h1>
				<p class="text-foreground/60 mt-2 text-sm font-semibold">{rangeLabel}</p>

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

			<Button href="/statistics" mode="secondary" class="inline-flex items-center gap-2">
				<ArrowLeft size={16} />
				Network
			</Button>
		</div>
	</section>

	<StatisticsFilterPanel />

	<StationKpiGrid eventPromise={eventSummary} benchmarkPromise={benchmark} />

	<section class="grid gap-4 xl:grid-cols-[minmax(0,1.25fr)_minmax(22rem,0.75fr)]">
		<StationTrendChart
			title="Station stop-event reliability"
			description="Reliability, broader punctuality and cancellations over time."
			promise={timeSeries}
			metrics={eventTrendMetrics}
			yDomain={[0, 100]}
		>
			{#snippet actions()}
				<MetricBucketControl value={statistics.station.timeSeries.bucket} onchange={statistics.setStationTimeSeriesBucket} />
			{/snippet}
		</StationTrendChart>

		<StationComparisonChart
			title="Arrival and departure"
			description="Stop-event quality split by schedule type."
			promise={arrivalDepartureComparison}
			mode="arrivalDeparture"
		/>
	</section>

	<section class="grid gap-4 xl:grid-cols-[minmax(0,1fr)_minmax(22rem,0.8fr)]">
		<StationHeatmap promise={weekdayHourHeatmap} />
		<StationComparisonChart
			title="Transport mix"
			description="Station activity share and reliability by transport type."
			promise={transportTypeMix}
			mode="transportMix"
		/>
	</section>

	<section class="grid gap-4 xl:grid-cols-2">
		<StationRankingList
			title="Line delay pressure"
			description="Lines at this station ranked by accumulated delay minutes."
			promise={lineRanking}
			mode="lines"
			onpagechange={statistics.setStationLineRankingOffset}
		/>
		<StationRankingList
			title="Directions"
			description="Connected origins and destinations ranked by delay pressure."
			promise={directions}
			mode="directions"
		/>
	</section>

	<StationLineHourMatrix promise={lineHourMatrix} />

	<StationEventDetailsTable promise={eventDetails} onpagechange={statistics.setStationEventDetailsOffset} />
</main>
