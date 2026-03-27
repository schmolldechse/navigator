<script module lang="ts">
	type TransportTypeSeries = {
		transportType: TransportType;
		value: number;
		color: string;
		label: string;
	};

	const transportTypeTitles: Partial<Record<TransportType, string>> = {
		[TransportType.HIGH_SPEED_TRAIN]: "High Speed",
		[TransportType.INTERCITY_TRAIN]: "Intercity",
		[TransportType.INTER_REGIONAL_TRAIN]: "Inter Regional",
		[TransportType.REGIONAL_TRAIN]: "Regional",
		[TransportType.CITY_TRAIN]: "Suburban",
		[TransportType.TRAM]: "Tram",
		[TransportType.BUS]: "Bus",
		[TransportType.UNKNOWN]: "Unknown"
	};
</script>

<script lang="ts">
	import {
		MetricSeriesType,
		TransportType,
		type BaseMetricDataPoint,
		type BaseMetricDataPointTransportTypeDataPoint,
		type MetricSeries
	} from "@lib/api";
	import { scaleOrdinal } from "d3-scale";
	import { schemeTableau10 } from "d3-scale-chromatic";
	import type { ClassValue } from "svelte/elements";
	import { defaultChartPadding, Legend, PieChart, Tooltip } from "layerchart";
	import MetricCardBase from "@lib/components/metric/MetricCardBase.svelte";
	import MetricCardTitle from "@lib/components/metric/MetricCardTitle.svelte";
	import MetricLoadingFailedWarning from "@lib/components/metric/MetricLoadingFailedWarning.svelte";
	import Separator from "@lib/components/interactable/Separator.svelte";

	type Props = {
		promise: Promise<MetricSeries>;
		class?: ClassValue;
	};
	let { promise, class: className }: Props = $props();

	let validatedPromise = $derived(
		promise.then((metric: MetricSeries) => {
			if (metric.seriesType !== MetricSeriesType.TRANSPORT_TYPES_TOTAL)
				throw new Error("TransportTypeDistributionMetricCard received invalid metric series type.");
			return metric;
		})
	);

	const colorScale = scaleOrdinal(schemeTableau10);

	const buildChartSeries = (metric: MetricSeries): TransportTypeSeries[] => {
		if (!metric.dataPoints.length) return [];

		return metric.dataPoints
			.map((baseDataPoint: BaseMetricDataPoint) => {
				const dataPoint = baseDataPoint as BaseMetricDataPointTransportTypeDataPoint;
				if (!dataPoint.transportType || typeof dataPoint.value !== "number") {
					console.warn("Invalid data point in Transport Type metric series:", baseDataPoint);
					return null;
				}

				return {
					transportType: dataPoint.transportType,
					value: Number(dataPoint.value),
					color: colorScale(dataPoint.transportType),
					label: transportTypeTitles[dataPoint.transportType] ?? dataPoint.transportType
				};
			})
			.filter((v): v is TransportTypeSeries => !!v);
	};
</script>

<MetricCardBase class={["gap-y-2", className]}>
	<MetricCardTitle title="Transport Type Distribution" />

	{#await promise}
		<div class="bg-muted h-64 w-full animate-pulse rounded-md"></div>
	{:then metrics}
		<PieChart
			data={buildChartSeries(metrics)}
			key="transportType"
			value="value"
			c={(data: TransportTypeSeries) => data.color}
			outerRadius={100}
			innerRadius={-20}
			cornerRadius={4}
			padAngle={0.02}
			placement="left"
			props={{ pie: { motion: "spring" } }}
			padding={defaultChartPadding()}
			height={256}
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
				/>
			{/snippet}

			{#snippet tooltip({ context })}
				{@const total = context.flatData.reduce((acc: number, curr: TransportTypeSeries) => acc + curr.value, 0)}

				<Tooltip.Root class="bg-background/90! rounded-lg border border-white/10! p-3 shadow-xl backdrop-blur-md select-none">
					{#snippet children({ data }: { data: TransportTypeSeries })}
						{@const percentage = ((data.value / total) * 100).toFixed(2)}

						<p class="text-sm font-bold" style:color={data.color}>{data.label}</p>

						<Separator class="my-1" />

						<div class="flex flex-row justify-between gap-x-4 text-xs">
							<span class="font-medium">{data.value.toLocaleString()}</span>
							<span class="text-emerald-400 italic">({percentage} %)</span>
						</div>
					{/snippet}
				</Tooltip.Root>
			{/snippet}
		</PieChart>

		<p class="text-muted-foreground text-xs italic">
			The distribution of transport types have been estimated based on the selected end date.
		</p>
	{:catch}
		<MetricLoadingFailedWarning />
	{/await}
</MetricCardBase>
