<script lang="ts">
	import type { EventHeatmapCell, StatisticsMetricResponse } from "@lib/api";
	import Skeleton from "@lib/components/ui/Skeleton.svelte";
	import type { RemoteQuery } from "@sveltejs/kit";
	import CalendarDays from "@lucide/svelte/icons/calendar-days";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";
	import { ScatterChart } from "layerchart";
	import DashboardPanel from "../shared/DashboardPanel.svelte";
	import { formatCount, formatMetric, toNumber } from "../shared/statistics-dashboard";

	type Props = {
		promise: RemoteQuery<StatisticsMetricResponse>;
	};

	type HeatmapPoint = {
		id: string;
		weekday: string;
		hour: string;
		reliability: number;
		plannedEvents: number;
	};

	let { promise }: Props = $props();

	const weekdays = [
		{ value: 1, label: "Mon" },
		{ value: 2, label: "Tue" },
		{ value: 3, label: "Wed" },
		{ value: 4, label: "Thu" },
		{ value: 5, label: "Fri" },
		{ value: 6, label: "Sat" },
		{ value: 7, label: "Sun" }
	];
	const hours = Array.from({ length: 24 }, (_, hour) => `${hour.toString().padStart(2, "0")}:00`);

	const getCells = (response: StatisticsMetricResponse): EventHeatmapCell[] =>
		"items" in response.result ? (response.result.items as EventHeatmapCell[]) : [];

	const createPoints = (cells: EventHeatmapCell[]): HeatmapPoint[] =>
		cells.map((cell) => ({
			id: `${cell.weekday}-${cell.hour}`,
			weekday: weekdays.find((item) => item.value === Number(cell.weekday))?.label ?? String(cell.weekday),
			hour: `${Number(cell.hour).toString().padStart(2, "0")}:00`,
			reliability: toNumber(cell.eventMetrics.customerReliability5Rate) ?? 0,
			plannedEvents: toNumber(cell.eventMetrics.plannedEvents) ?? 0
		}));
</script>

<DashboardPanel title="Weekday and hour" description="Station reliability by local weekday and hour." icon={CalendarDays}>
	{#if promise.loading}
		<div class="flex min-h-72 flex-col gap-y-3">
			<Skeleton class="h-64 w-full" />
			<Skeleton class="h-5 w-2/3" />
		</div>
	{:else if promise.error}
		<div
			class="border-destructive/30 bg-destructive/10 flex min-h-72 flex-col items-center justify-center gap-2 rounded-lg border text-center"
		>
			<CircleAlert size={32} class="text-destructive" />
			<p class="text-destructive text-sm font-semibold">{promise.error.message}</p>
		</div>
	{:else if promise.current}
		{@const points = createPoints(getCells(promise.current))}
		{#if points.length === 0}
			<div class="border-border bg-secondary/25 flex min-h-72 items-center justify-center rounded-lg border text-center">
				<p class="text-foreground/60 text-sm font-semibold">No heatmap cells available.</p>
			</div>
		{:else}
			<div class="bg-secondary/20 min-h-72 overflow-x-auto rounded-lg p-2">
				<div class="min-w-[48rem]">
					<ScatterChart
						data={points}
						x="hour"
						y="weekday"
						r="plannedEvents"
						c="reliability"
						xDomain={hours}
						yDomain={weekdays.map((weekday) => weekday.label)}
						rRange={[4, 15]}
						cDomain={[0, 1]}
						cRange={["#dc2626", "#ca8a04", "#16a34a"]}
						height={300}
						padding={{ top: 16, right: 20, bottom: 40, left: 48 }}
						tooltipContext={{ mode: "quadtree" }}
					/>
				</div>
			</div>
			<div class="text-foreground/55 flex items-center justify-between text-xs font-semibold">
				<span>Lower reliability</span>
				<span
					>{formatMetric(
						points.reduce((sum, point) => sum + point.plannedEvents, 0),
						"count",
						true
					)} events</span
				>
				<span>Higher reliability</span>
			</div>
		{/if}
	{/if}
</DashboardPanel>
