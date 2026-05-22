<script lang="ts">
	import { MetricUnit, TransportType, type MetricSeries } from "@lib/api";
	import type { ClassValue } from "svelte/elements";
	import { Axis, ChartClipPath, Highlight, Legend, LineChart, Spline, Tooltip } from "layerchart";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";
	import Card from "@lib/components/ui/card/Card.svelte";
	import Skeleton from "@lib/components/ui/Skeleton.svelte";
	import { loadMetric } from "@lib/remote/metrics.remote";
	import { getNetworkTimeSeriesOption } from "./network-time-series";
	import { scaleOrdinal } from "d3-scale";
	import { schemeTableau10 } from "d3-scale-chromatic";
	import DateTooltip from "@lib/components/layerchart/tooltips/DateTooltip.svelte";
	import { DateTime } from "luxon";
	import { formatMetricValue } from "../../metric-format";

	type Props = {
		promise: Promise<Awaited<ReturnType<typeof loadMetric>>>;
		class?: ClassValue;
	};
	let { promise, class: className }: Props = $props();

	let innerWidth: number = $state(1024);
	const isCompactLayout: boolean = $derived(innerWidth < 640);

	type NetworkTimeSeriesData = {
		date: Date;
	} & Partial<Record<TransportType, number>>;

	const formatTransportType = (value: TransportType) =>
		String(value)
			.toLowerCase()
			.split("_")
			.map((part: string) => part.charAt(0).toUpperCase() + part.slice(1))
			.join(" ");

	const colorScale = scaleOrdinal(schemeTableau10);
	const createChartView = (metric: MetricSeries) => {
		const rowsByTimestamp = new Map<number, NetworkTimeSeriesData>();
		const transportTypes = new Set<TransportType>();

		for (const dataPoint of metric.dataPoints) {
			if (!("timestamp" in dataPoint) || !("transportType" in dataPoint)) continue;

			const timestamp = new Date(dataPoint.timestamp);
			const timestampKey = timestamp.getTime();
			if (!Number.isFinite(timestampKey)) continue;

			const transportType = dataPoint.transportType;
			transportTypes.add(transportType);

			const row = rowsByTimestamp.get(timestampKey) ?? { date: timestamp };
			row[transportType] = Number(dataPoint.value);
			rowsByTimestamp.set(timestampKey, row);
		}

		const rows = [...rowsByTimestamp.entries()].sort(([left], [right]) => left - right).map(([, row]) => row);
		const series = [...transportTypes].sort().map((transportType: TransportType, index: number) => ({
			key: transportType,
			label: formatTransportType(transportType),
			value: (datum: NetworkTimeSeriesData) => datum[transportType],
			color: colorScale(transportType)
		}));

		return { rows, series };
	};

	const createYDomain = (unit: MetricUnit) => (unit === MetricUnit.PERCENT ? [0, 100] : [0, null]);
</script>

<svelte:window bind:innerWidth />

<Card class={["gap-y-4", className]}>
	<div class="bg-background/85 pointer-events-none z-10 w-fit rounded-lg px-3 py-2 shadow-sm backdrop-blur-md">
		<p class="text-foreground/60 text-xs font-medium">Time series metric</p>
		{#await promise}
			<Skeleton class="h-6 w-32" />
		{:then data}
			<h2 class="text-foreground text-sm font-semibold sm:text-base">
				{getNetworkTimeSeriesOption(data.seriesType)?.label ?? "???"}
			</h2>
		{/await}
	</div>

	{#await promise}
		<div class="flex min-h-96 flex-col gap-y-3">
			<Skeleton class="h-72 w-full" />
			<Skeleton class="h-5 w-3/4" />
		</div>
	{:then metric}
		{@const view = createChartView(metric)}

		{#if view.rows.length === 0 || view.series.length === 0}
			<div
				class="bg-secondary/30 border-border flex min-h-96 flex-col items-center justify-center rounded-lg border text-center"
			>
				<p class="text-foreground font-semibold">No time series data available</p>
				<p class="text-foreground/60 max-w-sm text-sm">Try a wider time range or another station-event metric.</p>
			</div>
		{:else}
			<LineChart
				data={view.rows}
				series={view.series}
				x="date"
				yDomain={createYDomain(metric.unit)}
				height={isCompactLayout ? 380 : 320}
				padding={{ right: 24, bottom: isCompactLayout ? 128 : 64, left: 48 }}
				tooltipContext={{ mode: "quadtree-x" }}
				brush
			>
				{#snippet axis()}
					<Axis placement="left" rule tickLabelProps={{ textAnchor: "end" }} classes={{ root: "select-none" }} />
					<Axis placement="bottom" rule classes={{ root: "select-none" }} />
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

				{#snippet legend()}
					<Legend
						variant="swatches"
						placement="bottom"
						classes={{
							root: "w-full px-2",
							items: "flex-wrap justify-center gap-x-4 gap-y-1",
							item: "pointer-events-auto cursor-pointer",
							label: "text-foreground/75 transition-colors hover:text-foreground",
							swatch: "size-2.5"
						}}
					/>
				{/snippet}

				{#snippet tooltip({ context })}
					<Tooltip.Root
						{context}
						anchor="top"
						contained="container"
						class="bg-background/95! border-border! w-72 rounded-lg border px-3 py-2 shadow-xl backdrop-blur-md select-none"
					>
						{#snippet children()}
							{@const tooltipSeries = context.tooltip.series.filter(
								(seriesItem: Tooltip.TooltipSeries) => seriesItem.visible !== false && Number.isFinite(seriesItem.value)
							)}
							{@const totalValue = context.tooltip.series.reduce(
								(total: number, seriesItem: Tooltip.TooltipSeries) =>
									total + (Number.isFinite(seriesItem.value) ? Number(seriesItem.value) : 0),
								0
							)}

							<Tooltip.List class="grid-cols-[minmax(0,1fr)_max-content] gap-x-8 gap-y-1">
								{#each tooltipSeries as seriesItem (seriesItem.key)}
									{@const { value, unit } = formatMetricValue(Number(seriesItem.value), metric.unit)}

									<Tooltip.Item
										label={seriesItem.label}
										color={seriesItem.color}
										valueAlign="right"
										data-highlighted={context.series.isHighlighted(seriesItem.key, true)}
										onpointerenter={() => (context.series.highlightKey = seriesItem.key)}
										onpointerleave={() => (context.series.highlightKey = null)}
										classes={{
											root: "px-2",
											value: "min-w-16"
										}}
									>
										<span
											class="text-foreground inline-flex items-baseline justify-end gap-x-1 text-right leading-none font-bold whitespace-nowrap tabular-nums"
										>
											{value}
											{#if unit}
												<span class="text-foreground/60 text-xs font-semibold">{unit}</span>
											{/if}
										</span>
									</Tooltip.Item>
								{/each}

								{#if tooltipSeries.length > 0}
									{@const { value, unit } = formatMetricValue(Number(totalValue), metric.unit)}

									<Tooltip.Separator class="bg-border/80 my-1" />
									<Tooltip.Item
										label="Total"
										valueAlign="right"
										classes={{
											root: "pt-0.5"
										}}
									>
										<span
											class="text-foreground inline-flex items-baseline justify-end gap-x-1 text-right leading-none font-bold whitespace-nowrap tabular-nums"
										>
											{value}
											{#if unit}
												<span class="text-foreground/60 text-xs font-semibold">{unit}</span>
											{/if}
										</span>
									</Tooltip.Item>
								{/if}
							</Tooltip.List>
						{/snippet}
					</Tooltip.Root>

					<!-- Date Tooltip on x-Axis -->
					<DateTooltip
						{context}
						value={(data: { date: Date; value: number }) =>
							DateTime.fromJSDate(data.date).toLocaleString(DateTime.DATETIME_MED)}
					/>
				{/snippet}
			</LineChart>
		{/if}
	{:catch error}
		<div
			class="bg-secondary/30 border-border flex min-h-96 flex-col items-center justify-center gap-2 rounded-lg border text-center"
		>
			<CircleAlert size={32} class="text-destructive" />
			<p class="text-foreground font-semibold">An error occurred while loading the network time series.</p>
			<p class="text-foreground/60 max-w-xl text-sm">{error.message}</p>
		</div>
	{/await}
</Card>
