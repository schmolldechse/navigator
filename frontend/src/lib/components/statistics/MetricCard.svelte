<script lang="ts">
	import TrendingUp from "@lucide/svelte/icons/trending-up";
	import TrendingDown from "@lucide/svelte/icons/trending-down";
	import { formatBytes, getUnit } from "$lib/util/bytes";
	import { MetricSeriesType, MetricUnit, type MetricSeries } from "$lib/api/types.gen";
	import { PieChart, Tooltip } from "layerchart";
	import type { SeriesTypeTitles } from "../../../routes/(navigation)/+page.svelte";

	interface Props {
		title: string;
		metricPromise: Promise<MetricSeries[]>;
		onselect?: (statistic: MetricSeries[]) => void;
		selectDisabled?: boolean;
		seriesTitles?: SeriesTypeTitles[];
	}

	let { title, metricPromise, onselect, selectDisabled = false, seriesTitles = [] }: Props = $props();

	const calculateChange = (metrics: MetricSeries[]): { percentage: number; isUp: boolean } => {
		const endValue = metrics.reduce((a, metric: MetricSeries) => {
			const value = Number(metric.summary.endValue);
			return a + (isNaN(value) ? 0 : value);
		}, 0);
		const startValue = metrics.reduce((a, metric: MetricSeries) => {
			const value = Number(metric.summary.startValue);
			return a + (isNaN(value) ? 0 : value);
		}, 0);
		const changedBy = endValue - startValue;

		if (startValue === 0) return { percentage: endValue > 0 ? 100 : 0, isUp: endValue >= 0 };
		const percentage = (changedBy / startValue) * 100;

		return {
			percentage: Math.abs(percentage),
			isUp: changedBy >= 0
		};
	};
</script>

<button
	class={[
		"group border-muted-foreground/20 bg-muted/20 hover:border-accent/20 relative flex flex-col gap-y-4 rounded-lg border-2 p-5 text-left transition-colors duration-300",
		{ "cursor-pointer": !selectDisabled }
	]}
	onclick={async () => {
		if (selectDisabled) return;

		const metric = await metricPromise;
		onselect?.(metric);
	}}
>
	<div class="flex items-center justify-between">
		<p class="text-muted-foreground text-xs font-semibold tracking-wider uppercase">{title}</p>

		{#await metricPromise}
			<div class="bg-muted h-6 w-16 animate-pulse rounded-full"></div>
		{:then metrics}
			{@const trend = calculateChange(metrics)}
			{@const TrendIcon = trend.isUp ? TrendingUp : TrendingDown}

			<div
				class={[
					"flex items-center gap-x-2 rounded-full border px-3 py-0.5",
					{ "border-emerald-500/20 bg-emerald-500/10 text-emerald-500": trend.isUp },
					{ "border-rose-500/20 bg-rose-500/10 text-rose-500": !trend.isUp }
				]}
			>
				<TrendIcon size={12} strokeWidth={3} />
				<span class="text-[10px] font-black tracking-tight">{trend.percentage.toFixed(2)} %</span>
			</div>
		{/await}
	</div>

	{#await metricPromise}
		<div class="bg-muted h-10 w-3/4 animate-pulse rounded-lg"></div>
	{:then metrics}
		{@const sortedMetrics = [...metrics].sort(
			(a: MetricSeries, b: MetricSeries) => Number(b.summary.endValue) - Number(a.summary.endValue)
		)}
		{@const isUnitBytes = metrics
			.map((metric: MetricSeries) => metric.unit)
			.every((unit: MetricUnit) => unit === MetricUnit.BYTES)}
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

		{#if metrics.length > 1}
			<div class="flex items-center gap-3">
				<div class="h-0.5 flex-1 bg-white/10"></div>
				<span class="font-mono text-xs font-bold tracking-tight text-white/30 uppercase">Series</span>
				<div class="h-0.5 flex-1 bg-white/10"></div>
			</div>

			<div class="h-[120px]">
				<PieChart
					data={sortedMetrics.map((metric: MetricSeries) => ({
						seriesType: metric.seriesType,
						value: Number(metric.summary.endValue)
					}))}
					key="seriesType"
					value="value"
					range={[-90, 90]}
					outerRadius={90}
					innerRadius={-15}
					cornerRadius={10}
					padAngle={0.05}
					placement="left"
					props={{ group: { y: 105, x: 105 } }}
					legend={{
						placement: "right",
						orientation: "vertical",
						classes: {
							items: "gap-y-1",
							item: "gap-x-3",
							label: "font-[Roboto_Mono] text-xs font-medium whitespace-nowrap text-muted-foreground",
							// dot-symbol for the legend-item
							swatch: "h-2.5 w-2.5"
						},
						tickFormat: (seriesType: MetricSeriesType) => {
							const metric = sortedMetrics.find((metric: MetricSeries) => metric.seriesType === seriesType);
							const value = metric ? Number(metric.summary.endValue) : 0;

							const title = seriesTitles.find((title: SeriesTypeTitles) => title.seriesType === seriesType)?.title;
							const formattedValue = isUnitBytes ? formatBytes(value, true) : value.toLocaleString();

							return `${title ?? seriesType} ${formattedValue}`;
						}
					}}
				>
					{#snippet tooltip({ context })}
						{@const total = context.flatData.reduce((acc, curr) => acc + curr.value, 0)}

						<Tooltip.Root class="bg-background/90! rounded-lg border border-white/10! p-3 shadow-xl backdrop-blur-md">
							{#snippet children({ data })}
								{@const title =
									seriesTitles.find((title: SeriesTypeTitles) => title.seriesType === data.seriesType)?.title ??
									data.seriesType}

								{@const percentage = ((data.value / total) * 100).toFixed(2)}
								{@const formattedValue = isUnitBytes
									? formatBytes(data.value, true) + " " + getUnit(data.value, true)
									: data.value.toLocaleString()}

								<Tooltip.Header class="text-muted-foreground! mb-1 text-sm font-medium">
									{title}
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
	{/await}
</button>
