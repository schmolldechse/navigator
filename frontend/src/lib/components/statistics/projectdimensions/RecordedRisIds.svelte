<script lang="ts">
	import { MetricSeriesType, type MetricSeries } from "@lib/api";
	import { scaleOrdinal } from "d3-scale";
	import { schemeTableau10 } from "d3-scale-chromatic";
	import type { ClassValue } from "svelte/elements";
	import MetricLoadingFailedWarning from "@lib/components/metric/MetricLoadingFailedWarning.svelte";
	import Kpi, { type KpiTrend } from "@lib/components/ui/Kpi.svelte";
	import Wrench from "@lucide/svelte/icons/wrench";
	import DistributionBar, { type DistributionBarItem } from "@lib/components/ui/DistributionBar.svelte";

	type Props = {
		promise: Promise<MetricSeries[]>;
		class?: ClassValue;
	};
	let { promise, class: className }: Props = $props();

	const validatedPromise = $derived(
		promise.then((metrics: MetricSeries[]) => {
			if (
				!metrics.every((metric: MetricSeries) =>
					[MetricSeriesType.RIS_IDS_ACTIVE, MetricSeriesType.RIS_IDS_INACTIVE].includes(metric.seriesType)
				)
			)
				throw new Error("Expected `RIS_IDS_ACTIVE` or `RIS_IDS_INACTIVE` metric series");
			if (!metrics.every((metric) => metric.dataPoints.length > 0)) throw new Error("Expected at least one data point");

			return metrics;
		})
	);

	const colorScale = scaleOrdinal([schemeTableau10[2], "rgb(64 64 64)"]);

	const buildRisIdDistributionItems = (metrics: MetricSeries[]): DistributionBarItem[] => {
		if (!metrics.length) return [];

		return metrics.map((metric: MetricSeries) => {
			const value = Number(metric.dataPoints.at(-1)?.value);
			const label = metric.seriesType === MetricSeriesType.RIS_IDS_ACTIVE ? "Active" : "Inactive";

			return {
				key: metric.seriesType,
				value,
				color: colorScale(metric.seriesType),
				label
			};
		});
	};
</script>

{#await validatedPromise}
	<Kpi title="RIS IDs" loading icon={Wrench} class={className} />
{:then metrics}
	{@const firstValue = metrics.reduce((sum: number, metric: MetricSeries) => sum + Number(metric.dataPoints.at(0)?.value), 0)}
	{@const lastValue = metrics.reduce((sum: number, metric: MetricSeries) => sum + Number(metric.dataPoints.at(-1)?.value), 0)}

	{@const trend: KpiTrend = {
		value: (lastValue - firstValue).toLocaleString(),
		label: "last 24h",
		direction: lastValue > firstValue ? "up" : lastValue < firstValue ? "down" : "neutral",
		tone: lastValue > firstValue ? "positive" : lastValue < firstValue ? "negative" : "neutral"
	}}

	<Kpi title="RIS IDs" icon={Wrench} metric={{ value: lastValue.toLocaleString() }} {trend} class={className}>
		{#snippet footer()}
			<DistributionBar
				items={buildRisIdDistributionItems(metrics)}
				ariaLabel="RIS ID active and inactive distribution"
				class="mt-3"
			/>
		{/snippet}
	</Kpi>
{:catch}
	<Kpi title="RIS IDs" icon={Wrench} class={className}>
		{#snippet footer()}
			<MetricLoadingFailedWarning />
		{/snippet}
	</Kpi>
{/await}
