<script module lang="ts">
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
</script>

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
	import { Axis, ChartClipPath, defaultChartPadding, Highlight, LineChart, Spline, Svg, Tooltip } from "layerchart";
	import { DateTime } from "luxon";
	import DateTooltip from "../layerchart/tooltips/DateTooltip.svelte";
	import MetricCardBase from "@lib/components/metric/MetricCardBase.svelte";
	import MetricCardTitle from "@lib/components/metric/MetricCardTitle.svelte";
	import MetricTrend from "@lib/components/metric/MetricTrend.svelte";
	import MetricLoadingFailedWarning from "@lib/components/metric/MetricLoadingFailedWarning.svelte";
	import MetricCardSummary from "@lib/components/metric/MetricCardSummary.svelte";

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

<MetricCardBase class={["gap-y-2", className]}>
	<MetricCardTitle title="Recorded Journeys" class="justify-between">
		{#await validatedPromise then metric}
			<MetricTrend metrics={[metric]} />
		{/await}
	</MetricCardTitle>

	{#await validatedPromise}
		<div class="bg-muted h-64 w-full animate-pulse rounded-md"></div>
	{:then metric}
		{@const series = buildChartSeries(metric)}

		<MetricCardSummary class="items-end">
			{@const endValue = series
				.map((recordedJourneySeries: RecordedJourneyChartSeries) => recordedJourneySeries.data.at(-1)?.value ?? 0)
				.reduce((acc: number, value: number) => acc + value, 0)}

			<span class="text-text text-2xl font-bold">{endValue.toLocaleString()}</span>
		</MetricCardSummary>

		<LineChart
			{series}
			data={series.flatMap((recordedJourneySeries: RecordedJourneyChartSeries) => recordedJourneySeries.data)}
			x="date"
			y="value"
			padding={defaultChartPadding()}
			yDomain={null}
			brush
			height={256}
		>
			{#snippet children({ context, visibleSeries, getSplineProps, getHighlightProps })}
				<Svg>
					<Axis
						placement="left"
						rule
						grid
						tickLabelProps={{
							textAnchor: "start",
							dx: 8
						}}
						classes={{ tickLabel: "text-xs stroke-0 text-muted-foreground select-none" }}
						format={(value: number) => value.toLocaleString()}
					/>
					<Axis placement="bottom" rule classes={{ tickLabel: "text-xs stroke-0 text-muted-foreground select-none" }} />

					<!-- ChartClipPath needed for brush -->
					<ChartClipPath>
						{#each visibleSeries as series, i (series.key)}
							<Spline {...getSplineProps(series, i)} stroke={series.color} strokeWidth={2} />
							<Highlight
								{...getHighlightProps(series, i)}
								points={{ stroke: series.color }}
								lines={{ class: "stroke-muted-foreground/30" }}
							/>
						{/each}
					</ChartClipPath>
				</Svg>

				<!-- Data Tooltip -->
				<Tooltip.Root
					anchor="bottom"
					contained="container"
					class="bg-background/90! rounded-lg border border-white/10! px-2 py-0.5 shadow-xl backdrop-blur-md select-none"
				>
					{#snippet children({ payload })}
						<div class="flex flex-col gap-y-1">
							{#each payload as item}
								<div class="flex items-center justify-between gap-x-4 text-xs">
									<div class="flex items-center gap-x-2">
										<div class="h-1.5 w-1.5 rounded-full" style:background-color={item.color}></div>
										<span class="text-muted-foreground text-left">{item.rawSeriesData?.label}</span>
									</div>

									<span class="text-text">{item.payload.value.toLocaleString()}</span>
								</div>
							{/each}
						</div>
					{/snippet}
				</Tooltip.Root>

				<!-- Date Tooltip on x-Axis -->
				<DateTooltip
					{context}
					value={(data: { date: Date; value: number }) => DateTime.fromJSDate(data.date).toLocaleString(DateTime.DATETIME_MED)}
				/>
			{/snippet}
		</LineChart>
	{:catch}
		<MetricLoadingFailedWarning />
	{/await}
</MetricCardBase>
