<script module lang="ts">
	type OperatorReliabilityPoint = {
		category: string;
		journeyCount: number;
		cancellationCount: number;
		cancellationRate: number;
	};

	type OperatorReliabilitySeries = {
		key: "cancellationRate";
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

	const series: OperatorReliabilitySeries[] = [{ key: "cancellationRate", color: "#e15759", label: "Cancellation rate" }];

	const getCategoryMetric = (metrics: MetricSeries[], seriesType: MetricSeriesType) =>
		metrics.find((metric: MetricSeries) => metric.seriesType === seriesType);

	const categoryValues = (metric?: MetricSeries): Map<string, number> => {
		const values = new Map<string, number>();
		metric?.dataPoints.forEach((baseDataPoint: BaseMetricDataPoint) => {
			const dataPoint = baseDataPoint as BaseMetricDataPointCategoryDataPoint;
			if (!dataPoint.category) return;
			values.set(dataPoint.category, Number(dataPoint.value));
		});
		return values;
	};

	const buildOperatorReliabilityData = (metrics: MetricSeries[]): OperatorReliabilityPoint[] => {
		const journeyCounts = categoryValues(getCategoryMetric(metrics, MetricSeriesType.JOURNEY_SERVICE_OPERATOR_JOURNEYS));
		const cancellationCounts = categoryValues(getCategoryMetric(metrics, MetricSeriesType.JOURNEY_SERVICE_OPERATOR_CANCELLATIONS));

		return [...journeyCounts.entries()]
			.map(([category, journeyCount]) => {
				const cancellationCount = cancellationCounts.get(category) ?? 0;
				return {
					category,
					journeyCount,
					cancellationCount,
					cancellationRate: journeyCount === 0 ? 0 : (cancellationCount / journeyCount) * 100
				};
			})
			.sort((a: OperatorReliabilityPoint, b: OperatorReliabilityPoint) => b.cancellationRate - a.cancellationRate)
			.slice(0, 12);
	};
</script>

<MetricCardBase class={["gap-y-4", className]}>
	<MetricCardTitle title="Operator Reliability" />

	{#await promise}
		<div class="bg-muted h-64 w-full animate-pulse rounded-md"></div>
	{:then metrics}
		{@const data = buildOperatorReliabilityData(metrics)}

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
				<Axis placement="left" rule grid format={(value: number) => `${value.toLocaleString(undefined, { maximumFractionDigits: 0 })}%`} />
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
						<div class="mt-1 grid grid-cols-2 gap-x-4 gap-y-1 text-xs">
							<span class="text-muted-foreground">Rate</span>
							<span class="text-right">{data.cancellationRate.toLocaleString(undefined, { maximumFractionDigits: 2 })}%</span>
							<span class="text-muted-foreground">Cancellations</span>
							<span class="text-right">{data.cancellationCount.toLocaleString()}</span>
							<span class="text-muted-foreground">Journeys</span>
							<span class="text-right">{data.journeyCount.toLocaleString()}</span>
						</div>
					{/snippet}
				</Tooltip.Root>
			{/snippet}
		</Chart>
	{:catch}
		<MetricLoadingFailedWarning />
	{/await}
</MetricCardBase>
