<script lang="ts">
	import type { EventTimeSeriesPoint } from "@lib/api";
	import * as Accordion from "@lib/components/ui/accordion";
	import NetworkDelayDistributionChart from "@lib/components/statistics/network/NetworkDelayDistributionChart.svelte";
	import NetworkDelaySeverityTimelineChart from "@lib/components/statistics/network/NetworkDelaySeverityTimelineChart.svelte";
	import NetworkFilterPanel from "@lib/components/statistics/network/NetworkFilterPanel.svelte";
	import NetworkHeatmap from "@lib/components/statistics/network/NetworkHeatmap.svelte";
	import NetworkHourlyProfileChart from "@lib/components/statistics/network/NetworkHourlyProfileChart.svelte";
	import NetworkHotspotMap from "@lib/components/statistics/network/NetworkHotspotMap.svelte";
	import NetworkKpiGrid from "@lib/components/statistics/network/NetworkKpiGrid.svelte";
	import NetworkJourneyOutcomeTimelineChart from "@lib/components/statistics/network/NetworkJourneyOutcomeTimelineChart.svelte";
	import NetworkReliabilityBalanceChart from "@lib/components/statistics/network/NetworkReliabilityBalanceChart.svelte";
	import NetworkTransportComparisonChart from "@lib/components/statistics/network/NetworkTransportComparisonChart.svelte";
	import NetworkTrendChart, {
		type TrendMetricDefinition,
		type TrendPanelDefinition
	} from "@lib/components/statistics/network/NetworkTrendChart.svelte";
	import {
		NetworkStatisticsContext,
		setNetworkStatisticsContext
	} from "@lib/components/statistics/network/network-context.svelte";
	import {
		createNetworkEventDelayDistributionRequest,
		createNetworkEventSummaryRequest,
		createNetworkEventTimeSeriesRequest,
		createNetworkJourneyOutcomeTimeSeriesRequest,
		createNetworkJourneySummaryRequest,
		createNetworkMapHotspotsRequest,
		createNetworkTransportTypeComparisonRequest,
		createNetworkWeekdayHourHeatmapRequest
	} from "@lib/components/statistics/network/network-statistics";
	import MetricBucketControl from "@lib/components/statistics/shared/options/MetricBucketControl.svelte";
	import MetricPerspectiveControl from "@lib/components/statistics/shared/options/MetricPerspectiveControl.svelte";
	import Separator from "@lib/components/ui/Separator.svelte";
	import {
		createPreviousGlobalScope,
		getStatisticsBucketOptions,
		rateToPercent
	} from "@lib/components/statistics/shared/statistics-dashboard";
	import { loadNetworkMapHotspots, loadNetworkMetric } from "@lib/remote/statistics.remote";

	const statistics = new NetworkStatisticsContext();
	setNetworkStatisticsContext(statistics);

	let faqValues: string[] = $state([]);
	const faqItems = [
		{
			value: "coverage",
			title: "What do these network statistics cover?",
			answer:
				"Navigator is a growing measurement system, not an official or complete national timetable archive. The results describe the records collected for the enabled stations and transport types in the selected period and should not be read as official nationwide figures."
		},
		{
			value: "reliability",
			title: "What is the difference between customer reliability and operative punctuality?",
			answer:
				"Customer reliability uses all planned stops as its denominator, so cancellations reduce the result. Operative punctuality only evaluates stops that actually ran. Both views are useful, but the customer measure reflects that a cancelled stop is not a neutral outcome."
		},
		{
			value: "percentages",
			title: "Why can 100% be misleading?",
			answer:
				"Percentages are only as strong as their denominator. A transport type with one punctual observed stop can show 100%, while a result based on thousands of planned stops is usually the stronger signal. Read rates together with the planned-stop volume, selected period and active filters shown in the dashboard."
		},
		{
			value: "schedule-type",
			title: "Why are arrivals and departures evaluated separately?",
			answer:
				"Arrivals and departures answer different questions. Arrival quality is closer to whether passengers reach a station on time, while departure quality shows how reliably services leave. The schedule filter applies this distinction to stop-event metrics so two operational moments with different delays and cancellations are not mixed."
		},
		{
			value: "replacement-services",
			title: "How are replacement services handled?",
			answer:
				"Replacement services are included by default so the dashboard reflects the full recorded service offer. The filters can exclude them when you want to focus on regular services. A service is treated as replacement transport when the recorded journey or transport information marks it that way."
		}
	];

	const previousGlobal = $derived(createPreviousGlobalScope(statistics.global));
	const bucketOptions = $derived(getStatisticsBucketOptions(statistics.global));
	const eventSummaryRequest = $derived(createNetworkEventSummaryRequest(statistics.global, statistics.event));
	const journeySummaryRequest = $derived(createNetworkJourneySummaryRequest(statistics.global));
	const previousEventSummaryRequest = $derived(createNetworkEventSummaryRequest(previousGlobal, statistics.event));
	const previousJourneySummaryRequest = $derived(createNetworkJourneySummaryRequest(previousGlobal));
	const eventSummary = $derived(loadNetworkMetric({ request: eventSummaryRequest }));
	const eventTimeSeries = $derived(
		loadNetworkMetric({
			request: createNetworkEventTimeSeriesRequest(statistics.global, statistics.event, statistics.network.eventTimeSeries)
		})
	);
	const journeyOutcomeTimeSeries = $derived(
		loadNetworkMetric({
			request: createNetworkJourneyOutcomeTimeSeriesRequest(statistics.global, statistics.network.journeyTimeSeries)
		})
	);
	const weekdayHourHeatmap = $derived(
		loadNetworkMetric({ request: createNetworkWeekdayHourHeatmapRequest(statistics.global, statistics.event) })
	);
	const transportTypeComparison = $derived(
		loadNetworkMetric({ request: createNetworkTransportTypeComparisonRequest(statistics.global, statistics.event) })
	);
	const mapHotspots = $derived(
		loadNetworkMapHotspots({
			request: createNetworkMapHotspotsRequest(statistics.global, statistics.event)
		})
	);
	const delayDistribution = $derived(
		loadNetworkMetric({ request: createNetworkEventDelayDistributionRequest(statistics.global, statistics.event) })
	);

	const eventTrendMetrics = $derived.by<TrendMetricDefinition[]>(() => {
		const operative = statistics.network.eventTimeSeries.perspective === "operative";

		return [
			{
				key: operative ? "operativePunctuality5Rate" : "customerReliability5Rate",
				label: operative ? "Operative punctual < 6 min" : "Customer reliable < 6 min",
				color: "var(--color-accent)",
				format: "percent",
				value: (item) =>
					rateToPercent(
						operative
							? (item as EventTimeSeriesPoint).eventMetrics.operativePunctuality5Rate
							: (item as EventTimeSeriesPoint).eventMetrics.customerReliability5Rate
					)
			},
			{
				key: operative ? "operativePunctuality15Rate" : "customerReliability15Rate",
				label: operative ? "Operative punctual < 15 min" : "Customer reliable < 15 min",
				color: "var(--color-foreground)",
				format: "percent",
				value: (item) =>
					rateToPercent(
						operative
							? (item as EventTimeSeriesPoint).eventMetrics.operativePunctuality15Rate
							: (item as EventTimeSeriesPoint).eventMetrics.customerReliability15Rate
					)
			},
			{
				key: "cancellationRate",
				label: "Cancelled planned stops",
				color: "#dc2626",
				format: "percent",
				value: (item) => rateToPercent((item as EventTimeSeriesPoint).eventMetrics.cancellationRate)
			}
		];
	});

	const eventTrendDescription = $derived(
		statistics.network.eventTimeSeries.perspective === "operative"
			? "Punctuality among served stops; cancellations remain a separate share of planned stops."
			: "Stops served under each delay threshold as a share of all planned stops, with cancellations shown separately."
	);

	const eventTrendPanels = $derived<TrendPanelDefinition[]>([
		{
			key: `event-${statistics.network.eventTimeSeries.perspective}`,
			metrics: eventTrendMetrics,
			yDomain: [0, 100],
			height: 310
		}
	]);
</script>

<svelte:head>
	<title>Network Statistics - Navigator</title>
</svelte:head>

<main class="container mx-auto flex flex-col gap-y-6 p-3 sm:p-4 sm:py-8 lg:gap-y-8">
	<section class="border-border bg-secondary/10 rounded-xl border-2 p-4 sm:p-5">
		<div class="min-w-0">
			<p class="text-accent text-xs font-bold tracking-wider uppercase">Network overview</p>
			<h1 class="text-foreground mt-1 text-3xl font-bold text-balance sm:text-4xl">Network statistics</h1>
			<p class="text-foreground/60 mt-2 max-w-3xl text-sm leading-relaxed font-semibold">
				Operational quality across German rail services, grouped by stops, journeys, time, transport families and high-impact
				hotspots.
			</p>
		</div>
	</section>

	<NetworkFilterPanel />

	{#key eventSummaryRequest}
		<NetworkKpiGrid
			eventRequest={eventSummaryRequest}
			journeyRequest={journeySummaryRequest}
			previousEventRequest={previousEventSummaryRequest}
			previousJourneyRequest={previousJourneySummaryRequest}
		/>
	{/key}

	<NetworkReliabilityBalanceChart promise={eventSummary} />

	<section class="grid gap-4">
		<NetworkTrendChart
			title="Stop reliability and punctuality"
			description={eventTrendDescription}
			promise={eventTimeSeries}
			panels={eventTrendPanels}
			bucket={statistics.network.eventTimeSeries.bucket}
		>
			{#snippet actions()}
				<MetricPerspectiveControl
					value={statistics.network.eventTimeSeries.perspective}
					onchange={statistics.setEventPerspective}
				/>
				<Separator orientation="vertical" />
				<MetricBucketControl
					value={statistics.network.eventTimeSeries.bucket}
					options={bucketOptions}
					onchange={statistics.setEventBucket}
				/>
			{/snippet}
		</NetworkTrendChart>

		<NetworkDelaySeverityTimelineChart
			promise={eventTimeSeries}
			bucket={statistics.network.eventTimeSeries.bucket}
			view={statistics.network.delaySeverity.view}
			onviewchange={statistics.setDelaySeverityView}
		/>

		<NetworkJourneyOutcomeTimelineChart
			promise={journeyOutcomeTimeSeries}
			bucket={statistics.network.journeyTimeSeries.bucket}
			{bucketOptions}
			onbucketchange={statistics.setJourneyBucket}
		/>
	</section>

	<section class="grid gap-4 lg:grid-cols-[minmax(0,1.15fr)_minmax(20rem,0.85fr)]">
		<NetworkHeatmap
			promise={weekdayHourHeatmap}
			metric={statistics.network.weekdayHourHeatmap.metric}
			onmetricchange={statistics.setHeatmapMetric}
		/>
		<NetworkDelayDistributionChart
			promise={delayDistribution}
			{eventSummary}
			mode={statistics.network.delayDistribution.mode}
			onmodechange={statistics.setDistributionMode}
		/>
	</section>

	<NetworkHourlyProfileChart
		promise={weekdayHourHeatmap}
		dayGroup={statistics.network.hourlyProfile.dayGroup}
		ondaygroupchange={statistics.setHourlyProfileDayGroup}
		perspective={statistics.network.hourlyProfile.perspective}
		onperspectivechange={statistics.setHourlyProfilePerspective}
		threshold={statistics.network.hourlyProfile.threshold}
		onthresholdchange={statistics.setHourlyProfileThreshold}
	/>

	<NetworkTransportComparisonChart
		promise={transportTypeComparison}
		threshold={statistics.network.transportTypeComparison.threshold}
		onthresholdchange={statistics.setTransportComparisonThreshold}
	/>

	<section>
		<NetworkHotspotMap
			promise={mapHotspots}
			metric={statistics.network.mapHotspots.metric}
			onmetricchange={statistics.setMapMetric}
		/>
	</section>

	<section class="flex flex-col gap-y-4">
		<div class="flex flex-col gap-y-1">
			<h2 class="text-2xl font-medium">FAQ</h2>
			<p class="text-foreground/60 max-w-3xl text-sm sm:text-base">
				How to interpret the network statistics, filters and underlying sample.
			</p>
		</div>

		<Accordion.Root type="multiple" bind:value={faqValues}>
			{#each faqItems as item (item.value)}
				<Accordion.Item value={item.value}>
					<Accordion.Trigger>{item.title}</Accordion.Trigger>
					<Accordion.Content>
						<p class="leading-relaxed text-pretty">{item.answer}</p>
					</Accordion.Content>
				</Accordion.Item>
			{/each}
		</Accordion.Root>
	</section>
</main>
