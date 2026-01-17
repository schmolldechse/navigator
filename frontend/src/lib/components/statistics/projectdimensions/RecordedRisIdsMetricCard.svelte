<script lang="ts">
	import {
		MetricSeriesType,
		type MetricDataPoint,
		type MetricDataPointTimestampMetricDataPoint,
		type MetricSeries
	} from "@lib/api";
	import { scaleOrdinal } from "d3-scale";
	import { schemeTableau10 } from "d3-scale-chromatic";
	import { onMount } from "svelte";
	import type { ClassValue } from "svelte/elements";
	import MetricCardBase from "../MetricCardBase.svelte";
	import MetricLoadingFailedWarning from "../MetricLoadingFailedWarning.svelte";
	import { Axis, ChartClipPath, Highlight, Legend, LineChart, PieChart, Spline, Svg, Text, Tooltip } from "layerchart";
	import { DateTime } from "luxon";

	interface RisIdDistributionSeries {
		seriesType: MetricSeriesType;
		value: number;
		color: string;
	}

	interface RisIdLineChartSeries {
		key: string;
		data: { date: Date; value: number }[];
		color: string;
	}

	interface KeyTitles {
		key: MetricSeriesType;
		title: string;
	}

	const keyTitles: KeyTitles[] = [
		{ key: MetricSeriesType.RIS_IDS_ACTIVE, title: "Active RIS IDs" },
		{ key: MetricSeriesType.RIS_IDS_INACTIVE, title: "Inactive RIS IDs" }
	];

	interface Props {
		promise: Promise<MetricSeries[]>;
		class?: ClassValue;
	}

	let { promise, class: className }: Props = $props();

	onMount(async () => {
		const metrics = await promise;

		if (
			metrics.some(
				(metric: MetricSeries) =>
					metric.seriesType !== MetricSeriesType.RIS_IDS_ACTIVE && metric.seriesType !== MetricSeriesType.RIS_IDS_INACTIVE
			)
		)
			throw new Error("RecordedRisIdsMetricCard received invalid metric series type.");
	});

	const colorScale = scaleOrdinal(schemeTableau10);
	const getAreaChartSeries = (metrics: MetricSeries[]): RisIdLineChartSeries[] => {
		if (!metrics.length) return [];

		return metrics.map((metric: MetricSeries) => {
			const dataPoints = [...metric.dataPoints]
				.sort(
					(a: MetricDataPoint, b: MetricDataPoint) =>
						new Date((a as MetricDataPointTimestampMetricDataPoint).timestamp).getTime() -
						new Date((b as MetricDataPointTimestampMetricDataPoint).timestamp).getTime()
				)
				.map((dataPoint: MetricDataPoint) => ({
					date: new Date((dataPoint as MetricDataPointTimestampMetricDataPoint).timestamp),
					value: Number(dataPoint.value)
				}));

			return {
				key: metric.seriesType,
				data: dataPoints,
				color: colorScale(metric.seriesType)
			};
		});
	};

	const getArcSeries = (metrics: MetricSeries[]): RisIdDistributionSeries[] => {
		if (!metrics.length) return [];

		return metrics.map((metric: MetricSeries) => ({
			seriesType: metric.seriesType,
			value: Number(metric.summary.endValue),
			color: colorScale(metric.seriesType)
		}));
	};
</script>

<MetricCardBase class={["gap-y-2", className]}>
	{#snippet children()}
		<p class="text-muted-foreground text-xs font-semibold tracking-wider uppercase">Recorded RIS IDs</p>

		{#await promise}
			<div class="bg-muted h-64 w-full animate-pulse rounded-md"></div>
		{:then metrics}
			{#if metrics === null || metrics.length === 0}
				<MetricLoadingFailedWarning />
			{:else}
				<div class="flex flex-col items-center lg:flex-row">
					<!-- Distribution of RIS IDs -->
					<div class="h-48 w-full lg:w-1/3">
						<PieChart
							data={getArcSeries(metrics)}
							key="seriesType"
							value="value"
							c={(data: RisIdDistributionSeries) => data.color}
							range={[-90, 90]}
							outerRadius={120}
							innerRadius={-20}
							cornerRadius={4}
							padAngle={0.02}
							placement="center"
							props={{ group: { y: 40 }, pie: { motion: "spring" } }}
						>
							{#snippet legend({ getLegendProps })}
								<Legend
									{...getLegendProps()}
									placement="bottom"
									orientation="horizontal"
									variant="swatches"
									classes={{
										root: "pointer-events-auto",
										label: "font-[Roboto_Mono] text-xs font-medium whitespace-nowrap",
										swatch: "w-2.5 h-2.5",
										item: "gap-x-2 cursor-pointer"
									}}
									tickFormat={(seriesType: string) => {
										const keyTitle = keyTitles.find((keyTitle: KeyTitles) => keyTitle.key === seriesType)?.title ?? seriesType;
										return keyTitle;
									}}
								/>
							{/snippet}

							{#snippet aboveMarks({ visibleData, highlightKey })}
								{@const total = visibleData.reduce((acc: number, curr: RisIdDistributionSeries) => acc + curr.value, 0)}
								{@const formatter = new Intl.NumberFormat(undefined, { notation: "compact", maximumFractionDigits: 1 })}

								{#if highlightKey}
									{@const hoveredItem = visibleData.find((data: RisIdDistributionSeries) => data.seriesType === highlightKey)}
									{#if hoveredItem}
										{@const percentage = ((hoveredItem.value / total) * 100).toFixed(2)}
										<Text
											value={formatter.format(hoveredItem.value) + " / " + formatter.format(total)}
											textAnchor="middle"
											verticalAnchor="middle"
											dy={4}
											class="text-base"
										/>
										<Text
											value={percentage + " %"}
											textAnchor="middle"
											verticalAnchor="middle"
											dy={26}
											class="text-muted-foreground text-sm"
										/>
									{/if}
								{:else}
									<Text value={formatter.format(total)} textAnchor="middle" verticalAnchor="middle" dy={4} class="text-2xl" />
									<Text
										value="Total"
										textAnchor="middle"
										verticalAnchor="middle"
										dy={26}
										class="text-muted-foreground text-sm"
									/>
								{/if}
							{/snippet}

							{#snippet tooltip({ context })}
								{@const total = context.flatData.reduce((acc: number, curr: RisIdDistributionSeries) => acc + curr.value, 0)}
								{@const formatter = new Intl.NumberFormat(undefined, { notation: "compact", maximumFractionDigits: 1 })}

								<Tooltip.Root class="bg-background/90! rounded-lg border border-white/10! p-3 shadow-xl backdrop-blur-md">
									{#snippet children({ data })}
										{@const title =
											keyTitles.find((keyTitle: KeyTitles) => keyTitle.key === data.seriesType)?.title ?? data.seriesType}

										{@const percentage = ((data.value / total) * 100).toFixed(2)}
										{@const formattedValue = formatter.format(data.value)}

										<p class="text-base font-bold" style:color={data.color}>{title}</p>
										<Tooltip.Separator class="border-muted-foreground/20 my-1 border-t" />
										<div class="flex flex-row justify-between gap-x-4">
											<span class="text-sm font-medium">{formattedValue}</span>
											<span class="text-xs text-emerald-400 italic">({percentage} %)</span>
										</div>
									{/snippet}
								</Tooltip.Root>
							{/snippet}
						</PieChart>
					</div>

					<!-- Area Chart for Historical Changes -->
					<div class="h-64 w-full lg:min-w-2/3">
						<LineChart
							data={getAreaChartSeries(metrics).flatMap((series: RisIdLineChartSeries) => series.data)}
							series={getAreaChartSeries(metrics)}
							x="date"
							y="value"
							padding={{ left: 64, top: 16, bottom: 12 }}
							yDomain={null}
							tooltip={{ mode: "quadtree-x" }}
							brush
						>
							{#snippet children({ context, visibleSeries, getSplineProps, getHighlightProps })}
								<Svg>
									<Axis
										placement="left"
										grid
										rule
										classes={{
											tickLabel: "text-xs stroke-0 text-muted-foreground",
											rule: "stroke-muted-foreground/20"
										}}
									/>
									<Axis
										placement="bottom"
										rule
										classes={{ tickLabel: "text-xs stroke-0 text-muted-foreground", rule: "stroke-0" }}
									/>

									<ChartClipPath>
										{#each visibleSeries as series, i (series.key)}
											<Spline {...getSplineProps(series, i)} class="stroke-2" />
											<Highlight {...getHighlightProps(series, i)} points lines={{ class: "stroke-muted-foreground/30" }} />
										{/each}
									</ChartClipPath>
								</Svg>

								<!-- Data Tooltip -->
								<Tooltip.Root
									anchor="bottom"
									contained="container"
									class="bg-background/90! rounded-lg border border-white/10! p-3 shadow-xl backdrop-blur-md"
								>
									{#snippet children({ payload })}
										{@const formatter = new Intl.NumberFormat(undefined, { notation: "compact", maximumFractionDigits: 1 })}
										{@const total = payload.reduce((acc: number, curr) => acc + curr.value, 0)}

										<div class="flex flex-col gap-y-1">
											{#each [...payload].reverse() as item}
												{@const title =
													keyTitles?.find((title: KeyTitles) => title.key === item.rawSeriesData?.key)?.title ??
													item.rawSeriesData?.key}
												<div class="flex justify-between gap-x-4">
													<div class="flex items-center gap-x-2">
														<div class="h-1.5 w-1.5 rounded-full" style:background-color={item.color}></div>
														<span class="text-muted-foreground text-left">{title}</span>
													</div>

													<span class="text-text">{formatter.format(item.value)}</span>
												</div>
											{/each}

											<Tooltip.Separator class="border-muted-foreground/20 my-1 border-t" />

											<div class="flex flex-row justify-between gap-x-4">
												<span>Total</span>
												<span>{formatter.format(total)}</span>
											</div>
										</div>
									{/snippet}
								</Tooltip.Root>

								<!-- Date Tooltip on x-Axis -->
								<Tooltip.Root
									x="pointer"
									y={context.height + context.padding.bottom + 8}
									anchor="top"
									variant="none"
									class="bg-background/90! rounded-lg border border-white/10! px-2 py-0.5 shadow-xl backdrop-blur-md"
								>
									{#snippet children({ data })}
										<Tooltip.Item class="text-text text-xs">
											{DateTime.fromJSDate(data.date).toLocaleString(DateTime.DATETIME_MED)}
										</Tooltip.Item>
									{/snippet}
								</Tooltip.Root>
							{/snippet}
						</LineChart>
					</div>
				</div>
			{/if}
		{:catch}
			<MetricLoadingFailedWarning />
		{/await}
	{/snippet}
</MetricCardBase>
