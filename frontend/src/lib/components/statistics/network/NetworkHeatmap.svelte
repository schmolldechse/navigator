<script lang="ts">
	import type { EventHeatmapCell, StatisticsMetricResponse } from "@lib/api";
	import Skeleton from "@lib/components/ui/Skeleton.svelte";
	import ToggleGroup from "@lib/components/ui/toggle-group/ToggleGroup.svelte";
	import ToggleGroupItem from "@lib/components/ui/toggle-group/ToggleGroupItem.svelte";
	import type { RemoteQuery } from "@sveltejs/kit";
	import CalendarDays from "@lucide/svelte/icons/calendar-days";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";
	import { ScatterChart } from "layerchart";
	import DashboardPanel from "../shared/DashboardPanel.svelte";
	import { formatCount, formatMetric, toNumber, type NetworkHeatmapMetric } from "../shared/statistics-dashboard";

	type Props = {
		promise: RemoteQuery<StatisticsMetricResponse>;
		metric: NetworkHeatmapMetric;
		onmetricchange: (metric: NetworkHeatmapMetric) => void;
	};

	type HeatmapPoint = {
		id: string;
		weekday: string;
		hour: string;
		value: number;
		plannedEvents: number;
		label: string;
	};

	type HeatmapMetricOption = {
		value: NetworkHeatmapMetric;
		label: string;
		format: "rate" | "count";
		getValue: (cell: EventHeatmapCell) => number;
	};

	let { promise, metric, onmetricchange }: Props = $props();

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
	const metricOptions: HeatmapMetricOption[] = [
		{
			value: "reliability5",
			label: "Reliable < 6 min",
			format: "rate",
			getValue: (cell) => toNumber(cell.eventMetrics.customerReliability5Rate) ?? 0
		},
		{
			value: "reliability15",
			label: "Reliable < 15 min",
			format: "rate",
			getValue: (cell) => toNumber(cell.eventMetrics.customerReliability15Rate) ?? 0
		},
		{
			value: "cancellation",
			label: "Cancellation rate",
			format: "rate",
			getValue: (cell) => toNumber(cell.eventMetrics.cancellationRate) ?? 0
		},
		{
			value: "plannedStops",
			label: "Planned stops",
			format: "count",
			getValue: (cell) => toNumber(cell.eventMetrics.plannedEvents) ?? 0
		}
	];
	const selectedMetric = $derived(metricOptions.find((option) => option.value === metric) ?? metricOptions[0]);

	const getCells = (response: StatisticsMetricResponse): EventHeatmapCell[] =>
		"items" in response.result ? (response.result.items as EventHeatmapCell[]) : [];

	const formatHeatmapValue = (value: number, kind: HeatmapMetricOption["format"]): string =>
		kind === "rate" ? formatMetric(value, "rate") : formatCount(value, true);

	const createPoints = (cells: EventHeatmapCell[], option: HeatmapMetricOption): HeatmapPoint[] =>
		cells.map((cell) => {
			const weekday = weekdays.find((item) => item.value === Number(cell.weekday))?.label ?? String(cell.weekday);
			const hour = `${Number(cell.hour).toString().padStart(2, "0")}:00`;
			const value = option.getValue(cell);
			const plannedEvents = toNumber(cell.eventMetrics.plannedEvents) ?? 0;

			return {
				id: `${weekday}-${hour}`,
				weekday,
				hour,
				value,
				plannedEvents,
				label: `${weekday} ${hour}: ${formatHeatmapValue(value, option.format)}, ${formatCount(plannedEvents, true)} planned stops`
			};
		});
</script>

<DashboardPanel title="Weekday and hour" description="Stop-event quality by local weekday and hour." icon={CalendarDays}>
	{#snippet actions()}
		<ToggleGroup
			mode="single"
			allowEmpty={false}
			selected={selectedMetric}
			keyFn={(option: HeatmapMetricOption) => option.value}
			onselect={(option: HeatmapMetricOption | undefined) => onmetricchange(option?.value ?? "reliability5")}
			class="gap-1"
		>
			{#each metricOptions as option (option.value)}
				<ToggleGroupItem
					item={option}
					class="data-active:border-accent data-active:bg-accent data-active:text-accent-foreground px-2.5 py-1.5 text-xs font-semibold"
				>
					{option.label}
				</ToggleGroupItem>
			{/each}
		</ToggleGroup>
	{/snippet}
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
		{@const points = createPoints(getCells(promise.current), selectedMetric)}
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
						c="value"
						xDomain={hours}
						yDomain={weekdays.map((weekday) => weekday.label)}
						rRange={[4, 15]}
						cDomain={selectedMetric.format === "rate" ? [0, 1] : undefined}
						cRange={["#e5e7eb", "#334155"]}
						height={300}
						padding={{ top: 16, right: 20, bottom: 40, left: 48 }}
						tooltipContext={{ mode: "quadtree" }}
						legend={{
							title: selectedMetric.label,
							placement: "bottom",
							classes: { root: "justify-center pt-2", title: "text-xs font-semibold" }
						}}
						props={{
							tooltip: {
								root: { class: "bg-background border-border rounded-lg border-2 px-2 py-1 shadow-xl" },
								item: { class: "text-xs font-semibold" },
								header: { class: "text-xs font-bold" },
								hideTotal: true
							}
						}}
					/>
				</div>
			</div>
		{/if}
	{/if}
</DashboardPanel>
