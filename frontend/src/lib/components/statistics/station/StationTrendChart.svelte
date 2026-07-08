<script module lang="ts">
	import type { EventTimeSeriesPoint } from "@lib/api";

	type TrendMetricDefinition = {
		key: string;
		label: string;
		color: string;
		value: (item: EventTimeSeriesPoint) => number | null;
		format: "percent" | "seconds" | "count" | "minutes";
	};

	export type { TrendMetricDefinition };
</script>

<script lang="ts">
	import type { StatisticsMetricResponse } from "@lib/api";
	import Skeleton from "@lib/components/ui/Skeleton.svelte";
	import type { RemoteQuery } from "@sveltejs/kit";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";
	import LineChartIcon from "@lucide/svelte/icons/chart-no-axes-combined";
	import { LineChart } from "layerchart";
	import type { Snippet } from "svelte";
	import DashboardPanel from "../shared/DashboardPanel.svelte";
	import { formatCount, formatDate, formatMetric } from "../shared/statistics-dashboard";

	type TrendRow = {
		date: Date;
		[key: string]: Date | number | null;
	};

	type Props = {
		title: string;
		description?: string;
		promise: RemoteQuery<StatisticsMetricResponse>;
		metrics: TrendMetricDefinition[];
		yDomain?: [number, number | null];
		actions?: Snippet;
	};

	let { title, description, promise, metrics, yDomain = [0, null], actions }: Props = $props();

	const createRows = (response: StatisticsMetricResponse): TrendRow[] => {
		const items = "items" in response.result ? response.result.items : [];

		return items
			.map((item) => {
				const bucketStart = typeof item === "object" && item !== null && "bucketStart" in item ? item.bucketStart : null;
				const date = typeof bucketStart === "string" ? new Date(bucketStart) : null;
				if (!date || Number.isNaN(date.getTime())) return null;

				const row: TrendRow = { date };
				for (const metric of metrics) row[metric.key] = metric.value(item as EventTimeSeriesPoint);
				return row;
			})
			.filter((row): row is TrendRow => row !== null)
			.sort((left, right) => left.date.getTime() - right.date.getTime());
	};

	const formatLegendValue = (value: unknown, kind: TrendMetricDefinition["format"]): string => {
		if (typeof value !== "number" || !Number.isFinite(value)) return "-";
		if (kind === "percent") return `${value.toFixed(1)}%`;
		if (kind === "count") return formatCount(value, true);

		return formatMetric(value, kind);
	};
</script>

<DashboardPanel {title} {description} icon={LineChartIcon} {actions}>
	{#if promise.loading}
		<div class="flex min-h-80 flex-col gap-y-3">
			<Skeleton class="h-64 w-full" />
			<Skeleton class="h-5 w-2/3" />
		</div>
	{:else if promise.error}
		<div
			class="border-destructive/30 bg-destructive/10 flex min-h-80 flex-col items-center justify-center gap-2 rounded-lg border text-center"
		>
			<CircleAlert size={32} class="text-destructive" />
			<p class="text-destructive text-sm font-semibold">{promise.error.message}</p>
		</div>
	{:else if promise.current}
		{@const rows = createRows(promise.current)}
		{#if rows.length === 0}
			<div class="border-border bg-secondary/25 flex min-h-80 items-center justify-center rounded-lg border text-center">
				<p class="text-foreground/60 text-sm font-semibold">No metric points available.</p>
			</div>
		{:else}
			<div class="flex flex-col gap-3">
				<div class="bg-secondary/20 min-h-80 overflow-hidden rounded-lg p-2">
					<LineChart
						data={rows}
						x="date"
						series={metrics.map((metric) => ({
							key: metric.key,
							label: metric.label,
							color: metric.color
						}))}
						{yDomain}
						height={310}
						padding={{ top: 16, right: 18, bottom: 34, left: 46 }}
						tooltipContext={{ mode: "bisect-x" }}
					/>
				</div>

				<div class="flex flex-wrap justify-center gap-x-4 gap-y-2">
					{#each metrics as metric (metric.key)}
						{@const lastValue = rows.at(-1)?.[metric.key]}
						<div class="text-foreground/75 flex items-center gap-2 text-xs font-semibold">
							<span class="size-2.5 rounded-full" style={`background-color: ${metric.color};`}></span>
							<span>{metric.label}</span>
							<span class="text-foreground/45 tabular-nums">{formatLegendValue(lastValue, metric.format)}</span>
						</div>
					{/each}
				</div>
				<p class="text-foreground/45 text-center text-xs font-semibold">
					{formatDate(rows[0]?.date)} - {formatDate(rows.at(-1)?.date)}
				</p>
			</div>
		{/if}
	{/if}
</DashboardPanel>
