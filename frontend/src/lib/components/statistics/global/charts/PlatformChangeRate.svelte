<script module lang="ts">
	type PlatformChangePoint = {
		date: Date;
		arrivals: number;
		departures: number;
	};

	type PlatformChangeSeries = {
		key: keyof Omit<PlatformChangePoint, "date">;
		color: string;
		label: string;
	};
</script>

<script lang="ts">
	import {
		MetricSeriesType,
		type BaseMetricDataPoint,
		type BaseMetricDataPointTimestampTransportTypeDataPoint,
		type MetricSeries
	} from "@lib/api";
	import DateTooltip from "@lib/components/layerchart/tooltips/DateTooltip.svelte";
	import MetricCardBase from "@lib/components/metric/MetricCardBase.svelte";
	import MetricCardTitle from "@lib/components/metric/MetricCardTitle.svelte";
	import MetricLoadingFailedWarning from "@lib/components/metric/MetricLoadingFailedWarning.svelte";
	import { scaleOrdinal } from "d3-scale";
	import { schemeTableau10 } from "d3-scale-chromatic";
	import { DateTime } from "luxon";
	import type { ClassValue } from "svelte/elements";
	import { Axis, Chart, ChartClipPath, defaultChartPadding, Highlight, Spline, Tooltip } from "layerchart";

	type Props = {
		promise: Promise<MetricSeries[]>;
		class?: ClassValue;
	};
	let { promise, class: className }: Props = $props();

	const colorScale = scaleOrdinal(schemeTableau10);
	const series: PlatformChangeSeries[] = [
		{ key: "arrivals", label: "Arrivals", color: colorScale("platform-arrivals") },
		{ key: "departures", label: "Departures", color: colorScale("platform-departures") }
	];

	const relevantTypes = [
		MetricSeriesType.HOURLY_GLOBAL_ARRIVAL_PLATFORM_CHANGES,
		MetricSeriesType.HOURLY_GLOBAL_ARRIVAL_DELAY_SAMPLE_COUNT,
		MetricSeriesType.HOURLY_GLOBAL_DEPARTURE_PLATFORM_CHANGES,
		MetricSeriesType.HOURLY_GLOBAL_DEPARTURE_DELAY_SAMPLE_COUNT
	];

	const buildLookup = (metrics: MetricSeries[]) => {
		type Lookup = Record<string, Record<string, Record<string, number>>>;
		const lookup: Lookup = {};

		metrics.forEach((metric: MetricSeries) => {
			if (!relevantTypes.includes(metric.seriesType)) return;

			metric.dataPoints.forEach((baseDataPoint: BaseMetricDataPoint) => {
				const dataPoint = baseDataPoint as BaseMetricDataPointTimestampTransportTypeDataPoint;
				if (!dataPoint.timestamp || !dataPoint.transportType) return;

				lookup[metric.seriesType] ??= {};
				lookup[metric.seriesType][dataPoint.timestamp] ??= {};
				lookup[metric.seriesType][dataPoint.timestamp][dataPoint.transportType] = Number(dataPoint.value);
			});
		});

		return lookup;
	};

	const sumAt = (lookup: Record<string, Record<string, Record<string, number>>>, type: MetricSeriesType, timestamp: string) =>
		Object.values(lookup[type]?.[timestamp] ?? {}).reduce((total: number, value: number) => total + value, 0);

	const buildPlatformChangeData = (metrics: MetricSeries[]): PlatformChangePoint[] => {
		const lookup = buildLookup(metrics);
		const timestamps = [
			...new Set(
				Object.values(lookup)
					.flatMap((timestampMap) => Object.keys(timestampMap))
					.sort((a: string, b: string) => DateTime.fromISO(a).toMillis() - DateTime.fromISO(b).toMillis())
			)
		];

		return timestamps.map((timestamp: string) => {
			const arrivalChanges = sumAt(lookup, MetricSeriesType.HOURLY_GLOBAL_ARRIVAL_PLATFORM_CHANGES, timestamp);
			const arrivalSamples = sumAt(lookup, MetricSeriesType.HOURLY_GLOBAL_ARRIVAL_DELAY_SAMPLE_COUNT, timestamp);
			const departureChanges = sumAt(lookup, MetricSeriesType.HOURLY_GLOBAL_DEPARTURE_PLATFORM_CHANGES, timestamp);
			const departureSamples = sumAt(lookup, MetricSeriesType.HOURLY_GLOBAL_DEPARTURE_DELAY_SAMPLE_COUNT, timestamp);

			return {
				date: DateTime.fromISO(timestamp).toJSDate(),
				arrivals: arrivalSamples === 0 ? 0 : (arrivalChanges / arrivalSamples) * 100,
				departures: departureSamples === 0 ? 0 : (departureChanges / departureSamples) * 100
			};
		});
	};
</script>

<MetricCardBase class={["gap-y-4", className]}>
	<MetricCardTitle title="Platform Change Rate" />

	{#await promise}
		<div class="bg-muted h-64 w-full animate-pulse rounded-md"></div>
	{:then metrics}
		{@const data = buildPlatformChangeData(metrics)}

		<Chart
			{data}
			{series}
			x="date"
			padding={defaultChartPadding({ left: 48 })}
			yDomain={null}
			brush
			height={320}
			tooltipContext={{ mode: "quadtree-x" }}
		>
			{#snippet axis()}
				<Axis placement="left" rule grid format={(value: number) => `${value.toLocaleString(undefined, { maximumFractionDigits: 0 })}%`} />
				<Axis placement="bottom" rule grid />
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
				<Tooltip.Root anchor="bottom" variant="none" class="bg-background/90! rounded-lg border border-white/10! px-2 py-0.5 shadow-xl backdrop-blur-md select-none">
					{#snippet children({ data })}
						<div class="flex flex-col gap-y-1">
							{#each context.series.visibleSeries as visibleSeries (visibleSeries.key)}
								<div class="flex items-center justify-between gap-x-4 text-xs">
									<div class="flex items-center gap-x-2">
										<div class="h-1.5 w-1.5 rounded-full" style:background-color={visibleSeries.color}></div>
										<span class="text-muted-foreground text-left">{visibleSeries.label}</span>
									</div>
									<span class="text-text">
										{Number(data[visibleSeries.key as keyof PlatformChangePoint]).toLocaleString(undefined, { maximumFractionDigits: 2 })}%
									</span>
								</div>
							{/each}
						</div>
					{/snippet}
				</Tooltip.Root>

				<DateTooltip
					{context}
					value={(data: PlatformChangePoint) => DateTime.fromJSDate(data.date).toLocaleString(DateTime.DATETIME_MED)}
				/>
			{/snippet}
		</Chart>
	{:catch}
		<MetricLoadingFailedWarning />
	{/await}
</MetricCardBase>
