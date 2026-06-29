<script lang="ts">
	import { MetricUnit, type MetricSample, type MetricSeries } from "@lib/api";
	import Card from "@lib/components/ui/card/Card.svelte";
	import Skeleton from "@lib/components/ui/Skeleton.svelte";
	import ToggleGroup from "@lib/components/ui/toggle-group/ToggleGroup.svelte";
	import ToggleGroupItem from "@lib/components/ui/toggle-group/ToggleGroupItem.svelte";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";
	import DateTooltip from "@lib/components/layerchart/tooltips/DateTooltip.svelte";
	import { formatMetricValue } from "../../metric-format";
	import { DateTime } from "luxon";
	import { Axis, ChartClipPath, Highlight, LineChart, Spline, Tooltip } from "layerchart";

	type Props = {
		cancellationRatePromise: Promise<MetricSeries>;
		cancellationCountPromise: Promise<MetricSeries>;
	};
	let { cancellationRatePromise, cancellationCountPromise }: Props = $props();

	type CancellationMode = {
		key: "rate" | "count";
		label: string;
		description: string;
		unit: MetricUnit;
		yDomain: [number, number | null];
	};

	type CancellationRow = {
		date: Date;
		rate?: number;
		count?: number;
	};

	type RateAccumulator = {
		date: Date;
		valueTotal: number;
		valueCount: number;
		sample: MetricSample;
	};

	const cancellationModes: CancellationMode[] = [
		{
			key: "rate",
			label: "Rate",
			description: "Share of station events that were cancelled.",
			unit: MetricUnit.PERCENT,
			yDomain: [0, 100]
		},
		{
			key: "count",
			label: "Count",
			description: "Number of cancelled station events.",
			unit: MetricUnit.COUNT,
			yDomain: [0, null]
		}
	];

	let selectedMode: CancellationMode = $state(cancellationModes[0]);

	const numberValue = (value: number | string | null | undefined): number => {
		const parsed = Number(value);
		return Number.isFinite(parsed) ? parsed : 0;
	};

	const sampleValue = (sample: MetricSample | null | undefined, unit: MetricUnit, fallback: number): number => {
		const numerator = numberValue(sample?.numerator);
		const denominator = numberValue(sample?.denominator);
		if (denominator <= 0) return fallback;

		return unit === MetricUnit.PERCENT ? 100 * (numerator / denominator) : numerator / denominator;
	};

	const addMetricToRows = (rowsByTimestamp: Map<number, CancellationRow>, metric: MetricSeries, key: "rate" | "count") => {
		const ratesByTimestamp = new Map<number, RateAccumulator>();
		const countSumsByTimestamp = new Map<number, number>();

		for (const dataPoint of metric.dataPoints) {
			if (!("timestamp" in dataPoint)) continue;

			const timestamp = new Date(dataPoint.timestamp);
			const timestampKey = timestamp.getTime();
			const value = Number(dataPoint.value);
			if (!Number.isFinite(timestampKey) || !Number.isFinite(value)) continue;

			if (key === "count") {
				countSumsByTimestamp.set(timestampKey, (countSumsByTimestamp.get(timestampKey) ?? 0) + value);
				continue;
			}

			const rate = ratesByTimestamp.get(timestampKey) ?? {
				date: timestamp,
				valueTotal: 0,
				valueCount: 0,
				sample: { numerator: 0, denominator: 0 }
			};
			rate.valueTotal += value;
			rate.valueCount += 1;
			rate.sample = {
				numerator: numberValue(rate.sample.numerator) + numberValue(dataPoint.sample?.numerator),
				denominator: numberValue(rate.sample.denominator) + numberValue(dataPoint.sample?.denominator)
			};
			ratesByTimestamp.set(timestampKey, rate);
		}

		for (const [timestampKey, count] of countSumsByTimestamp) {
			const row = rowsByTimestamp.get(timestampKey) ?? { date: new Date(timestampKey) };
			row.count = count;
			rowsByTimestamp.set(timestampKey, row);
		}

		for (const [timestampKey, rate] of ratesByTimestamp) {
			const row = rowsByTimestamp.get(timestampKey) ?? { date: rate.date };
			row.rate = sampleValue(rate.sample, metric.unit, rate.valueTotal / rate.valueCount);
			rowsByTimestamp.set(timestampKey, row);
		}
	};

	const createRows = (cancellationRate: MetricSeries, cancellationCount: MetricSeries): CancellationRow[] => {
		const rowsByTimestamp = new Map<number, CancellationRow>();
		addMetricToRows(rowsByTimestamp, cancellationRate, "rate");
		addMetricToRows(rowsByTimestamp, cancellationCount, "count");
		return [...rowsByTimestamp.entries()].sort(([left], [right]) => left - right).map(([, row]) => row);
	};

	const createSelectedRows = (rows: CancellationRow[], mode: CancellationMode) =>
		rows.flatMap((row) => {
			const value = row[mode.key];
			return typeof value === "number" && Number.isFinite(value) ? [{ date: row.date, value }] : [];
		});
</script>

<Card class="gap-y-4">
	<div class="flex flex-col gap-3 sm:flex-row sm:items-start sm:justify-between">
		<div>
			<p class="text-foreground/60 text-xs font-medium">Cancellations</p>
			<h3 class="text-foreground text-base font-semibold">Cancelled station events over time</h3>
		</div>

		<ToggleGroup
			mode="single"
			allowEmpty={false}
			selected={selectedMode}
			keyFn={(mode: CancellationMode) => mode.key}
			onselect={(mode: CancellationMode | undefined) => {
				if (mode) selectedMode = mode;
			}}
			class="gap-1.5"
		>
			{#each cancellationModes as mode (mode.key)}
				<ToggleGroupItem
					item={mode}
					title={mode.description}
					aria-label={`${mode.label}: ${mode.description}`}
					class="enabled:hover:bg-accent/15 enabled:hover:text-accent data-active:border-accent data-active:bg-accent data-active:text-accent-foreground data-active:hover:bg-accent data-active:hover:text-accent-foreground px-2.5 py-1.5 text-xs font-semibold transition-colors"
				>
					{mode.label}
				</ToggleGroupItem>
			{/each}
		</ToggleGroup>
	</div>

	{#await Promise.all([cancellationRatePromise, cancellationCountPromise])}
		<div class="flex min-h-80 flex-col gap-y-3">
			<Skeleton class="h-64 w-full" />
			<Skeleton class="h-5 w-2/3" />
		</div>
	{:then [cancellationRate, cancellationCount]}
		{@const rows = createSelectedRows(createRows(cancellationRate, cancellationCount), selectedMode)}

		{#if rows.length === 0}
			<div
				class="bg-secondary/30 border-border flex min-h-80 flex-col items-center justify-center rounded-lg border text-center"
			>
				<p class="text-foreground font-semibold">No cancellation data available</p>
				<p class="text-foreground/60 max-w-sm text-sm">Try a wider time range or another scope.</p>
			</div>
		{:else}
			<LineChart
				data={rows}
				x="date"
				y="value"
				yDomain={selectedMode.yDomain}
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
						{@const { value, unit } = formatMetricValue(Number(context.tooltip.data?.value ?? 0), selectedMode.unit)}
						<div class="grid gap-y-1">
							<span class="text-foreground/60 text-xs font-medium">Cancellation {selectedMode.label.toLowerCase()}</span>
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
			<p class="text-foreground font-semibold">The cancellation trend could not be loaded.</p>
			<p class="text-foreground/60 max-w-xl text-sm">{error.message}</p>
		</div>
	{/await}
</Card>
