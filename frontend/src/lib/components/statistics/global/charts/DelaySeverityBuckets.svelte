<script module lang="ts">
	type DelayBucketKey = "minor" | "major" | "severe";

	type DelayBucketSeries = {
		key: DelayBucketKey;
		color: string;
		label: string;
	};

	type DelayBucketPoint = {
		date: Date;
		minor: number;
		major: number;
		severe: number;
	};
</script>

<script lang="ts">
	import {
		MetricSeriesType,
		type BaseMetricDataPoint,
		type BaseMetricDataPointTimestampTransportTypeDataPoint,
		type MetricSeries
	} from "@lib/api";
	import MetricCardBase from "@lib/components/metric/MetricCardBase.svelte";
	import MetricCardTitle from "@lib/components/metric/MetricCardTitle.svelte";
	import MetricLoadingFailedWarning from "@lib/components/metric/MetricLoadingFailedWarning.svelte";
	import { scaleOrdinal } from "d3-scale";
	import { schemeTableau10 } from "d3-scale-chromatic";
	import { DateTime } from "luxon";
	import type { ClassValue } from "svelte/elements";
	import { Axis, Bars, Chart, ChartClipPath, defaultChartPadding, Highlight, Tooltip } from "layerchart";

	type Props = {
		promise: Promise<MetricSeries[]>;
		class?: ClassValue;
	};
	let { promise, class: className }: Props = $props();

	const colorScale = scaleOrdinal(schemeTableau10);
	const series: DelayBucketSeries[] = [
		{ key: "minor", label: "6-14 min", color: colorScale("minor") },
		{ key: "major", label: "15-59 min", color: colorScale("major") },
		{ key: "severe", label: "60+ min", color: colorScale("severe") }
	];

	const bucketTypes: Record<DelayBucketKey, MetricSeriesType[]> = {
		minor: [
			MetricSeriesType.HOURLY_GLOBAL_ARRIVAL_DELAY_MINOR_COUNT,
			MetricSeriesType.HOURLY_GLOBAL_DEPARTURE_DELAY_MINOR_COUNT
		],
		major: [
			MetricSeriesType.HOURLY_GLOBAL_ARRIVAL_DELAY_MAJOR_COUNT,
			MetricSeriesType.HOURLY_GLOBAL_DEPARTURE_DELAY_MAJOR_COUNT
		],
		severe: [
			MetricSeriesType.HOURLY_GLOBAL_ARRIVAL_DELAY_SEVERE_COUNT,
			MetricSeriesType.HOURLY_GLOBAL_DEPARTURE_DELAY_SEVERE_COUNT
		]
	};

	const buildBucketData = (metrics: MetricSeries[]): DelayBucketPoint[] => {
		const byDay = new Map<string, DelayBucketPoint>();

		metrics.forEach((metric: MetricSeries) => {
			const bucket = (Object.keys(bucketTypes) as DelayBucketKey[]).find((key: DelayBucketKey) =>
				bucketTypes[key].includes(metric.seriesType)
			);
			if (!bucket) return;

			metric.dataPoints.forEach((baseDataPoint: BaseMetricDataPoint) => {
				const dataPoint = baseDataPoint as BaseMetricDataPointTimestampTransportTypeDataPoint;
				if (!dataPoint.timestamp) return;

				const day = DateTime.fromISO(dataPoint.timestamp).startOf("day");
				const key = day.toISODate()!;
				const point = byDay.get(key) ?? { date: day.toJSDate(), minor: 0, major: 0, severe: 0 };
				point[bucket] += Number(dataPoint.value);
				byDay.set(key, point);
			});
		});

		return [...byDay.values()].sort((a: DelayBucketPoint, b: DelayBucketPoint) => a.date.getTime() - b.date.getTime());
	};
</script>

<MetricCardBase class={["gap-y-4", className]}>
	<MetricCardTitle title="Delay Severity Buckets" />

	{#await promise}
		<div class="bg-muted h-64 w-full animate-pulse rounded-md"></div>
	{:then metrics}
		{@const data = buildBucketData(metrics)}

		<Chart
			{data}
			{series}
			x="date"
			seriesLayout="stack"
			bandPadding={0.25}
			padding={defaultChartPadding({ left: 48, bottom: 48 })}
			height={320}
			tooltipContext={{ mode: "band" }}
		>
			{#snippet axis()}
				<Axis placement="left" rule grid />
				<Axis placement="bottom" rule />
			{/snippet}

			{#snippet marks({ context })}
				<ChartClipPath>
					{#each context.series.visibleSeries as visibleSeries (visibleSeries.key)}
						<Bars seriesKey={visibleSeries.key} fill={visibleSeries.color} />
					{/each}
				</ChartClipPath>
			{/snippet}

			{#snippet highlight()}
				<Highlight area />
			{/snippet}

			{#snippet tooltip({ context })}
				<Tooltip.Root
					anchor="bottom"
					variant="none"
					class="bg-background/90! rounded-lg border border-white/10! px-2 py-0.5 shadow-xl backdrop-blur-md select-none"
				>
					{#snippet children({ data })}
						<Tooltip.Header>
							<span class="text-text text-xs">{DateTime.fromJSDate(data.date).toLocaleString(DateTime.DATE_MED)}</span>
						</Tooltip.Header>

						<div class="flex flex-col gap-y-1">
							{#each [...context.series.visibleSeries].reverse() as visibleSeries (visibleSeries.key)}
								<div class="flex items-center justify-between gap-x-4 text-xs">
									<div class="flex items-center gap-x-2">
										<div class="h-1.5 w-1.5 rounded-full" style:background-color={visibleSeries.color}></div>
										<span class="text-muted-foreground text-left">{visibleSeries.label}</span>
									</div>
									<span class="text-text">{Number(data[visibleSeries.key as DelayBucketKey]).toLocaleString()}</span>
								</div>
							{/each}
						</div>
					{/snippet}
				</Tooltip.Root>
			{/snippet}
		</Chart>
	{:catch}
		<MetricLoadingFailedWarning />
	{/await}
</MetricCardBase>
