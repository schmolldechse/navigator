<script lang="ts">
	import type { MetricSeries } from "@lib/api";
	import TrendingUp from "@lucide/svelte/icons/trending-up";
	import TrendingDown from "@lucide/svelte/icons/trending-down";

	interface Props {
		metrics: MetricSeries[];
		togglingEnabled?: boolean;
		defaultDisplayMode?: "percentage" | "absolute";
	}

	let { metrics, togglingEnabled = true, defaultDisplayMode = "percentage" }: Props = $props();

	const calculateChange = (metrics: MetricSeries[]): { percentage: number; isUp: boolean } => {
		const endValue = metrics.reduce((a, metric: MetricSeries) => {
			const value = Number(metric.dataPoints[metric.dataPoints.length - 1].value);
			return a + (isNaN(value) ? 0 : value);
		}, 0);
		const startValue = metrics.reduce((a, metric: MetricSeries) => {
			const value = Number(metric.dataPoints[0].value);
			return a + (isNaN(value) ? 0 : value);
		}, 0);
		const changedBy = endValue - startValue;

		if (startValue === 0) return { percentage: endValue > 0 ? 100 : 0, isUp: endValue >= 0 };
		const percentage = (changedBy / startValue) * 100;

		return {
			percentage: Math.abs(percentage),
			isUp: changedBy >= 0
		};
	};

	let change: { percentage: number; isUp: boolean } = $derived(calculateChange(metrics));
</script>

<button
	class={[
		"flex items-center gap-x-2 rounded-full border px-3 py-1 text-xs font-semibold",
		change.isUp && "border-emerald-400/30 bg-green-500/10 text-emerald-500",
		!change.isUp && "border-rose-400/30 bg-red-500/10 text-rose-500",
		togglingEnabled && "cursor-pointer"
	]}
	onclick={() => {
		if (!togglingEnabled) return;
	}}
>
	{#if change.isUp}
		<TrendingUp size={14} />
	{:else}
		<TrendingDown size={14} />
	{/if}
	{change.percentage.toFixed(2)}%
</button>
