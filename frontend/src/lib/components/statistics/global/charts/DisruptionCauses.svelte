<script module lang="ts">
	type DisruptionCausePoint = {
		category: string;
		messageCount: number;
	};

	type DisruptionCauseSeries = {
		key: "messageCount";
		color: string;
		label: string;
	};
</script>

<script lang="ts">
	import {
		MetricSeriesType,
		type BaseMetricDataPoint,
		type BaseMetricDataPointCategoryDataPoint,
		type MetricSeries
	} from "@lib/api";
	import MetricCardBase from "@lib/components/metric/MetricCardBase.svelte";
	import MetricCardTitle from "@lib/components/metric/MetricCardTitle.svelte";
	import MetricLoadingFailedWarning from "@lib/components/metric/MetricLoadingFailedWarning.svelte";
	import type { ClassValue } from "svelte/elements";
	import { Axis, Bars, Chart, ChartClipPath, defaultChartPadding, Highlight, Tooltip } from "layerchart";

	type Props = {
		promise: Promise<MetricSeries[]>;
		class?: ClassValue;
	};
	let { promise, class: className }: Props = $props();

	const series: DisruptionCauseSeries[] = [{ key: "messageCount", color: "#f28e2b", label: "Messages" }];

	const buildDisruptionCauseData = (metrics: MetricSeries[]): DisruptionCausePoint[] => {
		const metric = metrics.find((metric: MetricSeries) => metric.seriesType === MetricSeriesType.MESSAGE_DISRUPTION_CAUSES);

		return (
			metric?.dataPoints
				.map((baseDataPoint: BaseMetricDataPoint) => {
					const dataPoint = baseDataPoint as BaseMetricDataPointCategoryDataPoint;
					if (!dataPoint.category) return null;

					return {
						category: dataPoint.category,
						messageCount: Number(dataPoint.value)
					};
				})
				.filter((point: DisruptionCausePoint | null): point is DisruptionCausePoint => !!point)
				.sort((a: DisruptionCausePoint, b: DisruptionCausePoint) => b.messageCount - a.messageCount)
				.slice(0, 12) ?? []
		);
	};
</script>

<MetricCardBase class={["gap-y-4", className]}>
	<MetricCardTitle title="Disruption Causes" />

	{#await promise}
		<div class="bg-muted h-64 w-full animate-pulse rounded-md"></div>
	{:then metrics}
		{@const data = buildDisruptionCauseData(metrics)}

		<Chart
			{data}
			{series}
			x="category"
			padding={defaultChartPadding({ left: 48, bottom: 112 })}
			height={360}
			bandPadding={0.24}
			tooltipContext={{ mode: "band" }}
		>
			{#snippet axis()}
				<Axis placement="left" rule grid />
				<Axis placement="bottom" rule tickLabelProps={{ rotate: 315, textAnchor: "end" }} />
			{/snippet}

			{#snippet marks({ context })}
				<ChartClipPath>
					{#each context.series.visibleSeries as visibleSeries (visibleSeries.key)}
						<Bars seriesKey={visibleSeries.key} fill={visibleSeries.color} />
					{/each}
				</ChartClipPath>
			{/snippet}

			{#snippet highlight()}
				<Highlight area />
			{/snippet}

			{#snippet tooltip()}
				<Tooltip.Root anchor="bottom" variant="none" class="bg-background/90! rounded-lg border border-white/10! px-2 py-0.5 shadow-xl backdrop-blur-md select-none">
					{#snippet children({ data })}
						<p class="text-text max-w-72 truncate text-xs font-semibold">{data.category}</p>
						<div class="mt-1 flex justify-between gap-x-4 text-xs">
							<span class="text-muted-foreground">Messages</span>
							<span>{data.messageCount.toLocaleString()}</span>
						</div>
					{/snippet}
				</Tooltip.Root>
			{/snippet}
		</Chart>
	{:catch}
		<MetricLoadingFailedWarning />
	{/await}
</MetricCardBase>
