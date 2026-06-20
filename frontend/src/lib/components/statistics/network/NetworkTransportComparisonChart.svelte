<script lang="ts">
	import type { StatisticsMetricResponse, TransportTypeComparisonItem } from "@lib/api";
	import PointTooltip, { type PointTooltipItem } from "@lib/components/layerchart/tooltips/PointTooltip.svelte";
	import Skeleton from "@lib/components/ui/Skeleton.svelte";
	import type { RemoteQuery } from "@sveltejs/kit";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";
	import GitCompare from "@lucide/svelte/icons/git-compare";
	import { BarChart, Points, Rule, type ChartState } from "layerchart";
	import DashboardPanel from "../shared/DashboardPanel.svelte";
	import type { NetworkTransportComparisonMode } from "./network-context.svelte";
	import TransportComparisonModeControl from "./options/TransportComparisonModeControl.svelte";
	import {
		createReliabilityOutcomeShares,
		formatCount,
		formatMetric,
		toNumber,
		transportTypeLabel
	} from "../shared/statistics-dashboard";

	type Props = {
		promise: RemoteQuery<StatisticsMetricResponse>;
		mode: NetworkTransportComparisonMode;
		onmodechange: (mode: NetworkTransportComparisonMode) => void;
	};

	type ComparisonRow = {
		id: string;
		name: string;
		label: string;
		plannedStops: number;
		servedStops: number;
		reliableUnder6: number;
		operativePunctualityUnder6: number;
		punctualityGap: number;
		late6To15: number;
		late15OrMore: number;
		cancelled: number;
	};

	const outcomeSeries = [
		{
			key: "reliableUnder6",
			label: "Customer reliable < 6 min",
			value: "reliableUnder6",
			color: "var(--color-accent)"
		},
		{
			key: "late6To15",
			label: "6–15 min late",
			value: "late6To15",
			color: "#ca8a04"
		},
		{
			key: "late15OrMore",
			label: "15+ min late",
			value: "late15OrMore",
			color: "#64748b"
		},
		{
			key: "cancelled",
			label: "Cancelled planned stops",
			value: "cancelled",
			color: "#dc2626"
		}
	];

	const comparisonTooltipItems: PointTooltipItem<ComparisonRow>[] = [
		{
			key: "plannedStops",
			label: "Planned stops",
			value: (row) => formatCount(row.plannedStops)
		},
		{
			key: "customerReliability",
			label: "Customer reliability < 6 min",
			value: (row) => formatMetric(row.reliableUnder6, "rate"),
			color: "var(--color-accent)"
		},
		{
			key: "operativePunctuality",
			label: "Operative punctuality < 6 min",
			value: (row) => formatMetric(row.operativePunctualityUnder6, "rate"),
			color: "var(--color-foreground)"
		},
		{
			key: "late6To15",
			label: "6-15 min late",
			value: (row) => formatMetric(row.late6To15, "rate"),
			color: "#ca8a04"
		},
		{
			key: "late15OrMore",
			label: "15+ min late",
			value: (row) => formatMetric(row.late15OrMore, "rate"),
			color: "#64748b"
		},
		{
			key: "cancelled",
			label: "Cancelled planned stops",
			value: (row) => formatMetric(row.cancelled, "rate"),
			color: "#dc2626"
		}
	];

	const gapTooltipItems: PointTooltipItem<ComparisonRow>[] = [
		{
			key: "customerReliability",
			label: "Customer reliability < 6 min",
			value: (row) => formatMetric(row.reliableUnder6, "rate"),
			color: "var(--color-accent)"
		},
		{
			key: "operativePunctuality",
			label: "Operative punctuality < 6 min",
			value: (row) => formatMetric(row.operativePunctualityUnder6, "rate"),
			color: "var(--color-foreground)"
		},
		{
			key: "punctualityGap",
			label: "Cancellation penalty",
			value: (row) => `${(row.punctualityGap * 100).toFixed(1)} pp`,
			color: "#dc2626"
		},
		{
			key: "cancellationRate",
			label: "Cancelled planned stops",
			value: (row) => formatMetric(row.cancelled, "rate"),
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
		}
	];

	let { promise, mode, onmodechange }: Props = $props();

	const createRows = (response: StatisticsMetricResponse): ComparisonRow[] => {
		if (!("items" in response.result)) return [];

		return (response.result.items as TransportTypeComparisonItem[])
			.map((item): ComparisonRow | null => {
				const plannedStops = toNumber(item.eventMetrics.plannedEvents) ?? 0;
				const servedStops = toNumber(item.eventMetrics.servedEvents) ?? 0;
				const operativePunctuality5 = toNumber(item.eventMetrics.operativePunctuality5Rate);
				const outcomes = createReliabilityOutcomeShares(item.eventMetrics);
				if (plannedStops <= 0 || operativePunctuality5 === null || !outcomes) return null;

				const name = transportTypeLabel(item.transportType);
				const operativePunctualityUnder6 = Math.min(1, Math.max(0, operativePunctuality5));

				return {
					id: item.transportType,
					name,
					label: name,
					plannedStops,
					servedStops,
					...outcomes,
					operativePunctualityUnder6,
					punctualityGap: Math.max(0, operativePunctualityUnder6 - outcomes.reliableUnder6)
				};
			})
			.filter((row): row is ComparisonRow => row !== null)
			.sort((left, right) => right.reliableUnder6 - left.reliableUnder6 || left.name.localeCompare(right.name));
	};
</script>

<DashboardPanel
	title="Transport type comparison"
	description="Compare the punctuality gap and planned-stop outcomes across transport types in the selected period."
	icon={GitCompare}
>
	{#snippet actions()}
		<TransportComparisonModeControl value={mode} onchange={onmodechange} />
	{/snippet}
	{#if promise.loading}
		<div class="flex min-h-96 flex-col gap-y-3">
			<Skeleton class="h-80 w-full" />
			<Skeleton class="h-5 w-2/3" />
		</div>
	{:else if promise.error}
		<div
			class="border-destructive/30 bg-destructive/10 flex min-h-96 flex-col items-center justify-center gap-2 rounded-lg border text-center"
		>
			<CircleAlert size={32} class="text-destructive" />
			<p class="text-destructive text-sm font-semibold">{promise.error.message}</p>
		</div>
	{:else if promise.current}
		{@const rows = createRows(promise.current)}
		{#if rows.length === 0}
			<div class="border-border bg-secondary/25 flex min-h-96 items-center justify-center rounded-lg border text-center">
				<p class="text-foreground/60 text-sm font-semibold">No comparison rows available.</p>
			</div>
		{:else}
			{@const mostReliable = rows[0]}
			{@const mostOperativelyPunctual = rows.reduce((highest, row) =>
				row.operativePunctualityUnder6 > highest.operativePunctualityUnder6 ? row : highest
			)}
			{@const mostCancelled = rows.reduce((highest, row) => (row.cancelled > highest.cancelled ? row : highest))}
			{@const gapRows = [...rows].sort(
				(left, right) => right.punctualityGap - left.punctualityGap || left.name.localeCompare(right.name)
			)}

			{#snippet insightCard(label: string, row: ComparisonRow, value: number, toneClass = "text-foreground")}
				<div class="border-border bg-secondary/20 rounded-lg border p-3">
					<p class="text-foreground/50 text-[0.65rem] font-bold tracking-wider uppercase">{label}</p>
					<p class="mt-1 text-sm font-bold">{row.name}</p>
					<p class={["mt-0.5 text-xl font-bold tabular-nums", toneClass]}>{formatMetric(value, "rate")}</p>
					<p class="text-foreground/50 mt-1 text-xs font-semibold">
						{formatCount(row.plannedStops, true)} planned stops in selected period
					</p>
				</div>
			{/snippet}

			{#snippet comparisonTooltip({ context }: { context: ChartState<ComparisonRow> })}
				<PointTooltip {context} items={comparisonTooltipItems} header={(row) => row.name} />
			{/snippet}

			{#if mode === "gap"}
				{#snippet gapTooltip({ context }: { context: ChartState<ComparisonRow> })}
					<PointTooltip {context} items={gapTooltipItems} header={(row) => row.name} />
				{/snippet}

				<div class="grid gap-3">
					<div class="bg-secondary/20 min-h-80 overflow-hidden rounded-lg p-2">
						<BarChart
							data={gapRows}
							x={["reliableUnder6", "operativePunctualityUnder6"]}
							y="label"
							orientation="horizontal"
							xDomain={[0, 1]}
							yDomain={gapRows.map((row) => row.label)}
							yReverse={true}
							height={Math.max(320, gapRows.length * 50 + 80)}
							padding={{ top: 16, right: 20, bottom: 44, left: 124 }}
							bandPadding={0.32}
							tooltipContext={{ mode: "band" }}
							tooltip={gapTooltip}
							grid={{ x: false, y: { opacity: 0.18 }, bandAlign: "between" }}
							rule={false}
							props={{
								highlight: { axis: "x", area: true, points: true },
								xAxis: { format: "percentRound" }
							}}
						>
							{#snippet marks()}
								<Rule stroke="#dc2626" strokeWidth={3} opacity={0.7} />
								<Points
									data={gapRows}
									x="reliableUnder6"
									y="label"
									r={6}
									fill="var(--color-accent)"
									stroke="var(--color-background)"
									strokeWidth={2}
								/>
								<Points
									data={gapRows}
									x="operativePunctualityUnder6"
									y="label"
									r={6}
									fill="var(--color-foreground)"
									stroke="var(--color-background)"
									strokeWidth={2}
								/>
							{/snippet}
						</BarChart>
					</div>
					<div class="text-foreground/60 flex flex-wrap items-center justify-center gap-x-5 gap-y-2 text-xs font-semibold">
						<span class="inline-flex items-center gap-2">
							<span class="bg-accent size-2.5 rounded-full"></span>
							Customer reliability · all planned stops
						</span>
						<span class="inline-flex items-center gap-2">
							<span class="bg-foreground size-2.5 rounded-full"></span>
							Operative punctuality · served stops only
						</span>
						<span class="inline-flex items-center gap-2">
							<span class="h-0.5 w-4 bg-red-600"></span>
							Cancellation penalty · percentage-point gap
						</span>
					</div>
				</div>
			{:else}
				<div class="grid gap-4">
					<div class="grid gap-2 sm:grid-cols-3">
						{#if rows.length === 1}
							{@render insightCard("Customer reliability < 6 min", rows[0], rows[0].reliableUnder6, "text-accent")}
							{@render insightCard("Operative punctuality < 6 min", rows[0], rows[0].operativePunctualityUnder6)}
							{@render insightCard("Cancellation rate", rows[0], rows[0].cancelled, "text-red-600")}
						{:else}
							{@render insightCard("Highest customer reliability", mostReliable, mostReliable.reliableUnder6, "text-accent")}
							{@render insightCard(
								"Highest operative punctuality",
								mostOperativelyPunctual,
								mostOperativelyPunctual.operativePunctualityUnder6
							)}
							{@render insightCard("Highest cancellation rate", mostCancelled, mostCancelled.cancelled, "text-red-600")}
						{/if}
					</div>

					<div class="bg-secondary/20 min-h-80 overflow-hidden rounded-lg p-2">
						<BarChart
							data={rows}
							x="reliableUnder6"
							y="label"
							orientation="horizontal"
							series={outcomeSeries}
							seriesLayout="stack"
							xDomain={[0, 1]}
							yDomain={rows.map((row) => row.label)}
							height={Math.max(340, rows.length * 48 + 100)}
							padding={{ top: 16, right: 20, bottom: 44, left: 124 }}
							bandPadding={0.28}
							tooltipContext={{ mode: "band" }}
							tooltip={comparisonTooltip}
							legend={{ placement: "bottom", classes: { root: "justify-center pt-2", item: "text-xs font-semibold" } }}
							props={{ xAxis: { format: "percentRound" } }}
						/>
					</div>
					<p class="text-foreground/50 text-center text-xs font-semibold">
						Bars use all planned stops (customer view). Operative punctuality uses only served stops and is shown separately in
						the tooltip and summary.
					</p>
				</div>
			{/if}
		{/if}
	{/if}
</DashboardPanel>
