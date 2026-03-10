<script lang="ts">
	import {
		MetricSeriesType,
		MetricUnit,
		type BaseMetricDataPoint,
		type BaseMetricDataPointTimestampDataPoint,
		type MetricSeries
	} from "@lib/api";
	import type { ClassValue } from "svelte/elements";
	import { onMount } from "svelte";
	import { scaleOrdinal } from "d3-scale";
	import { schemeTableau10 } from "d3-scale-chromatic";
	import { Axis, ChartClipPath, defaultChartPadding, Highlight, LineChart, Spline, Svg, Tooltip } from "layerchart";
	import { formatBytes, getUnit } from "@lib/util/bytes";
	import { DateTime } from "luxon";
	import MetricCardBase from "@lib/components/metric/MetricCardBase.svelte";
	import MetricCardTitle from "@lib/components/metric/MetricCardTitle.svelte";
	import MetricTrend from "@lib/components/metric/MetricTrend.svelte";
	import MetricLoadingFailedWarning from "@lib/components/metric/MetricLoadingFailedWarning.svelte";
	import MetricCardSummary from "@lib/components/metric/MetricCardSummary.svelte";
	import DateTooltip from "@lib/components/layerchart/tooltips/DateTooltip.svelte";

	interface DatabaseSizeSeries {
		key: string;
		data: { date: Date; value: number }[];
		color: string;
	}

	interface KeyTitles {
		key: MetricSeriesType;
		title: string;
	}
	const keyTitles: KeyTitles[] = [{ key: MetricSeriesType.DATABASE_SIZE, title: "Database Size" }];

	interface Props {
		promise: Promise<MetricSeries[]>;
		class?: ClassValue;
	}

	let { promise, class: className }: Props = $props();

	onMount(async () => {
		const metrics = await promise;

		if (metrics.some((metric: MetricSeries) => metric.seriesType !== MetricSeriesType.DATABASE_SIZE))
			throw new Error("DatabaseSizeMetricCard received invalid metric series type.");
		if (metrics.some((metric: MetricSeries) => metric.unit !== MetricUnit.BYTES))
			throw new Error("DatabaseSizeMetricCard received invalid metric unit.");
	});

	const colorScale = scaleOrdinal(schemeTableau10);
	const getChartSeries = (metrics: MetricSeries[]): DatabaseSizeSeries[] => {
		if (!metrics.length) return [];

		return metrics.map((metric: MetricSeries) => {
			const dataPoints = [...metric.dataPoints]
				.sort(
					(a: BaseMetricDataPoint, b: BaseMetricDataPoint) =>
						new Date((a as BaseMetricDataPointTimestampDataPoint).timestamp).getTime() -
						new Date((b as BaseMetricDataPointTimestampDataPoint).timestamp).getTime()
				)
				.map((dataPoint: BaseMetricDataPoint) => ({
					date: new Date((dataPoint as BaseMetricDataPointTimestampDataPoint).timestamp),
					value: dataPoint.value as number
				}));

			return {
				key: metric.seriesType,
				data: dataPoints,
				color: colorScale(metric.seriesType)
			};
		});
	};
</script>

<MetricCardBase class={["gap-y-2", className]}>
	{#snippet head()}
		<MetricCardTitle title="Database Size" class="justify-between">
			{#await promise then metrics}
				<MetricTrend {metrics} />
			{/await}
		</MetricCardTitle>
	{/snippet}

	{#snippet body()}
		{#await promise}
			<div class="bg-muted h-48 w-full animate-pulse rounded-md"></div>
		{:then metrics}
			<MetricCardSummary class="items-end">
				{@const endValue = metrics.reduce(
					(acc: number, metric: MetricSeries) => acc + Number(metric.dataPoints[metric.dataPoints.length - 1].value),
					0
				)}
				<span class="text-text text-2xl font-bold">{formatBytes(endValue, true)}</span>
				<span class="text-muted-foreground text-base font-medium">{getUnit(endValue, true)}</span>
			</MetricCardSummary>

			<LineChart
				data={getChartSeries(metrics).flatMap((series: DatabaseSizeSeries) => series.data)}
				series={getChartSeries(metrics)}
				x="date"
				y="value"
				padding={defaultChartPadding({ left: 48 })}
				yDomain={null}
				brush
				height={192}
			>
				{#snippet children({ context, visibleSeries, getSplineProps, getHighlightProps })}
					<Svg>
						<Axis
							placement="left"
							rule
							grid
							classes={{ tickLabel: "text-xs stroke-0 text-muted-foreground select-none" }}
							format={(value: number) => formatBytes(value, true) + " " + getUnit(value, true)}
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
								{#each [...payload].reverse() as item}
									{@const title =
										keyTitles?.find((title: KeyTitles) => title.key === item.rawSeriesData?.key)?.title ??
										item.rawSeriesData?.key}
									<div class="flex items-center justify-between gap-x-4 text-xs">
										<div class="flex items-center gap-x-2">
											<div class="h-1.5 w-1.5 rounded-full" style:background-color={item.color}></div>
											<span class="text-muted-foreground text-left">{title}</span>
										</div>

										<span class="text-text">
											{formatBytes(item.payload.value, true) + " " + getUnit(item.payload.value, true)}
										</span>
									</div>
								{/each}
							</div>
						{/snippet}
					</Tooltip.Root>

					<!-- Date Tooltip on x-Axis -->
					<DateTooltip
						{context}
						value={(data: { date: Date; value: number }) =>
							DateTime.fromJSDate(data.date).toLocaleString(DateTime.DATETIME_MED)}
					/>
				{/snippet}
			</LineChart>
		{:catch}
			<MetricLoadingFailedWarning />
		{/await}
	{/snippet}
</MetricCardBase>
