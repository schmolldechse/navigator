<script lang="ts">
	import type { LineHourMatrixItem, StatisticsMetricResponse } from "@lib/api";
	import Skeleton from "@lib/components/ui/Skeleton.svelte";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";
	import Grid2x2 from "@lucide/svelte/icons/grid-2x2";
	import DashboardPanel from "./DashboardPanel.svelte";
	import { formatCount, formatMetric, rateToPercent, transportTypeShortLabel } from "./statistics-dashboard";

	type Props = {
		promise: Promise<StatisticsMetricResponse>;
	};

	type MatrixLine = {
		key: string;
		label: string;
		transportType: string;
		totalEvents: number;
	};

	let { promise }: Props = $props();

	const hours = Array.from({ length: 24 }, (_, hour) => hour);

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

	const findCell = (items: LineHourMatrixItem[], line: MatrixLine, hour: number) =>
		items.find(
			(item) => item.lineName === line.label && item.transportType === line.transportType && Number(item.hour) === hour
		);

	const colorForRate = (rate: number | null): string => {
		if (rate === null) return "rgba(148, 163, 184, 0.18)";
		const hue = Math.max(0, Math.min(125, rate * 125));

		return `hsl(${hue} 66% 42% / 0.78)`;
	};
</script>

<DashboardPanel title="Line by hour" description="Observed station-line quality by local hour." icon={Grid2x2}>
	{#await promise}
		<div class="flex min-h-80 flex-col gap-y-3">
			<Skeleton class="h-72 w-full" />
			<Skeleton class="h-5 w-2/3" />
		</div>
	{:then response}
		{@const items = getItems(response)}
		{@const lines = createLines(items)}
		{#if lines.length === 0}
			<div class="border-border bg-secondary/25 flex min-h-80 items-center justify-center rounded-lg border text-center">
				<p class="text-foreground/60 text-sm font-semibold">No line-hour data available.</p>
			</div>
		{:else}
			<div class="overflow-x-auto">
				<div class="grid min-w-[54rem] gap-1" style="grid-template-columns: 10rem repeat(24, minmax(0, 1fr));">
					<div></div>
					{#each hours as hour (hour)}
						<div class="text-foreground/45 text-center text-[0.65rem] font-semibold tabular-nums">
							{hour % 3 === 0 ? hour : ""}
						</div>
					{/each}

					{#each lines as line (line.key)}
						<div class="text-foreground/75 flex min-w-0 items-center gap-2 text-xs font-semibold">
							<span class="border-border bg-secondary rounded px-1.5 py-0.5 text-[0.65rem]">
								{transportTypeShortLabel(line.transportType)}
							</span>
							<span class="truncate">{line.label}</span>
						</div>
						{#each hours as hour (hour)}
							{@const cell = findCell(items, line, hour)}
							{@const rate = rateToPercent(cell?.eventMetrics.customerReliability5Rate)}
							<div
								class="border-border/70 h-7 rounded border"
								title={cell
									? `${formatMetric(cell.eventMetrics.customerReliability5Rate, "rate")} reliable | ${formatCount(
											cell.eventMetrics.plannedEvents,
											true
										)} events`
									: "No events"}
								style={`background-color: ${colorForRate(rate === null ? null : rate / 100)};`}
							></div>
						{/each}
					{/each}
				</div>
			</div>
		{/if}
	{:catch error}
		<div
			class="border-destructive/30 bg-destructive/10 flex min-h-80 flex-col items-center justify-center gap-2 rounded-lg border text-center"
		>
			<CircleAlert size={32} class="text-destructive" />
			<p class="text-destructive text-sm font-semibold">{error.message}</p>
		</div>
	{/await}
</DashboardPanel>
