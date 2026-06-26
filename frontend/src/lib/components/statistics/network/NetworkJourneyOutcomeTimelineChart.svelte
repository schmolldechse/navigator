<script lang="ts">
	import type {
		JourneyOutcomeTimeSeriesPoint,
		StatisticsBucket,
		StatisticsMetricResponse,
		StatisticsMetricResultNetworkJourneyOutcomeTimeSeriesResult
	} from "@lib/api";
	import PointTooltip, { type PointTooltipItem } from "@lib/components/layerchart/tooltips/PointTooltip.svelte";
	import Skeleton from "@lib/components/ui/Skeleton.svelte";
	import type { RemoteQuery } from "@sveltejs/kit";
	import ChartArea from "@lucide/svelte/icons/chart-area";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";
	import { AreaChart, type ChartState } from "layerchart";
	import DashboardPanel from "../shared/DashboardPanel.svelte";
	import MetricBucketControl from "../shared/options/MetricBucketControl.svelte";
	import {
		expectMetricResult,
		formatCount,
		formatMetric,
		formatStatisticsBucketLabel,
		toNumber,
		type StatisticsBucketOption
	} from "../shared/statistics-dashboard";

	type Props = {
		promise: RemoteQuery<StatisticsMetricResponse>;
		bucket: StatisticsBucket;
		bucketOptions: StatisticsBucketOption[];
		onbucketchange: (bucket: StatisticsBucket) => void;
	};

	type OutcomeShareKey =
		| "completedShare"
		| "partiallyCancelledDestinationReachedShare"
		| "destinationNotReachedShare"
		| "fullyCancelledShare";
	type OutcomeCountKey =
		| "completedJourneys"
		| "partiallyCancelledDestinationReachedJourneys"
		| "destinationNotReachedJourneys"
		| "fullyCancelledJourneys";

	type OutcomeDefinition = {
		shareKey: OutcomeShareKey;
		countKey: OutcomeCountKey;
		label: string;
		shortLabel: string;
		color: string;
	};

	type JourneyOutcomeRow = {
		date: Date;
		plannedJourneys: number;
		completedJourneys: number;
		partiallyCancelledDestinationReachedJourneys: number;
		destinationNotReachedJourneys: number;
		fullyCancelledJourneys: number;
		completedShare: number;
		partiallyCancelledDestinationReachedShare: number;
		destinationNotReachedShare: number;
		fullyCancelledShare: number;
	};

	let { promise, bucket, bucketOptions, onbucketchange }: Props = $props();

	const outcomes: OutcomeDefinition[] = [
		{
			shareKey: "completedShare",
			countKey: "completedJourneys",
			label: "Completed without partial cancellation or destination failure",
			shortLabel: "Completed",
			color: "var(--color-accent)"
		},
		{
			shareKey: "partiallyCancelledDestinationReachedShare",
			countKey: "partiallyCancelledDestinationReachedJourneys",
			label: "Partially cancelled, destination reached",
			shortLabel: "Partial, reached",
			color: "#ca8a04"
		},
		{
			shareKey: "destinationNotReachedShare",
			countKey: "destinationNotReachedJourneys",
			label: "Destination not reached",
			shortLabel: "Destination missed",
			color: "#64748b"
		},
		{
			shareKey: "fullyCancelledShare",
			countKey: "fullyCancelledJourneys",
			label: "Fully cancelled",
			shortLabel: "Fully cancelled",
			color: "#dc2626"
		}
	];

	const clampRate = (value: number): number => Math.min(1, Math.max(0, value));
	const shareOfPlanned = (count: number, planned: number): number => (planned > 0 ? clampRate(count / planned) : 0);
	const countValue = (value: number | string | null | undefined): number => Math.max(0, toNumber(value) ?? 0);

	const createRows = (response: StatisticsMetricResponse): JourneyOutcomeRow[] => {
		const result = expectMetricResult<StatisticsMetricResultNetworkJourneyOutcomeTimeSeriesResult>(
			response,
			"NETWORK_JOURNEY_OUTCOME_TIME_SERIES"
		);

		return result.items
			.map((item: JourneyOutcomeTimeSeriesPoint): JourneyOutcomeRow | null => {
				const date = new Date(item.bucketStart);
				if (Number.isNaN(date.getTime())) return null;

				const metrics = item.journeyOutcomeMetrics;
				const plannedJourneys = countValue(metrics.plannedJourneys);
				const completedJourneys = countValue(metrics.completedJourneys);
				const partiallyCancelledDestinationReachedJourneys = countValue(metrics.partiallyCancelledDestinationReachedJourneys);
				const destinationNotReachedJourneys = countValue(metrics.destinationNotReachedJourneys);
				const fullyCancelledJourneys = countValue(metrics.fullyCancelledJourneys);

				return {
					date,
					plannedJourneys,
					completedJourneys,
					partiallyCancelledDestinationReachedJourneys,
					destinationNotReachedJourneys,
					fullyCancelledJourneys,
					completedShare: shareOfPlanned(completedJourneys, plannedJourneys),
					partiallyCancelledDestinationReachedShare: shareOfPlanned(
						partiallyCancelledDestinationReachedJourneys,
						plannedJourneys
					),
					destinationNotReachedShare: shareOfPlanned(destinationNotReachedJourneys, plannedJourneys),
					fullyCancelledShare: shareOfPlanned(fullyCancelledJourneys, plannedJourneys)
				};
			})
			.filter((row): row is JourneyOutcomeRow => row !== null)
			.sort((left, right) => left.date.getTime() - right.date.getTime());
	};

	const createTotals = (rows: JourneyOutcomeRow[]): Omit<JourneyOutcomeRow, "date"> => {
		const totals = rows.reduce(
			(accumulator, row) => ({
				plannedJourneys: accumulator.plannedJourneys + row.plannedJourneys,
				completedJourneys: accumulator.completedJourneys + row.completedJourneys,
				partiallyCancelledDestinationReachedJourneys:
					accumulator.partiallyCancelledDestinationReachedJourneys + row.partiallyCancelledDestinationReachedJourneys,
				destinationNotReachedJourneys: accumulator.destinationNotReachedJourneys + row.destinationNotReachedJourneys,
				fullyCancelledJourneys: accumulator.fullyCancelledJourneys + row.fullyCancelledJourneys
			}),
			{
				plannedJourneys: 0,
				completedJourneys: 0,
				partiallyCancelledDestinationReachedJourneys: 0,
				destinationNotReachedJourneys: 0,
				fullyCancelledJourneys: 0
			}
		);

		return {
			...totals,
			completedShare: shareOfPlanned(totals.completedJourneys, totals.plannedJourneys),
			partiallyCancelledDestinationReachedShare: shareOfPlanned(
				totals.partiallyCancelledDestinationReachedJourneys,
				totals.plannedJourneys
			),
			destinationNotReachedShare: shareOfPlanned(totals.destinationNotReachedJourneys, totals.plannedJourneys),
			fullyCancelledShare: shareOfPlanned(totals.fullyCancelledJourneys, totals.plannedJourneys)
		};
	};

	const createSeries = () =>
		outcomes.map((outcome) => ({
			key: outcome.shareKey,
			label: outcome.shortLabel,
			value: outcome.shareKey,
			color: outcome.color
		}));

	const tooltipItems: PointTooltipItem<JourneyOutcomeRow>[] = [
		{
			key: "plannedJourneys",
			label: "Planned journeys",
			value: (row) => formatCount(row.plannedJourneys)
		},
		...outcomes.map((outcome) => ({
			key: outcome.shareKey,
			label: outcome.label,
			value: (row: JourneyOutcomeRow) =>
				`${formatMetric(row[outcome.shareKey], "rate")} of planned journeys (${formatCount(row[outcome.countKey])})`,
			color: outcome.color
		}))
	];
</script>

<DashboardPanel
	title="Journey outcome timeline"
	description="Every planned journey is assigned to exactly one outcome per bucket, so the stacked shares add up to 100%."
	icon={ChartArea}
>
	{#snippet actions()}
		<MetricBucketControl value={bucket} options={bucketOptions} onchange={onbucketchange} />
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
		{@const rows = createRows(promise.current)}
		{@const availableRows = rows.filter((row) => row.plannedJourneys > 0)}
		{#if availableRows.length === 0}
			<div class="border-border bg-secondary/25 flex min-h-80 items-center justify-center rounded-lg border text-center">
				<p class="text-foreground/60 text-sm font-semibold">No journey outcome points available.</p>
			</div>
		{:else}
			{@const totals = createTotals(availableRows)}
			{@const outcomeSeries = createSeries()}

			<div class="grid gap-3">
				<div class="text-foreground/60 flex flex-wrap items-center justify-center gap-x-5 gap-y-2 text-xs font-semibold">
					<span>{formatCount(totals.plannedJourneys, true)} planned journeys</span>
					<span>{formatMetric(totals.completedShare, "rate")} completed</span>
					<span
						>{formatMetric(
							totals.partiallyCancelledDestinationReachedShare + totals.destinationNotReachedShare + totals.fullyCancelledShare,
							"rate"
						)} affected</span
					>
				</div>

				<div class="bg-secondary/20 min-h-80 overflow-hidden rounded-lg p-2">
					<AreaChart
						data={rows}
						x="date"
						series={outcomeSeries}
						seriesLayout="stack"
						yDomain={[0, 1]}
						height={330}
						padding={{ top: 18, right: 18, bottom: 56, left: 46 }}
						tooltipContext={{ mode: "bisect-x" }}
						legend={{
							placement: "bottom",
							classes: {
								root: "w-full px-2 pb-1",
								items: "flex-wrap justify-center gap-x-4 gap-y-1",
								item: "text-xs font-semibold"
							}
						}}
						props={{
							area: { fillOpacity: 0.74, line: { strokeWidth: 1.5 } },
							yAxis: { format: "percentRound" }
						}}
					>
						{#snippet tooltip({ context }: { context: ChartState<JourneyOutcomeRow> })}
							<PointTooltip {context} items={tooltipItems} header={(row) => formatStatisticsBucketLabel(row.date, bucket)} />
						{/snippet}
					</AreaChart>
				</div>
			</div>
		{/if}
	{/if}
</DashboardPanel>
