<script module lang="ts">
	type RisIdDistributionSeries = {
		seriesType: MetricSeriesType;
		value: number;
		color: string;
		label: string;
	};

	type RisIdLineChartSeries = {
		key: string;
		data: RisIdLineChartDataPoint[];
		color: string;
		label: string;
	};

	type RisIdLineChartDataPoint = {
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
	import {
		Axis,
		ChartClipPath,
		defaultChartPadding,
		Highlight,
		Legend,
		LineChart,
		PieChart,
		Spline,
		Svg,
		Text,
		Tooltip
	} from "layerchart";
	import { DateTime } from "luxon";
	import Info from "@lucide/svelte/icons/info";
	import RecordedRisIdsInformationDialog from "./RecordedRisIdsInformationDialog.svelte";
	import MetricCardBase from "@lib/components/metric/MetricCardBase.svelte";
	import MetricCardTitle from "@lib/components/metric/MetricCardTitle.svelte";
	import MetricTrend from "@lib/components/metric/MetricTrend.svelte";
	import MetricLoadingFailedWarning from "@lib/components/metric/MetricLoadingFailedWarning.svelte";
	import DateTooltip from "@lib/components/layerchart/tooltips/DateTooltip.svelte";
	import Separator from "@lib/components/interactable/Separator.svelte";

	type Props = {
		promise: Promise<MetricSeries[]>;
		class?: ClassValue;
	};
	let { promise, class: className }: Props = $props();

	let informationDialogVisible: boolean = $state(false);

	let validatedPromise = $derived(
		promise.then((metrics: MetricSeries[]) => {
			if (
				!metrics.every((metric: MetricSeries) =>
					[MetricSeriesType.RIS_IDS_ACTIVE, MetricSeriesType.RIS_IDS_INACTIVE].includes(metric.seriesType)
				)
			)
				throw new Error("RecordedRisIds received invalid metric series type.");
			return metrics;
		})
	);

	const colorScale = scaleOrdinal(schemeTableau10);

	const buildRisIdLineChartSeries = (metrics: MetricSeries[]): RisIdLineChartSeries[] => {
		if (!metrics.length) return [];

		return metrics.map((metric: MetricSeries) => {
			const dataPoints = metric.dataPoints
				.map((baseDataPoint: BaseMetricDataPoint) => {
					const dataPoint = baseDataPoint as BaseMetricDataPointTimestampDataPoint;
					if (!dataPoint.timestamp || typeof dataPoint.value !== "number") {
						console.warn("Invalid data point in RIS IDs metric series:", baseDataPoint);
						return null;
					}

					return {
						date: new Date(dataPoint.timestamp),
						value: dataPoint.value
					};
				})
				.filter((dataPoint: RisIdLineChartDataPoint | null): dataPoint is RisIdLineChartDataPoint => !!dataPoint)
				.sort((a, b) => a.date.getTime() - b.date.getTime());

			return {
				key: metric.seriesType,
				data: dataPoints,
				color: colorScale(metric.seriesType),
				label: metric.seriesType === MetricSeriesType.RIS_IDS_ACTIVE ? "Active RIS IDs" : "Inactive RIS IDs"
			};
		});
	};

	const buildRisIdDistributionSeries = (metrics: MetricSeries[]): RisIdDistributionSeries[] => {
		if (!metrics.length) return [];

		return metrics.map((metric: MetricSeries) => ({
			seriesType: metric.seriesType,
			value: Number(metric.dataPoints[metric.dataPoints.length - 1].value),
			color: colorScale(metric.seriesType),
			label: metric.seriesType === MetricSeriesType.RIS_IDS_ACTIVE ? "Active RIS IDs" : "Inactive RIS IDs"
		}));
	};
</script>

<MetricCardBase class={["gap-y-2", className]}>
	<MetricCardTitle title="Total RIS IDs" class="justify-between">
		<div class="flex gap-x-2">
			<!-- Info Button -->
			<button
				class={[
					"cursor-pointer transition-colors",
					informationDialogVisible && "text-accent",
					!informationDialogVisible && "text-muted-foreground hover:text-accent"
				]}
				onclick={(event: MouseEvent) => {
					event.stopPropagation();
					informationDialogVisible = !informationDialogVisible;
				}}
			>
				<Info size={18} />
			</button>

			{#await validatedPromise then metrics}
				<MetricTrend {metrics} />
			{/await}
		</div>

		<RecordedRisIdsInformationDialog bind:isVisible={informationDialogVisible} />
	</MetricCardTitle>

	{#await validatedPromise}
		<div class="bg-muted h-64 w-full animate-pulse rounded-md"></div>
	{:then metrics}
		<div class="flex flex-col items-center gap-y-6 lg:flex-row">
			<!-- Distribution of RIS IDs -->
			<div class="w-full lg:min-w-1/4">
				<PieChart
					data={buildRisIdDistributionSeries(metrics)}
					c={(data: RisIdDistributionSeries) => data.color}
					key="seriesType"
					value="value"
					range={[-90, 90]}
					outerRadius={100}
					innerRadius={-20}
					cornerRadius={4}
					padAngle={0.02}
					placement="center"
					props={{ group: { y: 40 }, pie: { motion: "spring" } }}
					padding={defaultChartPadding()}
					height={192}
				>
					{#snippet legend({ getLegendProps })}
						<Legend
							{...getLegendProps()}
							placement="bottom"
							orientation="vertical"
							variant="swatches"
							classes={{
								root: "pointer-events-auto",
								label: "font-[Roboto_Mono] text-xs font-medium whitespace-nowrap",
								swatch: "w-2.5 h-2.5",
								item: "gap-x-2 cursor-pointer"
							}}
						/>
					{/snippet}

					{#snippet aboveMarks({ visibleData })}
						{@const total = visibleData.reduce((acc: number, curr: RisIdDistributionSeries) => acc + curr.value, 0)}
						{@const formatter = new Intl.NumberFormat(undefined, { notation: "compact", maximumFractionDigits: 1 })}

						<Text
							value={formatter.format(total)}
							textAnchor="middle"
							verticalAnchor="middle"
							dy={4}
							class="text-xl font-bold"
						/>
						<Text value="IDs Total" textAnchor="middle" verticalAnchor="middle" dy={26} class="text-muted-foreground text-sm" />
					{/snippet}

					{#snippet tooltip({ context })}
						{@const total = context.flatData.reduce((acc: number, curr: RisIdDistributionSeries) => acc + curr.value, 0)}

						<Tooltip.Root
							class="bg-background/90! rounded-lg border border-white/10! p-3 shadow-xl backdrop-blur-md select-none"
						>
							{#snippet children({ data })}
								{@const percentage = ((data.value / total) * 100).toFixed(2)}

								<p class="text-sm font-bold" style:color={data.color}>{data.label}</p>

								<Separator class="my-1" />

								<div class="flex flex-row justify-between gap-x-4 text-xs">
									<span class="font-medium">{data.value.toLocaleString()}</span>
									<span class="text-emerald-400 italic">({percentage} %)</span>
								</div>
							{/snippet}
						</Tooltip.Root>
					{/snippet}
				</PieChart>
			</div>

			<!-- Line Chart for Historical Changes -->
			<div class="w-full lg:min-w-3/4">
				<LineChart
					data={buildRisIdLineChartSeries(metrics).flatMap((series: RisIdLineChartSeries) => series.data)}
					series={buildRisIdLineChartSeries(metrics)}
					x="date"
					y="value"
					padding={defaultChartPadding()}
					yDomain={null}
					tooltip={{ mode: "quadtree-x" }}
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
								classes={{
									tickLabel: "text-xs stroke-0 text-muted-foreground select-none"
								}}
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
								{@const total = payload.reduce((acc: number, curr) => acc + curr.value, 0)}

								<div class="flex flex-col gap-y-1">
									{#each payload as item}
										<div class="flex justify-between gap-x-4 text-xs">
											<div class="flex items-center gap-x-2">
												<div class="h-1.5 w-1.5 rounded-full" style:background-color={item.color}></div>
												<span class="text-muted-foreground text-left">{item.rawSeriesData?.label}</span>
											</div>

											<span class="text-text">{item.value.toLocaleString()}</span>
										</div>
									{/each}

									<Separator class="my-1" />

									<div class="flex flex-row justify-between gap-x-4 text-xs">
										<span>Total</span>
										<span>{total.toLocaleString()}</span>
									</div>
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
			</div>
		</div>
	{:catch}
		<MetricLoadingFailedWarning />
	{/await}
</MetricCardBase>
