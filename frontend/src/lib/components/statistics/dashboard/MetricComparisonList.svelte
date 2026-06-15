<script lang="ts">
	import type {
		ArrivalDepartureComparisonItem,
		StatisticsMetricResponse,
		TransportTypeComparisonItem,
		TransportTypeMixItem
	} from "@lib/api";
	import Skeleton from "@lib/components/ui/Skeleton.svelte";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";
	import GitCompare from "@lucide/svelte/icons/git-compare";
	import DashboardPanel from "./DashboardPanel.svelte";
	import { formatCount, formatMetric, toNumber, transportTypeLabel } from "./statistics-dashboard";

	type ComparisonMode = "transportComparison" | "arrivalDeparture" | "transportMix";

	type ComparisonRow = {
		id: string;
		label: string;
		value: number | null;
		displayValue: string;
		secondary: string;
	};

	type Props = {
		title: string;
		description?: string;
		promise: Promise<StatisticsMetricResponse>;
		mode: ComparisonMode;
	};

	let { title, description, promise, mode }: Props = $props();

	const createRows = (response: StatisticsMetricResponse, comparisonMode: ComparisonMode): ComparisonRow[] => {
		if (!("items" in response.result)) return [];

		if (comparisonMode === "transportComparison") {
			return (response.result.items as TransportTypeComparisonItem[]).map((item) => ({
				id: item.transportType,
				label: transportTypeLabel(item.transportType),
				value: toNumber(item.eventMetrics.customerReliability5Rate),
				displayValue: formatMetric(item.eventMetrics.customerReliability5Rate, "rate"),
				secondary: `${formatCount(item.eventMetrics.plannedEvents, true)} events | ${formatCount(
					item.journeyMetrics.plannedJourneys,
					true
				)} journeys`
			}));
		}

		if (comparisonMode === "transportMix") {
			return (response.result.items as TransportTypeMixItem[]).map((item) => ({
				id: item.transportType,
				label: transportTypeLabel(item.transportType),
				value: toNumber(item.share),
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
			value: toNumber(item.eventMetrics.customerReliability5Rate),
			displayValue: formatMetric(item.eventMetrics.customerReliability5Rate, "rate"),
			secondary: `${formatMetric(item.eventMetrics.cancellationRate, "rate")} cancelled | ${formatCount(
				item.eventMetrics.plannedEvents,
				true
			)} events`
		}));
	};
</script>

<DashboardPanel {title} {description} icon={GitCompare}>
	{#await promise}
		<div class="flex min-h-64 flex-col gap-y-3">
			{#each Array.from({ length: 5 }) as _, index (index)}
				<Skeleton class="h-12 w-full" />
			{/each}
		</div>
	{:then response}
		{@const rows = createRows(response, mode)}
		{#if rows.length === 0}
			<div class="border-border bg-secondary/25 flex min-h-64 items-center justify-center rounded-lg border text-center">
				<p class="text-foreground/60 text-sm font-semibold">No comparison rows available.</p>
			</div>
		{:else}
			<div class="grid gap-2">
				{#each rows as row (row.id)}
					<div class="border-border bg-secondary/10 rounded-lg border p-3">
						<div class="flex items-baseline justify-between gap-3">
							<p class="text-foreground font-semibold">{row.label}</p>
							<p class="text-foreground font-bold tabular-nums">{row.displayValue}</p>
						</div>
						<div class="bg-secondary mt-2 h-2 overflow-hidden rounded-full">
							<div class="bg-accent h-full rounded-full" style={`width: ${Math.max(2, (row.value ?? 0) * 100)}%;`}></div>
						</div>
						<p class="text-foreground/55 mt-1 text-xs">{row.secondary}</p>
					</div>
				{/each}
			</div>
		{/if}
	{:catch error}
		<div
			class="border-destructive/30 bg-destructive/10 flex min-h-64 flex-col items-center justify-center gap-2 rounded-lg border text-center"
		>
			<CircleAlert size={32} class="text-destructive" />
			<p class="text-destructive text-sm font-semibold">{error.message}</p>
		</div>
	{/await}
</DashboardPanel>
