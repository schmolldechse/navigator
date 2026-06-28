<script lang="ts">
	import type {
		EventMetrics,
		EventTimeSeriesPoint,
		StatisticsBucket,
		StatisticsMetricResponse,
		StatisticsMetricResultNetworkEventTimeSeriesResult
	} from "@lib/api";
	import PointTooltip, { type PointTooltipItem } from "@lib/components/layerchart/tooltips/PointTooltip.svelte";
	import Skeleton from "@lib/components/ui/Skeleton.svelte";
	import type { RemoteQuery } from "@sveltejs/kit";
	import ChartArea from "@lucide/svelte/icons/chart-area";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";
	import { AreaChart, type ChartState } from "layerchart";
	import DashboardPanel from "../shared/DashboardPanel.svelte";
	import MetricBucketControl from "../shared/options/MetricBucketControl.svelte";
	import MetricOptionControl, { type MetricOption } from "../shared/options/MetricOptionControl.svelte";
	import {
		expectMetricResult,
		formatCount,
		formatMetric,
		formatStatisticsBucketLabel,
		toNumber,
		type StatisticsBucketOption
	} from "../shared/statistics-dashboard";
	import type { NetworkDelaySeverityView } from "./network-context.svelte";

	type Props = {
		promise: RemoteQuery<StatisticsMetricResponse>;
		bucket: StatisticsBucket;
		bucketOptions: StatisticsBucketOption[];
		onbucketchange: (bucket: StatisticsBucket) => void;
		view: NetworkDelaySeverityView;
		onviewchange: (view: NetworkDelaySeverityView) => void;
	};

	type SeverityShareKey =
		| "reliableUnder6Share"
		| "late6To15Share"
		| "late15To30Share"
		| "late30To60Share"
		| "late60PlusShare"
		| "cancelledShare";

	type SeverityCountKey =
		| "reliableUnder6Stops"
		| "late6To15Stops"
		| "late15To30Stops"
		| "late30To60Stops"
		| "late60PlusStops"
		| "cancelledStops";

	type SeverityDefinition = {
		shareKey: SeverityShareKey;
		countKey: SeverityCountKey;
		label: string;
		shortLabel: string;
		color: string;
		includeInDelayedView: boolean;
	};

	type SeveritySeriesItem = {
		key: SeverityShareKey;
		label: string;
		value: SeverityShareKey;
		color: string;
	};

	type DelaySeverityRow = {
		date: Date;
		plannedStops: number;
		reliableUnder6Share: number;
		late6To15Share: number;
		late15To30Share: number;
		late30To60Share: number;
		late60PlusShare: number;
		cancelledShare: number;
		reliableUnder6Stops: number;
		late6To15Stops: number;
		late15To30Stops: number;
		late30To60Stops: number;
		late60PlusStops: number;
		cancelledStops: number;
	};

	let { promise, bucket, bucketOptions, onbucketchange, view, onviewchange }: Props = $props();

	const viewOptions: MetricOption<NetworkDelaySeverityView>[] = [
		{ value: "delayed", label: "Delayed stops" },
		{ value: "all", label: "All planned stops" }
	];

	const severityDefinitions: SeverityDefinition[] = [
		{
			shareKey: "reliableUnder6Share",
			countKey: "reliableUnder6Stops",
			label: "Served < 6 min",
			shortLabel: "< 6 min",
			color: "var(--color-accent)",
			includeInDelayedView: false
		},
		{
			shareKey: "late6To15Share",
			countKey: "late6To15Stops",
			label: "6 to < 15 min",
			shortLabel: "6 to < 15",
			color: "#ca8a04",
			includeInDelayedView: true
		},
		{
			shareKey: "late15To30Share",
			countKey: "late15To30Stops",
			label: "15 to < 30 min",
			shortLabel: "15 to < 30",
			color: "#f97316",
			includeInDelayedView: true
		},
		{
			shareKey: "late30To60Share",
			countKey: "late30To60Stops",
			label: "30 to < 60 min",
			shortLabel: "30 to < 60",
			color: "#b91c1c",
			includeInDelayedView: true
		},
		{
			shareKey: "late60PlusShare",
			countKey: "late60PlusStops",
			label: "60+ min",
			shortLabel: "60+",
			color: "#7f1d1d",
			includeInDelayedView: true
		},
		{
			shareKey: "cancelledShare",
			countKey: "cancelledStops",
			label: "Cancelled",
			shortLabel: "Cancelled",
			color: "#dc2626",
			includeInDelayedView: false
		}
	];

	const clampRate = (value: number): number => Math.min(1, Math.max(0, value));
	const clampCount = (value: number, maxValue: number): number => Math.min(Math.max(0, value), Math.max(0, maxValue));
	const metricRate = (value: number | string | null | undefined): number | null => {
		const numberValue = toNumber(value);
		return numberValue === null ? null : clampRate(numberValue);
	};
	const countValue = (value: number | string | null | undefined): number => Math.max(0, toNumber(value) ?? 0);
	const shareOfPlanned = (count: number, plannedStops: number): number =>
		plannedStops > 0 ? Math.max(0, count / plannedStops) : 0;

	const createSeverityRow = (item: EventTimeSeriesPoint): DelaySeverityRow | null => {
		const date = new Date(item.bucketStart);
		if (Number.isNaN(date.getTime())) return null;

		const metrics: EventMetrics = item.eventMetrics;
		const plannedStops = countValue(metrics.plannedEvents);
		const servedStops = countValue(metrics.servedEvents);
		const cancelledStops = clampCount(countValue(metrics.cancelledEvents), plannedStops);
		if (plannedStops <= 0) return null;

		const under6Rate = metricRate(metrics.customerReliability5Rate);
		const under15Rate = metricRate(metrics.customerReliability15Rate);
		const late30PlusShare = metricRate(metrics.late30Rate);
		const late60PlusShare = metricRate(metrics.late60Rate);
		if (under6Rate === null || under15Rate === null || late30PlusShare === null || late60PlusShare === null) {
			return null;
		}

		const reliableUnder6Stops = clampCount(under6Rate * plannedStops, servedStops);
		const reliableUnder15Stops = clampCount(Math.max(under15Rate * plannedStops, reliableUnder6Stops), servedStops);
		const late30PlusStops = clampCount(late30PlusShare * plannedStops, servedStops);
		const late60PlusStops = clampCount(late60PlusShare * plannedStops, late30PlusStops);
		const late6To15Stops = Math.max(0, reliableUnder15Stops - reliableUnder6Stops);
		const late30To60Stops = Math.max(0, late30PlusStops - late60PlusStops);
		const late15To30Stops = Math.max(0, servedStops - reliableUnder15Stops - late30PlusStops);

		return {
			date,
			plannedStops,
			reliableUnder6Share: shareOfPlanned(reliableUnder6Stops, plannedStops),
			late6To15Share: shareOfPlanned(late6To15Stops, plannedStops),
			late15To30Share: shareOfPlanned(late15To30Stops, plannedStops),
			late30To60Share: shareOfPlanned(late30To60Stops, plannedStops),
			late60PlusShare: shareOfPlanned(late60PlusStops, plannedStops),
			cancelledShare: shareOfPlanned(cancelledStops, plannedStops),
			reliableUnder6Stops,
			late6To15Stops,
			late15To30Stops,
			late30To60Stops,
			late60PlusStops,
			cancelledStops
		};
	};

	const createRows = (response: StatisticsMetricResponse): DelaySeverityRow[] => {
		const result = expectMetricResult<StatisticsMetricResultNetworkEventTimeSeriesResult>(
			response,
			"NETWORK_EVENT_TIME_SERIES"
		);

		return result.items
			.map(createSeverityRow)
			.filter((row): row is DelaySeverityRow => row !== null)
			.sort((left, right) => left.date.getTime() - right.date.getTime());
	};

	const createTotals = (rows: DelaySeverityRow[]): Omit<DelaySeverityRow, "date"> => {
		const totals = rows.reduce(
			(accumulator, row) => ({
				plannedStops: accumulator.plannedStops + row.plannedStops,
				reliableUnder6Stops: accumulator.reliableUnder6Stops + row.reliableUnder6Stops,
				late6To15Stops: accumulator.late6To15Stops + row.late6To15Stops,
				late15To30Stops: accumulator.late15To30Stops + row.late15To30Stops,
				late30To60Stops: accumulator.late30To60Stops + row.late30To60Stops,
				late60PlusStops: accumulator.late60PlusStops + row.late60PlusStops,
				cancelledStops: accumulator.cancelledStops + row.cancelledStops
			}),
			{
				plannedStops: 0,
				reliableUnder6Stops: 0,
				late6To15Stops: 0,
				late15To30Stops: 0,
				late30To60Stops: 0,
				late60PlusStops: 0,
				cancelledStops: 0
			}
		);
		const share = (count: number): number => shareOfPlanned(count, totals.plannedStops);

		return {
			...totals,
			reliableUnder6Share: share(totals.reliableUnder6Stops),
			late6To15Share: share(totals.late6To15Stops),
			late15To30Share: share(totals.late15To30Stops),
			late30To60Share: share(totals.late30To60Stops),
			late60PlusShare: share(totals.late60PlusStops),
			cancelledShare: share(totals.cancelledStops)
		};
	};

	const delayedStops = (row: Omit<DelaySeverityRow, "date">): number =>
		row.late6To15Stops + row.late15To30Stops + row.late30To60Stops + row.late60PlusStops;
	const severe30Stops = (row: Omit<DelaySeverityRow, "date">): number => row.late30To60Stops + row.late60PlusStops;

	const createSeries = (selectedView: NetworkDelaySeverityView): SeveritySeriesItem[] =>
		severityDefinitions
			.filter((severity) => selectedView === "all" || severity.includeInDelayedView)
			.map((severity) => ({
				key: severity.shareKey,
				label: severity.shortLabel,
				value: severity.shareKey,
				color: severity.color
			}));

	const createYDomain = (
		rows: DelaySeverityRow[],
		series: SeveritySeriesItem[],
		selectedView: NetworkDelaySeverityView
	): [number, number] => {
		const maxStack = Math.max(0, ...rows.map((row) => series.reduce((sum, item) => sum + row[item.key], 0)));
		const paddedMax = maxStack <= 0 ? 0.1 : Math.ceil(maxStack * 1.08 * 20) / 20;

		return [0, selectedView === "all" ? Math.max(1, paddedMax) : Math.max(0.1, paddedMax)];
	};

	const createTooltipItems = (selectedView: NetworkDelaySeverityView): PointTooltipItem<DelaySeverityRow>[] => [
		{
			key: "plannedStops",
			label: "Planned stops",
			value: (row) => formatCount(row.plannedStops)
		},
		...severityDefinitions
			.filter((severity) => selectedView === "all" || severity.includeInDelayedView)
			.map((severity) => ({
				key: severity.shareKey,
				label: severity.label,
				value: (row: DelaySeverityRow) =>
					`${formatMetric(row[severity.shareKey], "rate")} (${formatCount(row[severity.countKey])})`,
				color: severity.color
			}))
	];
</script>

<DashboardPanel
	title="Delay severity timeline"
	description="Planned-stop based delay severity over time, with cancellations only included in the full planned-stop stack."
	icon={ChartArea}
>
	{#snippet actions()}
		<MetricBucketControl value={bucket} options={bucketOptions} onchange={onbucketchange} />
		<MetricOptionControl title="View" value={view} options={viewOptions} onchange={onviewchange} />
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
		{#if rows.length === 0}
			<div class="border-border bg-secondary/25 flex min-h-80 items-center justify-center rounded-lg border text-center">
				<p class="text-foreground/60 text-sm font-semibold">No delay severity points available.</p>
			</div>
		{:else}
			{@const totals = createTotals(rows)}
			{@const severitySeries = createSeries(view)}
			{@const tooltipItems = createTooltipItems(view)}
			{@const yDomain = createYDomain(rows, severitySeries, view)}
			{@const delayedTotal = delayedStops(totals)}
			{@const severe30Total = severe30Stops(totals)}

			<div class="grid gap-3">
				<div class="text-foreground/60 flex flex-wrap items-center justify-center gap-x-5 gap-y-2 text-xs font-semibold">
					<span>{formatMetric(shareOfPlanned(delayedTotal, totals.plannedStops), "rate")} delayed &gt; 6 min</span>
					<span>{formatMetric(shareOfPlanned(severe30Total, totals.plannedStops), "rate")} &gt;= 30 min</span>
					<span>{formatMetric(totals.late60PlusShare, "rate")} &gt;= 60 min</span>
				</div>

				<div class="bg-secondary/20 min-h-80 overflow-hidden rounded-lg p-2">
					<AreaChart
						data={rows}
						x="date"
						series={severitySeries}
						seriesLayout="stack"
						{yDomain}
						height={330}
						padding={{ top: 18, right: 18, bottom: 56, left: 46 }}
						highlight={{ lines: true, points: true, axis: "x" }}
						brush={{ zoomOnBrush: true, clickToReset: true, handleSize: 6, axis: "x" }}
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
							area: { fillOpacity: view === "all" ? 0.72 : 0.78, line: { strokeWidth: 1.5 } },
							yAxis: { format: "percentRound" }
						}}
					>
						{#snippet tooltip({ context }: { context: ChartState<DelaySeverityRow> })}
							<PointTooltip {context} items={tooltipItems} header={(row) => formatStatisticsBucketLabel(row.date, bucket)} />
						{/snippet}
					</AreaChart>
				</div>
			</div>
		{/if}
	{/if}
</DashboardPanel>
