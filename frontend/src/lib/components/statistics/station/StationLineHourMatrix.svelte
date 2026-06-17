<script lang="ts">
	import type { LineHourMatrixItem, StatisticsMetricResponse } from "@lib/api";
	import Skeleton from "@lib/components/ui/Skeleton.svelte";
	import type { RemoteQuery } from "@sveltejs/kit";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";
	import Grid2x2 from "@lucide/svelte/icons/grid-2x2";
	import { ScatterChart } from "layerchart";
	import DashboardPanel from "../shared/DashboardPanel.svelte";
	import { formatCount, formatMetric, toNumber, transportTypeShortLabel } from "../shared/statistics-dashboard";

	type Props = {
		promise: RemoteQuery<StatisticsMetricResponse>;
	};

	type MatrixLine = {
		key: string;
		label: string;
		transportType: string;
		totalEvents: number;
	};

	type MatrixPoint = {
		id: string;
		line: string;
		hour: string;
		reliability: number;
		plannedEvents: number;
	};

	let { promise }: Props = $props();

	const hours = Array.from({ length: 24 }, (_, hour) => `${hour.toString().padStart(2, "0")}:00`);

	const getItems = (response: StatisticsMetricResponse): LineHourMatrixItem[] =>
		"items" in response.result ? (response.result.items as LineHourMatrixItem[]) : [];

	const createLines = (items: LineHourMatrixItem[]): MatrixLine[] => {
		const lines = new Map<string, MatrixLine>();

		for (const item of items) {
			const key = `${item.lineName}-${item.transportType}`;
			const current = lines.get(key) ?? {
				key,
				label: item.lineName,
				transportType: item.transportType,
				totalEvents: 0
			};
			current.totalEvents += Number(item.eventMetrics.plannedEvents ?? 0);
			lines.set(key, current);
		}

		return [...lines.values()].sort((left, right) => right.totalEvents - left.totalEvents).slice(0, 8);
	};

	const createPoints = (items: LineHourMatrixItem[], lines: MatrixLine[]): MatrixPoint[] =>
		items
			.filter((item) => lines.some((line) => line.label === item.lineName && line.transportType === item.transportType))
			.map((item) => ({
				id: `${item.lineName}-${item.transportType}-${item.hour}`,
				line: `${transportTypeShortLabel(item.transportType)} ${item.lineName}`,
				hour: `${Number(item.hour).toString().padStart(2, "0")}:00`,
				reliability: toNumber(item.eventMetrics.customerReliability5Rate) ?? 0,
				plannedEvents: toNumber(item.eventMetrics.plannedEvents) ?? 0
			}));
</script>

<DashboardPanel title="Line by hour" description="Observed station-line quality by local hour." icon={Grid2x2}>
	{#if promise.loading}
		<div class="flex min-h-80 flex-col gap-y-3">
			<Skeleton class="h-72 w-full" />
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
		{@const items = getItems(promise.current)}
		{@const lines = createLines(items)}
		{@const points = createPoints(items, lines)}
		{#if points.length === 0}
			<div class="border-border bg-secondary/25 flex min-h-80 items-center justify-center rounded-lg border text-center">
				<p class="text-foreground/60 text-sm font-semibold">No line-hour data available.</p>
			</div>
		{:else}
			<div class="bg-secondary/20 min-h-80 overflow-x-auto rounded-lg p-2">
				<div class="min-w-[54rem]">
					<ScatterChart
						data={points}
						x="hour"
						y="line"
						r="plannedEvents"
						c="reliability"
						xDomain={hours}
						yDomain={lines.map((line) => `${transportTypeShortLabel(line.transportType)} ${line.label}`)}
						rRange={[4, 15]}
						cDomain={[0, 1]}
						cRange={["#dc2626", "#ca8a04", "#16a34a"]}
						height={330}
						padding={{ top: 16, right: 20, bottom: 40, left: 116 }}
						tooltipContext={{ mode: "quadtree" }}
					/>
				</div>
			</div>
			<p class="text-foreground/55 text-xs font-semibold">
				{formatCount(
					points.reduce((sum, point) => sum + point.plannedEvents, 0),
					true
				)} events | color shows
				{formatMetric(points.reduce((sum, point) => sum + point.reliability, 0) / Math.max(1, points.length), "rate")} average reliability
			</p>
		{/if}
	{/if}
</DashboardPanel>
