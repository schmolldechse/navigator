<script lang="ts">
	import type { EventMetrics, StatisticsMetricResponse, TransportTypeComparisonItem } from "@lib/api";
	import PointTooltip, { type PointTooltipItem } from "@lib/components/layerchart/tooltips/PointTooltip.svelte";
	import Skeleton from "@lib/components/ui/Skeleton.svelte";
	import * as Accordion from "@lib/components/ui/accordion";
	import type { RemoteQuery } from "@sveltejs/kit";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";
	import GitCompare from "@lucide/svelte/icons/git-compare";
	import { BarChart, Labels, Points, Rule, type ChartState } from "layerchart";
	import DashboardPanel from "../shared/DashboardPanel.svelte";
	import MetricThresholdControl from "../shared/options/MetricThresholdControl.svelte";
	import type { NetworkTransportComparisonThreshold } from "./network-context.svelte";
	import {
		createReliabilityOutcomeShares,
		formatCount,
		formatMetric,
		toNumber,
		transportTypeLabel
	} from "../shared/statistics-dashboard";

	type Props = {
		promise: RemoteQuery<StatisticsMetricResponse>;
		threshold: NetworkTransportComparisonThreshold;
		onthresholdchange: (threshold: NetworkTransportComparisonThreshold) => void;
	};

	type ThresholdDefinition = {
		value: NetworkTransportComparisonThreshold;
		label: string;
		customerLabel: string;
		operativeLabel: string;
		getCustomerReliability: (metrics: EventMetrics) => number | null;
		getOperativePunctuality: (metrics: EventMetrics) => number | null;
	};

	type ComparisonRow = {
		id: string;
		name: string;
		label: string;
		plannedStops: number;
		servedStops: number;
		customerReliability: number;
		operativePunctuality: number;
		punctualityGap: number;
		reliableUnder6: number;
		late6To15: number;
		late15OrMore: number;
		cancelled: number;
	};

	const clampRate = (value: number): number => Math.min(1, Math.max(0, value));

	const thresholdDefinitions: ThresholdDefinition[] = [
		{
			value: "under6",
			label: "< 6 min",
			customerLabel: "Customer reliability < 6 min",
			operativeLabel: "Operative punctuality < 6 min",
			getCustomerReliability: (metrics) => toNumber(metrics.customerReliability5Rate),
			getOperativePunctuality: (metrics) => toNumber(metrics.operativePunctuality5Rate)
		},
		{
			value: "under15",
			label: "< 15 min",
			customerLabel: "Customer reliability < 15 min",
			operativeLabel: "Operative punctuality < 15 min",
			getCustomerReliability: (metrics) => toNumber(metrics.customerReliability15Rate),
			getOperativePunctuality: (metrics) => toNumber(metrics.operativePunctuality15Rate)
		}
	];

	const getThresholdDefinition = (value: NetworkTransportComparisonThreshold): ThresholdDefinition =>
		thresholdDefinitions.find((definition) => definition.value === value) ?? thresholdDefinitions[0];

	const outcomeSeries = [
		{
			key: "reliableUnder6",
			label: "Under 6 min",
			value: "reliableUnder6",
			color: "var(--color-accent)"
		},
		{
			key: "late6To15",
			label: "6 to < 15 min",
			value: "late6To15",
			color: "#ca8a04"
		},
		{
			key: "late15OrMore",
			label: "15 min or more",
			value: "late15OrMore",
			color: "#64748b"
		},
		{
			key: "cancelled",
			label: "Cancelled",
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
			key: "under6",
			label: "Under 6 min",
			value: (row) => formatMetric(row.reliableUnder6, "rate"),
			color: "var(--color-accent)"
		},
		{
			key: "late6To15",
			label: "6 to < 15 min",
			value: (row) => formatMetric(row.late6To15, "rate"),
			color: "#ca8a04"
		},
		{
			key: "late15OrMore",
			label: "15 min or more",
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

	let { promise, threshold, onthresholdchange }: Props = $props();
	let outcomeMixValue = $state<string | undefined>(undefined);
	let chartWidth = $state(0);

	const selectedThreshold = $derived(getThresholdDefinition(threshold));
	const outcomeMixAccordionValue = "planned-stop-outcome-mix";
	const compactChart = $derived(chartWidth > 0 && chartWidth < 560);
	const gapChartPadding = $derived({
		top: 28,
		right: compactChart ? 58 : 86,
		bottom: 44,
		left: compactChart ? 84 : 116
	});
	const outcomeChartPadding = $derived({
		top: 16,
		right: compactChart ? 12 : 20,
		bottom: 44,
		left: compactChart ? 84 : 116
	});

	const formatGapLabel = (gap: number): string => (gap <= 0 ? "0.0 pp" : `-${(gap * 100).toFixed(1)} pp`);

	const gapTooltipItems = $derived.by<PointTooltipItem<ComparisonRow>[]>(() => [
		{
			key: "customerReliability",
			label: selectedThreshold.customerLabel,
			value: (row) => formatMetric(row.customerReliability, "rate"),
			color: "var(--color-accent)"
		},
		{
			key: "operativePunctuality",
			label: selectedThreshold.operativeLabel,
			value: (row) => formatMetric(row.operativePunctuality, "rate"),
			color: "var(--color-foreground)"
		},
		{
			key: "punctualityGap",
			label: "Customer-view gap",
			value: (row) => formatGapLabel(row.punctualityGap),
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
	]);

	const createRows = (response: StatisticsMetricResponse, thresholdDefinition: ThresholdDefinition): ComparisonRow[] => {
		if (!("items" in response.result)) return [];

		return (response.result.items as TransportTypeComparisonItem[])
			.map((item): ComparisonRow | null => {
				const plannedStops = toNumber(item.eventMetrics.plannedEvents) ?? 0;
				const servedStops = toNumber(item.eventMetrics.servedEvents) ?? 0;
				const customerReliability = thresholdDefinition.getCustomerReliability(item.eventMetrics);
				const operativePunctuality = thresholdDefinition.getOperativePunctuality(item.eventMetrics);
				const outcomes = createReliabilityOutcomeShares(item.eventMetrics);
				if (plannedStops <= 0 || customerReliability === null || operativePunctuality === null || !outcomes) return null;

				const name = transportTypeLabel(item.transportType);
				const customerRate = clampRate(customerReliability);
				const operativeRate = clampRate(operativePunctuality);

				return {
					id: item.transportType,
					name,
					label: name,
					plannedStops,
					servedStops,
					customerReliability: customerRate,
					operativePunctuality: operativeRate,
					punctualityGap: Math.max(0, operativeRate - customerRate),
					...outcomes
				};
			})
			.filter((row): row is ComparisonRow => row !== null)
			.sort((left, right) => right.customerReliability - left.customerReliability || left.name.localeCompare(right.name));
	};
</script>

<DashboardPanel
	title="Transport type comparison"
	description="Compare customer-view reliability and operative punctuality across transport types."
	icon={GitCompare}
>
	{#snippet actions()}
		<MetricThresholdControl title="Gap threshold" value={threshold} onchange={onthresholdchange} />
	{/snippet}
	{#if promise.loading}
		<div class="flex min-h-80 flex-col gap-y-3">
			<Skeleton class="h-64 w-full" />
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
		{@const rows = createRows(promise.current, selectedThreshold)}
		{#if rows.length === 0}
			<div class="border-border bg-secondary/25 flex min-h-80 items-center justify-center rounded-lg border text-center">
				<p class="text-foreground/60 text-sm font-semibold">No comparison rows available.</p>
			</div>
		{:else}
			{@const gapRows = [...rows].sort(
				(left, right) => right.punctualityGap - left.punctualityGap || left.name.localeCompare(right.name)
			)}

			<div bind:clientWidth={chartWidth} class="grid gap-4">
				<section class="grid gap-3" aria-label={`Punctuality gap by transport type for ${selectedThreshold.label}`}>
					<div class="flex flex-col gap-1">
						<p class="text-foreground/50 text-[0.65rem] font-bold tracking-wider uppercase">Punctuality gap</p>
						<p class="text-foreground/60 text-sm font-semibold">
							Red labels show the percentage-point distance between operative punctuality and customer reliability.
						</p>
					</div>

					<div class="bg-secondary/20 min-h-72 overflow-hidden rounded-lg p-2">
						<BarChart
							data={gapRows}
							x={["customerReliability", "operativePunctuality"]}
							y="label"
							orientation="horizontal"
							xDomain={[0, 1]}
							yDomain={gapRows.map((row) => row.label)}
							yReverse={true}
							height={Math.max(300, gapRows.length * 42 + 76)}
							padding={gapChartPadding}
							bandPadding={0.32}
							tooltipContext={{ mode: "band" }}
							grid={{ x: false, y: { opacity: 0.18 }, bandAlign: "between" }}
							rule={false}
							props={{
								highlight: { axis: "x", area: true, points: true },
								xAxis: { format: "percentRound" }
							}}
						>
							{#snippet marks()}
								<Rule stroke="#dc2626" strokeWidth={5} opacity={0.82} style="stroke: #dc2626;" />
								<Points
									data={gapRows}
									x="customerReliability"
									y="label"
									r={6}
									fill="var(--color-accent)"
									stroke="var(--color-background)"
									strokeWidth={2}
								/>
								<Points
									data={gapRows}
									x="operativePunctuality"
									y="label"
									r={6}
									fill="var(--color-foreground)"
									stroke="var(--color-background)"
									strokeWidth={2}
								/>
								<Labels
									x="operativePunctuality"
									y="label"
									value={(row: ComparisonRow) => formatGapLabel(row.punctualityGap)}
									fill="#dc2626"
									fontSize={12}
									dx={8}
									textAnchor="start"
									verticalAnchor="middle"
								/>
							{/snippet}

							{#snippet tooltip({ context }: { context: ChartState<ComparisonRow> })}
								<PointTooltip {context} items={gapTooltipItems} header={(row) => row.name} />
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
							<span class="h-1 w-5 rounded-full bg-red-600"></span>
							Customer-view gap · percentage points
						</span>
					</div>
				</section>

				<Accordion.Root
					type="single"
					value={outcomeMixValue}
					onchange={(value) => (outcomeMixValue = value)}
					class="border-border/70 bg-secondary/10 overflow-hidden rounded-lg border [&>[role=separator]]:hidden"
				>
					<Accordion.Item value={outcomeMixAccordionValue}>
						<Accordion.Trigger>
							<span class="flex min-w-0 flex-col gap-0.5">
								<span class="text-foreground text-sm font-semibold"> Planned-stop outcome mix by transport type </span>
								<span class="text-foreground/55 text-xs leading-5">
									Detail view: every planned stop is classified as reliable, delayed or cancelled.
								</span>
							</span>
						</Accordion.Trigger>
						<Accordion.Content class="border-border/70 border-t px-3 pt-3 pb-0">
							<section class="grid gap-3" aria-label="Planned stop outcome mix by transport type">
								<div class="flex flex-col gap-1">
									<p class="text-foreground/50 text-[0.65rem] font-bold tracking-wider uppercase">Planned stop outcome mix</p>
									<p class="text-foreground/60 max-w-3xl text-sm font-semibold">
										Every planned stop is counted once as under 6 minutes, 6 to under 15 minutes, 15 minutes or more, or
										cancelled.
									</p>
								</div>

								<div class="bg-secondary/20 min-h-72 overflow-hidden rounded-lg p-2">
									<BarChart
										data={rows}
										x="reliableUnder6"
										y="label"
										orientation="horizontal"
										series={outcomeSeries}
										seriesLayout="stack"
										xDomain={[0, 1]}
										yDomain={rows.map((row) => row.label)}
										height={Math.max(300, rows.length * 44 + 88)}
										padding={outcomeChartPadding}
										bandPadding={0.28}
										tooltipContext={{ mode: "band" }}
										legend={{ placement: "bottom", classes: { root: "justify-center pt-2", item: "text-xs font-semibold" } }}
										props={{ xAxis: { format: "percentRound" } }}
									>
										{#snippet tooltip({ context }: { context: ChartState<ComparisonRow> })}
											<PointTooltip {context} items={comparisonTooltipItems} header={(row) => row.name} />
										{/snippet}
									</BarChart>
								</div>
							</section>
						</Accordion.Content>
					</Accordion.Item>
				</Accordion.Root>
			</div>
		{/if}
	{/if}
</DashboardPanel>
