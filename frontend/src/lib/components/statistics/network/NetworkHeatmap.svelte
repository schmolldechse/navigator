<script lang="ts">
	import type { EventHeatmapCell, StatisticsMetricResponse } from "@lib/api";
	import PointTooltip, { type PointTooltipItem } from "@lib/components/layerchart/tooltips/PointTooltip.svelte";
	import Skeleton from "@lib/components/ui/Skeleton.svelte";
	import type { RemoteQuery } from "@sveltejs/kit";
	import CalendarDays from "@lucide/svelte/icons/calendar-days";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";
	import { Axis, Cell, Chart, Layer, type ChartState } from "layerchart";
	import DashboardPanel from "../shared/DashboardPanel.svelte";
	import MetricOptionControl from "../shared/options/MetricOptionControl.svelte";
	import type { NetworkHeatmapMetric } from "./network-context.svelte";
	import { formatCount, formatMetric, toNumber } from "../shared/statistics-dashboard";

	type Props = {
		promise: RemoteQuery<StatisticsMetricResponse>;
		metric: NetworkHeatmapMetric;
		onmetricchange: (metric: NetworkHeatmapMetric) => void;
	};

	type HeatmapPoint = {
		id: string;
		weekday: string;
		hour: string;
		weekdayLabel: string;
		value: number | null;
		plannedEvents: number;
		color: string;
		label: string;
	};

	type HeatmapMetricOption = {
		value: NetworkHeatmapMetric;
		label: string;
		format: "rate" | "count";
		getValue: (cell: EventHeatmapCell) => number | null;
	};

	let { promise, metric, onmetricchange }: Props = $props();

	const weekdays = [
		{ value: 1, label: "Mon", fullLabel: "Monday" },
		{ value: 2, label: "Tue", fullLabel: "Tuesday" },
		{ value: 3, label: "Wed", fullLabel: "Wednesday" },
		{ value: 4, label: "Thu", fullLabel: "Thursday" },
		{ value: 5, label: "Fri", fullLabel: "Friday" },
		{ value: 6, label: "Sat", fullLabel: "Saturday" },
		{ value: 7, label: "Sun", fullLabel: "Sunday" }
	];
	const hours = Array.from({ length: 24 }, (_, hour) => `${hour.toString().padStart(2, "0")}:00`);
	const mobileHourTicks = hours.filter((_, index) => index % 6 === 0);
	const desktopHourTicks = hours.filter((_, index) => index % 3 === 0);
	const yDomain = weekdays.map((weekday) => weekday.label);
	const metricOptions: HeatmapMetricOption[] = [
		{
			value: "reliability5",
			label: "Customer < 6 min",
			format: "rate",
			getValue: (cell) => toNumber(cell.eventMetrics.customerReliability5Rate)
		},
		{
			value: "reliability15",
			label: "Customer < 15 min",
			format: "rate",
			getValue: (cell) => toNumber(cell.eventMetrics.customerReliability15Rate)
		},
		{
			value: "operative5",
			label: "Operative < 6 min",
			format: "rate",
			getValue: (cell) => toNumber(cell.eventMetrics.operativePunctuality5Rate)
		},
		{
			value: "operative15",
			label: "Operative < 15 min",
			format: "rate",
			getValue: (cell) => toNumber(cell.eventMetrics.operativePunctuality15Rate)
		},
		{
			value: "cancellation",
			label: "Cancellation rate",
			format: "rate",
			getValue: (cell) => toNumber(cell.eventMetrics.cancellationRate)
		},
		{
			value: "plannedStops",
			label: "Planned stops",
			format: "count",
			getValue: (cell) => toNumber(cell.eventMetrics.plannedEvents)
		}
	];
	const selectedMetric = $derived(metricOptions.find((option) => option.value === metric) ?? metricOptions[0]);
	const reliabilityColors = [
		"var(--color-secondary)",
		"color-mix(in oklch, var(--color-accent) 28%, var(--color-secondary))",
		"color-mix(in oklch, var(--color-accent) 58%, var(--color-secondary))",
		"var(--color-accent)"
	];
	const cancellationColors = [
		"var(--color-secondary)",
		"color-mix(in oklch, var(--color-destructive) 28%, var(--color-secondary))",
		"color-mix(in oklch, var(--color-destructive) 58%, var(--color-secondary))",
		"var(--color-destructive)"
	];
	const legendColors = $derived(selectedMetric.value === "cancellation" ? cancellationColors : reliabilityColors);

	const getCells = (response: StatisticsMetricResponse): EventHeatmapCell[] =>
		"items" in response.result ? (response.result.items as EventHeatmapCell[]) : [];

	const formatHeatmapValue = (value: number | null, kind: HeatmapMetricOption["format"]): string => {
		if (value === null) return "No data";
		return kind === "rate" ? formatMetric(value, "rate") : formatCount(value, true);
	};

	const getColor = (value: number | null, option: HeatmapMetricOption, values: number[]): string => {
		if (value === null) return "var(--color-secondary)";
		if (
			option.value === "reliability5" ||
			option.value === "reliability15" ||
			option.value === "operative5" ||
			option.value === "operative15"
		) {
			if (value >= 0.9) return reliabilityColors[3];
			if (value >= 0.8) return reliabilityColors[2];
			if (value >= 0.7) return reliabilityColors[1];
			return reliabilityColors[0];
		}
		if (option.value === "cancellation") {
			if (value <= 0.01) return cancellationColors[0];
			if (value <= 0.02) return cancellationColors[1];
			if (value <= 0.05) return cancellationColors[2];
			return cancellationColors[3];
		}

		const min = Math.min(...values);
		const max = Math.max(...values);
		const ratio = max === min ? 0.5 : (value - min) / (max - min);
		if (ratio >= 0.75) return reliabilityColors[3];
		if (ratio >= 0.5) return reliabilityColors[2];
		if (ratio >= 0.25) return reliabilityColors[1];
		return reliabilityColors[0];
	};

	const createPoints = (cells: EventHeatmapCell[], option: HeatmapMetricOption): HeatmapPoint[] => {
		const byCell = new Map(cells.map((cell) => [`${Number(cell.weekday)}-${Number(cell.hour)}`, cell]));
		const values = cells.map((cell) => option.getValue(cell)).filter((value): value is number => value !== null);

		return weekdays.flatMap((weekday) =>
			hours.map((hour, hourIndex) => {
				const cell = byCell.get(`${weekday.value}-${hourIndex}`);
				const value = cell ? option.getValue(cell) : null;
				const plannedEvents = cell ? (toNumber(cell.eventMetrics.plannedEvents) ?? 0) : 0;
				const formatted = formatHeatmapValue(value, option.format);

				return {
					id: `${weekday.value}-${hourIndex}`,
					weekday: weekday.label,
					hour,
					weekdayLabel: weekday.fullLabel,
					value,
					plannedEvents,
					color: getColor(value, option, values),
					label: `${weekday.label} ${hour}: ${formatted}${cell ? `, ${formatCount(plannedEvents, true)} planned stops` : ""}`
				};
			})
		);
	};

	const tooltipItems = $derived.by<PointTooltipItem<HeatmapPoint>[]>(() => {
		const metricItem: PointTooltipItem<HeatmapPoint> = {
			key: "metric",
			label: selectedMetric.label,
			value: (point) => formatHeatmapValue(point.value, selectedMetric.format),
			color: (point) => point.color
		};

		if (selectedMetric.value === "plannedStops") return [metricItem];

		return [
			metricItem,
			{
				key: "plannedStops",
				label: "Planned stops",
				value: (point) => formatCount(point.plannedEvents, true),
				color: "var(--color-foreground)"
			}
		];
	});
</script>

<DashboardPanel
	title="Weekday and hour"
	description="Customer reliability, operative punctuality, cancellations and planned stop volume by local weekday and hour."
	icon={CalendarDays}
>
	{#snippet actions()}
		<MetricOptionControl value={metric} options={metricOptions} onchange={onmetricchange} />
	{/snippet}
	{#if promise.loading}
		<div class="flex min-h-64 flex-col gap-y-3 sm:min-h-72">
			<Skeleton class="h-56 w-full sm:h-64" />
			<Skeleton class="h-5 w-2/3" />
		</div>
	{:else if promise.error}
		<div
			class="border-destructive/30 bg-destructive/10 flex min-h-64 flex-col items-center justify-center gap-2 rounded-lg border text-center sm:min-h-72"
		>
			<CircleAlert size={32} class="text-destructive" />
			<p class="text-destructive text-sm font-semibold">{promise.error.message}</p>
		</div>
	{:else if promise.current}
		{@const points = createPoints(getCells(promise.current), selectedMetric)}
		{@const availablePoints = points.filter((point) => point.value !== null)}
		{#if availablePoints.length === 0}
			<div
				class="border-border bg-secondary/25 flex min-h-64 items-center justify-center rounded-lg border text-center sm:min-h-72"
			>
				<p class="text-foreground/60 text-sm font-semibold">No heatmap cells available.</p>
			</div>
		{:else}
			{#snippet heatmapChart(chartHeight: number, hourTicks: string[])}
				<Chart
					data={points}
					x="hour"
					y="weekday"
					xDomain={hours}
					{yDomain}
					height={chartHeight}
					padding={{ top: 12, right: 8, bottom: 32, left: 42 }}
					tooltipContext={{ mode: "bounds" }}
				>
					{#snippet children({ context }: { context: ChartState<HeatmapPoint> })}
						<Layer type="svg">
							<Axis placement="left" tickMarks={false} rule={false} tickLabelProps={{ class: "text-xs font-semibold" }} />
							<Axis
								placement="bottom"
								ticks={hourTicks}
								tickMarks={false}
								rule={false}
								tickLabelProps={{ class: "text-[10px] font-semibold" }}
							/>
							<Cell x="hour" y="weekday" fill={(point: HeatmapPoint) => point.color} insets={{ x: 1.5, y: 1.5 }} corners={4} />
						</Layer>
						<PointTooltip
							{context}
							items={tooltipItems}
							header={(point) => `${point.weekdayLabel} | ${point.hour}-${point.hour.slice(0, 2)}:59`}
						/>
					{/snippet}
				</Chart>
			{/snippet}

			<div class="bg-secondary/20 rounded-lg p-2" role="img" aria-label={`${selectedMetric.label} by weekday and hour`}>
				<div class="sm:hidden">{@render heatmapChart(230, mobileHourTicks)}</div>
				<div class="hidden sm:block">{@render heatmapChart(290, desktopHourTicks)}</div>
				<div class="text-foreground/55 mt-1 flex items-center gap-2 px-1 text-[11px] font-semibold">
					<span>Lower</span>
					<span class="flex min-w-20 flex-1 gap-1" aria-hidden="true">
						{#each legendColors as color, index (`${selectedMetric.value}-${index}`)}
							<span class="h-2 flex-1 rounded-sm" style:background={color}></span>
						{/each}
					</span>
					<span>Higher</span>
				</div>
			</div>
		{/if}
	{/if}
</DashboardPanel>
