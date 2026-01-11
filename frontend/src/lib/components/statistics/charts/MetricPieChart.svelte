<script lang="ts">
	import { Legend, PieChart, Text, Tooltip, type Accessor } from "layerchart";
	import type { UnknownTypeTitle } from "../../../../routes/(navigation)/+page.svelte";
	import { MetricUnit, type MetricSeries } from "@lib/api";
	import { formatBytes, getUnit } from "@lib/util/bytes";
	import { getMetricTypeProperty } from "@lib/util/datapoint";

	type ChartDataPoint = { type: string; value: number };

	interface Props {
		metrics: MetricSeries[];
		typeTitles?: UnknownTypeTitle<any>[];
	}

	let { metrics, typeTitles = [] }: Props = $props();

	const data: ChartDataPoint[] = $derived(
		metrics
			.flatMap((metric: MetricSeries) => metric.dataPoints)
			.map((dataPoint) => {
				const typeValue = getMetricTypeProperty(dataPoint);
				return {
					type: String(typeValue),
					value: Number(dataPoint.value)
				};
			})
	);
	const isUnitBytes = $derived(metrics.every((metric: MetricSeries) => metric.unit === MetricUnit.BYTES));
</script>

<div class="h-[175px]">
	<PieChart
		key="type"
		value="value"
		c="type"
		{data}
		innerRadius={-10}
		cornerRadius={5}
		padAngle={0.02}
		padding={{ right: 200 }}
	>
		{#snippet legend({ getLegendProps })}
			<Legend
				{...getLegendProps()}
				placement="right"
				orientation="vertical"
				variant="swatches"
				classes={{
					root: "h-full w-[200px] flex flex-col justify-center pointer-events-auto pl-4",
					items: "gap-y-1",
					label: "font-[Roboto_Mono] text-xs font-medium whitespace-nowrap",
					// dot-symbol for the legend-item
					swatch: "w-2.5 h-2.5",
					item: "gap-x-2 cursor-pointer"
				}}
				tickFormat={(type: string) => {
					const typeTitle = typeTitles.find((typeTitle: UnknownTypeTitle<any>) => typeTitle.type === type);
					return typeTitle ? typeTitle.title : type;
				}}
			/>
		{/snippet}

		{#snippet aboveMarks({
			visibleData,
			value,
			key,
			highlightKey
		}: {
			visibleData: ChartDataPoint[];
			value: Accessor<ChartDataPoint>;
			key: Accessor<ChartDataPoint>;
			highlightKey: string | null;
		})}
			{@const valueFn = value as (d: ChartDataPoint) => number}
			{@const keyFn = key as (d: ChartDataPoint) => string}

			{@const totalValue = visibleData.reduce((acc, d) => acc + valueFn(d), 0)}

			{@const highlightedPoint = visibleData.find((d) => keyFn(d) === highlightKey)}

			{@const activeValue = highlightedPoint ? valueFn(highlightedPoint) : totalValue}
			{@const percentage = activeValue / totalValue}

			<Text
				value={totalValue.toLocaleString(undefined, { notation: "compact", maximumFractionDigits: 2 })}
				textAnchor="middle"
				verticalAnchor="middle"
				dy={-8}
				class="text-text pointer-events-none text-base font-bold"
			/>

			<Text
				value={percentage.toLocaleString(undefined, { style: "percent", minimumFractionDigits: 1 })}
				textAnchor="middle"
				verticalAnchor="middle"
				dy={10}
				class="text-muted-foreground pointer-events-none text-xs font-medium"
			/>
		{/snippet}

		{#snippet tooltip({ context })}
			{@const total = context.flatData.reduce((acc, curr) => acc + curr.value, 0)}

			<Tooltip.Root class="bg-background/90! rounded-lg border border-white/10! p-3 shadow-xl backdrop-blur-md">
				{#snippet children({ data })}
					{@const title = typeTitles.find((typeTitle: UnknownTypeTitle<any>) => typeTitle.type === data.type)}

					{@const percentage = ((data.value / total) * 100).toFixed(2)}
					{@const formattedValue = isUnitBytes
						? formatBytes(data.value, true) + " " + getUnit(data.value, true)
						: data.value.toLocaleString()}

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
