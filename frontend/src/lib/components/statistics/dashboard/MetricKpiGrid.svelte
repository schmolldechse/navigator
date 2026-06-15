<script lang="ts">
	import type {
		StatisticsMetricResponse,
		StatisticsMetricResultEventKpisResult,
		StatisticsMetricResultJourneyKpisResult,
		StatisticsMetricResultStationBenchmarkResult
	} from "@lib/api";
	import Kpi, { type KpiTrend } from "@lib/components/ui/Kpi.svelte";
	import Activity from "@lucide/svelte/icons/activity";
	import Ban from "@lucide/svelte/icons/ban";
	import CircleCheck from "@lucide/svelte/icons/circle-check";
	import Clock from "@lucide/svelte/icons/clock";
	import Route from "@lucide/svelte/icons/route";
	import ShieldCheck from "@lucide/svelte/icons/shield-check";
	import { expectMetricResult, formatCount, formatMetric, formatRateDelta, toNumber } from "./statistics-dashboard";

	type Props = {
		eventPromise: Promise<StatisticsMetricResponse>;
		journeyPromise?: Promise<StatisticsMetricResponse>;
		benchmarkPromise?: Promise<StatisticsMetricResponse>;
	};

	let { eventPromise, journeyPromise, benchmarkPromise }: Props = $props();

	const resolveKpis = async (
		eventMetricPromise: Promise<StatisticsMetricResponse>,
		journeyMetricPromise?: Promise<StatisticsMetricResponse>,
		benchmarkMetricPromise?: Promise<StatisticsMetricResponse>
	) => ({
		eventResponse: await eventMetricPromise,
		journeyResponse: journeyMetricPromise ? await journeyMetricPromise : undefined,
		benchmarkResponse: benchmarkMetricPromise ? await benchmarkMetricPromise : undefined
	});

	const reliabilityBenchmarkTrend = (response: StatisticsMetricResponse | undefined): KpiTrend | undefined => {
		if (!response) return undefined;

		const benchmark = expectMetricResult<StatisticsMetricResultStationBenchmarkResult>(response, "STATION_BENCHMARK");
		const stationReliability = toNumber(benchmark.station.customerReliability5Rate);
		const networkReliability = toNumber(benchmark.network.customerReliability5Rate);
		if (stationReliability === null || networkReliability === null) return undefined;

		const delta = stationReliability - networkReliability;
		return {
			value: formatRateDelta(delta),
			label: "vs network",
			direction: delta > 0 ? "up" : delta < 0 ? "down" : "neutral",
			tone: delta > 0 ? "positive" : delta < 0 ? "negative" : "neutral"
		};
	};
</script>

{#await resolveKpis(eventPromise, journeyPromise, benchmarkPromise)}
	<div class="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
		<Kpi title="Planned events" loading icon={Activity} />
		<Kpi title="Reliable <= 5:59" loading icon={ShieldCheck} />
		<Kpi title="Cancelled events" loading icon={Ban} />
		<Kpi title="Avg stop delay" loading icon={Clock} />
	</div>
{:then { eventResponse, journeyResponse, benchmarkResponse }}
	{@const eventMetrics = expectMetricResult<StatisticsMetricResultEventKpisResult>(eventResponse, "EVENT_KPIS").metrics}
	{@const journeyMetrics = journeyResponse
		? expectMetricResult<StatisticsMetricResultJourneyKpisResult>(journeyResponse, "JOURNEY_KPIS").metrics
		: undefined}
	<div class="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
		<Kpi title="Planned events" metric={{ value: formatCount(eventMetrics.plannedEvents, true) }} icon={Activity}>
			{#snippet footer()}
				<p class="text-foreground/60 text-sm font-semibold">{formatCount(eventMetrics.servedEvents, true)} served</p>
			{/snippet}
		</Kpi>
		<Kpi
			title="Reliable <= 5:59"
			metric={{ value: formatMetric(eventMetrics.customerReliability5Rate, "rate") }}
			trend={reliabilityBenchmarkTrend(benchmarkResponse)}
			icon={ShieldCheck}
		>
			{#snippet footer()}
				<p class="text-foreground/60 text-sm font-semibold">
					{formatMetric(eventMetrics.operativePunctuality5Rate, "rate")} operative
				</p>
			{/snippet}
		</Kpi>
		<Kpi title="Cancelled events" metric={{ value: formatMetric(eventMetrics.cancellationRate, "rate") }} icon={Ban}>
			{#snippet footer()}
				<p class="text-foreground/60 text-sm font-semibold">{formatCount(eventMetrics.cancelledEvents, true)} stops</p>
			{/snippet}
		</Kpi>
		<Kpi title="Avg stop delay" metric={{ value: formatMetric(eventMetrics.averageDelaySeconds, "seconds") }} icon={Clock}>
			{#snippet footer()}
				<p class="text-foreground/60 text-sm font-semibold">
					{formatMetric(eventMetrics.delayDebtMinutes, "minutes", true)} delay debt
				</p>
			{/snippet}
		</Kpi>

		{#if journeyMetrics}
			<Kpi title="Planned journeys" metric={{ value: formatCount(journeyMetrics.plannedJourneys, true) }} icon={Route}>
				{#snippet footer()}
					<p class="text-foreground/60 text-sm font-semibold">
						{formatCount(journeyMetrics.completedJourneys, true)} completed
					</p>
				{/snippet}
			</Kpi>
			<Kpi
				title="Journey completion"
				metric={{ value: formatMetric(journeyMetrics.journeyCompletionRate, "rate") }}
				icon={CircleCheck}
			>
				{#snippet footer()}
					<p class="text-foreground/60 text-sm font-semibold">
						{formatMetric(journeyMetrics.destinationNotReachedRate, "rate")} not reached
					</p>
				{/snippet}
			</Kpi>
			<Kpi
				title="Destination <= 5:59"
				metric={{ value: formatMetric(journeyMetrics.destinationPunctuality5Rate, "rate") }}
				icon={ShieldCheck}
			>
				{#snippet footer()}
					<p class="text-foreground/60 text-sm font-semibold">
						{formatMetric(journeyMetrics.averageDestinationDelaySeconds, "seconds")} avg delay
					</p>
				{/snippet}
			</Kpi>
			<Kpi title="Full cancellations" metric={{ value: formatMetric(journeyMetrics.fullCancellationRate, "rate") }} icon={Ban}>
				{#snippet footer()}
					<p class="text-foreground/60 text-sm font-semibold">
						{formatCount(journeyMetrics.fullyCancelledJourneys, true)} journeys
					</p>
				{/snippet}
			</Kpi>
		{/if}
	</div>
{:catch error}
	<div class="border-destructive/30 bg-destructive/10 text-destructive rounded-xl border p-4 text-sm font-semibold">
		{error.message}
	</div>
{/await}
