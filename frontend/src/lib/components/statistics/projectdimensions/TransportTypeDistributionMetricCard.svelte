<script lang="ts">
	import {
		MetricSeriesType,
		TransportType,
		type MetricDataPoint,
		type MetricDataPointTransportTypeMetricDataPoint,
		type MetricSeries
	} from "@lib/api";
	import { scaleOrdinal } from "d3-scale";
	import { schemeTableau10 } from "d3-scale-chromatic";
	import { onMount } from "svelte";
	import type { ClassValue } from "svelte/elements";
	import MetricCardBase from "../MetricCardBase.svelte";
	import MetricCardTitle from "../MetricCardTitle.svelte";
	import MetricLoadingFailedWarning from "../MetricLoadingFailedWarning.svelte";
	import { Legend, PieChart, Tooltip } from "layerchart";

	interface TransportTypeSeries {
		transportType: TransportType;
		value: number;
		color: string;
	}

	interface KeyTitles {
		key: TransportType;
		title: string;
	}
	const keyTitles: KeyTitles[] = [
		{ key: TransportType.HIGH_SPEED_TRAIN, title: "High Speed" },
		{ key: TransportType.INTERCITY_TRAIN, title: "Intercity" },
		{ key: TransportType.INTER_REGIONAL_TRAIN, title: "Inter Regional" },
		{ key: TransportType.REGIONAL_TRAIN, title: "Regional" },
		{ key: TransportType.CITY_TRAIN, title: "Suburban" },
		{ key: TransportType.TRAM, title: "Tram" },
		{ key: TransportType.BUS, title: "Bus" },
		{ key: TransportType.UNKNOWN, title: "Unknown" }
	];

	interface Props {
		promise: Promise<MetricSeries[]>;
		class?: ClassValue;
	}

	let { promise, class: className }: Props = $props();

	onMount(async () => {
		const metrics = await promise;

		if (metrics.some((metric: MetricSeries) => metric.seriesType !== MetricSeriesType.TRANSPORT_TYPES_TOTAL))
			throw new Error("TransportTypeDistributionMetricCard received invalid metric series type.");
	});

	const colorScale = scaleOrdinal(schemeTableau10);
	const getChartSeries = (metrics: MetricSeries[]): TransportTypeSeries[] => {
		if (!metrics.length) return [];

		return metrics.flatMap((metric: MetricSeries) =>
			metric.dataPoints.map((dataPoint: MetricDataPoint) => {
				const transportTypeDataPoint = dataPoint as MetricDataPointTransportTypeMetricDataPoint;
				return {
					transportType: transportTypeDataPoint.transportType,
					value: Number(transportTypeDataPoint.value),
					color: colorScale(transportTypeDataPoint.transportType)
				};
			})
		);
	};
</script>

<MetricCardBase class={["gap-y-2", className]}>
	{#snippet head()}
		<MetricCardTitle title="Transport Type Distribution" />
	{/snippet}

	{#snippet body()}
		{#await promise}
			<div class="bg-muted h-64 w-full animate-pulse rounded-md"></div>
		{:then metrics}
			{#if !metrics.length}
				<MetricLoadingFailedWarning />
			{:else}
				<div class="h-64">
					<PieChart
						data={getChartSeries(metrics)}
						key="transportType"
						value="value"
						c={(data: TransportTypeSeries) => data.color}
						outerRadius={100}
						innerRadius={-20}
						cornerRadius={4}
						padAngle={0.02}
						placement="left"
						props={{ pie: { motion: "spring" } }}
					>
						{#snippet legend({ getLegendProps })}
							<Legend
								{...getLegendProps()}
								placement="right"
								orientation="vertical"
								variant="swatches"
								classes={{
									root: "pointer-events-auto",
									label: "font-[Roboto_Mono] text-xs font-medium whitespace-nowrap",
									swatch: "w-2.5 h-2.5",
									item: "gap-x-2 cursor-pointer",
									items: "flex gap-y-1"
								}}
								tickFormat={(seriesType: string) => {
									const keyTitle = keyTitles.find((keyTitle: KeyTitles) => keyTitle.key === seriesType)?.title ?? seriesType;
									return keyTitle;
								}}
							/>
						{/snippet}

						{#snippet tooltip({ context })}
							{@const total = context.flatData.reduce((acc: number, curr: TransportTypeSeries) => acc + curr.value, 0)}
							{@const formatter = new Intl.NumberFormat(undefined, { notation: "compact", maximumFractionDigits: 1 })}

							<Tooltip.Root class="bg-background/90! rounded-lg border border-white/10! p-3 shadow-xl backdrop-blur-md">
								{#snippet children({ data }: { data: TransportTypeSeries })}
									{@const title =
										keyTitles.find((keyTitle: KeyTitles) => keyTitle.key === data.transportType)?.title ?? data.transportType}

									{@const percentage = ((data.value / total) * 100).toFixed(2)}
									{@const formattedValue = formatter.format(data.value)}

									<p class="text-sm font-bold" style:color={data.color}>{title}</p>
									<Tooltip.Separator class="border-muted-foreground/20 my-1 border-t" />
									<div class="flex flex-row justify-between gap-x-4 text-xs">
										<span class="font-medium">{formattedValue}</span>
										<span class="text-emerald-400 italic">({percentage} %)</span>
									</div>
								{/snippet}
							</Tooltip.Root>
						{/snippet}
					</PieChart>
				</div>

				<p class="text-muted-foreground text-xs italic">
					The distribution of transport types have been estimated based on the selected end date.
				</p>
			{/if}
		{/await}
	{/snippet}
</MetricCardBase>
