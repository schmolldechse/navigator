<script lang="ts">
	import { MetricSeriesType, MetricUnit, type MetricSeries } from "@lib/api";
	import Kpi, { type KpiTrend } from "@lib/components/ui/Kpi.svelte";
	import Database from "@lucide/svelte/icons/database";
	import MetricLoadingFailedWarning from "@lib/components/metric/MetricLoadingFailedWarning.svelte";
	import type { ClassValue } from "svelte/elements";
	import type { loadMetric } from "@lib/remote/metrics.remote";

	type Props = {
		promise: Promise<Awaited<ReturnType<typeof loadMetric>>>;
		class?: ClassValue;
	};
	let { promise, class: className }: Props = $props();

	const validatedPromise = $derived(
		promise.then((metric) => {
			if (metric.seriesType !== MetricSeriesType.DATABASE_SIZE_BYTES)
				throw new Error("Expected `DATABASE_SIZE_BYTES` metric series");
			if (metric.unit !== MetricUnit.BYTES) throw new Error("Expected metric unit to be bytes");
			if (!metric.dataPoints.length) throw new Error("Expected at least one data point");

			return metric;
		})
	);

	const getUnit = (bytes: number, useDecimal: boolean = false): string => {
		if (bytes === 0) return "Bytes";

		const k = useDecimal ? 1000 : 1024;
		const sizes = useDecimal
			? ["Bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"]
			: ["Bytes", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB", "ZiB", "YiB"];

		const i = Math.floor(Math.log(bytes) / Math.log(k));
		return sizes[i];
	};

	const formatBytes = (bytes: number, useDecimal: boolean = false, decimals = 2): number => {
		if (bytes === 0) return 0;

		const k = useDecimal ? 1000 : 1024;
		const dm = decimals < 0 ? 0 : decimals;
		const i = Math.floor(Math.log(bytes) / Math.log(k));
		return parseFloat((bytes / Math.pow(k, i)).toFixed(dm));
	};
</script>

{#await validatedPromise}
	<Kpi title="Database Size" loading icon={Database} class={className} />
{:then metric}
	{@const firstValue = Number(metric.dataPoints.at(0)?.value)}
	{@const lastValue = Number(metric.dataPoints.at(-1)?.value)}
	{@const changedBy = metric.dataPoints.length > 1 ? lastValue - firstValue : 0}

	{@const kpiMetric = { value: formatBytes(lastValue, true), unit: getUnit(lastValue, true) }}
	{@const trend: KpiTrend = {
		value: formatBytes(changedBy, true) + " " + getUnit(Math.abs(changedBy), true),
		label: "last 24h",
		direction: lastValue > firstValue ? "up" : lastValue < firstValue ? "down" : "neutral",
		tone: lastValue > firstValue ? "positive" : lastValue < firstValue ? "negative" : "neutral"
	}}

	<Kpi title="Database Size" metric={kpiMetric} {trend} icon={Database} class={className} />
{:catch}
	<Kpi title="Database Size" icon={Database} class={className}>
		{#snippet footer()}
			<MetricLoadingFailedWarning />
		{/snippet}
	</Kpi>
{/await}
