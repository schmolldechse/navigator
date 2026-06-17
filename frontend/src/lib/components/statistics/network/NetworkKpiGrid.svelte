<script lang="ts">
	import type {
		StatisticsMetricResponse,
		StatisticsMetricResultEventSummaryResult,
		StatisticsMetricResultJourneySummaryResult
	} from "@lib/api";
	import type { RemoteQuery } from "@sveltejs/kit";
	import Kpi from "@lib/components/ui/Kpi.svelte";
	import Activity from "@lucide/svelte/icons/activity";
	import Ban from "@lucide/svelte/icons/ban";
	import CircleCheck from "@lucide/svelte/icons/circle-check";
	import Clock from "@lucide/svelte/icons/clock";
	import ShieldCheck from "@lucide/svelte/icons/shield-check";
	import {
		expectMetricResult,
		formatCount,
		formatMetric,
		formatRateDelta,
		formatSeconds,
		toNumber
	} from "../shared/statistics-dashboard";

	type Props = {
		eventPromise: RemoteQuery<StatisticsMetricResponse>;
		journeyPromise: RemoteQuery<StatisticsMetricResponse>;
		previousEventPromise?: RemoteQuery<StatisticsMetricResponse>;
		previousJourneyPromise?: RemoteQuery<StatisticsMetricResponse>;
	};

	let { eventPromise, journeyPromise, previousEventPromise, previousJourneyPromise }: Props = $props();

	const countTrend = (current: number | string | null | undefined, previous: number | string | null | undefined) => {
		const currentValue = toNumber(current);
		const previousValue = toNumber(previous);
		if (currentValue === null || previousValue === null || previousValue === 0) return undefined;

		const delta = (currentValue - previousValue) / previousValue;
		return {
			value: `${delta >= 0 ? "+" : ""}${(delta * 100).toFixed(1)}%`,
			label: "vs previous period",
			direction: delta > 0 ? ("up" as const) : delta < 0 ? ("down" as const) : ("neutral" as const),
			tone: "neutral" as const
		};
	};

	const rateTrend = (
		current: number | string | null | undefined,
		previous: number | string | null | undefined,
		lowerIsBetter = false
	) => {
		const currentValue = toNumber(current);
		const previousValue = toNumber(previous);
		if (currentValue === null || previousValue === null) return undefined;

		const delta = currentValue - previousValue;
		const improved = lowerIsBetter ? delta < 0 : delta > 0;
		const worsened = lowerIsBetter ? delta > 0 : delta < 0;

		return {
			value: formatRateDelta(delta),
			label: "vs previous period",
			direction: delta > 0 ? ("up" as const) : delta < 0 ? ("down" as const) : ("neutral" as const),
			tone: improved ? ("positive" as const) : worsened ? ("negative" as const) : ("neutral" as const)
		};
	};

	const secondsTrend = (current: number | string | null | undefined, previous: number | string | null | undefined) => {
		const currentValue = toNumber(current);
		const previousValue = toNumber(previous);
		if (currentValue === null || previousValue === null) return undefined;

		const delta = currentValue - previousValue;
		return {
			value: `${delta >= 0 ? "+" : ""}${formatSeconds(delta)}`,
			label: "vs previous period",
			direction: delta > 0 ? ("up" as const) : delta < 0 ? ("down" as const) : ("neutral" as const),
			tone: delta < 0 ? ("positive" as const) : delta > 0 ? ("negative" as const) : ("neutral" as const)
		};
	};
</script>

{#if eventPromise.loading || journeyPromise.loading}
	<div class="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
		<Kpi title="Planned stops" loading icon={Activity} />
		<Kpi title="Cancelled stops" loading icon={Ban} />
		<Kpi title="Cancellation rate" loading icon={Ban} />
		<Kpi title="Reliable < 6 min" loading icon={ShieldCheck} />
	</div>
{:else if eventPromise.error || journeyPromise.error}
	<div class="border-destructive/30 bg-destructive/10 text-destructive rounded-xl border p-4 text-sm font-semibold">
		{eventPromise.error?.message ?? journeyPromise.error?.message}
	</div>
{:else if eventPromise.current && journeyPromise.current}
	{@const eventMetrics = expectMetricResult<StatisticsMetricResultEventSummaryResult>(
		eventPromise.current,
		"EVENT_SUMMARY"
	).metrics}
	{@const journeyMetrics = expectMetricResult<StatisticsMetricResultJourneySummaryResult>(
		journeyPromise.current,
		"JOURNEY_SUMMARY"
	).metrics}
	{@const previousEventMetrics =
		previousEventPromise?.current && !previousEventPromise.error
			? expectMetricResult<StatisticsMetricResultEventSummaryResult>(previousEventPromise.current, "EVENT_SUMMARY").metrics
			: null}
	{@const previousJourneyMetrics =
		previousJourneyPromise?.current && !previousJourneyPromise.error
			? expectMetricResult<StatisticsMetricResultJourneySummaryResult>(previousJourneyPromise.current, "JOURNEY_SUMMARY")
					.metrics
			: null}
	<div class="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
		<Kpi
			title="Planned stops"
			metric={{ value: formatCount(eventMetrics.plannedEvents, true) }}
			trend={countTrend(eventMetrics.plannedEvents, previousEventMetrics?.plannedEvents)}
			icon={Activity}
		>
			{#snippet footer()}
				<p class="text-foreground/60 text-sm font-semibold">{formatCount(eventMetrics.servedEvents, true)} served</p>
			{/snippet}
		</Kpi>
		<Kpi
			title="Cancelled stops"
			metric={{ value: formatCount(eventMetrics.cancelledEvents, true) }}
			trend={countTrend(eventMetrics.cancelledEvents, previousEventMetrics?.cancelledEvents)}
			icon={Ban}
		>
			{#snippet footer()}
				<p class="text-foreground/60 text-sm font-semibold">
					{formatMetric(eventMetrics.cancellationRate, "rate")} of planned stops
				</p>
			{/snippet}
		</Kpi>
		<Kpi
			title="Cancellation rate"
			metric={{ value: formatMetric(eventMetrics.cancellationRate, "rate") }}
			trend={rateTrend(eventMetrics.cancellationRate, previousEventMetrics?.cancellationRate, true)}
			icon={Ban}
		>
			{#snippet footer()}
				<p class="text-foreground/60 text-sm font-semibold">{formatCount(eventMetrics.cancelledEvents, true)} cancelled</p>
			{/snippet}
		</Kpi>
		<Kpi
			title="Reliable < 6 min"
			metric={{ value: formatMetric(eventMetrics.customerReliability5Rate, "rate") }}
			trend={rateTrend(eventMetrics.customerReliability5Rate, previousEventMetrics?.customerReliability5Rate)}
			icon={ShieldCheck}
		>
			{#snippet footer()}
				<p class="text-foreground/60 text-sm font-semibold">
					{formatMetric(eventMetrics.operativePunctuality5Rate, "rate")} operative
				</p>
			{/snippet}
		</Kpi>

		<Kpi
			title="Reliable < 15 min"
			metric={{ value: formatMetric(eventMetrics.customerReliability15Rate, "rate") }}
			trend={rateTrend(eventMetrics.customerReliability15Rate, previousEventMetrics?.customerReliability15Rate)}
			icon={ShieldCheck}
		>
			{#snippet footer()}
				<p class="text-foreground/60 text-sm font-semibold">
					{formatMetric(eventMetrics.operativePunctuality15Rate, "rate")} operative
				</p>
			{/snippet}
		</Kpi>
		<Kpi
			title="Avg delay"
			metric={{ value: formatMetric(eventMetrics.averageDelaySeconds, "seconds") }}
			trend={secondsTrend(eventMetrics.averageDelaySeconds, previousEventMetrics?.averageDelaySeconds)}
			icon={Clock}
		>
			{#snippet footer()}
				<p class="text-foreground/60 text-sm font-semibold">
					{formatMetric(eventMetrics.delayDebtMinutes, "minutes", true)} delay debt
				</p>
			{/snippet}
		</Kpi>
		<Kpi
			title="Journey completion"
			metric={{ value: formatMetric(journeyMetrics.journeyCompletionRate, "rate") }}
			trend={rateTrend(journeyMetrics.journeyCompletionRate, previousJourneyMetrics?.journeyCompletionRate)}
			icon={CircleCheck}
		>
			{#snippet footer()}
				<p class="text-foreground/60 text-sm font-semibold">
					{formatCount(journeyMetrics.completedJourneys, true)} of {formatCount(journeyMetrics.plannedJourneys, true)}
				</p>
			{/snippet}
		</Kpi>
	</div>
{/if}
