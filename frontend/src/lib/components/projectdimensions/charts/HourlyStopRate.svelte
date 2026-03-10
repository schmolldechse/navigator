<script module lang="ts">
	import { MetricSeriesType } from "@lib/api";
	import { DateTime } from "luxon";

	type ValidType = {
		key: MetricSeriesType;
		title: string;
	};
	const validTypes: ValidType[] = [
		{ key: MetricSeriesType.HOURLY_GLOBAL_ARRIVALS, title: "Arrivals" },
		{ key: MetricSeriesType.HOURLY_GLOBAL_DEPARTURES, title: "Departures" }
	];

	const hoursArray: DateTime[] = Array.from({ length: 24 }, (_, index: number) =>
		DateTime.local().startOf("day").plus({ hour: index })
	);

	type HourlyStopSeries = {
		key: MetricSeriesType;
		color: string;
		label: string;
	};

	type HourlyStopDataPoint = {
		date: Date;
		[MetricSeriesType.HOURLY_GLOBAL_ARRIVALS]: number;
		[MetricSeriesType.HOURLY_GLOBAL_DEPARTURES]: number;
	};
</script>

<script lang="ts">
	import type { BaseMetricDataPoint, BaseMetricDataPointTimestampTransportTypeDataPoint, MetricSeries } from "@lib/api";
	import { scaleOrdinal } from "d3-scale";
	import { schemeTableau10 } from "d3-scale-chromatic";
	import type { ClassValue } from "svelte/elements";
	import { Axis, BarChart, Bars, ChartClipPath, defaultChartPadding, Highlight, Svg, Tooltip } from "layerchart";
	import MetricCardBase from "@lib/components/metric/MetricCardBase.svelte";
	import MetricCardTitle from "@lib/components/metric/MetricCardTitle.svelte";
	import MetricLoadingFailedWarning from "@lib/components/metric/MetricLoadingFailedWarning.svelte";

	type Props = {
		promise: Promise<MetricSeries[]>;
		class?: ClassValue;
	};
	let { promise, class: className }: Props = $props();

	const colorScale = scaleOrdinal(schemeTableau10);

	const hourlyStopHistogram = (metrics: MetricSeries[]): { series: HourlyStopSeries[]; data: HourlyStopDataPoint[] } => {
		const targetMetrics: MetricSeries[] = metrics.filter((metric: MetricSeries) =>
			validTypes.some((validType: ValidType) => validType.key === metric.seriesType)
		);

		const series: HourlyStopSeries[] = targetMetrics.map((metric: MetricSeries) => ({
			key: metric.seriesType,
			color: colorScale(metric.seriesType),
			label: validTypes.find((validType: ValidType) => validType.key === metric.seriesType)?.title ?? ""
		}));

		type GroupedSums = {
			[T in MetricSeriesType]?: Record<number, number>;
		};

		const groupedSums = targetMetrics.reduce<GroupedSums>((acc: GroupedSums, metric: MetricSeries) => {
			acc[metric.seriesType] ??= {};

			metric.dataPoints.forEach((baseDataPoint: BaseMetricDataPoint) => {
				const dataPointTimestamp = baseDataPoint as BaseMetricDataPointTimestampTransportTypeDataPoint;
				if (dataPointTimestamp.timestamp === undefined) return;

				const hour = DateTime.fromISO(dataPointTimestamp.timestamp).hour;

				// initialized before so it's safe to use non-null assertion
				const currentSeries = acc[metric.seriesType]!;
				currentSeries[hour] = (currentSeries[hour] ?? 0) + Number(dataPointTimestamp.value);
			});
			return acc;
		}, {} as GroupedSums);

		const data: HourlyStopDataPoint[] = hoursArray.map((hour: DateTime) => {
			const hourNum = hour.hour;
			const point: HourlyStopDataPoint = {
				date: hour.toJSDate(),
				[MetricSeriesType.HOURLY_GLOBAL_ARRIVALS]: 0,
				[MetricSeriesType.HOURLY_GLOBAL_DEPARTURES]: 0
			};

			targetMetrics.forEach((metric: MetricSeries) => {
				const seriesType = metric.seriesType as
					| MetricSeriesType.HOURLY_GLOBAL_ARRIVALS
					| MetricSeriesType.HOURLY_GLOBAL_DEPARTURES;
				point[seriesType] = groupedSums[seriesType]?.[hourNum] ?? 0;
			});

			return point;
		});

		return { series, data };
	};
</script>

<MetricCardBase class={["gap-y-4", className]}>
	{#snippet head()}
		<MetricCardTitle title="Hourly Stop Rate" />
	{/snippet}

	{#snippet body()}
		{#await promise}
			<div class="bg-muted h-64 w-full animate-pulse rounded-md"></div>
		{:then metrics}
			{@const { series, data } = hourlyStopHistogram(metrics)}

			<BarChart
				{data}
				{series}
				x="date"
				height={312}
				seriesLayout="stack"
				bandPadding={0.2}
				padding={defaultChartPadding({ left: 48, bottom: 48 })}
			>
				{#snippet children({ visibleSeries, getBarsProps, getHighlightProps })}
					<Svg>
						<Axis placement="left" rule grid classes={{ tickLabel: "text-xs stroke-0 text-muted-foreground select-none" }} />
						<Axis
							placement="bottom"
							rule
							classes={{
								tickLabel: "text-xs stroke-0 text-muted-foreground select-none max-sm:hidden"
							}}
							tickLabelProps={{
								rotate: 315,
								textAnchor: "end"
							}}
						/>

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
						class="bg-background/90! rounded-lg border border-white/10! px-2 py-0.5 shadow-xl backdrop-blur-md select-none"
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
									<div class="flex justify-between gap-x-4 text-xs">
										<div class="flex items-center gap-x-2">
											<div class="h-1.5 w-1.5 rounded-full" style:background-color={item.color}></div>
											<span class="text-muted-foreground text-left">{item.rawSeriesData?.label}</span>
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
		{:catch}
			<MetricLoadingFailedWarning />
		{/await}
	{/snippet}
</MetricCardBase>
