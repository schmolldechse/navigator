<script lang="ts" module>
	const hoursArray: DateTime[] = Array.from({ length: 24 }, (_, index: number) =>
		DateTime.local().startOf("day").plus({ hour: index })
	);

	// 24H Stop Histogram
	type HourlyStopSeries = {
		key: MetricSeriesType.GLOBAL_STOP_ARRIVALS_COUNT | MetricSeriesType.GLOBAL_STOP_DEPARTURES_COUNT;
		color: string;
	};

	type HourlyStopDataPoint = {
		date: Date;
		[MetricSeriesType.GLOBAL_STOP_ARRIVALS_COUNT]: number;
		[MetricSeriesType.GLOBAL_STOP_DEPARTURES_COUNT]: number;
	};

	// Key Titles
	interface KeyTitles {
		key: MetricSeriesType;
		title: string;
	}
	const keyTitles: KeyTitles[] = [
		{ key: MetricSeriesType.GLOBAL_STOP_ARRIVALS_COUNT, title: "Arrival stops" },
		{ key: MetricSeriesType.GLOBAL_STOP_DEPARTURES_COUNT, title: "Departure stops" }
	];
</script>

<script lang="ts">
	import {
		MetricSeriesType,
		type BaseMetricDataPoint,
		type BaseMetricDataPointTimestampTransportTypeMetricDataPoint,
		type MetricSeries
	} from "@lib/api";
	import { scaleOrdinal } from "d3-scale";
	import { schemeTableau10 } from "d3-scale-chromatic";
	import { onMount } from "svelte";
	import type { ClassValue } from "svelte/elements";
	import MetricCardBase from "../MetricCardBase.svelte";
	import MetricCardTitle from "../MetricCardTitle.svelte";
	import MetricLoadingFailedWarning from "../MetricLoadingFailedWarning.svelte";
	import { DateTime } from "luxon";
	import { Axis, BarChart, Bars, ChartClipPath, Highlight, Svg, Tooltip } from "layerchart";

	type Props = {
		promise: Promise<MetricSeries[]>;
		class?: ClassValue;
	};
	let { promise, class: className }: Props = $props();

	onMount(async () => {
		const metrics = await promise;

		const validTypes = [
			MetricSeriesType.GLOBAL_STOP_ARRIVALS_COUNT,
			MetricSeriesType.GLOBAL_STOP_ARRIVAL_CANCELLATIONS,
			MetricSeriesType.GLOBAL_STOP_ARRIVAL_DELAYS,
			MetricSeriesType.GLOBAL_STOP_DEPARTURES_COUNT,
			MetricSeriesType.GLOBAL_STOP_DEPARTURE_CANCELLATIONS,
			MetricSeriesType.GLOBAL_STOP_DEPARTURE_DELAYS
		];

		if (metrics.some((metric: MetricSeries) => !validTypes.includes(metric.seriesType)))
			throw new Error("GlobalStopSummaryMetricCard received invalid metric series type.");
	});

	const colorScale = scaleOrdinal(schemeTableau10);

	const hourlyStopHistogram = (metrics: MetricSeries[]): { series: HourlyStopSeries[]; chartData: HourlyStopDataPoint[] } => {
		const targetTypes = [MetricSeriesType.GLOBAL_STOP_ARRIVALS_COUNT, MetricSeriesType.GLOBAL_STOP_DEPARTURES_COUNT];
		const targetMetrics: MetricSeries[] = metrics.filter((metric: MetricSeries) => targetTypes.includes(metric.seriesType));

		const series: HourlyStopSeries[] = targetMetrics.map((metric: MetricSeries) => ({
			key: metric.seriesType as MetricSeriesType.GLOBAL_STOP_ARRIVALS_COUNT | MetricSeriesType.GLOBAL_STOP_DEPARTURES_COUNT,
			color: colorScale(metric.seriesType)
		}));

		type GroupedSums = {
			[T in MetricSeriesType]?: Record<number, number>;
		};

		const groupedSums = targetMetrics.reduce<GroupedSums>((acc: GroupedSums, metric: MetricSeries) => {
			acc[metric.seriesType] ??= {};

			metric.dataPoints.forEach((baseDataPoint: BaseMetricDataPoint) => {
				const dataPointTimestamp = baseDataPoint as BaseMetricDataPointTimestampTransportTypeMetricDataPoint;
				if (dataPointTimestamp.timestamp === undefined) return;

				const hour = DateTime.fromISO(dataPointTimestamp.timestamp).hour;

				// initialized before so it's safe to use non-null assertion
				const currentSeries = acc[metric.seriesType]!;
				currentSeries[hour] = (currentSeries[hour] ?? 0) + Number(dataPointTimestamp.value);
			});
			return acc;
		}, {} as GroupedSums);

		const chartData: HourlyStopDataPoint[] = hoursArray.map((hour: DateTime) => {
			const hourNum = hour.hour;
			const point: HourlyStopDataPoint = {
				date: hour.toJSDate(),
				[MetricSeriesType.GLOBAL_STOP_ARRIVALS_COUNT]: 0,
				[MetricSeriesType.GLOBAL_STOP_DEPARTURES_COUNT]: 0
			};

			targetMetrics.forEach((metric: MetricSeries) => {
				const seriesType = metric.seriesType as
					| MetricSeriesType.GLOBAL_STOP_ARRIVALS_COUNT
					| MetricSeriesType.GLOBAL_STOP_DEPARTURES_COUNT;
				point[seriesType] = groupedSums[seriesType]?.[hourNum] ?? 0;
			});

			return point;
		});

		return { series, chartData };
	};
</script>

<MetricCardBase class={["gap-y-2", className]}>
	{#snippet head()}
		<MetricCardTitle title="Global Stop Summaries" />
	{/snippet}

	{#snippet body()}
		{#await promise}
			<div class="bg-muted h-64 w-full animate-pulse rounded-md"></div>
		{:then metrics}
			{#if !metrics.length}
				<MetricLoadingFailedWarning />
			{:else}
				{@const { series, chartData } = hourlyStopHistogram(metrics)}
				<BarChart data={chartData} {series} x="date" height={256} seriesLayout="stack" bandPadding={0.2}>
					{#snippet children({ visibleSeries, getBarsProps, getHighlightProps })}
						<Svg>
							<Axis
								placement="bottom"
								rule
								classes={{ tickLabel: "text-xs stroke-0 text-muted-foreground max-sm:hidden", rule: "stroke-0" }}
								tickLabelProps={{
									rotate: 315,
									textAnchor: "end"
								}}
							/>
							<Axis placement="left" rule classes={{ tickLabel: "text-xs stroke-0 text-muted-foreground", rule: "stroke-0" }} />

							<ChartClipPath>
								{#each visibleSeries as series, i (series.key)}
									<Bars {...getBarsProps(series, i)} />
								{/each}
							</ChartClipPath>

							<ChartClipPath full>
								<Highlight {...getHighlightProps()} />
							</ChartClipPath>
						</Svg>

						<!-- Data Tooltip -->
						<Tooltip.Root
							anchor="bottom"
							contained="container"
							class="bg-background/90! rounded-lg border border-white/10! px-2 py-0.5 shadow-xl backdrop-blur-md"
						>
							{#snippet children({ data, payload })}
								{@const total = payload.reduce((acc: number, current) => acc + current.value, 0)}

								<Tooltip.Header>
									{@const startDate = DateTime.fromJSDate(data.date)}
									{@const endDate = startDate.plus({ hour: 1 })}

									<span class="text-text text-xs">
										{startDate.toLocaleString(DateTime.TIME_SIMPLE)} – {endDate.toLocaleString(DateTime.TIME_SIMPLE)}
									</span>
								</Tooltip.Header>

								<div class="flex flex-col gap-y-1">
									{#each [...payload].reverse() as item}
										{@const title = keyTitles?.find((title: KeyTitles) => title.key === item.key)!.title}
										<div class="flex justify-between gap-x-4 text-xs">
											<div class="flex items-center gap-x-2">
												<div class="h-1.5 w-1.5 rounded-full" style:background-color={item.color}></div>
												<span class="text-muted-foreground text-left">{title}</span>
											</div>

											<span class="text-text">{item.value.toLocaleString()}</span>
										</div>
									{/each}

									<Tooltip.Separator class="border-muted-foreground/20 my-1 border-t" />

									<div class="flex flex-row justify-between gap-x-4 text-xs">
										<span>Total</span>
										<span>{total.toLocaleString()}</span>
									</div>
								</div>
							{/snippet}
						</Tooltip.Root>
					{/snippet}
				</BarChart>
			{/if}
		{/await}
	{/snippet}
</MetricCardBase>
