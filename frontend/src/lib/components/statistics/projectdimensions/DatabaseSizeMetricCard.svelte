<script lang="ts">
	import {
		MetricSeriesType,
		MetricUnit,
		type MetricDataPoint,
		type MetricDataPointTimestampMetricDataPoint,
		type MetricSeries
	} from "@lib/api";
	import MetricCardBase from "../MetricCardBase.svelte";
	import type { ClassValue } from "svelte/elements";
	import { onMount } from "svelte";
	import { scaleOrdinal } from "d3-scale";
	import { schemeTableau10 } from "d3-scale-chromatic";
	import MetricLoadingFailedWarning from "../MetricLoadingFailedWarning.svelte";
	import { Area, AreaChart, Axis, chartDataArray, Highlight, LinearGradient, Svg, Tooltip } from "layerchart";
	import { formatBytes, getUnit } from "@lib/util/bytes";
	import { DateTime } from "luxon";

	interface LineChartSeries {
		key: string;
		values: { date: Date; value: number }[];
		color: string;
	}

	interface SeriesTypeTitles {
		seriesType: MetricSeriesType;
		title: string;
	}

	const seriesTitles: SeriesTypeTitles[] = [{ seriesType: MetricSeriesType.DATABASE_SIZE, title: "Database Size" }];

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
	const getChartSeries = (metrics: MetricSeries[]): LineChartSeries[] => {
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
					value: dataPoint.value as number
				}));

			return {
				values: dataPoints,
				key: metric.seriesType,
				color: colorScale(metric.seriesType)
			};
		});
	};
</script>

<MetricCardBase class={["gap-y-2", className]}>
	{#snippet children()}
		<p class="text-muted-foreground text-xs font-semibold tracking-wider uppercase">Database Size</p>

		{#await promise}
			<div class="bg-muted h-6 w-16 animate-pulse rounded-full"></div>
		{:then metrics}
			{#if metrics === null || metrics.length === 0}
				<MetricLoadingFailedWarning />
			{:else}
				<div class="h-64">
					<AreaChart
						data={getChartSeries(metrics).flatMap((series) => series.values)}
						series={getChartSeries(metrics)}
						x="date"
						y="value"
						padding={{ left: 64, top: 16, bottom: 12 }}
						yNice
						yDomain={null}
						tooltip={{ mode: "quadtree-x" }}
					>
						{#snippet children({ context })}
							<Svg>
								<Highlight points lines={{ class: "stroke-muted-foreground/30" }} />
								<Axis
									placement="left"
									grid
									rule
									format={(value) => {
										const formattedValue = Math.abs(value);
										return (value < 0 ? "-" : "") + formatBytes(formattedValue, true) + " " + getUnit(formattedValue, true);
									}}
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

								{#each chartDataArray(getChartSeries(metrics)) as seriesData}
									<LinearGradient
										vertical
										stops={[
											[0, seriesData.color],
											[1, "transparent"]
										]}
									>
										{#snippet children({ gradient })}
											<Area
												data={seriesData.data}
												fill={gradient}
												fillOpacity={0.5}
												line={{ stroke: seriesData.color, strokeWidth: 2 }}
											/>
										{/snippet}
									</LinearGradient>
								{/each}
							</Svg>

							<!-- Data Tooltip -->
							<Tooltip.Root
								anchor="bottom"
								contained="container"
								class="bg-background/90! rounded-lg border border-white/10! p-3 shadow-xl backdrop-blur-md"
							>
								{#snippet children({ payload })}
									<div class="flex flex-col gap-y-1">
										{#each [...payload].reverse() as item}
											{@const title =
												seriesTitles?.find((title: SeriesTypeTitles) => title.seriesType === item.rawSeriesData?.key)?.title ??
												item.rawSeriesData?.key}
											<div class="flex justify-between gap-x-4">
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
					</AreaChart>
				</div>
			{/if}
		{/await}
	{/snippet}
</MetricCardBase>
