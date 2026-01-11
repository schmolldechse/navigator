<script lang="ts">
	import TrendingUp from "@lucide/svelte/icons/trending-up";
	import TrendingDown from "@lucide/svelte/icons/trending-down";
	import { MetricUnit, type MetricSeries } from "$lib/api/types.gen";
	import type { UnknownTypeTitle } from "../../../routes/(navigation)/+page.svelte";
	import MetricPieChart from "./charts/MetricPieChart.svelte";
	import { calculateChange, isTrendAvailable } from "@lib/util/chart";
	import { formatBytes, getUnit } from "@lib/util/bytes";
	import { Legend, PieChart, Text, Tooltip, type Accessor } from "layerchart";

	type ChartDataPoint = { type: string; value: number };

	interface Props {
		title: string;
		promise: Promise<MetricSeries[]>;
		onselect?: (metrics: MetricSeries[]) => void;
		selectDisabled?: boolean;
		typeTitles?: UnknownTypeTitle<any>[];
		class?: string;
	}

	let { title, promise, onselect, selectDisabled = false, typeTitles = [], class: className = "" }: Props = $props();

	/**
	 *
	 * TODO:
	 * 1) MetricCard has the title required
	 * 2) boolean property to show trend
	 * 3) Snippet for Chart
	 */
</script>

<button
	class={[
		"group border-muted-foreground/20 bg-muted/20 hover:border-accent/20 relative flex flex-col gap-y-4 rounded-lg border-2 p-5 text-left transition-colors duration-300",
		{ "cursor-pointer": !selectDisabled },
		className
	]}
	onclick={async () => {
		if (selectDisabled) return;

		const metrics = await promise;
		onselect?.(metrics);
	}}
>
	<div class="flex items-center justify-between py-0.5">
		<p class="text-muted-foreground text-xs font-semibold tracking-wider uppercase">{title}</p>

		{#await promise}
			<div class="bg-muted h-6 w-16 animate-pulse rounded-full"></div>
		{:then metrics}
			{#if isTrendAvailable(metrics)}
				{@const trend = calculateChange(metrics)}
				{@const TrendIcon = trend.isUp ? TrendingUp : TrendingDown}

				<div
					class={[
						"flex items-center gap-x-2 rounded-full border px-3",
						{ "border-emerald-500/20 bg-emerald-500/10 text-emerald-500": trend.isUp },
						{ "border-rose-500/20 bg-rose-500/10 text-rose-500": !trend.isUp }
					]}
				>
					<TrendIcon size={12} strokeWidth={3} />
					<span class="text-[10px] font-black tracking-tight">{trend.percentage.toFixed(2)} %</span>
				</div>
			{:else}
				<!-- No trend available -->
			{/if}
		{/await}
	</div>

	{#await promise}
		<div class="bg-muted h-10 w-3/4 animate-pulse rounded-lg"></div>
	{:then metrics}
		{#if isTrendAvailable(metrics)}
			{@const isUnitBytes = metrics
				.map((metric: MetricSeries) => metric.unit)
				.every((unit: MetricUnit) => unit === MetricUnit.BYTES)}
			{@const sortedMetrics = [...metrics].sort(
				(a: MetricSeries, b: MetricSeries) => Number(b.summary.endValue) - Number(a.summary.endValue)
			)}
			{@const totalValue = sortedMetrics.reduce((a: number, metric: MetricSeries) => {
				const value = Number(metric.summary.endValue);
				return a + (isNaN(value) ? 0 : value);
			}, 0)}

			<div class="flex items-baseline gap-x-2">
				<span class="group-hover:text-accent text-4xl font-bold transition-colors duration-300">
					{isUnitBytes ? formatBytes(totalValue, true) : totalValue.toLocaleString()}
				</span>
				{#if isUnitBytes}
					<span class="text-muted-foreground text-lg font-medium">{getUnit(totalValue, true)}</span>
				{/if}
			</div>

			<!-- Distribution of Metric Series -->
			{#if metrics.length > 1}
				{@const arcChatData: ChartDataPoint[] = metrics.map((metric: MetricSeries) => ({
					type: metric.seriesType,
					value: Number(metric.summary.endValue)
				}))}

				<div class="flex items-center gap-3">
					<div class="h-0.5 flex-1 bg-white/10"></div>
					<span class="font-mono text-xs font-bold tracking-tight text-white/30 uppercase">Series</span>
					<div class="h-0.5 flex-1 bg-white/10"></div>
				</div>

				<div class="h-[120px]">
					<PieChart
						key="type"
						value="value"
						c="type"
						data={arcChatData}
						range={[-90, 90]}
						innerRadius={-10}
						cornerRadius={5}
						padAngle={0.02}
						padding={{ bottom: -90, right: 200 }}
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
								dy={-32}
								class="text-text pointer-events-none text-base font-bold"
							/>

							<Text
								value={percentage.toLocaleString(undefined, { style: "percent", minimumFractionDigits: 1 })}
								textAnchor="middle"
								verticalAnchor="middle"
								dy={-16}
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
			{/if}
		{:else}
			<MetricPieChart {metrics} {typeTitles} />
		{/if}
	{/await}
</button>
