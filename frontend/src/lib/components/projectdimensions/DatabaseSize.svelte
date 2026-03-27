<script module lang="ts">
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
</script>

<script lang="ts">
	import {
		MetricSeriesType,
		MetricUnit,
		type BaseMetricDataPoint,
		type BaseMetricDataPointTimestampDataPoint,
		type MetricSeries
	} from "@lib/api";
	import type { ClassValue } from "svelte/elements";
	import { scaleOrdinal } from "d3-scale";
	import { schemeTableau10 } from "d3-scale-chromatic";
	import { Axis, ChartClipPath, defaultChartPadding, Highlight, LineChart, Spline, Svg, Tooltip } from "layerchart";
	import { DateTime } from "luxon";
	import MetricCardBase from "@lib/components/metric/MetricCardBase.svelte";
	import MetricCardTitle from "@lib/components/metric/MetricCardTitle.svelte";
	import MetricTrend from "@lib/components/metric/MetricTrend.svelte";
	import MetricLoadingFailedWarning from "@lib/components/metric/MetricLoadingFailedWarning.svelte";
	import MetricCardSummary from "@lib/components/metric/MetricCardSummary.svelte";
	import DateTooltip from "@lib/components/layerchart/tooltips/DateTooltip.svelte";

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
			.sort((a, b) => a.date.getTime() - b.date.getTime());

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

<MetricCardBase class={["gap-y-2", className]}>
	<MetricCardTitle title="Database Size" class="justify-between">
		{#await validatedPromise then metric}
			<MetricTrend metrics={[metric]} />
		{/await}
	</MetricCardTitle>

	{#await validatedPromise}
		<div class="bg-muted h-64 w-full animate-pulse rounded-md"></div>
	{:then metric}
		{@const series = buildDatabaseSizeSeries(metric)}

		<MetricCardSummary class="items-end">
			{@const endValue = series
				.map((databaseSeries: DatabaseSizeSeries) => databaseSeries.data.at(-1)?.value ?? 0)
				.reduce((acc: number, value: number) => acc + value, 0)}

			<span class="text-text text-2xl font-bold">{formatBytes(endValue, true)}</span>
			<span class="text-muted-foreground text-base font-medium">{getUnit(endValue, true)}</span>
		</MetricCardSummary>

		<LineChart
			{series}
			data={series.flatMap((databaseSeries: DatabaseSizeSeries) => databaseSeries.data)}
			x="date"
			y="value"
			padding={defaultChartPadding({ left: 48 })}
			yDomain={null}
			brush
			height={216}
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
							{#each payload as item}
								<div class="flex items-center justify-between gap-x-4 text-xs">
									<div class="flex items-center gap-x-2">
										<div class="h-1.5 w-1.5 rounded-full" style:background-color={item.color}></div>
										<span class="text-muted-foreground text-left">{item.rawSeriesData?.label}</span>
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
					value={(data: { date: Date; value: number }) => DateTime.fromJSDate(data.date).toLocaleString(DateTime.DATETIME_MED)}
				/>
			{/snippet}
		</LineChart>
	{:catch}
		<MetricLoadingFailedWarning />
	{/await}
</MetricCardBase>
