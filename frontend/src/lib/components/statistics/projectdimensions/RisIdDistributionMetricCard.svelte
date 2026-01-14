<script lang="ts">
	import {
		MetricSeriesType,
		TransportType,
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
	import { Legend, PieChart, Tooltip } from "layerchart";

	interface SeriesTypeTitles {
		seriesType: MetricSeriesType | TransportType;
		title: string;
	}

	const seriesTitles: SeriesTypeTitles[] = [
		{ seriesType: MetricSeriesType.RIS_IDS_ACTIVE, title: "Active RIS IDs" },
		{ seriesType: MetricSeriesType.RIS_IDS_INACTIVE, title: "Inactive RIS IDs" }
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
			throw new Error("RisIdDistributionMetricCard received invalid metric series type.");
	});

	const colorScale = scaleOrdinal(schemeTableau10);
	const getChartSeries = (metrics: MetricSeries[]) => {
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
				summary: metric.summary,
				values: dataPoints,
				key: metric.seriesType,
				color: colorScale(metric.seriesType)
			};
		});
	};
</script>

<MetricCardBase class={["gap-y-2", className]}>
	{#snippet children()}
		<p class="text-muted-foreground text-xs font-semibold tracking-wider uppercase">Distribution of RIS IDs</p>

		{#await promise}
			<div class="bg-muted h-6 w-16 animate-pulse rounded-full"></div>
		{:then metrics}
			{#if metrics === null || metrics.length === 0}
				<MetricLoadingFailedWarning />
			{:else}
				<!-- Pie Chart-->
				<div class="h-75 p-2">
					<PieChart
						data={getChartSeries(metrics).flatMap((series) => ({
							seriesType: series.key,
							value: Number(series.summary.endValue)
						}))}
						key="seriesType"
						value="value"
						c="seriesType"
						range={[-90, 90]}
						outerRadius={90}
						innerRadius={-15}
						cornerRadius={10}
						padAngle={0.05}
						placement="left"
					>
						{#snippet legend({ getLegendProps })}
							<Legend
								{...getLegendProps()}
								placement="bottom"
								orientation="horizontal"
								variant="swatches"
								classes={{
									root: "pointer-events-auto",
									items: "gap-y-1",
									label: "font-[Roboto_Mono] text-xs font-medium whitespace-nowrap",
									// dot symbol for the legend-item
									swatch: "w-2.5 h-2.5",
									item: "gap-x-2 cursor-pointer"
								}}
								tickFormat={(seriesType: string) => {
									const typeTitle = seriesTitles.find(
										(seriesTypeTitle: SeriesTypeTitles) => seriesTypeTitle.seriesType === (seriesType as MetricSeriesType)
									);
									return typeTitle ? typeTitle.title : seriesType;
								}}
							/>
						{/snippet}

						{#snippet tooltip({ context })}
							{@const total = context.flatData.reduce((acc, curr) => acc + curr.value, 0)}

							<Tooltip.Root class="bg-background/90! rounded-lg border border-white/10! p-3 shadow-xl backdrop-blur-md">
								{#snippet children({ data })}
									{@const title = seriesTitles.find(
										(seriesTypeTitle: SeriesTypeTitles) => seriesTypeTitle.seriesType === data.type
									)}

									{@const percentage = ((data.value / total) * 100).toFixed(2)}
									{@const formattedValue = data.value.toLocaleString()}

									<Tooltip.Header class="text-muted-foreground! mb-1 text-sm font-medium">
										{title ? title.title : data.type}
									</Tooltip.Header>

									<Tooltip.List>
										<div class="flex items-baseline gap-x-1">
											<Tooltip.Item class="text-text! text-xs font-bold" value={formattedValue} />
											<Tooltip.Item class="text-xs font-semibold text-emerald-400!" value={"(" + percentage + " %)"} />
										</div>
									</Tooltip.List>
								{/snippet}
							</Tooltip.Root>
						{/snippet}
					</PieChart>
				</div>
			{/if}
		{:catch}
			<MetricLoadingFailedWarning />
		{/await}
		<div class="flex flex-row gap-x-4"></div>
	{/snippet}
</MetricCardBase>
