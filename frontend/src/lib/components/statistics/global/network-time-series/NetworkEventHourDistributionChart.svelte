<script lang="ts">
	import { MetricUnit, ScheduleType, type MetricSeries } from "@lib/api";
	import Card from "@lib/components/ui/card/Card.svelte";
	import Skeleton from "@lib/components/ui/Skeleton.svelte";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";
	import { Axis, BarChart, Tooltip } from "layerchart";
	import { formatMetricValue } from "../../metric-format";
	import { DateTime } from "luxon";

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
	<div>
		<p class="text-foreground/60 text-xs font-medium">Daily shape</p>
		<h3 class="text-foreground text-base font-semibold">{eventLabel} by hour</h3>
	</div>

	{#await promise}
		<div class="flex min-h-80 flex-col gap-y-3">
			<Skeleton class="h-64 w-full" />
			<Skeleton class="h-5 w-2/3" />
		</div>
	{:then metric}
		{@const rows = createRows(metric)}

		<BarChart
			data={rows}
			{series}
			x="hour"
			yDomain={[0, null]}
			height={320}
			padding={{ left: 52, right: 16, bottom: 32 }}
			tooltipContext={{ mode: "band" }}
		>
			{#snippet axis()}
				<Axis placement="left" rule tickLabelProps={{ textAnchor: "end" }} classes={{ root: "select-none" }} />
				<Axis
					placement="bottom"
					rule
					format={(value: number) => {
						const startOfDay = DateTime.local().startOf("day");

						const start = startOfDay.plus({ hours: value });
						const end = start.plus({ hours: 1 });

						return `${start.toFormat("HH")} - ${end.toFormat("HH")}`;
					}}
					tickLabelProps={{ textAnchor: "end", rotate: -45 }}
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
					{#snippet children()}
						<Tooltip.Header>{context.tooltip.data?.label}</Tooltip.Header>

						<Tooltip.List class="grid-cols-[minmax(0,1fr)_max-content] gap-x-6 gap-y-1">
							{#each context.tooltip.series as seriesItem (seriesItem.key)}
								{@const { value, unit } = formatMetricValue(Number(seriesItem.value), MetricUnit.COUNT)}
								<Tooltip.Item label={seriesItem.label} color={seriesItem.color} valueAlign="right">
									<span class="text-foreground font-bold tabular-nums">
										{value}
										{#if unit}
											<span class="text-foreground/60 ml-1 text-xs">{unit}</span>
										{/if}
									</span>
								</Tooltip.Item>
							{/each}
						</Tooltip.List>
					{/snippet}
				</Tooltip.Root>
			{/snippet}
		</BarChart>
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
