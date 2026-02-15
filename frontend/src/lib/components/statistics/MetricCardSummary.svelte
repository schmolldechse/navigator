<script lang="ts">
	import type { MetricSeries, MetricSummary } from "@lib/api";
	import type { Snippet } from "svelte";
	import type { ClassValue } from "svelte/elements";

	interface Props {
		metrics: MetricSeries[];
		title: Snippet<[{ summary: MetricSummary[] }]>;
		class?: ClassValue;
	}

	let { metrics, title, class: className }: Props = $props();
</script>

<div class={["flex flex-row gap-x-4", className]}>
	{@render title({ summary: metrics.map((metric: MetricSeries) => metric.summary) })}
</div>
