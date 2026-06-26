<script module lang="ts">
	import type { EventTimeSeriesPoint, JourneyTimeSeriesPoint, StatisticsMetricResponse } from "@lib/api";

	type TrendMetricDefinition = {
		key: string;
		label: string;
		color: string;
		value: (item: EventTimeSeriesPoint | JourneyTimeSeriesPoint) => number | null;
		format: "percent" | "seconds" | "count" | "minutes";
	};

	type TrendPanelDefinition = {
		key: string;
		label?: string;
		description?: string;
		metrics: TrendMetricDefinition[];
		yDomain?: [number, number | null];
		height?: number;
		showXAxis?: boolean;
		showLegend?: boolean;
	};

	export type { TrendMetricDefinition, TrendPanelDefinition };
</script>

<script lang="ts">
	import type { StatisticsBucket } from "@lib/api";
	import Skeleton from "@lib/components/ui/Skeleton.svelte";
	import type { RemoteQuery } from "@sveltejs/kit";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";
	import LineChartIcon from "@lucide/svelte/icons/chart-no-axes-combined";
	import { defaultChartPadding, LineChart, type ChartState } from "layerchart";
	import SeriesTooltip, { type SeriesTooltipItem } from "@lib/components/layerchart/tooltips/SeriesTooltip.svelte";
	import DashboardPanel from "../shared/DashboardPanel.svelte";
	import { formatCount, formatMetric, formatStatisticsBucketLabel } from "../shared/statistics-dashboard";

	type TrendRow = {
		date: Date;
		[key: string]: Date | number | null;
	};

	type Props = {
		title: string;
		description?: string;
		promise: RemoteQuery<StatisticsMetricResponse>;
		panels: TrendPanelDefinition[];
		bucket: StatisticsBucket;
		actions?: import("svelte").Snippet;
	};

	let { title, description, promise, panels, bucket, actions }: Props = $props();

	let chartWidth = $state(0);
	const metrics = $derived(panels.flatMap((panel) => panel.metrics));

	const createRows = (response: StatisticsMetricResponse): TrendRow[] => {
		const items = "items" in response.result ? response.result.items : [];

		return items
			.map((item) => {
				const bucketStart = typeof item === "object" && item !== null && "bucketStart" in item ? item.bucketStart : null;
				const date = typeof bucketStart === "string" ? new Date(bucketStart) : null;
				if (!date || Number.isNaN(date.getTime())) return null;

				const row: TrendRow = { date };
				for (const metric of metrics) row[metric.key] = metric.value(item as EventTimeSeriesPoint | JourneyTimeSeriesPoint);
				return row;
			})
			.filter((row): row is TrendRow => row !== null)
			.sort((left, right) => left.date.getTime() - right.date.getTime());
	};

	const formatTrendValue = (value: unknown, kind: TrendMetricDefinition["format"]): string => {
		if (typeof value !== "number" || !Number.isFinite(value)) return "-";
		if (kind === "percent") return `${value.toFixed(1)}%`;
		if (kind === "count") return formatCount(value, true);

		return formatMetric(value, kind);
	};

	const createTooltipItems = (panelMetrics: TrendMetricDefinition[]): SeriesTooltipItem<TrendRow>[] =>
		panelMetrics.map((metric) => ({
			key: metric.key,
			label: metric.label,
			color: metric.color,
			formatValue: (value) => formatTrendValue(value, metric.format)
		}));

	const showPanelLegend = (panel: TrendPanelDefinition): boolean => panel.showLegend ?? panel.metrics.length > 1;

	const panelPadding = (panel: TrendPanelDefinition) => {
		const showLegend = showPanelLegend(panel);
		const seriesPerRow = chartWidth > 0 && chartWidth < 520 ? 2 : 3;
		const legendRows = showLegend ? Math.max(1, Math.ceil(panel.metrics.length / seriesPerRow)) : 0;
		const extraLegendPadding = Math.max(0, legendRows - 1) * 24;
		const showXAxis = panel.showXAxis ?? true;

		return defaultChartPadding({
			axis: showXAxis ? true : "y",
			legend: showLegend,
			top: 16,
			right: 18,
			bottom: (showXAxis ? 24 : 8) + extraLegendPadding,
			left: 46
		});
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
			<div bind:clientWidth={chartWidth} class="flex flex-col gap-4">
				{#each panels as panel (panel.key)}
					{@const tooltipItems = createTooltipItems(panel.metrics)}
					{@const showLegend = showPanelLegend(panel)}
					{@const showXAxis = panel.showXAxis ?? true}

					<div class="bg-secondary/20 overflow-hidden rounded-lg p-2">
						{#if panel.label || panel.description}
							<div class="px-2 pt-1">
								{#if panel.label}
									<p class="text-foreground text-sm font-bold">{panel.label}</p>
								{/if}
								{#if panel.description}
									<p class="text-foreground/55 mt-0.5 text-xs font-semibold">{panel.description}</p>
								{/if}
							</div>
						{/if}

						<LineChart
							data={rows}
							x="date"
							series={panel.metrics.map((metric) => ({
								key: metric.key,
								label: metric.label,
								color: metric.color
							}))}
							yDomain={panel.yDomain ?? [0, null]}
							height={panel.height ?? 310}
							axis={showXAxis ? true : "y"}
							padding={panelPadding(panel)}
							tooltipContext={{ mode: "bisect-x" }}
							legend={showLegend
								? {
										placement: "bottom",
										classes: {
											root: "w-full px-2 pb-1",
											items: "flex-wrap justify-center gap-x-4 gap-y-1",
											item: "text-xs font-semibold"
										}
									}
								: false}
							props={{
								spline: { strokeWidth: 3 }
							}}
						>
							{#snippet tooltip({ context }: { context: ChartState<TrendRow> })}
								<SeriesTooltip
									{context}
									items={tooltipItems}
									header={(data) => formatStatisticsBucketLabel(data.date, bucket)}
								/>
							{/snippet}
						</LineChart>
					</div>
				{/each}
			</div>
		{/if}
	{/if}
</DashboardPanel>
