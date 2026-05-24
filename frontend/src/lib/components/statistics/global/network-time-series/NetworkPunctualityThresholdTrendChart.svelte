<script lang="ts">
	import { MetricUnit, type MetricSeries } from "@lib/api";
	import Card from "@lib/components/ui/card/Card.svelte";
	import Skeleton from "@lib/components/ui/Skeleton.svelte";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";
	import DateTooltip from "@lib/components/layerchart/tooltips/DateTooltip.svelte";
	import { formatMetricValue } from "../../metric-format";
	import { DateTime } from "luxon";
	import { Axis, ChartClipPath, Highlight, Legend, LineChart, Spline, Tooltip } from "layerchart";

	type Props = {
		punctuality5Promise: Promise<MetricSeries>;
		punctuality15Promise: Promise<MetricSeries>;
	};
	let { punctuality5Promise, punctuality15Promise }: Props = $props();

	type ThresholdRow = {
		date: Date;
		punctuality5?: number;
		punctuality15?: number;
	};

	const addMetricToRows = (
		rowsByTimestamp: Map<number, ThresholdRow>,
		metric: MetricSeries,
		key: "punctuality5" | "punctuality15"
	) => {
		for (const dataPoint of metric.dataPoints) {
			if (!("timestamp" in dataPoint)) continue;

			const timestamp = new Date(dataPoint.timestamp);
			const timestampKey = timestamp.getTime();
			if (!Number.isFinite(timestampKey)) continue;

			const row = rowsByTimestamp.get(timestampKey) ?? { date: timestamp };
			row[key] = Number(dataPoint.value);
			rowsByTimestamp.set(timestampKey, row);
		}
	};

	const createRows = (punctuality5: MetricSeries, punctuality15: MetricSeries) => {
		const rowsByTimestamp = new Map<number, ThresholdRow>();
		addMetricToRows(rowsByTimestamp, punctuality5, "punctuality5");
		addMetricToRows(rowsByTimestamp, punctuality15, "punctuality15");
		return [...rowsByTimestamp.entries()].sort(([left], [right]) => left - right).map(([, row]) => row);
	};

	const series = [
		{
			key: "punctuality5",
			label: "Below 5 min",
			value: (datum: ThresholdRow) => datum.punctuality5,
			color: "var(--color-accent)"
		},
		{
			key: "punctuality15",
			label: "Below 15 min",
			value: (datum: ThresholdRow) => datum.punctuality15,
			color: "var(--color-foreground)"
		}
	];
</script>

<Card class="gap-y-4">
	<div>
		<p class="text-foreground/60 text-xs font-medium">Punctuality thresholds</p>
		<h3 class="text-foreground text-base font-semibold">Five and fifteen minute rule</h3>
	</div>

	{#await Promise.all([punctuality5Promise, punctuality15Promise])}
		<div class="flex min-h-80 flex-col gap-y-3">
			<Skeleton class="h-64 w-full" />
			<Skeleton class="h-5 w-2/3" />
		</div>
	{:then [punctuality5, punctuality15]}
		{@const rows = createRows(punctuality5, punctuality15)}

		{#if rows.length === 0}
			<div
				class="bg-secondary/30 border-border flex min-h-80 flex-col items-center justify-center rounded-lg border text-center"
			>
				<p class="text-foreground font-semibold">No punctuality data available</p>
				<p class="text-foreground/60 max-w-sm text-sm">Try a wider time range or another scope.</p>
			</div>
		{:else}
			<LineChart
				data={rows}
				{series}
				x="date"
				yDomain={[0, 100]}
				height={320}
				padding={{ left: 16, right: 8, bottom: 48 }}
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
							<Tooltip.List class="grid-cols-[minmax(0,1fr)_max-content] gap-x-8 gap-y-1">
								{#each context.tooltip.series as seriesItem (seriesItem.key)}
									{@const { value, unit } = formatMetricValue(Number(seriesItem.value), MetricUnit.PERCENT)}
									<Tooltip.Item label={seriesItem.label} color={seriesItem.color} valueAlign="right">
										<span class="text-foreground font-bold tabular-nums">
											{value}
											{#if unit}
												<span class="text-foreground/60 ml-1 text-xs">{unit}</span>
											{/if}
										</span>
									</Tooltip.Item>
								{/each}
							</Tooltip.List>
						{/snippet}
					</Tooltip.Root>

					<DateTooltip
						{context}
						value={(data: { date: Date }) => DateTime.fromJSDate(data.date).toLocaleString(DateTime.DATETIME_MED)}
					/>
				{/snippet}
			</LineChart>
		{/if}
	{:catch error}
		<div
			class="bg-secondary/30 border-border flex min-h-80 flex-col items-center justify-center gap-2 rounded-lg border text-center"
		>
			<CircleAlert size={32} class="text-destructive" />
			<p class="text-foreground font-semibold">The punctuality trend could not be loaded.</p>
			<p class="text-foreground/60 max-w-xl text-sm">{error.message}</p>
		</div>
	{/await}
</Card>
