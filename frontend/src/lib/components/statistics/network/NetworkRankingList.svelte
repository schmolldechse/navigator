<script lang="ts">
	import type {
		EventMetrics,
		JourneyMetrics,
		LineJourneyRankingItem,
		MetricPage,
		StationEventRankingItem,
		StatisticsMetricResponse
	} from "@lib/api";
	import Pagination from "@lib/components/ui/Pagination.svelte";
	import Skeleton from "@lib/components/ui/Skeleton.svelte";
	import type { RemoteQuery } from "@sveltejs/kit";
	import CircleAlert from "@lucide/svelte/icons/circle-alert";
	import ListOrdered from "@lucide/svelte/icons/list-ordered";
	import { BarChart } from "layerchart";
	import DashboardPanel from "../shared/DashboardPanel.svelte";
	import {
		formatCount,
		formatMetric,
		pageHasMore,
		pageTotalPages,
		pageValue,
		toNumber,
		transportTypeShortLabel
	} from "../shared/statistics-dashboard";

	type RankingMode = "stations" | "lines";

	type RankingRow = {
		id: string;
		label: string;
		value: number;
		displayValue: string;
		volume: string;
	};

	type Props = {
		title: string;
		description?: string;
		promise: RemoteQuery<StatisticsMetricResponse>;
		mode: RankingMode;
		onpagechange: (offset: number) => void;
	};

	let { title, description, promise, mode, onpagechange }: Props = $props();

	const eventValue = (metrics: EventMetrics) => toNumber(metrics.delayDebtMinutes) ?? 0;
	const journeyValue = (metrics: JourneyMetrics) => toNumber(metrics.destinationDelayDebtMinutes) ?? 0;

	const createRows = (response: StatisticsMetricResponse, rankingMode: RankingMode): RankingRow[] => {
		if (!("items" in response.result)) return [];

		if (rankingMode === "stations") {
			return (response.result.items as StationEventRankingItem[]).map((item) => ({
				id: String(item.station.stationEvaNumber),
				label: item.station.stationName ?? "Unnamed station",
				value: eventValue(item.eventMetrics),
				displayValue: formatMetric(item.eventMetrics.delayDebtMinutes, "minutes", true),
				volume: `${formatCount(item.eventMetrics.plannedEvents, true)} stops`
			}));
		}

		return (response.result.items as LineJourneyRankingItem[]).map((item, index) => ({
			id: `${item.line.lineName}-${index}`,
			label: item.line.lineName,
			value: journeyValue(item.journeyMetrics),
			displayValue: formatMetric(item.journeyMetrics.destinationDelayDebtMinutes, "minutes", true),
			volume: `${transportTypeShortLabel(item.line.transportType)} | ${formatCount(item.journeyMetrics.plannedJourneys, true)} journeys`
		}));
	};

	const getPage = (response: StatisticsMetricResponse): MetricPage | null =>
		"page" in response.result ? (response.result.page as MetricPage) : null;
</script>

<DashboardPanel {title} {description} icon={ListOrdered}>
	{#if promise.loading}
		<div class="flex min-h-96 flex-col gap-y-3">
			<Skeleton class="h-52 w-full" />
			{#each Array.from({ length: 5 }) as _, index (index)}
				<Skeleton class="h-12 w-full" />
			{/each}
		</div>
	{:else if promise.error}
		<div
			class="border-destructive/30 bg-destructive/10 flex min-h-80 flex-col items-center justify-center gap-2 rounded-lg border text-center"
		>
			<CircleAlert size={32} class="text-destructive" />
			<p class="text-destructive text-sm font-semibold">{promise.error.message}</p>
		</div>
	{:else if promise.current}
		{@const rows = createRows(promise.current, mode)}
		{@const page = getPage(promise.current)}
		{#if rows.length === 0}
			<div class="border-border bg-secondary/25 flex min-h-80 items-center justify-center rounded-lg border text-center">
				<p class="text-foreground/60 text-sm font-semibold">No ranking rows available.</p>
			</div>
		{:else}
			<div class="flex flex-col gap-4">
				<div class="bg-secondary/20 min-h-56 rounded-lg p-2">
					<BarChart
						data={rows}
						x="value"
						y="label"
						orientation="horizontal"
						yDomain={rows.map((row) => row.label)}
						height={230}
						padding={{ top: 10, right: 18, bottom: 30, left: 138 }}
						tooltipContext={{ mode: "band" }}
						series={[{ key: "value", label: "Delay debt", color: "var(--color-accent)" }]}
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
				</div>

				{#if page}
					<Pagination
						offset={pageValue(page.offset)}
						limit={Math.max(1, pageValue(page.limit))}
						totalItems={pageValue(page.totalItems)}
						totalPages={pageTotalPages(page)}
						hasMore={pageHasMore(page)}
						{onpagechange}
					/>
				{/if}
			</div>
		{/if}
	{/if}
</DashboardPanel>
