<script lang="ts">
	import type { EventMetrics, StatisticsMetricResponse, StatisticsMetricResultEventSummaryResult } from "@lib/api";
	import PointTooltip, { type PointTooltipItem } from "@lib/components/layerchart/tooltips/PointTooltip.svelte";
	import Skeleton from "@lib/components/ui/Skeleton.svelte";
	import type { RemoteQuery } from "@sveltejs/kit";
	import BarChartIcon from "@lucide/svelte/icons/chart-column";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";
	import { curveMonotoneX } from "d3-shape";
	import { BarChart, LineChart, type ChartState } from "layerchart";
	import DashboardPanel from "../shared/DashboardPanel.svelte";
	import type { NetworkDistributionMode } from "./network-context.svelte";
	import DistributionModeControl from "./options/DistributionModeControl.svelte";
	import { formatCount, formatMetric, toNumber } from "../shared/statistics-dashboard";

	type Props = {
		promise: RemoteQuery<StatisticsMetricResponse>;
		eventSummary: RemoteQuery<StatisticsMetricResponse>;
		mode: NetworkDistributionMode;
		onmodechange: (mode: NetworkDistributionMode) => void;
	};

	type DistributionSummary = {
		sampleCount: number | string;
		medianDelaySeconds: number | string | null;
		p95DelaySeconds: number | string | null;
	};

	type DistributionBin = {
		lowerBoundSeconds: number | string;
		upperBoundSeconds: number | string;
		count: number | string;
		cumulativeShare: number | string;
	};

	type DistributionResult = {
		summary: DistributionSummary;
		bins: DistributionBin[];
	};

	type DistributionRow = {
		id: string;
		label: string;
		upperMinutes: number;
		count: number;
		sharePercent: number;
		cumulativeShare: number;
		cumulativePercent: number;
		customerCumulativePercent: number | null;
		cumulativeLabel: string;
	};

	let { promise, eventSummary, mode, onmodechange }: Props = $props();

	const getResult = (response: StatisticsMetricResponse): DistributionResult | null =>
		"summary" in response.result && "bins" in response.result ? (response.result as unknown as DistributionResult) : null;

	const getEventMetrics = (response: StatisticsMetricResponse | undefined): EventMetrics | null => {
		if (!response || !("metrics" in response.result)) return null;

		return (response.result as StatisticsMetricResultEventSummaryResult).metrics;
	};

	const clampRate = (value: number): number => Math.min(1, Math.max(0, value));

	const getServedShare = (metrics: EventMetrics | null): number | null => {
		const plannedEvents = toNumber(metrics?.plannedEvents);
		const servedEvents = toNumber(metrics?.servedEvents);
		if (plannedEvents === null || servedEvents === null || plannedEvents <= 0) return null;

		return clampRate(servedEvents / plannedEvents);
	};

	const formatBound = (seconds: number): string => {
		if (seconds === Number.MAX_SAFE_INTEGER || seconds >= 2_147_483_647) return "240 min+";
		if (seconds === 0) return "0";
		if (Math.abs(seconds) < 60) return `${seconds}s`;

		return `${Math.round(seconds / 60)} min`;
	};

	const formatBin = (lower: number, upper: number): string => {
		if (lower <= -2_147_483_648) return `< ${formatBound(upper)}`;
		if (upper >= 2_147_483_647) return `>= ${formatBound(lower)}`;
		return `${formatBound(lower)} to ${formatBound(upper)}`;
	};

	const createRows = (result: DistributionResult, servedShare: number | null): DistributionRow[] => {
		const binTotal = result.bins.reduce((total, bin) => total + (toNumber(bin.count) ?? 0), 0);
		const sampleCount = toNumber(result.summary.sampleCount) ?? binTotal;

		return result.bins
			.map((bin) => {
				const lower = toNumber(bin.lowerBoundSeconds) ?? 0;
				const upper = toNumber(bin.upperBoundSeconds) ?? 0;
				const count = toNumber(bin.count) ?? 0;
				const isOpenEnded = upper >= 2_147_483_647;
				const upperForChart = upper >= 2_147_483_647 ? lower : upper;
				const cumulativeShare = toNumber(bin.cumulativeShare) ?? 0;

				return {
					id: `${lower}-${upper}`,
					label: formatBin(lower, upper),
					upperMinutes: upperForChart / 60,
					count,
					sharePercent: sampleCount > 0 ? (count / sampleCount) * 100 : 0,
					cumulativeShare,
					cumulativePercent: cumulativeShare * 100,
					customerCumulativePercent: servedShare === null ? null : cumulativeShare * servedShare * 100,
					cumulativeLabel: isOpenEnded ? `${formatBound(lower)} or more · final share` : `Below ${formatBound(upper)}`
				};
			})
			.filter((row) => row.count > 0 || row.cumulativeShare > 0)
			.sort((left, right) => left.upperMinutes - right.upperMinutes);
	};

	const formatPercent = (value: number): string => `${value.toFixed(value < 1 ? 2 : 1)}%`;
	const formatMinuteTick = (value: unknown): string => `${Number(value).toLocaleString()} min`;

	const histogramTooltipItems: PointTooltipItem<DistributionRow>[] = [
		{
			key: "sharePercent",
			label: "Share of served stops",
			value: (row) => formatPercent(row.sharePercent),
			color: "var(--color-accent)"
		},
		{
			key: "count",
			label: "Served stops",
			value: (row) => formatCount(row.count, true),
			color: "var(--color-foreground)"
		}
	];

	const createCumulativeTooltipItems = (hasCustomerCurve: boolean): PointTooltipItem<DistributionRow>[] => [
		...(hasCustomerCurve
			? [
					{
						key: "customerCumulativePercent",
						label: "Customer view · all planned stops",
						value: (row: DistributionRow) =>
							row.customerCumulativePercent === null ? "-" : formatPercent(row.customerCumulativePercent),
						color: "var(--color-accent)"
					}
				]
			: []),
		{
			key: "cumulativePercent",
			label: "Operative · served stops only",
			value: (row) => formatPercent(row.cumulativePercent),
			color: "var(--color-foreground)"
		},
		{
			key: "sharePercent",
			label: "Interval share of served stops",
			value: (row) => formatPercent(row.sharePercent),
			color: "var(--color-muted-foreground)"
		},
		{
			key: "count",
			label: "Stops in interval",
			value: (row) => formatCount(row.count, true)
		}
	];

	const createCumulativeSeries = (hasCustomerCurve: boolean) =>
		hasCustomerCurve
			? [
					{
						key: "customerCumulativePercent",
						label: "Customer view · all planned stops",
						color: "var(--color-accent)"
					},
					{
						key: "cumulativePercent",
						label: "Operative · served stops only",
						color: "var(--color-foreground)"
					}
				]
			: [{ key: "cumulativePercent", label: "Operative · served stops only", color: "var(--color-accent)" }];
</script>

<DashboardPanel
	title="Delay distribution"
	description="Delay severity for served stops. In cumulative view, the customer-view curve scales this by the share of planned stops that actually ran."
	icon={BarChartIcon}
>
	{#snippet actions()}
		<DistributionModeControl value={mode} onchange={onmodechange} />
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
		{@const result = getResult(promise.current)}
		{#if !result}
			<div class="border-border bg-secondary/25 flex min-h-80 items-center justify-center rounded-lg border text-center">
				<p class="text-foreground/60 text-sm font-semibold">No distribution result available.</p>
			</div>
		{:else}
			{@const eventMetrics = getEventMetrics(eventSummary.current)}
			{@const servedShare = getServedShare(eventMetrics)}
			{@const rows = createRows(result, servedShare)}
			{#if rows.length === 0}
				<div class="border-border bg-secondary/25 flex min-h-80 items-center justify-center rounded-lg border text-center">
					<p class="text-foreground/60 text-sm font-semibold">No delay samples available.</p>
				</div>
			{:else}
				<div class="grid gap-3">
					<div class="text-foreground/60 flex flex-wrap gap-x-4 gap-y-1 text-xs font-semibold">
						<span>{formatCount(result.summary.sampleCount, true)} delay samples</span>
						{#if eventMetrics}
							<span>{formatCount(eventMetrics.plannedEvents, true)} planned stops</span>
							<span>{formatCount(eventMetrics.servedEvents, true)} served stops</span>
						{/if}
						<span>Median {formatMetric(result.summary.medianDelaySeconds, "seconds")}</span>
						<span>P95 {formatMetric(result.summary.p95DelaySeconds, "seconds")}</span>
					</div>

					<div class="bg-secondary/20 min-h-64 rounded-lg p-1 sm:min-h-72 sm:p-2">
						<div class="min-w-0">
							{#if mode === "histogram"}
								{#snippet histogramTooltip({ context }: { context: ChartState<DistributionRow> })}
									<PointTooltip {context} items={histogramTooltipItems} header={(row) => row.label} />
								{/snippet}

								<BarChart
									data={rows}
									x="label"
									y="sharePercent"
									yDomain={[0, null]}
									height={310}
									padding={{ top: 14, right: 10, bottom: 90, left: 46 }}
									tooltipContext={{ mode: "band" }}
									tooltip={histogramTooltip}
									series={[{ key: "sharePercent", label: "Share of served stops", color: "var(--color-accent)" }]}
									legend={false}
									props={{
										xAxis: { tickLabelProps: { rotate: -45, textAnchor: "end", class: "text-[10px]" } },
										yAxis: { format: (value: unknown) => `${Number(value).toFixed(0)}%` }
									}}
								/>
							{:else}
								{@const hasCustomerCurve = servedShare !== null}
								{@const cumulativeTooltipItems = createCumulativeTooltipItems(hasCustomerCurve)}
								{@const cumulativeSeries = createCumulativeSeries(hasCustomerCurve)}

								{#snippet cumulativeTooltip({ context }: { context: ChartState<DistributionRow> })}
									<PointTooltip {context} items={cumulativeTooltipItems} header={(row) => row.cumulativeLabel} />
								{/snippet}

								<LineChart
									data={rows}
									x="upperMinutes"
									series={cumulativeSeries}
									yDomain={[0, 100]}
									height={300}
									padding={{ top: 14, right: 10, bottom: 34, left: 46 }}
									tooltipContext={{ mode: "bisect-x" }}
									tooltip={cumulativeTooltip}
									legend={{
										placement: "bottom",
										classes: {
											root: "w-full px-2 pb-1",
											items: "flex-wrap justify-center gap-x-4 gap-y-1",
											item: "text-xs font-semibold"
										}
									}}
									props={{
										spline: { strokeWidth: 3, curve: curveMonotoneX },
										xAxis: { format: formatMinuteTick },
										yAxis: { format: (value: unknown) => `${Number(value).toFixed(0)}%` }
									}}
								/>
							{/if}
						</div>
					</div>
				</div>
			{/if}
		{/if}
	{/if}
</DashboardPanel>
