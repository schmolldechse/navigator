<script module lang="ts">
	import type { StatisticsMetricResponse } from "@lib/api";

	type TrendMetricDefinition = {
		key: string;
		label: string;
		color: string;
		value: (item: unknown) => number | null;
		format: "percent" | "seconds" | "count" | "minutes";
	};

	export type { TrendMetricDefinition };
</script>

<script lang="ts">
	import Skeleton from "@lib/components/ui/Skeleton.svelte";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";
	import LineChartIcon from "@lucide/svelte/icons/chart-no-axes-combined";
	import { DateTime } from "luxon";
	import DashboardPanel from "./DashboardPanel.svelte";
	import { formatCount, formatLocalDateTime, formatMetric, SERVICE_ZONE } from "./statistics-dashboard";

	type TrendRow = {
		date: Date;
		[key: string]: Date | number | null;
	};

	type Props = {
		title: string;
		description?: string;
		promise: Promise<StatisticsMetricResponse>;
		metrics: TrendMetricDefinition[];
		yDomain?: [number, number | null];
	};

	let { title, description, promise, metrics, yDomain = [0, null] }: Props = $props();

	const chartWidth = 760;
	const chartHeight = 320;
	const padding = {
		top: 18,
		right: 24,
		bottom: 42,
		left: 54
	};
	const plotWidth = chartWidth - padding.left - padding.right;
	const plotHeight = chartHeight - padding.top - padding.bottom;

	const createRows = (response: StatisticsMetricResponse): TrendRow[] => {
		const items = "items" in response.result ? response.result.items : [];

		return items
			.map((item: unknown) => {
				const bucketStart = typeof item === "object" && item !== null && "bucketStart" in item ? item.bucketStart : null;
				const date = typeof bucketStart === "string" ? new Date(bucketStart) : null;
				if (!date || Number.isNaN(date.getTime())) return null;

				const row: TrendRow = { date };
				for (const metric of metrics) row[metric.key] = metric.value(item);
				return row;
			})
			.filter((row): row is TrendRow => row !== null)
			.sort((left, right) => left.date.getTime() - right.date.getTime());
	};

	const formatTooltipValue = (value: unknown, kind: TrendMetricDefinition["format"]): string => {
		if (typeof value !== "number" || !Number.isFinite(value)) return "-";
		if (kind === "percent") return `${value.toFixed(1)}%`;
		if (kind === "count") return formatCount(value, true);

		return formatMetric(value, kind);
	};

	const getAllValues = (rows: TrendRow[], definitions: TrendMetricDefinition[]): number[] =>
		rows.flatMap((row) =>
			definitions
				.map((definition) => row[definition.key])
				.filter((value): value is number => typeof value === "number" && Number.isFinite(value))
		);

	const createYDomain = (rows: TrendRow[], definitions: TrendMetricDefinition[]): [number, number] => {
		const values = getAllValues(rows, definitions);
		const minValue = yDomain[0];
		const maxValue = yDomain[1] ?? Math.max(...values, 1);
		const paddedMaxValue = yDomain[1] ?? Math.ceil(maxValue * 1.08);

		if (paddedMaxValue <= minValue) return [minValue, minValue + 1];
		return [minValue, paddedMaxValue];
	};

	const xScale = (date: Date, rows: TrendRow[]) => {
		if (rows.length <= 1) return padding.left + plotWidth / 2;

		const minTime = rows[0].date.getTime();
		const maxTime = rows[rows.length - 1].date.getTime();
		const ratio = (date.getTime() - minTime) / Math.max(1, maxTime - minTime);

		return padding.left + ratio * plotWidth;
	};

	const yScale = (value: number, domain: [number, number]) => {
		const ratio = (value - domain[0]) / Math.max(1, domain[1] - domain[0]);

		return padding.top + plotHeight - ratio * plotHeight;
	};

	const createLinePath = (rows: TrendRow[], definition: TrendMetricDefinition, domain: [number, number]) =>
		rows
			.map((row) => {
				const value = row[definition.key];
				if (typeof value !== "number" || !Number.isFinite(value)) return null;

				return `${xScale(row.date, rows).toFixed(2)},${yScale(value, domain).toFixed(2)}`;
			})
			.filter((point): point is string => point !== null)
			.map((point, index) => `${index === 0 ? "M" : "L"}${point}`)
			.join(" ");

	const createXTicks = (rows: TrendRow[]) => {
		if (rows.length <= 4) return rows;

		return Array.from({ length: 5 }, (_, index) => {
			const rowIndex = Math.round((index / 4) * (rows.length - 1));
			return rows[rowIndex];
		});
	};

	const createYTicks = (domain: [number, number]) =>
		Array.from({ length: 5 }, (_, index) => domain[0] + ((domain[1] - domain[0]) / 4) * index);

	const formatAxisDate = (date: Date): string => DateTime.fromJSDate(date).setZone(SERVICE_ZONE).toFormat("dd LLL");
	const formatAxisValue = (value: number, definitions: TrendMetricDefinition[]): string => {
		const kind = definitions[0]?.format ?? "count";
		if (kind === "percent") return `${Math.round(value)}%`;
		if (kind === "count") return formatCount(value, true);

		return formatMetric(value, kind);
	};
</script>

<DashboardPanel {title} {description} icon={LineChartIcon}>
	{#await promise}
		<div class="flex min-h-80 flex-col gap-y-3">
			<Skeleton class="h-64 w-full" />
			<Skeleton class="h-5 w-2/3" />
		</div>
	{:then response}
		{@const rows = createRows(response)}
		{@const domain = createYDomain(rows, metrics)}
		{@const xTicks = createXTicks(rows)}
		{@const yTicks = createYTicks(domain)}
		{#if rows.length === 0}
			<div class="border-border bg-secondary/25 flex min-h-80 items-center justify-center rounded-lg border text-center">
				<p class="text-foreground/60 text-sm font-semibold">No metric points available.</p>
			</div>
		{:else}
			<div class="flex flex-col gap-3">
				<div class="overflow-x-auto">
					<svg
						viewBox={`0 0 ${chartWidth} ${chartHeight}`}
						role="img"
						aria-label={title}
						class="text-foreground h-80 min-w-[42rem] rounded-lg"
					>
						<rect width={chartWidth} height={chartHeight} rx="10" class="fill-secondary/20" />

						{#each yTicks as tick (tick)}
							{@const y = yScale(tick, domain)}
							<line
								x1={padding.left}
								x2={chartWidth - padding.right}
								y1={y}
								y2={y}
								class="stroke-border"
								stroke-dasharray="4 4"
							/>
							<text
								x={padding.left - 10}
								y={y + 4}
								text-anchor="end"
								class="fill-foreground/50 text-[11px] font-semibold tabular-nums"
							>
								{formatAxisValue(tick, metrics)}
							</text>
						{/each}

						<line
							x1={padding.left}
							x2={chartWidth - padding.right}
							y1={chartHeight - padding.bottom}
							y2={chartHeight - padding.bottom}
							class="stroke-border"
						/>
						<line
							x1={padding.left}
							x2={padding.left}
							y1={padding.top}
							y2={chartHeight - padding.bottom}
							class="stroke-border"
						/>

						{#each xTicks as row (row.date.getTime())}
							{@const x = xScale(row.date, rows)}
							<line
								x1={x}
								x2={x}
								y1={chartHeight - padding.bottom}
								y2={chartHeight - padding.bottom + 5}
								class="stroke-border"
							/>
							<text {x} y={chartHeight - 16} text-anchor="middle" class="fill-foreground/50 text-[11px] font-semibold">
								{formatAxisDate(row.date)}
							</text>
						{/each}

						{#each metrics as metric (metric.key)}
							{@const path = createLinePath(rows, metric, domain)}
							{#if path}
								<path
									d={path}
									fill="none"
									stroke={metric.color}
									stroke-width="3"
									stroke-linecap="round"
									stroke-linejoin="round"
								/>
								{#each rows as row (metric.key + row.date.getTime())}
									{@const value = row[metric.key]}
									{#if typeof value === "number" && Number.isFinite(value)}
										<circle cx={xScale(row.date, rows)} cy={yScale(value, domain)} r="3.5" fill={metric.color}>
											<title>
												{metric.label}, {formatLocalDateTime(row.date.toISOString())}: {formatTooltipValue(
													value,
													metric.format
												)}
											</title>
										</circle>
									{/if}
								{/each}
							{/if}
						{/each}
					</svg>
				</div>

				<div class="flex flex-wrap justify-center gap-x-4 gap-y-2">
					{#each metrics as metric (metric.key)}
						<div class="text-foreground/75 flex items-center gap-2 text-xs font-semibold">
							<span class="size-2.5 rounded-full" style={`background-color: ${metric.color};`}></span>
							<span>{metric.label}</span>
						</div>
					{/each}
				</div>
			</div>
		{/if}
	{:catch error}
		<div
			class="border-destructive/30 bg-destructive/10 flex min-h-80 flex-col items-center justify-center gap-2 rounded-lg border text-center"
		>
			<CircleAlert size={32} class="text-destructive" />
			<p class="text-destructive text-sm font-semibold">{error.message}</p>
		</div>
	{/await}
</DashboardPanel>
