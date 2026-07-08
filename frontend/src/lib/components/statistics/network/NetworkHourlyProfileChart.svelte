<script lang="ts">
	import type { EventHeatmapCell, EventMetrics, StatisticsMetricResponse } from "@lib/api";
	import PointTooltip, { type PointTooltipItem } from "@lib/components/layerchart/tooltips/PointTooltip.svelte";
	import Skeleton from "@lib/components/ui/Skeleton.svelte";
	import type { RemoteQuery } from "@sveltejs/kit";
	import CalendarClock from "@lucide/svelte/icons/calendar-clock";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";
	import { Axis, Bars, Chart, Spline, type ChartState } from "layerchart";
	import DashboardPanel from "../shared/DashboardPanel.svelte";
	import MetricOptionControl, { type MetricOption } from "../shared/options/MetricOptionControl.svelte";
	import MetricPerspectiveControl from "../shared/options/MetricPerspectiveControl.svelte";
	import MetricThresholdControl from "../shared/options/MetricThresholdControl.svelte";
	import { formatCount, formatMetric, toNumber } from "../shared/statistics-dashboard";
	import type {
		NetworkHourlyProfileDayGroup,
		NetworkPunctualityPerspective,
		NetworkTransportComparisonThreshold
	} from "./network-context.svelte";

	type Props = {
		promise: RemoteQuery<StatisticsMetricResponse>;
		dayGroup: NetworkHourlyProfileDayGroup;
		ondaygroupchange: (dayGroup: NetworkHourlyProfileDayGroup) => void;
		perspective: NetworkPunctualityPerspective;
		onperspectivechange: (perspective: NetworkPunctualityPerspective) => void;
		threshold: NetworkTransportComparisonThreshold;
		onthresholdchange: (threshold: NetworkTransportComparisonThreshold) => void;
	};

	type HourlyProfileRow = {
		hourIndex: number;
		hour: string;
		endHour: string;
		plannedStops: number;
		servedStops: number;
		cancelledStops: number;
		punctualityRate: number | null;
		cancellationRate: number | null;
		volumeBand: number;
	};

	type HourAggregate = {
		plannedStops: number;
		servedStops: number;
		cancelledStops: number;
		punctualityStops: number;
		punctualityDenominator: number;
	};

	type PunctualityMetricDefinition = {
		perspective: NetworkPunctualityPerspective;
		threshold: NetworkTransportComparisonThreshold;
		lineLabel: string;
		shortLabel: string;
		denominatorLabel: string;
		getRate: (metrics: EventMetrics) => number | null;
	};

	let { promise, dayGroup, ondaygroupchange, perspective, onperspectivechange, threshold, onthresholdchange }: Props = $props();

	const volumeBandCap = 0.3;
	const hours = Array.from({ length: 24 }, (_, hour) => `${hour.toString().padStart(2, "0")}:00`);
	const mobileHourTicks = hours.filter((_, index) => index % 6 === 0);
	const desktopHourTicks = hours.filter((_, index) => index % 3 === 0);
	const dayGroupOptions: MetricOption<NetworkHourlyProfileDayGroup>[] = [
		{ value: "all", label: "All days" },
		{ value: "weekday", label: "Mon-Fri" },
		{ value: "weekend", label: "Weekend" }
	];
	const metricDefinitions: PunctualityMetricDefinition[] = [
		{
			perspective: "customer",
			threshold: "under6",
			lineLabel: "Customer reliable < 6 min",
			shortLabel: "Customer < 6 min",
			denominatorLabel: "planned stops",
			getRate: (metrics) => toNumber(metrics.customerReliability5Rate)
		},
		{
			perspective: "customer",
			threshold: "under15",
			lineLabel: "Customer reliable < 15 min",
			shortLabel: "Customer < 15 min",
			denominatorLabel: "planned stops",
			getRate: (metrics) => toNumber(metrics.customerReliability15Rate)
		},
		{
			perspective: "operative",
			threshold: "under6",
			lineLabel: "Operative punctual < 6 min",
			shortLabel: "Operative < 6 min",
			denominatorLabel: "served stops",
			getRate: (metrics) => toNumber(metrics.operativePunctuality5Rate)
		},
		{
			perspective: "operative",
			threshold: "under15",
			lineLabel: "Operative punctual < 15 min",
			shortLabel: "Operative < 15 min",
			denominatorLabel: "served stops",
			getRate: (metrics) => toNumber(metrics.operativePunctuality15Rate)
		}
	];

	const selectedDayGroup = $derived(dayGroupOptions.find((option) => option.value === dayGroup) ?? dayGroupOptions[0]);
	const selectedMetric = $derived(
		metricDefinitions.find((definition) => definition.perspective === perspective && definition.threshold === threshold) ??
			metricDefinitions[0]
	);
	const panelDescription = $derived(
		perspective === "operative"
			? "Operative punctuality among served stops and cancellations by local hour, with planned stop volume as a subdued background band."
			: "Customer-view reliability and cancellations by local hour, with planned stop volume as a subdued background band."
	);

	const clampRate = (value: number): number => Math.min(1, Math.max(0, value));

	const getCells = (response: StatisticsMetricResponse): EventHeatmapCell[] =>
		"items" in response.result ? (response.result.items as EventHeatmapCell[]) : [];

	const includeWeekday = (weekday: number, group: NetworkHourlyProfileDayGroup): boolean => {
		if (group === "weekday") return weekday >= 1 && weekday <= 5;
		if (group === "weekend") return weekday === 6 || weekday === 7;
		return weekday >= 1 && weekday <= 7;
	};

	const createRows = (
		cells: EventHeatmapCell[],
		group: NetworkHourlyProfileDayGroup,
		metricDefinition: PunctualityMetricDefinition
	): HourlyProfileRow[] => {
		const aggregates: HourAggregate[] = Array.from({ length: 24 }, () => ({
			plannedStops: 0,
			servedStops: 0,
			cancelledStops: 0,
			punctualityStops: 0,
			punctualityDenominator: 0
		}));

		for (const cell of cells) {
			const weekday = Number(cell.weekday);
			const hour = Number(cell.hour);
			if (!Number.isInteger(hour) || hour < 0 || hour > 23 || !includeWeekday(weekday, group)) continue;

			const plannedStops = toNumber(cell.eventMetrics.plannedEvents) ?? 0;
			const servedStops = toNumber(cell.eventMetrics.servedEvents) ?? 0;
			const cancelledStops = toNumber(cell.eventMetrics.cancelledEvents) ?? 0;
			const punctualityRate = metricDefinition.getRate(cell.eventMetrics);
			const punctualityDenominator = metricDefinition.perspective === "customer" ? plannedStops : servedStops;
			const aggregate = aggregates[hour];

			aggregate.plannedStops += plannedStops;
			aggregate.servedStops += servedStops;
			aggregate.cancelledStops += cancelledStops;

			if (punctualityDenominator > 0 && punctualityRate !== null) {
				aggregate.punctualityStops += clampRate(punctualityRate) * punctualityDenominator;
				aggregate.punctualityDenominator += punctualityDenominator;
			}
		}

		const maxPlannedStops = Math.max(0, ...aggregates.map((aggregate) => aggregate.plannedStops));

		return aggregates.map((aggregate, hourIndex) => {
			const hourPrefix = hourIndex.toString().padStart(2, "0");
			const punctualityRate =
				aggregate.punctualityDenominator > 0 ? clampRate(aggregate.punctualityStops / aggregate.punctualityDenominator) : null;
			const cancellationRate = aggregate.plannedStops > 0 ? clampRate(aggregate.cancelledStops / aggregate.plannedStops) : null;

			return {
				hourIndex,
				hour: `${hourPrefix}:00`,
				endHour: `${hourPrefix}:59`,
				plannedStops: aggregate.plannedStops,
				servedStops: aggregate.servedStops,
				cancelledStops: aggregate.cancelledStops,
				punctualityRate,
				cancellationRate,
				volumeBand: maxPlannedStops > 0 ? (aggregate.plannedStops / maxPlannedStops) * volumeBandCap : 0
			};
		});
	};

	const tooltipItems = $derived.by<PointTooltipItem<HourlyProfileRow>[]>(() => [
		{
			key: "punctualityRate",
			label: selectedMetric.lineLabel,
			value: (row) => `${formatMetric(row.punctualityRate, "rate")} of ${selectedMetric.denominatorLabel}`,
			color: "var(--color-accent)"
		},
		{
			key: "cancellationRate",
			label: "Cancelled planned stops",
			value: (row) => `${formatMetric(row.cancellationRate, "rate")} of planned stops`,
			color: "#dc2626"
		},
		{
			key: "plannedStops",
			label: "Planned stops",
			value: (row) => formatCount(row.plannedStops)
		},
		{
			key: "servedStops",
			label: "Served stops",
			value: (row) => formatCount(row.servedStops)
		},
		{
			key: "cancelledStops",
			label: "Cancelled stops",
			value: (row) => formatCount(row.cancelledStops)
		}
	]);
</script>

<DashboardPanel title="Hourly reliability profile" description={panelDescription} icon={CalendarClock}>
	{#snippet actions()}
		<MetricPerspectiveControl value={perspective} onchange={onperspectivechange} />
		<MetricThresholdControl value={threshold} onchange={onthresholdchange} />
		<MetricOptionControl title="Days" value={dayGroup} options={dayGroupOptions} onchange={ondaygroupchange} />
	{/snippet}
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
		{@const rows = createRows(getCells(promise.current), dayGroup, selectedMetric)}
		{@const availableRows = rows.filter((row) => row.plannedStops > 0)}
		{#if availableRows.length === 0}
			<div class="border-border bg-secondary/25 flex min-h-80 items-center justify-center rounded-lg border text-center">
				<p class="text-foreground/60 text-sm font-semibold">No hourly profile data available.</p>
			</div>
		{:else}
			{#snippet profileChart(chartHeight: number, hourTicks: string[])}
				<Chart
					data={rows}
					x="hour"
					y="volumeBand"
					xDomain={hours}
					yDomain={[0, 1]}
					valueAxis="y"
					yBaseline={0}
					height={chartHeight}
					padding={{ top: 18, right: 18, bottom: 36, left: 48 }}
					tooltipContext={{ mode: "band" }}
					grid={false}
					rule={false}
					highlight={false}
				>
					{#snippet axis()}
						<Axis
							placement="left"
							format="percentRound"
							ticks={5}
							tickMarks={false}
							grid={{ opacity: 0.14 }}
							rule={false}
							tickLabelProps={{ class: "text-xs font-semibold" }}
						/>
						<Axis
							placement="bottom"
							ticks={hourTicks}
							tickMarks={false}
							rule={false}
							tickLabelProps={{ class: "text-[10px] font-semibold" }}
						/>
					{/snippet}

					{#snippet belowMarks()}
						<Bars fill="var(--color-foreground)" opacity={0.14} radius={3} rounded="top" />
					{/snippet}

					{#snippet marks()}
						<Spline
							y={(row: HourlyProfileRow) => row.punctualityRate ?? 0}
							defined={(row: HourlyProfileRow) => row.punctualityRate !== null}
							stroke="var(--color-accent)"
							strokeWidth={3}
							fill="none"
						/>
						<Spline
							y={(row: HourlyProfileRow) => row.cancellationRate ?? 0}
							defined={(row: HourlyProfileRow) => row.cancellationRate !== null}
							stroke="#dc2626"
							strokeWidth={2.5}
							fill="none"
							opacity={0.92}
						/>
					{/snippet}

					{#snippet tooltip({ context }: { context: ChartState<HourlyProfileRow> })}
						<PointTooltip {context} items={tooltipItems} header={(row) => `${row.hour}-${row.endHour}`} />
					{/snippet}
				</Chart>
			{/snippet}

			<div
				class="bg-secondary/20 rounded-lg p-2"
				role="img"
				aria-label={`Hourly ${selectedMetric.shortLabel} profile for ${selectedDayGroup.label}`}
			>
				<div class="sm:hidden">{@render profileChart(270, mobileHourTicks)}</div>
				<div class="hidden sm:block">{@render profileChart(320, desktopHourTicks)}</div>
			</div>

			<div class="text-foreground/60 flex flex-wrap items-center justify-center gap-x-5 gap-y-2 text-xs font-semibold">
				<span class="inline-flex items-center gap-2">
					<span class="bg-accent size-2.5 rounded-full"></span>
					{selectedMetric.shortLabel}
				</span>
				<span class="inline-flex items-center gap-2">
					<span class="size-2.5 rounded-full bg-red-600"></span>
					Cancellation rate
				</span>
				<span class="inline-flex items-center gap-2">
					<span class="bg-foreground/30 h-2.5 w-5 rounded-sm"></span>
					Planned stop volume
				</span>
				<span
					>{formatCount(
						availableRows.reduce((sum, row) => sum + row.plannedStops, 0),
						true
					)} planned stops</span
				>
			</div>
		{/if}
	{/if}
</DashboardPanel>
