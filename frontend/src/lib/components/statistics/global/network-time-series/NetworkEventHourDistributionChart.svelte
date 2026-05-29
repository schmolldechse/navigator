<script lang="ts">
	import { MetricUnit, ScheduleType, type MetricSeries } from "@lib/api";
	import Card from "@lib/components/ui/card/Card.svelte";
	import Skeleton from "@lib/components/ui/Skeleton.svelte";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";
	import { Axis, BarChart, Cell, Chart, Tooltip } from "layerchart";
	import { formatMetricValue } from "../../metric-format";
	import { DateTime } from "luxon";
	import ToggleGroup from "@lib/components/ui/toggle-group/ToggleGroup.svelte";
	import ToggleGroupItem from "@lib/components/ui/toggle-group/ToggleGroupItem.svelte";

	type Props = {
		promise: Promise<MetricSeries>;
		scheduleType: ScheduleType;
	};
	let { promise, scheduleType }: Props = $props();

	type HourRow = {
		hour: number;
		value: number;
		label: string;
	};

	type ViewMode = {
		id: "hour" | "weekday-hour";
		label: string;
	};

	type WeekdayHourCell = {
		key: string;
		weekday: number;
		weekdayLabel: string;
		hour: number;
		hourLabel: string;
		value: number;
		label: string;
	};

	const viewModes: ViewMode[] = [
		{ id: "hour", label: "Hour" },
		{ id: "weekday-hour", label: "Weekday x hour" }
	];
	let selectedViewMode: ViewMode = $state(viewModes[1]);

	const getWeekdayLabel = (weekday: number) =>
		DateTime.local()
			.startOf("week")
			.plus({ days: weekday - 1 })
			.toFormat("ccc");

	const hourLabels = Array.from({ length: 24 }, (_, hour) => hour.toString().padStart(2, "0"));
	const weekdayLabels = Array.from({ length: 7 }, (_, index) => getWeekdayLabel(index + 1));

	const getCellColor = (value: number, maxValue: number) => {
		const intensity = Math.min(1, Math.max(0, value / maxValue));
		const alpha = 0.14 + intensity * 0.82;
		return `color-mix(in srgb, var(--color-accent) ${Math.round(alpha * 100)}%, transparent)`;
	};

	const createRows = (metric: MetricSeries) => {
		const startOfDay = DateTime.local().startOf("day");
		const rows: HourRow[] = Array.from({ length: 24 }, (_, hour) => {
			const start = startOfDay.plus({ hours: hour });
			const end = start.plus({ hours: 1 });

			return {
				hour: start.hour,
				value: 0,
				label: `${start.toFormat("HH:mm")} - ${end.toFormat("HH:mm")}`
			};
		});

		for (const dataPoint of metric.dataPoints) {
			if (!("timestamp" in dataPoint)) continue;

			const timestamp = new Date(dataPoint.timestamp);
			const hour = timestamp.getHours();
			if (!Number.isFinite(hour)) continue;

			rows[hour].value += Number(dataPoint.value);
		}
		return rows;
	};

	const createWeekdayHourCells = (metric: MetricSeries) => {
		const startOfDay = DateTime.local().startOf("day");
		const cells: WeekdayHourCell[] = Array.from({ length: 7 * 24 }, (_, index) => {
			const weekday = Math.floor(index / 24) + 1;
			const hour = index % 24;
			return {
				key: `${weekday}-${hour}`,
				weekday,
				weekdayLabel: getWeekdayLabel(weekday),
				hour,
				hourLabel: hour.toString().padStart(2, "0"),
				value: 0,
				label: `${getWeekdayLabel(weekday)} ${startOfDay.plus({ hours: hour }).toFormat("HH:mm")}`
			};
		});

		for (const dataPoint of metric.dataPoints) {
			if (!("timestamp" in dataPoint)) continue;

			const timestamp = DateTime.fromJSDate(new Date(dataPoint.timestamp));
			if (!timestamp.isValid) continue;

			const index = (timestamp.weekday - 1) * 24 + timestamp.hour;
			cells[index].value += Number(dataPoint.value);
		}

		return cells;
	};

	const eventLabel = $derived(scheduleType === ScheduleType.ARRIVAL ? "Arrivals" : "Departures");
	const series = $derived([
		{
			key: "value",
			label: eventLabel,
			value: (row: HourRow) => row.value,
			color: "var(--color-accent)"
		}
	]);
</script>

<Card class="gap-y-4">
	<div class="flex flex-col gap-3 sm:flex-row sm:items-start sm:justify-between">
		<div>
			<p class="text-foreground/60 text-xs font-medium">Daily shape</p>
			<h3 class="text-foreground text-base font-semibold">{eventLabel} by hour</h3>
		</div>

		<ToggleGroup
			mode="single"
			allowEmpty={false}
			selected={selectedViewMode}
			keyFn={(option: ViewMode) => option.id}
			onselect={(option: ViewMode | undefined) => {
				if (option) selectedViewMode = option;
			}}
			class="gap-1"
		>
			{#each viewModes as option (option.id)}
				<ToggleGroupItem
					item={option}
					class="enabled:hover:bg-accent/15 enabled:hover:text-accent data-active:border-accent data-active:bg-accent data-active:text-accent-foreground data-active:hover:bg-accent data-active:hover:text-accent-foreground px-2 py-1 text-xs font-semibold transition-colors"
				>
					{option.label}
				</ToggleGroupItem>
			{/each}
		</ToggleGroup>
	</div>

	{#await promise}
		<div class="flex min-h-80 flex-col gap-y-3">
			<Skeleton class="h-64 w-full" />
			<Skeleton class="h-5 w-2/3" />
		</div>
	{:then metric}
		{@const rows = createRows(metric)}

		{#if selectedViewMode.id === "hour"}
			<BarChart
				data={rows}
				{series}
				x="hour"
				yDomain={[0, null]}
				height={320}
				padding={{ left: 52, bottom: 32 }}
				tooltipContext={{ mode: "band" }}
			>
				{#snippet axis()}
					<Axis placement="left" rule classes={{ root: "select-none" }} />
					<Axis
						placement="bottom"
						rule
						format={(value: number) => DateTime.fromObject({ hour: value }).toFormat("H")}
						classes={{ root: "select-none" }}
					/>
				{/snippet}

				{#snippet tooltip({ context })}
					<Tooltip.Root
						{context}
						anchor="top"
						contained="container"
						class="bg-background/95! border-border! w-64 rounded-lg border px-3 py-2 shadow-xl backdrop-blur-md select-none"
					>
						<Tooltip.Header>{context.tooltip.data?.label}</Tooltip.Header>

						<Tooltip.List class="grid-cols-[minmax(0,1fr)_max-content] gap-x-6 gap-y-1">
							{#each context.tooltip.series as seriesItem (seriesItem.key)}
								{@const { value, unit } = formatMetricValue(Number(seriesItem.value), MetricUnit.COUNT)}
								<Tooltip.Item label={seriesItem.label} color={seriesItem.color}>
									<span class="text-foreground font-bold tabular-nums">
										{value}
										{#if unit}
											<span class="text-foreground/60 ml-1 text-xs">{unit}</span>
										{/if}
									</span>
								</Tooltip.Item>
							{/each}
						</Tooltip.List>
					</Tooltip.Root>
				{/snippet}
			</BarChart>
		{:else}
			{@const cells = createWeekdayHourCells(metric)}
			{@const maxValue = Math.max(1, ...cells.map((cell) => cell.value))}

			<Chart
				data={cells}
				x="hourLabel"
				y="weekdayLabel"
				xDomain={hourLabels}
				yDomain={weekdayLabels}
				height={320}
				padding={{ left: 24, bottom: 32 }}
				tooltipContext={{ mode: "band" }}
			>
				{#snippet axis()}
					<Axis placement="left" rule classes={{ root: "select-none" }} />
					<Axis placement="bottom" rule classes={{ root: "select-none" }} />
					<Cell
						x="hourLabel"
						y="weekdayLabel"
						fill={(cell: WeekdayHourCell) => getCellColor(cell.value, maxValue)}
						stroke="var(--color-background)"
						strokeWidth={2}
						rx={5}
					/>
				{/snippet}

				{#snippet tooltip({ context })}
					<Tooltip.Root
						{context}
						anchor="top"
						contained="container"
						class="bg-background/95! border-border! w-64 rounded-lg border px-3 py-2 shadow-xl backdrop-blur-md select-none"
					>
						{@const cell = context.tooltip.data as WeekdayHourCell | undefined}

						{#if cell}
							<Tooltip.Header>{context.tooltip.data?.label}</Tooltip.Header>
							<Tooltip.List class="grid-cols-[minmax(0,1fr)_max-content] gap-x-6 gap-y-1">
								<Tooltip.Item label={eventLabel} color={getCellColor(cell.value, maxValue)}>
									{@const { value } = formatMetricValue(cell.value, MetricUnit.COUNT)}
									<span class="text-foreground font-bold tabular-nums">{value}</span>
								</Tooltip.Item>
							</Tooltip.List>
						{/if}
					</Tooltip.Root>
				{/snippet}
			</Chart>
		{/if}
	{:catch error}
		<div
			class="bg-secondary/30 border-border flex min-h-80 flex-col items-center justify-center gap-2 rounded-lg border text-center"
		>
			<CircleAlert size={32} class="text-destructive" />
			<p class="text-foreground font-semibold">The hourly distribution could not be loaded.</p>
			<p class="text-foreground/60 max-w-xl text-sm">{error.message}</p>
		</div>
	{/await}
</Card>
