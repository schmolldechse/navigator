<script lang="ts">
	import { MetricSeriesType, type MetricSeries } from "@lib/api";
	import MetricLoadingFailedWarning from "@lib/components/metric/MetricLoadingFailedWarning.svelte";
	import Kpi, { type KpiTrend } from "@lib/components/ui/Kpi.svelte";
	import type { loadMetric } from "@lib/remote/metrics.remote";
	import TrainFront from "@lucide/svelte/icons/train-front";
	import type { ClassValue } from "svelte/elements";

	type Props = {
		promise: Promise<Awaited<ReturnType<typeof loadMetric>>>;
		class?: ClassValue;
	};
	let { promise, class: className }: Props = $props();

	let validatedPromise = $derived(
		promise.then((metric: MetricSeries) => {
			if (metric.seriesType !== MetricSeriesType.JOURNEY_TOTAL_COUNT)
				throw new Error("Expected `JOURNEY_TOTAL_COUNT` metric series");
			if (!metric.dataPoints.length) throw new Error("Expected at least one data point");
			return metric;
		})
	);
</script>

{#await validatedPromise}
	<Kpi title="Recorded Journeys" loading icon={TrainFront} class={className} />
{:then metric}
	{@const firstValue = Number(metric.dataPoints.at(0)?.value)}
	{@const lastValue = Number(metric.dataPoints.at(-1)?.value)}
	{@const changedBy = metric.dataPoints.length > 1 ? lastValue - firstValue : 0}

	{@const trend: KpiTrend = {
		value: changedBy.toLocaleString(),
		label: "last 24h",
		direction: lastValue > firstValue ? "up" : lastValue < firstValue ? "down" : "neutral",
		tone: lastValue > firstValue ? "positive" : lastValue < firstValue ? "negative" : "neutral"
	}}

	<Kpi title="Recorded Journeys" metric={{ value: lastValue.toLocaleString() }} {trend} icon={TrainFront} class={className} />
{:catch}
	<Kpi title="Recorded Journeys" icon={TrainFront} class={className}>
		{#snippet footer()}
			<MetricLoadingFailedWarning />
		{/snippet}
	</Kpi>
{/await}
