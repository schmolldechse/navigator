<script lang="ts">
	import type { StatisticsMetricResponse, StatisticsMetricResultEventSummaryResult } from "@lib/api";
	import PointTooltip, { type PointTooltipItem } from "@lib/components/layerchart/tooltips/PointTooltip.svelte";
	import Skeleton from "@lib/components/ui/Skeleton.svelte";
	import type { RemoteQuery } from "@sveltejs/kit";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";
	import ShieldCheck from "@lucide/svelte/icons/shield-check";
	import { BarChart, type ChartState } from "layerchart";
	import DashboardPanel from "../shared/DashboardPanel.svelte";
	import {
		createReliabilityOutcomeShares,
		expectMetricResult,
		formatCount,
		formatMetric,
		toNumber,
		type ReliabilityOutcomeShares
	} from "../shared/statistics-dashboard";

	type Props = {
		promise: RemoteQuery<StatisticsMetricResponse>;
	};

	type OutcomeKey = keyof ReliabilityOutcomeShares;

	type BalanceRow = ReliabilityOutcomeShares & {
		plannedStops: number;
	};

	type OutcomeDefinition = {
		key: OutcomeKey;
		label: string;
		shortLabel: string;
		color: string;
	};

	let { promise }: Props = $props();

	const outcomes: OutcomeDefinition[] = [
		{
			key: "reliableUnder6",
			label: "Served with under 6 min delay",
			shortLabel: "Under 6 min",
			color: "var(--color-accent)"
		},
		{
			key: "late6To15",
			label: "Served with 6 to under 15 min delay",
			shortLabel: "6 to < 15 min",
			color: "#ca8a04"
		},
		{
			key: "late15OrMore",
			label: "Served with at least 15 min delay",
			shortLabel: "15 min or more",
			color: "#64748b"
		},
		{
			key: "cancelled",
			label: "Cancelled planned stops",
			shortLabel: "Cancelled",
			color: "#dc2626"
		}
	];

	const createRow = (response: StatisticsMetricResponse): BalanceRow | null => {
		const metrics = expectMetricResult<StatisticsMetricResultEventSummaryResult>(response, "EVENT_SUMMARY").metrics;
		const plannedStops = toNumber(metrics.plannedEvents) ?? 0;
		const shares = createReliabilityOutcomeShares(metrics);
		if (plannedStops <= 0 || !shares) return null;

		return {
			plannedStops,
			...shares
		};
	};

	const createOutcomeSeries = () =>
		outcomes.map((outcome) => ({
			key: outcome.key,
			label: outcome.shortLabel,
			value: outcome.key,
			color: outcome.color
		}));

	const tooltipItems: PointTooltipItem<BalanceRow>[] = [
		{
			key: "plannedStops",
			label: "Planned stops",
			value: (row) => formatCount(row.plannedStops)
		},
		...outcomes.map((outcome) => ({
			key: outcome.key,
			label: outcome.label,
			value: (row: BalanceRow) => formatMetric(row[outcome.key], "rate"),
			color: outcome.color
		}))
	];
</script>

<DashboardPanel
	title="What happens to every 100 planned stops?"
	description="A customer-view balance of the selected network: every planned stop is classified as reliable, delayed or cancelled."
	icon={ShieldCheck}
>
	{#if promise.loading}
		<div class="flex min-h-56 flex-col gap-y-3">
			<Skeleton class="h-40 w-full" />
			<Skeleton class="mx-auto h-4 w-56" />
		</div>
	{:else if promise.error}
		<div
			class="border-destructive/30 bg-destructive/10 flex min-h-56 flex-col items-center justify-center gap-2 rounded-lg border text-center"
		>
			<CircleAlert size={32} class="text-destructive" />
			<p class="text-destructive text-sm font-semibold">{promise.error.message}</p>
		</div>
	{:else if promise.current}
		{@const row = createRow(promise.current)}
		{#if !row}
			<div class="border-border bg-secondary/25 flex min-h-56 items-center justify-center rounded-lg border text-center">
				<p class="text-foreground/60 text-sm font-semibold">No planned stops available for this selection.</p>
			</div>
		{:else}
			{@const outcomeSeries = createOutcomeSeries()}
			{#snippet balanceTooltip({ context }: { context: ChartState<BalanceRow> })}
				<PointTooltip {context} items={tooltipItems} header={() => "Selected network"} />
			{/snippet}

			<div class="grid gap-3">
				<div class="bg-secondary/15 min-h-40 overflow-hidden rounded-lg p-2">
					<BarChart
						data={[row]}
						x="reliableUnder6"
						y={() => "balance"}
						orientation="horizontal"
						series={outcomeSeries}
						seriesLayout="stack"
						xDomain={[0, 1]}
						yDomain={["balance"]}
						height={160}
						padding={{ top: 8, right: 16, bottom: 50, left: 16 }}
						bandPadding={0.34}
						axis="x"
						tooltipContext={{ mode: "band" }}
						tooltip={balanceTooltip}
						legend={{ placement: "bottom", classes: { root: "justify-center pt-2", item: "text-xs font-semibold" } }}
						props={{ xAxis: { format: "percentRound" } }}
					/>
				</div>

				<p class="text-foreground/45 text-center text-xs font-semibold">
					Based on approx. {formatCount(row.plannedStops, true)} planned stops in the selected period.
				</p>
			</div>
		{/if}
	{/if}
</DashboardPanel>
