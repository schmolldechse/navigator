<script module lang="ts">
	type JourneyTypePoint = {
		date: Date;
		[key: string]: Date | number;
	};

	type JourneyTypeSeries = {
		key: string;
		color: string;
		label: string;
	};
</script>

<script lang="ts">
	import {
		MetricSeriesType,
		type BaseMetricDataPoint,
		type BaseMetricDataPointTimestampCategoryDataPoint,
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
	const labelFor = (category: string) => category.charAt(0) + category.slice(1).toLowerCase().replaceAll("_", " ");

	const buildJourneyTypeChart = (metrics: MetricSeries[]): { data: JourneyTypePoint[]; series: JourneyTypeSeries[] } => {
		const metric = metrics.find(
			(metric: MetricSeries) => metric.seriesType === MetricSeriesType.JOURNEY_SERVICE_JOURNEY_TYPE_JOURNEYS
		);
		const dayMap = new Map<string, Record<string, number>>();
		const categories = new Set<string>();

		metric?.dataPoints.forEach((baseDataPoint: BaseMetricDataPoint) => {
			const dataPoint = baseDataPoint as BaseMetricDataPointTimestampCategoryDataPoint;
			if (!dataPoint.timestamp || !dataPoint.category) return;

			const day = DateTime.fromISO(dataPoint.timestamp).startOf("day").toISODate()!;
			dayMap.set(day, dayMap.get(day) ?? {});
			dayMap.get(day)![dataPoint.category] = (dayMap.get(day)![dataPoint.category] ?? 0) + Number(dataPoint.value);
			categories.add(dataPoint.category);
		});

		const orderedCategories = [...categories].sort();
		const data = [...dayMap.entries()]
			.map(([day, values]) => {
				const total = Object.values(values).reduce((sum: number, value: number) => sum + value, 0);
				const point: JourneyTypePoint = { date: DateTime.fromISO(day).toJSDate() };

				orderedCategories.forEach((category: string) => {
					point[category] = total === 0 ? 0 : ((values[category] ?? 0) / total) * 100;
				});

				return point;
			})
			.sort((a: JourneyTypePoint, b: JourneyTypePoint) => a.date.getTime() - b.date.getTime());

		return {
			data,
			series: orderedCategories.map((category: string) => ({
				key: category,
				color: colorScale(category),
				label: labelFor(category)
			}))
		};
	};
</script>

<MetricCardBase class={["gap-y-4", className]}>
	<MetricCardTitle title="Journey Type Share" />

	{#await promise}
		<div class="bg-muted h-64 w-full animate-pulse rounded-md"></div>
	{:then metrics}
		{@const chart = buildJourneyTypeChart(metrics)}

		<Chart
			data={chart.data}
			series={chart.series}
			x="date"
			seriesLayout="stack"
			bandPadding={0.24}
			padding={defaultChartPadding({ left: 48, bottom: 48 })}
			height={320}
			tooltipContext={{ mode: "band" }}
		>
			{#snippet axis()}
				<Axis
					placement="left"
					rule
					grid
					format={(value: number) => `${value.toLocaleString(undefined, { maximumFractionDigits: 0 })}%`}
				/>
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
									<span class="text-text">
										{Number(data[visibleSeries.key]).toLocaleString(undefined, { maximumFractionDigits: 1 })}%
									</span>
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
