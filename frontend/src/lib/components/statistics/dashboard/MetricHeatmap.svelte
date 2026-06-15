<script lang="ts">
	import type { EventHeatmapCell, StatisticsMetricResponse } from "@lib/api";
	import Skeleton from "@lib/components/ui/Skeleton.svelte";
	import CalendarDays from "@lucide/svelte/icons/calendar-days";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";
	import DashboardPanel from "./DashboardPanel.svelte";
	import { formatCount, formatMetric, rateToPercent, toNumber } from "./statistics-dashboard";

	type Props = {
		title: string;
		description?: string;
		promise: Promise<StatisticsMetricResponse>;
	};

	let { title, description, promise }: Props = $props();

	const weekdays = [
		{ value: 1, label: "Mon" },
		{ value: 2, label: "Tue" },
		{ value: 3, label: "Wed" },
		{ value: 4, label: "Thu" },
		{ value: 5, label: "Fri" },
		{ value: 6, label: "Sat" },
		{ value: 7, label: "Sun" }
	];
	const hours = Array.from({ length: 24 }, (_, hour) => hour);

	const getCells = (response: StatisticsMetricResponse): EventHeatmapCell[] =>
		"items" in response.result ? (response.result.items as EventHeatmapCell[]) : [];

	const getCell = (cells: EventHeatmapCell[], weekday: number, hour: number) =>
		cells.find((cell) => Number(cell.weekday) === weekday && Number(cell.hour) === hour);

	const colorForRate = (rate: number | null): string => {
		if (rate === null) return "rgba(148, 163, 184, 0.18)";
		const hue = Math.max(0, Math.min(125, rate * 125));
		const alpha = 0.25 + Math.min(0.6, Math.max(0, rate) * 0.6);

		return `hsl(${hue} 66% 42% / ${alpha})`;
	};

	const tooltipForCell = (cell: EventHeatmapCell | undefined): string => {
		if (!cell) return "No events";

		return [
			`${formatMetric(cell.eventMetrics.customerReliability5Rate, "rate")} reliable <= 5:59`,
			`${formatCount(cell.eventMetrics.plannedEvents, true)} planned`,
			`${formatMetric(cell.eventMetrics.cancellationRate, "rate")} cancelled`
		].join(" | ");
	};
</script>

<DashboardPanel {title} {description} icon={CalendarDays}>
	{#await promise}
		<div class="flex min-h-72 flex-col gap-y-3">
			<Skeleton class="h-64 w-full" />
			<Skeleton class="h-5 w-2/3" />
		</div>
	{:then response}
		{@const cells = getCells(response)}
		{#if cells.length === 0}
			<div class="border-border bg-secondary/25 flex min-h-72 items-center justify-center rounded-lg border text-center">
				<p class="text-foreground/60 text-sm font-semibold">No heatmap cells available.</p>
			</div>
		{:else}
			<div class="overflow-x-auto">
				<div class="grid min-w-[48rem] gap-1" style="grid-template-columns: 3rem repeat(24, minmax(0, 1fr));">
					<div></div>
					{#each hours as hour (hour)}
						<div class="text-foreground/45 text-center text-[0.65rem] font-semibold tabular-nums">
							{hour % 3 === 0 ? hour : ""}
						</div>
					{/each}

					{#each weekdays as weekday (weekday.value)}
						<div class="text-foreground/60 flex items-center text-xs font-semibold">{weekday.label}</div>
						{#each hours as hour (hour)}
							{@const cell = getCell(cells, weekday.value, hour)}
							{@const rate = rateToPercent(cell?.eventMetrics.customerReliability5Rate)}
							<div
								class="border-border/70 h-7 rounded border"
								title={tooltipForCell(cell)}
								style={`background-color: ${colorForRate(rate === null ? null : rate / 100)};`}
							>
								<span class="sr-only">
									{weekday.label}
									{hour}:00,
									{cell ? formatMetric(cell.eventMetrics.customerReliability5Rate, "rate") : "no data"}
								</span>
							</div>
						{/each}
					{/each}
				</div>
			</div>

			<div class="text-foreground/55 flex items-center justify-between text-xs font-semibold">
				<span>Lower reliability</span>
				<span>Higher reliability</span>
			</div>
		{/if}
	{:catch error}
		<div
			class="border-destructive/30 bg-destructive/10 flex min-h-72 flex-col items-center justify-center gap-2 rounded-lg border text-center"
		>
			<CircleAlert size={32} class="text-destructive" />
			<p class="text-destructive text-sm font-semibold">{error.message}</p>
		</div>
	{/await}
</DashboardPanel>
