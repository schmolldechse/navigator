<script lang="ts">
	import {
		MetricSeriesType,
		type BaseMetricDataPoint,
		type BaseMetricDataPointTimestampDataPoint,
		type MetricSeries
	} from "@lib/api";
	import { scaleOrdinal } from "d3-scale";
	import { schemeTableau10 } from "d3-scale-chromatic";
	import type { ClassValue } from "svelte/elements";
	import { Axis, Chart, ChartClipPath, defaultChartPadding, Highlight, LineChart, Spline, Svg, Tooltip } from "layerchart";
	import { DateTime } from "luxon";
	import DateTooltip from "../../layerchart/tooltips/DateTooltip.svelte";
	import Card from "@lib/components/ui/card/Card.svelte";
	import CardHeader from "@lib/components/ui/card/CardHeader.svelte";
	import MetricTrend from "@lib/components/metric/MetricTrend.svelte";
	import MetricLoadingFailedWarning from "@lib/components/metric/MetricLoadingFailedWarning.svelte";

	type Props = {
		promise: Promise<MetricSeries>;
		class?: ClassValue;
	};
	let { promise, class: className }: Props = $props();

	let validatedPromise = $derived(
		promise.then((metric: MetricSeries) => {
			if (metric.seriesType !== MetricSeriesType.JOURNEY_TOTAL)
				throw new Error("RecordedJourneyMetricCard received invalid metric series type.");
			return metric;
		})
	);

	type RecordedJourneyChartSeries = {
		key: MetricSeriesType.JOURNEY_TOTAL;
		data: RecordedJourneysDataPoint[];
		color: string;
		label: string;
	};

	type RecordedJourneysDataPoint = {
		date: Date;
		value: number;
	};

	const colorScale = scaleOrdinal(schemeTableau10);

	const buildChartSeries = (metric: MetricSeries): RecordedJourneyChartSeries[] => {
		if (!metric.dataPoints.length) return [];

		const dataPoints = metric.dataPoints
			.map((baseDataPoint: BaseMetricDataPoint) => {
				const dataPoint = baseDataPoint as BaseMetricDataPointTimestampDataPoint;
				if (!dataPoint.timestamp || typeof dataPoint.value !== "number") {
					console.warn("Invalid data point in RecordedJourneys metric series:", baseDataPoint);
					return null;
				}

				return {
					date: new Date(dataPoint.timestamp),
					value: dataPoint.value
				};
			})
			.filter((dataPoint: RecordedJourneysDataPoint | null): dataPoint is RecordedJourneysDataPoint => !!dataPoint)
			.sort((a, b) => a.date.getTime() - b.date.getTime());

		return [
			{
				key: metric.seriesType as MetricSeriesType.JOURNEY_TOTAL,
				data: dataPoints,
				color: colorScale(metric.seriesType),
				label: "Recorded Journeys"
			}
		];
	};
</script>

<Card class={["gap-y-2", className]}>
	<CardHeader title="Recorded Journeys" class="justify-between">
		{#await validatedPromise then metric}
			<MetricTrend metrics={[metric]} />
		{/await}
	</CardHeader>

	{#await validatedPromise}
		<div class="bg-muted h-64 w-full animate-pulse rounded-md"></div>
	{:then metric}
		{@const series = buildChartSeries(metric)}

		<Chart
			{series}
			data={series.flatMap((recordedJourneySeries: RecordedJourneyChartSeries) => recordedJourneySeries.data)}
			x="date"
			y="value"
			padding={defaultChartPadding({ left: 64 })}
			yDomain={null}
			brush
			height={250}
			tooltipContext={{ mode: "quadtree-x" }}
		>
			{#snippet axis()}
				<Axis placement="left" rule grid tickLength={8} format={(value: number) => value.toLocaleString()} />
				<Axis placement="bottom" rule tickLength={8} />
			{/snippet}

			{#snippet marks({ context })}
				<ChartClipPath>
					{#each context.series.visibleSeries as visibleSeries (visibleSeries.key)}
						<Spline seriesKey={visibleSeries.key} stroke={visibleSeries.color} strokeWidth={2} />
					{/each}
				</ChartClipPath>
			{/snippet}

			{#snippet highlight()}
				<Highlight points lines />
			{/snippet}

			{#snippet tooltip({ context })}
				<Tooltip.Root anchor="top" variant="none" class="bg-background border-border rounded-lg border-2 px-2 py-0.5">
					{#snippet children({ data })}
						<div class="flex flex-col gap-y-1">
							{#each context.series.visibleSeries as visibleSeries (visibleSeries.key)}
								<div class="flex items-center justify-between gap-x-4 text-xs">
									<div class="flex items-center gap-x-2">
										<div class="h-1.5 w-1.5 rounded-full" style:background-color={visibleSeries.color}></div>
										<span class="text-foreground">{visibleSeries.label}:</span>
									</div>
									<span class="text-foreground">{data.value.toLocaleString()}</span>
								</div>
							{/each}
						</div>
					{/snippet}
				</Tooltip.Root>

				<!-- Date Tooltip on x-Axis -->
				<DateTooltip
					{context}
					value={(dataPoint: RecordedJourneysDataPoint) =>
						DateTime.fromJSDate(dataPoint.date).toLocaleString(DateTime.DATETIME_MED)}
				/>
			{/snippet}
		</Chart>
	{:catch}
		<MetricLoadingFailedWarning />
	{/await}
</Card>
