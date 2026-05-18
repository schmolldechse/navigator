<script module lang="ts">
	type PunctualityPoint = {
		date: Date;
		arrivals: number;
		departures: number;
	};

	type PunctualitySeries = {
		key: keyof Omit<PunctualityPoint, "date">;
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
	const series: PunctualitySeries[] = [
		{ key: "arrivals", label: "Arrivals", color: colorScale("punctual-arrivals") },
		{ key: "departures", label: "Departures", color: colorScale("punctual-departures") }
	];

	const relevantTypes = [
		MetricSeriesType.HOURLY_GLOBAL_ARRIVAL_PUNCTUAL_COUNT,
		MetricSeriesType.HOURLY_GLOBAL_ARRIVAL_DELAY_SAMPLE_COUNT,
		MetricSeriesType.HOURLY_GLOBAL_DEPARTURE_PUNCTUAL_COUNT,
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

	const buildPunctualityData = (metrics: MetricSeries[]): PunctualityPoint[] => {
		const lookup = buildLookup(metrics);
		const timestamps = [
			...new Set(
				Object.values(lookup)
					.flatMap((timestampMap) => Object.keys(timestampMap))
					.sort((a: string, b: string) => DateTime.fromISO(a).toMillis() - DateTime.fromISO(b).toMillis())
			)
		];

		return timestamps.map((timestamp: string) => {
			const arrivalPunctual = sumAt(lookup, MetricSeriesType.HOURLY_GLOBAL_ARRIVAL_PUNCTUAL_COUNT, timestamp);
			const arrivalSamples = sumAt(lookup, MetricSeriesType.HOURLY_GLOBAL_ARRIVAL_DELAY_SAMPLE_COUNT, timestamp);
			const departurePunctual = sumAt(lookup, MetricSeriesType.HOURLY_GLOBAL_DEPARTURE_PUNCTUAL_COUNT, timestamp);
			const departureSamples = sumAt(lookup, MetricSeriesType.HOURLY_GLOBAL_DEPARTURE_DELAY_SAMPLE_COUNT, timestamp);

			return {
				date: DateTime.fromISO(timestamp).toJSDate(),
				arrivals: arrivalSamples === 0 ? 0 : (arrivalPunctual / arrivalSamples) * 100,
				departures: departureSamples === 0 ? 0 : (departurePunctual / departureSamples) * 100
			};
		});
	};
</script>

<MetricCardBase class={["gap-y-4", className]}>
	<MetricCardTitle title="Punctuality Rate" />

	{#await promise}
		<div class="bg-muted h-64 w-full animate-pulse rounded-md"></div>
	{:then metrics}
		{@const data = buildPunctualityData(metrics)}

		<Chart
			{data}
			{series}
			x="date"
			padding={defaultChartPadding({ left: 48 })}
			yDomain={[0, 100]}
			brush
			height={320}
			tooltipContext={{ mode: "quadtree-x" }}
		>
			{#snippet axis()}
				<Axis
					placement="left"
					rule
					grid
					format={(value: number) => `${value.toLocaleString(undefined, { maximumFractionDigits: 0 })}%`}
				/>
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
				<Tooltip.Root
					anchor="bottom"
					variant="none"
					class="bg-background/90! rounded-lg border border-white/10! px-2 py-0.5 shadow-xl backdrop-blur-md select-none"
				>
					{#snippet children({ data })}
						<div class="flex flex-col gap-y-1">
							{#each context.series.visibleSeries as visibleSeries (visibleSeries.key)}
								<div class="flex items-center justify-between gap-x-4 text-xs">
									<div class="flex items-center gap-x-2">
										<div class="h-1.5 w-1.5 rounded-full" style:background-color={visibleSeries.color}></div>
										<span class="text-muted-foreground text-left">{visibleSeries.label}</span>
									</div>
									<span class="text-text">
										{Number(data[visibleSeries.key as keyof PunctualityPoint]).toLocaleString(undefined, {
											maximumFractionDigits: 1
										})}%
									</span>
								</div>
							{/each}
						</div>
					{/snippet}
				</Tooltip.Root>

				<DateTooltip
					{context}
					value={(data: PunctualityPoint) => DateTime.fromJSDate(data.date).toLocaleString(DateTime.DATETIME_MED)}
				/>
			{/snippet}
		</Chart>
	{:catch}
		<MetricLoadingFailedWarning />
	{/await}
</MetricCardBase>
