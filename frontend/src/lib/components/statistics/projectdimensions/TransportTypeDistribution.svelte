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
	import Card from "@lib/components/ui/card/Card.svelte";
	import MetricLoadingFailedWarning from "@lib/components/metric/MetricLoadingFailedWarning.svelte";
	import CardHeader from "@lib/components/ui/card/CardHeader.svelte";

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

<Card class={["gap-y-2", className]}>
	<CardHeader title="Transport Type Distribution" />

	{#await validatedPromise}
		<div class="bg-foreground/10 h-64 w-full animate-pulse rounded-md"></div>
	{:then metrics}
		<PieChart
			data={buildChartSeries(metrics)}
			key="transportType"
			value="value"
			c={(series: TransportTypeSeries) => series.color}
			outerRadius={95}
			innerRadius={65}
			cornerRadius={2}
			padAngle={0.02}
			props={{ pie: { motion: "spring" } }}
			padding={defaultChartPadding()}
			height={250}
			placement="left"
		>
			{#snippet legend()}
				<Legend placement="right" orientation="vertical" variant="swatches" />
			{/snippet}

			{#snippet tooltip({ context })}
				{@const total = context.flatData.reduce((acc: number, curr: TransportTypeSeries) => acc + curr.value, 0)}

				<Tooltip.Root variant="none" class="bg-background border-border rounded-lg border-2 px-2 py-0.5">
					{#snippet children({ data }: { data: TransportTypeSeries })}
						{@const percentage = ((data.value / total) * 100).toFixed(2)}

						<p class="text-sm font-bold" style:color={data.color}>{data.label}</p>

						<div class="flex flex-row justify-between gap-x-4 text-xs">
							<span class="font-medium">{data.value.toLocaleString()}</span>
							<span class="text-emerald-400 italic">({percentage} %)</span>
						</div>
					{/snippet}
				</Tooltip.Root>
			{/snippet}
		</PieChart>
	{:catch}
		<MetricLoadingFailedWarning />
	{/await}
</Card>
