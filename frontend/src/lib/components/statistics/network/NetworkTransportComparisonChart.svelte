<script lang="ts">
	import type { StatisticsMetricResponse, TransportTypeComparisonItem } from "@lib/api";
	import Skeleton from "@lib/components/ui/Skeleton.svelte";
	import ToggleGroup from "@lib/components/ui/toggle-group/ToggleGroup.svelte";
	import ToggleGroupItem from "@lib/components/ui/toggle-group/ToggleGroupItem.svelte";
	import type { RemoteQuery } from "@sveltejs/kit";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";
	import GitCompare from "@lucide/svelte/icons/git-compare";
	import { BarChart, ScatterChart } from "layerchart";
	import DashboardPanel from "../shared/DashboardPanel.svelte";
	import { toNumber, transportTypeLabel, type NetworkComparisonMetric } from "../shared/statistics-dashboard";

	type Props = {
		promise: RemoteQuery<StatisticsMetricResponse>;
		metric: NetworkComparisonMetric;
		onmetricchange: (metric: NetworkComparisonMetric) => void;
	};

	type ComparisonRow = {
		id: string;
		label: string;
		value: number;
		volume: number;
	};

	type ComparisonMetricOption = {
		value: NetworkComparisonMetric;
		label: string;
		kind: "rate" | "count";
		sort: "asc" | "desc";
		getValue: (item: TransportTypeComparisonItem) => number;
	};

	let { promise, metric, onmetricchange }: Props = $props();

	const metricOptions: ComparisonMetricOption[] = [
		{
			value: "plannedStops",
			label: "Planned stops",
			kind: "count",
			sort: "desc",
			getValue: (item) => toNumber(item.eventMetrics.plannedEvents) ?? 0
		},
		{
			value: "cancellationRate",
			label: "Cancellation rate",
			kind: "rate",
			sort: "asc",
			getValue: (item) => toNumber(item.eventMetrics.cancellationRate) ?? 0
		},
		{
			value: "reliability5",
			label: "Reliable < 6 min",
			kind: "rate",
			sort: "desc",
			getValue: (item) => toNumber(item.eventMetrics.customerReliability5Rate) ?? 0
		},
		{
			value: "reliability15",
			label: "Reliable < 15 min",
			kind: "rate",
			sort: "desc",
			getValue: (item) => toNumber(item.eventMetrics.customerReliability15Rate) ?? 0
		},
		{
			value: "journeyCompletion",
			label: "Journey completion",
			kind: "rate",
			sort: "desc",
			getValue: (item) => toNumber(item.journeyMetrics.journeyCompletionRate) ?? 0
		}
	];
	const selectedMetric = $derived(metricOptions.find((option) => option.value === metric) ?? metricOptions[2]);

	const createRows = (response: StatisticsMetricResponse, option: ComparisonMetricOption): ComparisonRow[] => {
		if (!("items" in response.result)) return [];

		return (response.result.items as TransportTypeComparisonItem[])
			.map((item) => ({
				id: item.transportType,
				label: transportTypeLabel(item.transportType),
				value: option.getValue(item),
				volume: toNumber(item.eventMetrics.plannedEvents) ?? 0
			}))
			.sort((left, right) => (option.sort === "desc" ? right.value - left.value : left.value - right.value));
	};
</script>

<DashboardPanel title="Transport families" description="Network quality by transport type." icon={GitCompare}>
	{#snippet actions()}
		<ToggleGroup
			mode="single"
			allowEmpty={false}
			selected={selectedMetric}
			keyFn={(option: ComparisonMetricOption) => option.value}
			onselect={(option: ComparisonMetricOption | undefined) => onmetricchange(option?.value ?? "reliability5")}
			class="gap-1"
		>
			{#each metricOptions as option (option.value)}
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
		<div class="flex min-h-72 flex-col gap-y-3">
			<Skeleton class="h-64 w-full" />
			<Skeleton class="h-5 w-2/3" />
		</div>
	{:else if promise.error}
		<div
			class="border-destructive/30 bg-destructive/10 flex min-h-72 flex-col items-center justify-center gap-2 rounded-lg border text-center"
		>
			<CircleAlert size={32} class="text-destructive" />
			<p class="text-destructive text-sm font-semibold">{promise.error.message}</p>
		</div>
	{:else if promise.current}
		{@const rows = createRows(promise.current, selectedMetric)}
		{#if rows.length === 0}
			<div class="border-border bg-secondary/25 flex min-h-72 items-center justify-center rounded-lg border text-center">
				<p class="text-foreground/60 text-sm font-semibold">No comparison rows available.</p>
			</div>
		{:else}
			<div class="grid gap-4">
				<div class="bg-secondary/20 min-h-72 rounded-lg p-2">
					{#if selectedMetric.kind === "rate"}
						<ScatterChart
							data={rows}
							x="value"
							y="label"
							r="volume"
							xDomain={[0, 1]}
							yDomain={rows.map((row) => row.label)}
							rRange={[6, 13]}
							height={300}
							padding={{ top: 16, right: 20, bottom: 34, left: 116 }}
							tooltipContext={{ mode: "quadtree" }}
							cRange={["var(--color-accent)"]}
							props={{
								tooltip: {
									root: { class: "bg-background border-border rounded-lg border-2 px-2 py-1 shadow-xl" },
									item: { class: "text-xs font-semibold" },
									header: { class: "text-xs font-bold" },
									hideTotal: true
								}
							}}
						/>
					{:else}
						<BarChart
							data={rows}
							x="value"
							y="label"
							orientation="horizontal"
							yDomain={rows.map((row) => row.label)}
							height={300}
							padding={{ top: 12, right: 16, bottom: 32, left: 116 }}
							tooltipContext={{ mode: "band" }}
							series={[{ key: "value", label: selectedMetric.label, color: "var(--color-accent)" }]}
							legend={{ placement: "bottom", classes: { root: "justify-center pt-2", item: "text-xs font-semibold" } }}
							props={{
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
		{/if}
	{/if}
</DashboardPanel>
