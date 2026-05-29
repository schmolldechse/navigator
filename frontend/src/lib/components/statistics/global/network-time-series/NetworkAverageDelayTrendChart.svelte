<script lang="ts">
	import { MetricUnit, type MetricSample, type MetricSeries } from "@lib/api";
	import Card from "@lib/components/ui/card/Card.svelte";
	import Skeleton from "@lib/components/ui/Skeleton.svelte";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";
	import DateTooltip from "@lib/components/layerchart/tooltips/DateTooltip.svelte";
	import { formatMetricValue } from "../../metric-format";
	import { DateTime } from "luxon";
	import { Axis, ChartClipPath, Highlight, LineChart, Spline, Tooltip } from "layerchart";

	type Props = {
		promise: Promise<MetricSeries>;
	};
	let { promise }: Props = $props();

	type DelayRow = {
		date: Date;
		value: number;
	};

	type DelayAccumulator = {
		date: Date;
		valueTotal: number;
		valueCount: number;
		sample: MetricSample;
	};

	const numberValue = (value: number | string | null | undefined): number => {
		const parsed = Number(value);
		return Number.isFinite(parsed) ? parsed : 0;
	};

	const createRows = (metric: MetricSeries): DelayRow[] => {
		const rowsByTimestamp = new Map<number, DelayAccumulator>();

		for (const dataPoint of metric.dataPoints) {
			if (!("timestamp" in dataPoint)) continue;

			const timestamp = new Date(dataPoint.timestamp);
			const timestampKey = timestamp.getTime();
			const value = Number(dataPoint.value);
			if (!Number.isFinite(timestampKey) || !Number.isFinite(value)) continue;

			const row = rowsByTimestamp.get(timestampKey) ?? {
				date: timestamp,
				valueTotal: 0,
				valueCount: 0,
				sample: { numerator: 0, denominator: 0 }
			};

			row.valueTotal += value;
			row.valueCount += 1;
			row.sample = {
				numerator: numberValue(row.sample.numerator) + numberValue(dataPoint.sample?.numerator),
				denominator: numberValue(row.sample.denominator) + numberValue(dataPoint.sample?.denominator)
			};
			rowsByTimestamp.set(timestampKey, row);
		}

		return [...rowsByTimestamp.entries()]
			.sort(([left], [right]) => left - right)
			.map(([, row]) => {
				const denominator = numberValue(row.sample.denominator);
				const numerator = numberValue(row.sample.numerator);
				return {
					date: row.date,
					value: denominator > 0 ? numerator / denominator : row.valueTotal / row.valueCount
				};
			});
	};
</script>

<Card class="gap-y-4">
	<div>
		<p class="text-foreground/60 text-xs font-medium">Average delay</p>
		<h3 class="text-foreground text-base font-semibold">Measured station-event delay over time</h3>
	</div>

	{#await promise}
		<div class="flex min-h-80 flex-col gap-y-3">
			<Skeleton class="h-64 w-full" />
			<Skeleton class="h-5 w-2/3" />
		</div>
	{:then metric}
		{@const rows = createRows(metric)}
		{#if rows.length === 0}
			<div
				class="bg-secondary/30 border-border flex min-h-80 flex-col items-center justify-center rounded-lg border text-center"
			>
				<p class="text-foreground font-semibold">No delay data available</p>
				<p class="text-foreground/60 max-w-sm text-sm">Try a wider time range or another scope.</p>
			</div>
		{:else}
			<LineChart
				data={rows}
				x="date"
				y="value"
				yDomain={[0, null]}
				height={320}
				padding={{ left: 48, right: 24, bottom: 16 }}
				tooltipContext={{ mode: "quadtree-x" }}
				brush
			>
				{#snippet axis()}
					<Axis placement="left" rule tickLabelProps={{ textAnchor: "end" }} classes={{ root: "select-none" }} />
					<Axis placement="bottom" rule classes={{ root: "select-none" }} />
				{/snippet}

				{#snippet marks()}
					<ChartClipPath>
						<Spline stroke="var(--color-accent)" strokeWidth={2} />
					</ChartClipPath>
				{/snippet}

				{#snippet highlight()}
					<Highlight points lines />
				{/snippet}

				{#snippet tooltip({ context })}
					<Tooltip.Root
						{context}
						anchor="top"
						contained="container"
						class="bg-background/95! border-border! w-64 rounded-lg border px-3 py-2 shadow-xl backdrop-blur-md select-none"
					>
						{@const { value, unit } = formatMetricValue(Number(context.tooltip.data?.value ?? 0), MetricUnit.SECONDS)}
						<div class="grid gap-y-1">
							<span class="text-foreground/60 text-xs font-medium">Average delay</span>
							<span class="text-foreground text-lg font-bold tabular-nums">
								{value}
								{#if unit}
									<span class="text-foreground/60 ml-1 text-xs">{unit}</span>
								{/if}
							</span>
						</div>
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
			<p class="text-foreground font-semibold">The delay trend could not be loaded.</p>
			<p class="text-foreground/60 max-w-xl text-sm">{error.message}</p>
		</div>
	{/await}
</Card>
