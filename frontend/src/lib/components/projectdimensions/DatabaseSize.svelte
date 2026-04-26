<script lang="ts">
	import {
		MetricSeriesType,
		MetricUnit,
		type BaseMetricDataPoint,
		type BaseMetricDataPointTimestampDataPoint,
		type MetricSeries
	} from "@lib/api";
	import { scaleOrdinal } from "d3-scale";
	import { schemeTableau10 } from "d3-scale-chromatic";
	import type { ClassValue } from "svelte/elements";
	import Card from "../ui/card/Card.svelte";
	import CardHeader from "../ui/card/CardHeader.svelte";
	import MetricTrend from "../metric/MetricTrend.svelte";
	import { Axis, Chart, ChartClipPath, defaultChartPadding, Highlight, Spline, Tooltip } from "layerchart";
	import DateTooltip from "../layerchart/tooltips/DateTooltip.svelte";
	import { DateTime } from "luxon";
	import MetricLoadingFailedWarning from "../metric/MetricLoadingFailedWarning.svelte";

	type Props = {
		promise: Promise<MetricSeries>;
		class?: ClassValue;
	};
	let { promise, class: className }: Props = $props();

	let validatedPromise = $derived(
		promise.then((metric: MetricSeries) => {
			if (metric.seriesType !== MetricSeriesType.DATABASE_SIZE)
				throw new Error("DatabaseSizeMetricCard received invalid metric series type.");
			if (metric.unit !== MetricUnit.BYTES) throw new Error("DatabaseSizeMetricCard received invalid metric unit.");
			return metric;
		})
	);

	type DatabaseSizeSeries = {
		key: MetricSeriesType.DATABASE_SIZE;
		data: DatabaseSizeDataPoint[];
		color: string;
		label: string;
	};

	type DatabaseSizeDataPoint = {
		date: Date;
		value: number;
	};

	const colorScale = scaleOrdinal(schemeTableau10);

	const buildDatabaseSizeSeries = (metric: MetricSeries): DatabaseSizeSeries[] => {
		if (!metric.dataPoints.length) return [];

		const dataPoints = metric.dataPoints
			.map((baseDataPoint: BaseMetricDataPoint) => {
				const dataPoint = baseDataPoint as BaseMetricDataPointTimestampDataPoint;
				if (!dataPoint.timestamp || typeof dataPoint.value !== "number") {
					console.warn("Invalid data point in DatabaseSize metric series:", baseDataPoint);
					return null;
				}

				return {
					date: new Date(dataPoint.timestamp),
					value: dataPoint.value
				};
			})
			.filter((dataPoint: DatabaseSizeDataPoint | null): dataPoint is DatabaseSizeDataPoint => !!dataPoint)
			.sort((dataPoint: DatabaseSizeDataPoint, b: DatabaseSizeDataPoint) => dataPoint.date.getTime() - b.date.getTime());

		return [
			{
				key: metric.seriesType as MetricSeriesType.DATABASE_SIZE,
				data: dataPoints,
				color: colorScale(metric.seriesType),
				label: "Database Size"
			}
		];
	};

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

<Card class={["gap-y-2", className]}>
	<CardHeader title="Database Size" class="justify-between">
		{#await validatedPromise then metric}
			<MetricTrend metrics={[metric]} />
		{/await}
	</CardHeader>

	{#await validatedPromise}
		<div class="bg-muted h-64 w-full animate-pulse rounded-md"></div>
	{:then metric}
		{@const series = buildDatabaseSizeSeries(metric)}

		<Chart
			{series}
			data={series.flatMap((databaseSeries: DatabaseSizeSeries) => databaseSeries.data)}
			x="date"
			y="value"
			padding={defaultChartPadding({ left: 48 })}
			yDomain={null}
			brush
			height={250}
			tooltipContext={{ mode: "quadtree-x" }}
		>
			{#snippet axis()}
				<Axis
					placement="left"
					rule
					grid
					tickLength={8}
					format={(value: number) => formatBytes(value, true) + " " + getUnit(value, true)}
				/>
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
									<span class="text-foreground">
										{formatBytes(data.value, true)}
										{getUnit(data.value, true)}
									</span>
								</div>
							{/each}
						</div>
					{/snippet}
				</Tooltip.Root>

				<!-- Date Tooltip on x-Axis -->
				<DateTooltip
					{context}
					value={(dataPoint: DatabaseSizeDataPoint) =>
						DateTime.fromJSDate(dataPoint.date).toLocaleString(DateTime.DATETIME_MED)}
				/>
			{/snippet}
		</Chart>
	{:catch}
		<MetricLoadingFailedWarning />
	{/await}
</Card>
