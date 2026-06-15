<script lang="ts">
	import type {
		EventMetrics,
		JourneyMetrics,
		LineJourneyRankingItem,
		StationDirectionItem,
		StationEventRankingItem,
		StationLineRankingItem,
		StatisticsMetricResponse
	} from "@lib/api";
	import Skeleton from "@lib/components/ui/Skeleton.svelte";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";
	import ListOrdered from "@lucide/svelte/icons/list-ordered";
	import DashboardPanel from "./DashboardPanel.svelte";
	import { formatCount, formatMetric, toNumber, transportTypeShortLabel } from "./statistics-dashboard";

	type RankingMode = "networkStations" | "networkLines" | "stationLines" | "stationDirections";

	type RankingRow = {
		id: string;
		label: string;
		subtitle?: string;
		value: number | null;
		displayValue: string;
		volume: string;
		href?: string;
	};

	type Props = {
		title: string;
		description?: string;
		promise: Promise<StatisticsMetricResponse>;
		mode: RankingMode;
	};

	let { title, description, promise, mode }: Props = $props();

	const eventValue = (metrics: EventMetrics) => toNumber(metrics.delayDebtMinutes);
	const journeyValue = (metrics: JourneyMetrics) => toNumber(metrics.destinationDelayDebtMinutes);

	const createRows = (response: StatisticsMetricResponse, rankingMode: RankingMode): RankingRow[] => {
		if (!("items" in response.result)) return [];

		if (rankingMode === "networkStations") {
			return (response.result.items as StationEventRankingItem[]).map((item) => ({
				id: String(item.station.stationEvaNumber),
				label: item.station.stationName ?? String(item.station.stationEvaNumber),
				subtitle: `EVA ${item.station.stationEvaNumber}`,
				value: eventValue(item.eventMetrics),
				displayValue: formatMetric(item.eventMetrics.delayDebtMinutes, "minutes", true),
				volume: `${formatCount(item.eventMetrics.plannedEvents, true)} events`,
				href: `/statistics/${item.station.stationEvaNumber}`
			}));
		}

		if (rankingMode === "networkLines") {
			return (response.result.items as LineJourneyRankingItem[]).map((item, index) => ({
				id: `${item.line.lineName}-${index}`,
				label: item.line.lineName,
				subtitle: [
					transportTypeShortLabel(item.line.transportType),
					item.line.origin?.stationName,
					item.line.destination?.stationName
				]
					.filter(Boolean)
					.join(" | "),
				value: journeyValue(item.journeyMetrics),
				displayValue: formatMetric(item.journeyMetrics.destinationDelayDebtMinutes, "minutes", true),
				volume: `${formatCount(item.journeyMetrics.plannedJourneys, true)} journeys`
			}));
		}

		if (rankingMode === "stationDirections") {
			return (response.result.items as StationDirectionItem[]).map((item, index) => ({
				id: `${item.directionType}-${item.station.stationEvaNumber}-${index}`,
				label: item.station.stationName ?? String(item.station.stationEvaNumber),
				subtitle: `${item.directionType.toLowerCase()} | EVA ${item.station.stationEvaNumber}`,
				value: eventValue(item.eventMetrics),
				displayValue: formatMetric(item.eventMetrics.delayDebtMinutes, "minutes", true),
				volume: `${formatCount(item.eventMetrics.plannedEvents, true)} events`
			}));
		}

		return (response.result.items as StationLineRankingItem[]).map((item, index) => ({
			id: `${item.line.lineName}-${index}`,
			label: item.line.lineName,
			subtitle: transportTypeShortLabel(item.line.transportType),
			value: eventValue(item.eventMetrics),
			displayValue: formatMetric(item.eventMetrics.delayDebtMinutes, "minutes", true),
			volume: `${formatCount(item.eventMetrics.plannedEvents, true)} events`
		}));
	};

	const maxValue = (rows: RankingRow[]): number => Math.max(...rows.map((row) => row.value ?? 0), 1);
</script>

<DashboardPanel {title} {description} icon={ListOrdered}>
	{#await promise}
		<div class="flex min-h-80 flex-col gap-y-3">
			{#each Array.from({ length: 6 }) as _, index (index)}
				<Skeleton class="h-12 w-full" />
			{/each}
		</div>
	{:then response}
		{@const rows = createRows(response, mode)}
		{@const maximum = maxValue(rows)}
		{#if rows.length === 0}
			<div class="border-border bg-secondary/25 flex min-h-80 items-center justify-center rounded-lg border text-center">
				<p class="text-foreground/60 text-sm font-semibold">No ranking rows available.</p>
			</div>
		{:else}
			<div class="flex flex-col gap-2">
				{#each rows as row, index (row.id)}
					<svelte:element
						this={row.href ? "a" : "div"}
						href={row.href}
						class="border-border bg-secondary/10 hover:bg-accent/10 grid grid-cols-[2rem_minmax(0,1fr)_auto] items-center gap-3 rounded-lg border p-3 transition-colors"
					>
						<div class="text-foreground/45 text-right text-sm font-bold tabular-nums">{index + 1}</div>
						<div class="min-w-0">
							<div class="flex min-w-0 items-baseline gap-2">
								<p class="text-foreground truncate font-semibold">{row.label}</p>
								<p class="text-foreground/45 shrink-0 text-xs font-semibold">{row.volume}</p>
							</div>
							{#if row.subtitle}
								<p class="text-foreground/55 truncate text-xs">{row.subtitle}</p>
							{/if}
							<div class="bg-secondary mt-2 h-1.5 overflow-hidden rounded-full">
								<div
									class="bg-accent h-full rounded-full"
									style={`width: ${Math.max(4, ((row.value ?? 0) / maximum) * 100)}%;`}
								></div>
							</div>
						</div>
						<p class="text-foreground text-sm font-bold tabular-nums">{row.displayValue}</p>
					</svelte:element>
				{/each}
			</div>
		{/if}
	{:catch error}
		<div
			class="border-destructive/30 bg-destructive/10 flex min-h-80 flex-col items-center justify-center gap-2 rounded-lg border text-center"
		>
			<CircleAlert size={32} class="text-destructive" />
			<p class="text-destructive text-sm font-semibold">{error.message}</p>
		</div>
	{/await}
</DashboardPanel>
