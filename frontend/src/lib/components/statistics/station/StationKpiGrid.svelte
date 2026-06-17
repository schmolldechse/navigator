<script lang="ts">
	import type {
		StatisticsMetricResponse,
		StatisticsMetricResultEventSummaryResult,
		StatisticsMetricResultStationBenchmarkResult
	} from "@lib/api";
	import Kpi, { type KpiTrend } from "@lib/components/ui/Kpi.svelte";
	import type { RemoteQuery } from "@sveltejs/kit";
	import Activity from "@lucide/svelte/icons/activity";
	import Ban from "@lucide/svelte/icons/ban";
	import Clock from "@lucide/svelte/icons/clock";
	import ShieldCheck from "@lucide/svelte/icons/shield-check";
	import TriangleAlert from "@lucide/svelte/icons/triangle-alert";
	import { expectMetricResult, formatCount, formatMetric, formatRateDelta, toNumber } from "../shared/statistics-dashboard";

	type Props = {
		eventPromise: RemoteQuery<StatisticsMetricResponse>;
		benchmarkPromise: RemoteQuery<StatisticsMetricResponse>;
	};

	let { eventPromise, benchmarkPromise }: Props = $props();

	const reliabilityBenchmarkTrend = (response: StatisticsMetricResponse): KpiTrend | undefined => {
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

{#if eventPromise.loading || benchmarkPromise.loading}
	<div class="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
		<Kpi title="Planned events" loading icon={Activity} />
		<Kpi title="Reliable <= 5:59" loading icon={ShieldCheck} />
		<Kpi title="Cancelled events" loading icon={Ban} />
		<Kpi title="Avg stop delay" loading icon={Clock} />
	</div>
{:else if eventPromise.error || benchmarkPromise.error}
	<div class="border-destructive/30 bg-destructive/10 text-destructive rounded-xl border p-4 text-sm font-semibold">
		{eventPromise.error?.message ?? benchmarkPromise.error?.message}
	</div>
{:else if eventPromise.current && benchmarkPromise.current}
	{@const eventMetrics = expectMetricResult<StatisticsMetricResultEventSummaryResult>(
		eventPromise.current,
		"EVENT_SUMMARY"
	).metrics}
	<div class="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
		<Kpi title="Planned events" metric={{ value: formatCount(eventMetrics.plannedEvents, true) }} icon={Activity}>
			{#snippet footer()}
				<p class="text-foreground/60 text-sm font-semibold">{formatCount(eventMetrics.servedEvents, true)} served</p>
			{/snippet}
		</Kpi>
		<Kpi
			title="Reliable <= 5:59"
			metric={{ value: formatMetric(eventMetrics.customerReliability5Rate, "rate") }}
			trend={reliabilityBenchmarkTrend(benchmarkPromise.current)}
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
		<Kpi title="Severe lateness" metric={{ value: formatMetric(eventMetrics.late30Rate, "rate") }} icon={TriangleAlert}>
			{#snippet footer()}
				<p class="text-foreground/60 text-sm font-semibold">{formatMetric(eventMetrics.late60Rate, "rate")} late >= 60 min</p>
			{/snippet}
		</Kpi>
	</div>
{/if}
