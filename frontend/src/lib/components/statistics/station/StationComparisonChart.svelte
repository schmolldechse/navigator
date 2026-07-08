<script lang="ts">
	import type { ArrivalDepartureComparisonItem, StatisticsMetricResponse, TransportTypeMixItem } from "@lib/api";
	import Skeleton from "@lib/components/ui/Skeleton.svelte";
	import type { RemoteQuery } from "@sveltejs/kit";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";
	import GitCompare from "@lucide/svelte/icons/git-compare";
	import { BarChart } from "layerchart";
	import DashboardPanel from "../shared/DashboardPanel.svelte";
	import { formatCount, formatMetric, toNumber, transportTypeLabel } from "../shared/statistics-dashboard";

	type ComparisonMode = "arrivalDeparture" | "transportMix";

	type ComparisonRow = {
		id: string;
		label: string;
		value: number;
		displayValue: string;
		secondary: string;
	};

	type Props = {
		title: string;
		description?: string;
		promise: RemoteQuery<StatisticsMetricResponse>;
		mode: ComparisonMode;
	};

	let { title, description, promise, mode }: Props = $props();

	const createRows = (response: StatisticsMetricResponse, comparisonMode: ComparisonMode): ComparisonRow[] => {
		if (!("items" in response.result)) return [];

		if (comparisonMode === "transportMix") {
			return (response.result.items as TransportTypeMixItem[]).map((item) => ({
				id: item.transportType,
				label: transportTypeLabel(item.transportType),
				value: toNumber(item.share) ?? 0,
				displayValue: formatMetric(item.share, "rate"),
				secondary: `${formatMetric(item.eventMetrics.customerReliability5Rate, "rate")} reliable | ${formatCount(
					item.eventMetrics.plannedEvents,
					true
				)} events`
			}));
		}

		return (response.result.items as ArrivalDepartureComparisonItem[]).map((item) => ({
			id: item.scheduleType,
			label: item.scheduleType === "ARRIVAL" ? "Arrivals" : "Departures",
			value: toNumber(item.eventMetrics.customerReliability5Rate) ?? 0,
			displayValue: formatMetric(item.eventMetrics.customerReliability5Rate, "rate"),
			secondary: `${formatMetric(item.eventMetrics.cancellationRate, "rate")} cancelled | ${formatCount(
				item.eventMetrics.plannedEvents,
				true
			)} events`
		}));
	};
</script>

<DashboardPanel {title} {description} icon={GitCompare}>
	{#if promise.loading}
		<div class="flex min-h-64 flex-col gap-y-3">
			<Skeleton class="h-52 w-full" />
			<Skeleton class="h-5 w-2/3" />
		</div>
	{:else if promise.error}
		<div
			class="border-destructive/30 bg-destructive/10 flex min-h-64 flex-col items-center justify-center gap-2 rounded-lg border text-center"
		>
			<CircleAlert size={32} class="text-destructive" />
			<p class="text-destructive text-sm font-semibold">{promise.error.message}</p>
		</div>
	{:else if promise.current}
		{@const rows = createRows(promise.current, mode)}
		{#if rows.length === 0}
			<div class="border-border bg-secondary/25 flex min-h-64 items-center justify-center rounded-lg border text-center">
				<p class="text-foreground/60 text-sm font-semibold">No comparison rows available.</p>
			</div>
		{:else}
			<div class="grid gap-4">
				<div class="bg-secondary/20 min-h-52 rounded-lg p-2">
					<BarChart
						data={rows}
						x="value"
						y="label"
						orientation="horizontal"
						xDomain={[0, 1]}
						height={220}
						padding={{ top: 12, right: 16, bottom: 32, left: 96 }}
						tooltipContext={{ mode: "band" }}
						series={[{ key: "value", label: "Value", color: "var(--color-accent)" }]}
					/>
				</div>

				<div class="grid gap-2">
					{#each rows as row (row.id)}
						<div class="flex items-baseline justify-between gap-3 text-sm">
							<p class="text-foreground font-semibold">{row.label}</p>
							<p class="text-foreground/60 text-right text-xs font-semibold">{row.displayValue} | {row.secondary}</p>
						</div>
					{/each}
				</div>
			</div>
		{/if}
	{/if}
</DashboardPanel>
