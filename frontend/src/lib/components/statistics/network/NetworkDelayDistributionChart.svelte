<script lang="ts">
	import type { StatisticsMetricResponse } from "@lib/api";
	import Skeleton from "@lib/components/ui/Skeleton.svelte";
	import ToggleGroup from "@lib/components/ui/toggle-group/ToggleGroup.svelte";
	import ToggleGroupItem from "@lib/components/ui/toggle-group/ToggleGroupItem.svelte";
	import type { RemoteQuery } from "@sveltejs/kit";
	import BarChartIcon from "@lucide/svelte/icons/chart-column";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";
	import { BarChart, LineChart } from "layerchart";
	import DashboardPanel from "../shared/DashboardPanel.svelte";
	import { formatCount, formatMetric, toNumber, type NetworkDistributionMode } from "../shared/statistics-dashboard";

	type Props = {
		promise: RemoteQuery<StatisticsMetricResponse>;
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
		cumulativeShare: number;
		cumulativePercent: number;
	};

	type DistributionModeOption = {
		value: NetworkDistributionMode;
		label: string;
	};

	let { promise, mode, onmodechange }: Props = $props();

	const modeOptions: DistributionModeOption[] = [
		{ value: "histogram", label: "Histogram" },
		{ value: "cdf", label: "Cumulative" }
	];
	const selectedMode = $derived(modeOptions.find((option) => option.value === mode) ?? modeOptions[0]);

	const getResult = (response: StatisticsMetricResponse): DistributionResult | null =>
		"summary" in response.result && "bins" in response.result ? (response.result as unknown as DistributionResult) : null;

	const formatBound = (seconds: number): string => {
		if (seconds === Number.MAX_SAFE_INTEGER || seconds >= 2_147_483_647) return "240 min+";
		if (seconds === 0) return "0";
		if (Math.abs(seconds) < 60) return `${seconds}s`;

		return `${Math.round(seconds / 60)} min`;
	};

	const formatBin = (lower: number, upper: number): string => {
		if (upper >= 2_147_483_647) return `>= ${formatBound(lower)}`;
		return `${formatBound(lower)} to ${formatBound(upper)}`;
	};

	const createRows = (result: DistributionResult): DistributionRow[] =>
		result.bins
			.map((bin) => {
				const lower = toNumber(bin.lowerBoundSeconds) ?? 0;
				const upper = toNumber(bin.upperBoundSeconds) ?? 0;
				const upperForChart = upper >= 2_147_483_647 ? lower : upper;
				const cumulativeShare = toNumber(bin.cumulativeShare) ?? 0;

				return {
					id: `${lower}-${upper}`,
					label: formatBin(lower, upper),
					upperMinutes: upperForChart / 60,
					count: toNumber(bin.count) ?? 0,
					cumulativeShare,
					cumulativePercent: cumulativeShare * 100
				};
			})
			.filter((row) => row.count > 0 || row.cumulativeShare > 0);
</script>

<DashboardPanel
	title="Delay distribution"
	description="Served stop-event delay spread for the selected network scope."
	icon={BarChartIcon}
>
	{#snippet actions()}
		<ToggleGroup
			mode="single"
			allowEmpty={false}
			selected={selectedMode}
			keyFn={(option: DistributionModeOption) => option.value}
			onselect={(option: DistributionModeOption | undefined) => onmodechange(option?.value ?? "histogram")}
			class="gap-1"
		>
			{#each modeOptions as option (option.value)}
				<ToggleGroupItem
					item={option}
					class="data-active:border-accent data-active:bg-accent data-active:text-accent-foreground px-2.5 py-1.5 text-xs font-semibold"
				>
					{option.label}
				</ToggleGroupItem>
			{/each}
		</ToggleGroup>
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
			{@const rows = createRows(result)}
			{#if rows.length === 0}
				<div class="border-border bg-secondary/25 flex min-h-80 items-center justify-center rounded-lg border text-center">
					<p class="text-foreground/60 text-sm font-semibold">No delay samples available.</p>
				</div>
			{:else}
				<div class="grid gap-3">
					<div class="text-foreground/60 flex flex-wrap gap-x-4 gap-y-1 text-xs font-semibold">
						<span>{formatCount(result.summary.sampleCount, true)} samples</span>
						<span>Median {formatMetric(result.summary.medianDelaySeconds, "seconds")}</span>
						<span>P95 {formatMetric(result.summary.p95DelaySeconds, "seconds")}</span>
					</div>

					<div class="bg-secondary/20 min-h-72 overflow-x-auto rounded-lg p-2">
						<div class="min-w-[44rem]">
							{#if mode === "histogram"}
								<BarChart
									data={rows}
									x="label"
									y="count"
									height={300}
									padding={{ top: 14, right: 18, bottom: 76, left: 56 }}
									tooltipContext={{ mode: "band" }}
									series={[{ key: "count", label: "Samples", color: "var(--color-accent)" }]}
									legend={{ placement: "bottom", classes: { root: "justify-center pt-2", item: "text-xs font-semibold" } }}
									props={{
										tooltip: {
											root: { class: "bg-background border-border rounded-lg border-2 px-2 py-1 shadow-xl" },
											item: { class: "text-xs font-semibold" },
											header: { class: "text-xs font-bold" },
											hideTotal: true
										},
										xAxis: { tickLabelProps: { rotate: -35, textAnchor: "end" } }
									}}
								/>
							{:else}
								<LineChart
									data={rows}
									x="upperMinutes"
									y="cumulativePercent"
									yDomain={[0, 100]}
									height={300}
									padding={{ top: 14, right: 18, bottom: 42, left: 56 }}
									tooltipContext={{ mode: "bisect-x" }}
									series={[{ key: "cumulativePercent", label: "Cumulative share", color: "var(--color-accent)" }]}
									legend={{ placement: "bottom", classes: { root: "justify-center pt-2", item: "text-xs font-semibold" } }}
									props={{
										spline: { strokeWidth: 3 },
										tooltip: {
											root: { class: "bg-background border-border rounded-lg border-2 px-2 py-1 shadow-xl" },
											item: { class: "text-xs font-semibold" },
											header: { class: "text-xs font-bold" },
											hideTotal: true
										}
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
